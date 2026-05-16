using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Diagnostics;

namespace Mediflow.Infrastructure.Diagnostics;

internal sealed class InMemoryDiagnosticsWorkflowService(IAuditLogService auditLogService) : IDiagnosticsWorkflowService
{
    private static readonly object Sync = new();
    private static readonly List<LabTestCatalogDto> LabCatalog = [];
    private static readonly List<LabOrderDto> LabOrders = [];
    private static readonly List<ImagingServiceCatalogDto> ImagingCatalog = [];
    private static readonly List<ImagingOrderDto> ImagingOrders = [];

    public LabTestCatalogDto AddLabTest(CreateLabTestRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var test = new LabTestCatalogDto(Guid.NewGuid(), request.Code, request.Name, request.Unit, request.ReferenceRange);
            LabCatalog.Add(test);
            auditLogService.Add(actorUserId, "Lab.Catalog.Add", "LabTest", test.Id.ToString(), test.Code);
            return test;
        }
    }

    public IReadOnlyCollection<LabTestCatalogDto> GetLabCatalog() => LabCatalog.ToArray();

    public LabOrderDto CreateLabOrder(CreateLabOrderRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var test = LabCatalog.FirstOrDefault(x => x.Code.Equals(request.TestCode, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Lab test not found.");
            var order = new LabOrderDto(Guid.NewGuid(), request.AdmissionId, request.EncounterId, request.PatientName, test.Code, test.Name, LabOrderStatus.Ordered, false, null, null, DateTime.UtcNow, null, null);
            LabOrders.Add(order);
            auditLogService.Add(actorUserId, "Lab.Order.Create", "LabOrder", order.Id.ToString(), order.TestCode);
            return order;
        }
    }

    public LabOrderDto? CollectSample(Guid labOrderId, Guid? actorUserId)
    {
        lock (Sync)
        {
            var order = LabOrders.FirstOrDefault(x => x.Id == labOrderId);
            if (order is null) return null;
            order = order with { Status = LabOrderStatus.SampleCollected, CollectedAtUtc = DateTime.UtcNow };
            Replace(LabOrders, order, x => x.Id == labOrderId);
            auditLogService.Add(actorUserId, "Lab.Sample.Collect", "LabOrder", order.Id.ToString(), order.TestCode);
            return order;
        }
    }

    public LabOrderDto? EnterLabResult(Guid labOrderId, EnterLabResultRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var order = LabOrders.FirstOrDefault(x => x.Id == labOrderId);
            if (order is null) return null;
            order = order with { Status = LabOrderStatus.ResultEntered, ResultValue = request.ResultValue, ResultNotes = request.ResultNotes, IsCritical = request.IsCritical };
            Replace(LabOrders, order, x => x.Id == labOrderId);
            auditLogService.Add(actorUserId, "Lab.Result.Enter", "LabOrder", order.Id.ToString(), request.IsCritical ? "Critical" : "Normal");
            return order;
        }
    }

    public LabOrderDto? ApproveLabResult(Guid labOrderId, Guid? actorUserId)
    {
        lock (Sync)
        {
            var order = LabOrders.FirstOrDefault(x => x.Id == labOrderId);
            if (order is null) return null;
            order = order with { Status = LabOrderStatus.Approved, ApprovedAtUtc = DateTime.UtcNow };
            Replace(LabOrders, order, x => x.Id == labOrderId);
            auditLogService.Add(actorUserId, "Lab.Result.Approve", "LabOrder", order.Id.ToString(), order.TestCode);
            return order;
        }
    }

    public IReadOnlyCollection<LabOrderDto> GetLabOrders() => LabOrders.OrderByDescending(x => x.OrderedAtUtc).ToArray();

    public ImagingServiceCatalogDto AddImagingService(CreateImagingServiceRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var service = new ImagingServiceCatalogDto(Guid.NewGuid(), request.Code, request.Name, request.Modality);
            ImagingCatalog.Add(service);
            auditLogService.Add(actorUserId, "Imaging.Catalog.Add", "ImagingService", service.Id.ToString(), service.Code);
            return service;
        }
    }

    public IReadOnlyCollection<ImagingServiceCatalogDto> GetImagingCatalog() => ImagingCatalog.ToArray();

    public ImagingOrderDto CreateImagingOrder(CreateImagingOrderRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var service = ImagingCatalog.FirstOrDefault(x => x.Code.Equals(request.ServiceCode, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Imaging service not found.");
            var order = new ImagingOrderDto(Guid.NewGuid(), request.AdmissionId, request.EncounterId, request.PatientName, service.Code, service.Name, ImagingOrderStatus.Ordered, DateTime.UtcNow, null, null, null, null);
            ImagingOrders.Add(order);
            auditLogService.Add(actorUserId, "Imaging.Order.Create", "ImagingOrder", order.Id.ToString(), order.ServiceCode);
            return order;
        }
    }

    public ImagingOrderDto? ScheduleImaging(Guid imagingOrderId, ScheduleImagingRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var order = ImagingOrders.FirstOrDefault(x => x.Id == imagingOrderId);
            if (order is null) return null;
            order = order with { Status = ImagingOrderStatus.Scheduled, ScheduledAtUtc = request.ScheduledAtUtc };
            Replace(ImagingOrders, order, x => x.Id == imagingOrderId);
            auditLogService.Add(actorUserId, "Imaging.Schedule", "ImagingOrder", order.Id.ToString(), request.ScheduledAtUtc.ToString("O"));
            return order;
        }
    }

    public ImagingOrderDto? EnterImagingReport(Guid imagingOrderId, EnterImagingReportRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var order = ImagingOrders.FirstOrDefault(x => x.Id == imagingOrderId);
            if (order is null) return null;
            order = order with { Status = ImagingOrderStatus.ReportEntered, ReportText = request.ReportText, FileReference = request.FileReference };
            Replace(ImagingOrders, order, x => x.Id == imagingOrderId);
            auditLogService.Add(actorUserId, "Imaging.Report.Enter", "ImagingOrder", order.Id.ToString(), request.FileReference);
            return order;
        }
    }

    public ImagingOrderDto? ApproveImagingReport(Guid imagingOrderId, Guid? actorUserId)
    {
        lock (Sync)
        {
            var order = ImagingOrders.FirstOrDefault(x => x.Id == imagingOrderId);
            if (order is null) return null;
            order = order with { Status = ImagingOrderStatus.Approved, ApprovedAtUtc = DateTime.UtcNow };
            Replace(ImagingOrders, order, x => x.Id == imagingOrderId);
            auditLogService.Add(actorUserId, "Imaging.Report.Approve", "ImagingOrder", order.Id.ToString(), order.ServiceCode);
            return order;
        }
    }

    public IReadOnlyCollection<ImagingOrderDto> GetImagingOrders() => ImagingOrders.OrderByDescending(x => x.OrderedAtUtc).ToArray();

    private static void Replace<T>(List<T> list, T updated, Func<T, bool> predicate)
    {
        var idx = list.FindIndex(x => predicate(x));
        if (idx >= 0) list[idx] = updated;
    }
}
