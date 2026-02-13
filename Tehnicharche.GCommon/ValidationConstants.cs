
namespace Tehnicharche.GCommon
{
    public static class ValidationConstants
    {
        public static class Category
        {
            public const int NameMaxLength = 100;
        }

        public static class Post
        {
            // Title
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 200;

            // Description
            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 1000;

            // Image URL
            public const int ImageUrlMaxLength = 500;

        }
    }
}
