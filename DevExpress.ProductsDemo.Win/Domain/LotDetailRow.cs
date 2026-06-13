namespace DevExpress.ProductsDemo.Win.UI
{
    public class LotDetailRow
    {
        public string Field { get; set; }
        public string Value { get; set; }

        public LotDetailRow(string field, string value)
        {
            Field = field;
            Value = value;
        }
    }
}