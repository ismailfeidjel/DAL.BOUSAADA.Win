namespace DevExpress.ProductsDemo.Win.Domain
{
    public class ProjectGridModel
    {
        public int Id { get; set; }

        public string OperationNumber { get; set; }

        public string OperationName { get; set; }

        public string Daira { get; set; }

        public string Commune { get; set; }

        public string Sector { get; set; }

        public int LotsCount { get; set; }

        public decimal TotalBudget { get; set; }

        public decimal RegisteredAmount { get; set; }

        public decimal ConsumedAmount { get; set; }

        public decimal PhysicalProgress { get; set; }
    }
}