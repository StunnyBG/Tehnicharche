
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
            public const int TitleMaxLength = 100;

            // Description
            public const int DescriptionMaxLength = 2000;

            // Price
            public const string PriceColumnType = "decimal(9,2)";
            public const double PriceMinValue = 0.01;
            public const double PriceMaxValue = 9999999.99;

            // Image URL
            public const int ImageUrlMaxLength = 2048;

        }
    }
}
