namespace DevExpress.ProductsDemo.Win.Domain
{
    public static class UserRoles
    {
        public const string Admin = "admin";
        public const string Manager = "manager";
        public const string DataEntry = "data_entry";
        public const string Viewer = "viewer";

        public static readonly string[] All = { Admin, Manager, DataEntry, Viewer };
    }

    public class UserItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }        // one of UserRoles.*
        public bool IsActive { get; set; }

        // Transient only — used when creating/resetting a password, never read back from DB as-is
        public string PlainPassword { get; set; }
    }
}