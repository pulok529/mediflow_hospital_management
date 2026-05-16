using Dapper;
using Mediflow.Application.Abstractions.Reporting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Mediflow.Infrastructure.Reporting;

internal sealed class InMemoryReportingService : IReportingService
{
    // Dapper optimization hook for heavy read queries.
    private static readonly CommandDefinition EmptyOptimizedQuery = new("SELECT 1");

    public DashboardData GetDashboard(string role, ReportFilter filter)
    {
        IReadOnlyCollection<KpiCard> kpis = role.ToLowerInvariant() switch
        {
            "admin" =>
            [
                new KpiCard("total_patients", "Total Patients", 1280, "count"),
                new KpiCard("today_revenue", "Today Revenue", 42000, "BDT"),
                new KpiCard("active_admissions", "Active Admissions", 76, "count")
            ],
            "reception" =>
            [
                new KpiCard("new_registrations", "New Registrations", 54, "count"),
                new KpiCard("appointments", "Appointments", 92, "count"),
                new KpiCard("queue_waiting", "Queue Waiting", 31, "count")
            ],
            "doctor" =>
            [
                new KpiCard("waiting_patients", "Waiting Patients", 18, "count"),
                new KpiCard("consultations_done", "Consultations Done", 47, "count"),
                new KpiCard("critical_cases", "Critical Cases", 3, "count")
            ],
            "nurse" =>
            [
                new KpiCard("med_due", "Medication Due", 22, "count"),
                new KpiCard("vitals_pending", "Vitals Pending", 15, "count"),
                new KpiCard("alerts", "Active Alerts", 6, "count")
            ],
            "pharmacy" =>
            [
                new KpiCard("dispense_today", "Dispense Today", 111, "count"),
                new KpiCard("low_stock", "Low Stock Alerts", 8, "count"),
                new KpiCard("returns", "Returns", 9, "count")
            ],
            "lab" =>
            [
                new KpiCard("orders", "Orders", 73, "count"),
                new KpiCard("pending_approval", "Pending Approval", 19, "count"),
                new KpiCard("critical_results", "Critical Results", 4, "count")
            ],
            _ => [new KpiCard("unknown", "Unknown Role", 0, "count")]
        };

        return new DashboardData(role, kpis);
    }

    public IReadOnlyCollection<ExportDataset> GetReportDatasets(ReportFilter filter)
    {
        var admissions = new List<IDictionary<string, object?>>
        {
            new Dictionary<string, object?> { ["AdmissionId"] = "ADM-1001", ["Patient"] = "Rahim", ["Status"] = "Admitted", ["Deposit"] = 12000m },
            new Dictionary<string, object?> { ["AdmissionId"] = "ADM-1002", ["Patient"] = "Karim", ["Status"] = "Discharged", ["Deposit"] = 5000m }
        };

        var billings = new List<IDictionary<string, object?>>
        {
            new Dictionary<string, object?> { ["BillNo"] = "B-2001", ["Patient"] = "Rahim", ["Gross"] = 3500m, ["Paid"] = 3000m },
            new Dictionary<string, object?> { ["BillNo"] = "B-2002", ["Patient"] = "Karim", ["Gross"] = 4200m, ["Paid"] = 4200m }
        };

        return
        [
            new ExportDataset("Admissions", admissions),
            new ExportDataset("Billing", billings)
        ];
    }

    public byte[] GeneratePrintableSummary(string title, ReportFilter filter)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Content().Column(col =>
                {
                    col.Item().Text(title).FontSize(20).SemiBold();
                    col.Item().Text($"From: {filter.FromDate} To: {filter.ToDate}");
                    col.Item().Text($"Department: {filter.Department ?? "All"}");
                    col.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");
                });
            });
        }).GeneratePdf();
    }
}

