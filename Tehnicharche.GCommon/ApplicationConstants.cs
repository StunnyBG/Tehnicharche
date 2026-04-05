namespace Tehnicharche.GCommon
{
    public static class ApplicationConstants
    {
        // Date
        public const string DateFormat = "dd-MM-yyyy";

        // Pagination
        public const int DefaultPage = 1;
        public const int IndexPageSize = 6;
        public const int MyListingsPageSize = 3;
        public const int AdminPageSize = 10;

        // Dashboard
        public const int RecentListingsCount = 6;
        public const int RecentMessagesCount = 5;


        // Areas
        public const string AdminArea = "Admin";

        // Identity roles
        public const string AdminRole = "Administrator";
        public const string UserRole = "User";

        // Custom claims
        public const string BannedClaimType = "Banned";
        public const string BannedClaimValue = "true";

        // Memory-cache keys
        public const string CategoriesCacheKey = "Categories:All";
        public const string RegionsCacheKey = "Regions:All";
        public const string CitiesCacheKey = "Cities:All";
    }
}