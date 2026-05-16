using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Pharmacy;

namespace Mediflow.Infrastructure.Pharmacy;

internal sealed class InMemoryPharmacyInventoryService(IAuditLogService auditLogService) : IPharmacyInventoryService
{
    private static readonly object Sync = new();
    private static readonly List<MedicineDto> Medicines = [];
    private static readonly List<BatchDto> Batches = [];
    private static readonly List<DispenseDto> Dispenses = [];
    private static readonly List<StockMovementDto> Movements = [];

    private static readonly List<SupplierDto> Suppliers = [];
    private static readonly List<PurchaseRequestDto> PurchaseRequests = [];
    private static readonly List<PurchaseOrderDto> PurchaseOrders = [];
    private static readonly List<GoodsReceiveDto> GoodsReceives = [];

    public MedicineDto AddMedicine(CreateMedicineRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var med = new MedicineDto(Guid.NewGuid(), request.Code, request.Name, request.Form, request.Strength, request.ReorderLevel);
            Medicines.Add(med);
            auditLogService.Add(actorUserId, "Pharmacy.Medicine.Add", "Medicine", med.Id.ToString(), med.Code);
            return med;
        }
    }

    public BatchDto AddBatch(AddBatchRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var med = Medicines.FirstOrDefault(x => x.Code.Equals(request.MedicineCode, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Medicine not found.");
            var batch = new BatchDto(Guid.NewGuid(), med.Id, request.BatchNo, request.ExpiryDate, request.UnitCost, request.Quantity);
            Batches.Add(batch);
            Movements.Add(new StockMovementDto(Guid.NewGuid(), med.Id, med.Code, StockMovementType.PurchaseReceive, request.Quantity, DateTime.UtcNow, $"BATCH:{request.BatchNo}"));
            return batch;
        }
    }

    public IReadOnlyCollection<MedicineDto> GetMedicines() => Medicines.ToArray();

    public IReadOnlyCollection<BatchDto> GetBatches(string? medicineCode = null)
    {
        if (string.IsNullOrWhiteSpace(medicineCode)) return Batches.ToArray();
        var med = Medicines.FirstOrDefault(x => x.Code.Equals(medicineCode, StringComparison.OrdinalIgnoreCase));
        return med is null ? [] : Batches.Where(x => x.MedicineId == med.Id).ToArray();
    }

    public DispenseDto OpdDispense(OpdDispenseRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var med = Medicines.First(x => x.Code.Equals(request.MedicineCode, StringComparison.OrdinalIgnoreCase));
            ConsumeStock(med.Id, request.Quantity);
            var d = new DispenseDto(Guid.NewGuid(), null, request.EncounterId, request.PatientName, "OPD", med.Code, request.Quantity, DateTime.UtcNow);
            Dispenses.Add(d);
            Movements.Add(new StockMovementDto(Guid.NewGuid(), med.Id, med.Code, StockMovementType.OpdDispense, -request.Quantity, DateTime.UtcNow, "OPD"));
            auditLogService.Add(actorUserId, "Pharmacy.OPD.Dispense", "Dispense", d.Id.ToString(), med.Code);
            return d;
        }
    }

    public DispenseDto IpdIssue(IpdIssueRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var med = Medicines.First(x => x.Code.Equals(request.MedicineCode, StringComparison.OrdinalIgnoreCase));
            ConsumeStock(med.Id, request.Quantity);
            var d = new DispenseDto(Guid.NewGuid(), request.AdmissionId, null, request.PatientName, "IPD", med.Code, request.Quantity, DateTime.UtcNow);
            Dispenses.Add(d);
            Movements.Add(new StockMovementDto(Guid.NewGuid(), med.Id, med.Code, StockMovementType.IpdIssue, -request.Quantity, DateTime.UtcNow, "IPD"));
            auditLogService.Add(actorUserId, "Pharmacy.IPD.Issue", "Dispense", d.Id.ToString(), med.Code);
            return d;
        }
    }

    public StockMovementDto ReturnToStock(ReturnRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var med = Medicines.First(x => x.Code.Equals(request.MedicineCode, StringComparison.OrdinalIgnoreCase));
            AddStock(med.Id, request.Quantity);
            var m = new StockMovementDto(Guid.NewGuid(), med.Id, med.Code, StockMovementType.ReturnIn, request.Quantity, DateTime.UtcNow, request.Reason);
            Movements.Add(m);
            auditLogService.Add(actorUserId, "Pharmacy.Return", "StockMovement", m.Id.ToString(), request.Reason);
            return m;
        }
    }

    public SupplierDto AddSupplier(CreateSupplierRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var s = new SupplierDto(Guid.NewGuid(), request.Name, request.Contact, request.Address);
            Suppliers.Add(s);
            auditLogService.Add(actorUserId, "Inventory.Supplier.Add", "Supplier", s.Id.ToString(), s.Name);
            return s;
        }
    }

    public PurchaseRequestDto CreatePurchaseRequest(CreatePurchaseRequestRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var pr = new PurchaseRequestDto(Guid.NewGuid(), $"PR-{DateTime.UtcNow:yyyyMMddHHmmss}", request.Department, DateTime.UtcNow, "Requested");
            PurchaseRequests.Add(pr);
            auditLogService.Add(actorUserId, "Inventory.PR.Create", "PurchaseRequest", pr.Id.ToString(), pr.RequestNo);
            return pr;
        }
    }

    public PurchaseOrderDto CreatePurchaseOrder(CreatePurchaseOrderRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var po = new PurchaseOrderDto(Guid.NewGuid(), $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}", request.SupplierId, DateTime.UtcNow, "Ordered");
            PurchaseOrders.Add(po);
            auditLogService.Add(actorUserId, "Inventory.PO.Create", "PurchaseOrder", po.Id.ToString(), po.PoNo);
            return po;
        }
    }

    public GoodsReceiveDto ReceiveGoods(GoodsReceiveRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var po = PurchaseOrders.FirstOrDefault(x => x.Id == request.PurchaseOrderId) ?? throw new InvalidOperationException("PO not found.");
            foreach (var line in request.Lines)
            {
                var med = Medicines.FirstOrDefault(x => x.Code.Equals(line.MedicineCode, StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidOperationException($"Medicine not found: {line.MedicineCode}");
                Batches.Add(new BatchDto(Guid.NewGuid(), med.Id, line.BatchNo, line.ExpiryDate, line.UnitCost, line.Quantity));
                Movements.Add(new StockMovementDto(Guid.NewGuid(), med.Id, med.Code, StockMovementType.PurchaseReceive, line.Quantity, DateTime.UtcNow, po.PoNo));
            }
            var gr = new GoodsReceiveDto(Guid.NewGuid(), $"GRN-{DateTime.UtcNow:yyyyMMddHHmmss}", po.Id, DateTime.UtcNow);
            GoodsReceives.Add(gr);
            return gr;
        }
    }

    public StockMovementDto IssueToDepartment(IssueToDepartmentRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var med = Medicines.First(x => x.Code.Equals(request.MedicineCode, StringComparison.OrdinalIgnoreCase));
            ConsumeStock(med.Id, request.Quantity);
            var m = new StockMovementDto(Guid.NewGuid(), med.Id, med.Code, StockMovementType.DepartmentIssue, -request.Quantity, DateTime.UtcNow, request.Department);
            Movements.Add(m);
            return m;
        }
    }

    public IReadOnlyCollection<SupplierDto> GetSuppliers() => Suppliers.ToArray();
    public IReadOnlyCollection<PurchaseRequestDto> GetPurchaseRequests() => PurchaseRequests.ToArray();
    public IReadOnlyCollection<PurchaseOrderDto> GetPurchaseOrders() => PurchaseOrders.ToArray();
    public IReadOnlyCollection<GoodsReceiveDto> GetGoodsReceives() => GoodsReceives.ToArray();
    public IReadOnlyCollection<DispenseDto> GetDispenses() => Dispenses.ToArray();
    public IReadOnlyCollection<StockMovementDto> GetStockMovements() => Movements.OrderByDescending(x => x.AtUtc).ToArray();

    public IReadOnlyCollection<LowStockAlertDto> GetLowStockAlerts()
    {
        return Medicines.Select(m =>
        {
            var total = Batches.Where(b => b.MedicineId == m.Id).Sum(b => b.QuantityOnHand);
            return new LowStockAlertDto(m.Id, m.Code, total, m.ReorderLevel);
        }).Where(x => x.TotalStock <= x.ReorderLevel).ToArray();
    }

    private static void ConsumeStock(Guid medicineId, int qty)
    {
        var available = Batches.Where(x => x.MedicineId == medicineId).OrderBy(x => x.ExpiryDate).ToList();
        var total = available.Sum(x => x.QuantityOnHand);
        if (total < qty) throw new InvalidOperationException("Insufficient stock.");

        var left = qty;
        for (var i = 0; i < available.Count && left > 0; i++)
        {
            var b = available[i];
            var take = Math.Min(left, b.QuantityOnHand);
            left -= take;
            available[i] = b with { QuantityOnHand = b.QuantityOnHand - take };
            Replace(Batches, available[i], x => x.Id == b.Id);
        }
    }

    private static void AddStock(Guid medicineId, int qty)
    {
        var batch = Batches.Where(x => x.MedicineId == medicineId).OrderBy(x => x.ExpiryDate).FirstOrDefault();
        if (batch == null) throw new InvalidOperationException("No batch exists for medicine.");
        Replace(Batches, batch with { QuantityOnHand = batch.QuantityOnHand + qty }, x => x.Id == batch.Id);
    }

    private static void Replace<T>(List<T> list, T updated, Func<T, bool> predicate)
    {
        var idx = list.FindIndex(x => predicate(x));
        if (idx >= 0) list[idx] = updated;
    }
}
