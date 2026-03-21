
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

        public static class ContactForm
        {
            // Name
            public const int NameMaxLength = 100;

            // Email
            public const int EmailMaxLength = 256;

            // Phone number
            public const int PhoneNumberMaxLength = 20;

            // Subject
            public const int SubjectMaxLength = 150;

            // Message
            public const int MessageMaxLength = 2000;
            public const int MessageMinLength = 10;
        }
    }
}
