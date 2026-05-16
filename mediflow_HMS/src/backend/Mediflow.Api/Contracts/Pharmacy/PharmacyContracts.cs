namespace Mediflow.Api.Contracts.Pharmacy;

public sealed record CreateMedicineRequest(string Code, string Name, string Form, string Strength, int ReorderLevel);
public sealed record AddBatchRequest(string MedicineCode, string BatchNo, DateOnly ExpiryDate, decimal UnitCost, int Quantity);
public sealed record OpdDispenseRequest(Guid? EncounterId, string PatientName, string MedicineCode, int Quantity);
public sealed record IpdIssueRequest(Guid AdmissionId, string PatientName, string MedicineCode, int Quantity);
public sealed record ReturnRequest(string MedicineCode, int Quantity, string Reason);

public sealed record CreateSupplierRequest(string Name, string Contact, string Address);
public sealed record CreatePurchaseRequestRequest(string Department);
public sealed record CreatePurchaseOrderRequest(Guid SupplierId);
public sealed record ReceiveLine(string MedicineCode, string BatchNo, DateOnly ExpiryDate, decimal UnitCost, int Quantity);
public sealed record GoodsReceiveRequest(Guid PurchaseOrderId, IReadOnlyCollection<ReceiveLine> Lines);
public sealed record IssueToDepartmentRequest(string Department, string MedicineCode, int Quantity);
