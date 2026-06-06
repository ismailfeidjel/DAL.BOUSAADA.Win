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

        public bool HasLots { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

      

     
    }
}