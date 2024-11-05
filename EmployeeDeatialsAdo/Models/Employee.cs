namespace EmployeeDeatialsAdo.Models
{
    public class Employee
    {
        public int id { get; set; }

        public string? fName { get; set; }

        public string? lName { get; set; }

        public int? age { get; set; }

        public string? email { get; set; }

        public decimal? salary { get; set; }

        public string? phone { get; set; }

        public string? country { get; set; }

        public string? state { get; set; }

        public string? gender { get; set; }

        public List<EmployeeDetails> EmployeeDetails { get; set; }
    }


    public class EmployeeDetails
    {
        public int id { get; set; }

        public string? city { get; set; }

        public int? experience { get; set; }

        public string? jobTitle { get; set; }
    }

}