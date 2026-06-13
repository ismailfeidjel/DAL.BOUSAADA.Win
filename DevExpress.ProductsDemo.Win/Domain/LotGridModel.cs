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

    public string Daira { get; set; }

    public string Commune { get; set; }

    public string Domain { get; set; }

    public string Sector { get; set; }

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

    public decimal PhysicalProgress { get; set; }
    public decimal FinancialProgress =>
       RegisteredAmount <= 0
           ? 0
           : (ConsumedAmount / RegisteredAmount) * 100;

    // وضعيات
    public string AdministrativeProcedure { get; set; }

    public string SpecialStatus1 { get; set; }

    public string SpecialStatus2 { get; set; }

    public string SpecialStatus3 { get; set; }

    public string ProjectStatus { get; set; }


    // ملاحظات
    public string Notes { get; set; }
}