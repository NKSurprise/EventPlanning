namespace EventPlanningAndManagementSystem.EventPlanningAndManagementSystem.GCommon
{
    public static  class ValidationConstatnts
    {
        public static class Event
        {
            public const int NameMaxLength = 100;
            public const int NameMinLength = 3;

            public const int DescriptionMaxLength = 1000;
            public const int DescriptionMinLength = 10;

            public const int PublishedOnLength = 10;
            public const string PublishedOnFormat = "dd-MM-yyyy";
        }

        public static class Location
        {
            public const int NameMaxLength = 100;
            public const int NameMinLength = 3;
        }
    }
}
