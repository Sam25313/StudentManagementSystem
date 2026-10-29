namespace StudentManagementSystem.Models
{
    public class SchoolEnums
    {

        public enum StudentType
        {
            DayScholar = 1,
            Boarder = 2
        }

        public enum StudentStatus
        {
            Active = 1,
            Inactive = 2,
            Withdrawn = 3
        }


        public enum Gender
        {
            Male = 1,
            Female = 2,
            NotSpecified = 3
        }
    }
}