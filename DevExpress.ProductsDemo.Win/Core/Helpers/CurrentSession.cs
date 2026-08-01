using DevExpress.ProductsDemo.Win.Domain;

namespace DevExpress.ProductsDemo.Win.Core.Helpers
{
    public static class CurrentSession
    {
        public static UserItem User { get; private set; }

        public static void SignIn(UserItem user) => User = user;
        public static void SignOut() => User = null;

        public static bool IsAdmin => User != null && User.Role == UserRoles.Admin;
        public static bool IsManagerOrAbove => User != null &&
            (User.Role == UserRoles.Admin || User.Role == UserRoles.Manager);
        public static bool CanEdit => User != null &&
            (User.Role == UserRoles.Admin || User.Role == UserRoles.Manager || User.Role == UserRoles.DataEntry);
        // Viewer: read-only — CanEdit is false, everything else stays enabled for browsing/printing
    }
}