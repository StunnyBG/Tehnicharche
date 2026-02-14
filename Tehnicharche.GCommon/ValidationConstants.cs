
namespace Tehnicharche.GCommon
{
    public static class ValidationConstants
    {
        public static class Category
        {
            public const int NameMaxLength = 100;
        }

        public static class City
        {
            public const int NameMaxLength = 50;
        }

        public static class Region
        {
            public const int NameMaxLength = 50;
        }

        public static class Listing
        {
            // Title
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 200;

            // Description
            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 2000;

            // Price
            public const string PriceColumnType = "decimal(9,2)";

            // Image URL
            public const int ImageUrlMaxLength = 2048;

        }
    }
}
