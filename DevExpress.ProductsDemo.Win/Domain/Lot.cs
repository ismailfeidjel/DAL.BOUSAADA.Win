using System;

namespace DevExpress.ProductsDemo.Win.Domain
{
    public class Lot
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public int LotNumber { get; set; }

        public string LotName { get; set; }

        public string Contractor { get; set; }

        public int? ExecutionDuration { get; set; }

        public DateTime? StartDate { get; set; }

        public decimal PhysicalProgress { get; set; }

        public int? AdministrativeProcedureId { get; set; }

        public int? SpecialStatus1Id { get; set; }

        public int? SpecialStatus2Id { get; set; }

        public int? SpecialStatus3Id { get; set; }

        public int? ProjectStatusId { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}