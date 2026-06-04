using System;

namespace DevExpress.ProductsDemo.Win.Domain
{
    public class Project
    {
        public int Id { get; set; }

        public string OperationNumber { get; set; }

        public string OperationName { get; set; }

        public int ProgramId { get; set; }

        public int DairaId { get; set; }

        public int CommuneId { get; set; }

        public int DomainId { get; set; }

        public int SectorId { get; set; }

        public decimal TotalBudget { get; set; }

        public decimal RegisteredAmount { get; set; }

        public decimal ConsumedAmount { get; set; }

        public bool HasLots { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public decimal RemainingAmount
        {
            get { return RegisteredAmount - ConsumedAmount; }
        }

        public decimal FinancialProgress
        {
            get
            {
                if (RegisteredAmount <= 0)
                    return 0;

                return (ConsumedAmount / RegisteredAmount) * 100;
            }
        }
    }
}