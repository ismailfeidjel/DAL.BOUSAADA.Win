namespace DevExpress.ProductsDemo.Win.Domain
{
    /// <summary>
    /// Generic Id/Name pair used by all lookup tables.
    /// </summary>
    public class LookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public LookupItem() { }
        public LookupItem(int id, string name) { Id = id; Name = name; }

        public override string ToString() => Name;
    }
}