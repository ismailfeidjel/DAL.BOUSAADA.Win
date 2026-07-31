namespace DevExpress.ProductsDemo.Win.Domain
{
    public class CommuneItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DairaId { get; set; }

        public CommuneItem() { }

        public CommuneItem(int id, string name, int dairaId)
        {
            Id = id;
            Name = name;
            DairaId = dairaId;
        }
    }
}