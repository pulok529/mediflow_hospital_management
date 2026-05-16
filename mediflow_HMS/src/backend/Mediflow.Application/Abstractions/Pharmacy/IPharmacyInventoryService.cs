namespace Mediflow.Application.Abstractions.Pharmacy;

public enum StockMovementType { PurchaseReceive, OpdDispense, IpdIssue, ReturnIn, DepartmentIssue }

public sealed record MedicineDto(Guid Id, string Code, string Name, string Form, string Strength, int ReorderLevel);
public sealed record BatchDto(Guid Id, Guid MedicineId, string BatchNo, DateOnly ExpiryDate, decimal UnitCost, int QuantityOnHand);
public sealed record DispenseDto(Guid Id, Guid? AdmissionId, Guid? EncounterId, string PatientName, string Mode, string MedicineCode, int Quantity, DateTime AtUtc);
public sealed record StockMovementDto(Guid Id, Guid MedicineId, string MedicineCode, StockMovementType Type, int Quantity, DateTime AtUtc, string Reference);
public sealed record LowStockAlertDto(Guid MedicineId, string MedicineCode, int TotalStock, int ReorderLevel);

public sealed record SupplierDto(Guid Id, string Name, string Contact, string Address);
public sealed record PurchaseRequestDto(Guid Id, string RequestNo, string Department, DateTime RequestedAtUtc, string Status);
public sealed record PurchaseOrderDto(Guid Id, string PoNo, Guid SupplierId, DateTime OrderedAtUtc, string Status);
public sealed record GoodsReceiveDto(Guid Id, string GrnNo, Guid PurchaseOrderId, DateTime ReceivedAtUtc);

public sealed record CreateMedicineRequest(string Code, string Name, string Form, string Strength, int ReorderLevel);
public sealed record AddBatchRequest(string MedicineCode, string BatchNo, DateOnly ExpiryDate, decimal UnitCost, int Quantity);
public sealed record OpdDispenseRequest(Guid? EncounterId, string PatientName, string MedicineCode, int Quantity);
public sealed record IpdIssueRequest(Guid AdmissionId, string PatientName, string MedicineCode, int Quantity);
public sealed record ReturnRequest(string MedicineCode, int Quantity, string Reason);

public sealed record CreateSupplierRequest(string Name, string Contact, string Address);
public sealed record CreatePurchaseRequestRequest(string Department);
public sealed record CreatePurchaseOrderRequest(Guid SupplierId);
public sealed record GoodsReceiveRequest(Guid PurchaseOrderId, IReadOnlyCollection<ReceiveLine> Lines);
public sealed record IssueToDepartmentRequest(string Department, string MedicineCode, int Quantity);
public sealed record ReceiveLine(string MedicineCode, string BatchNo, DateOnly ExpiryDate, decimal UnitCost, int Quantity);

public interface IPharmacyInventoryService
{
    MedicineDto AddMedicine(CreateMedicineRequest request, Guid? actorUserId);
    BatchDto AddBatch(AddBatchRequest request, Guid? actorUserId);
    IReadOnlyCollection<MedicineDto> GetMedicines();
    IReadOnlyCollection<BatchDto> GetBatches(string? medicineCode = null);

    DispenseDto OpdDispense(OpdDispenseRequest request, Guid? actorUserId);
    DispenseDto IpdIssue(IpdIssueRequest request, Guid? actorUserId);
    StockMovementDto ReturnToStock(ReturnRequest request, Guid? actorUserId);

    SupplierDto AddSupplier(CreateSupplierRequest request, Guid? actorUserId);
    PurchaseRequestDto CreatePurchaseRequest(CreatePurchaseRequestRequest request, Guid? actorUserId);
    PurchaseOrderDto CreatePurchaseOrder(CreatePurchaseOrderRequest request, Guid? actorUserId);
    GoodsReceiveDto ReceiveGoods(GoodsReceiveRequest request, Guid? actorUserId);
    StockMovementDto IssueToDepartment(IssueToDepartmentRequest request, Guid? actorUserId);

    IReadOnlyCollection<SupplierDto> GetSuppliers();
    IReadOnlyCollection<PurchaseRequestDto> GetPurchaseRequests();
    IReadOnlyCollection<PurchaseOrderDto> GetPurchaseOrders();
    IReadOnlyCollection<GoodsReceiveDto> GetGoodsReceives();
    IReadOnlyCollection<DispenseDto> GetDispenses();
    IReadOnlyCollection<StockMovementDto> GetStockMovements();
    IReadOnlyCollection<LowStockAlertDto> GetLowStockAlerts();
}
