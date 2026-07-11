using System;

public class LotGridModel
{
    public int Id { get; set; }

    public int ProjectId { get; set; }


    // العملية
    public string OperationNumber { get; set; }

    public string OperationName { get; set; }

    // البرنامج والموقع
    public string Program { get; set; }
    public int? ProgramId { get; set; }        // ← add


    public string Daira { get; set; }
    public int? DairaId { get; set; }          // ← add


    public string Commune { get; set; }
    public int? CommuneId { get; set; }        // ← add


    public string Domain { get; set; }
    public int? DomainId { get; set; }         // ← add

    public string Sector { get; set; }
    public int? SectorId { get; set; }         // ← add

    // الحصة
    public int LotNumber { get; set; }

    public string LotName { get; set; }

    // مالي
    public decimal LotBudget { get; set; }

    public decimal RegisteredAmount { get; set; }

    public decimal ConsumedAmount { get; set; }
    public decimal Remaining => RegisteredAmount - ConsumedAmount;

    // متابعة
    public string Contractor { get; set; }

    public int? ExecutionDuration { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? ExpectedEndDate =>
    StartDate.HasValue && ExecutionDuration.HasValue
        ? StartDate.Value.AddDays(ExecutionDuration.Value)
        : (DateTime?)null;

    public int? DaysRemaining =>
    ExpectedEndDate.HasValue
        ? (int)(ExpectedEndDate.Value - DateTime.Today).TotalDays
        : (int?)null;

    public decimal PhysicalProgress { get; set; }
    public decimal FinancialProgress =>
       RegisteredAmount <= 0
           ? 0
           : (ConsumedAmount / RegisteredAmount) * 100;

    // وضعيات
    public string AdministrativeProcedure { get; set; }
    public int? AdministrativeProcedureId { get; set; }         // ← add


    public string SpecialStatus1 { get; set; }
    public int? SpecialStatus1Id { get; set; }         // ← add


    public string SpecialStatus2 { get; set; }
    public int? SpecialStatus2Id { get; set; }         // ← add

    public string SpecialStatus3 { get; set; }
    public int? SpecialStatus3Id { get; set; }         // ← add


    public string ProjectStatus { get; set; }
    public int? ProjectStatusId { get; set; }         // ← add



    // ملاحظات
    public string Notes { get; set; }

    public string UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}