namespace Application.DTOs.Student
{
    public class StudentDto
    {
        public int StudentNumber { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int? ClassNumber { get; set; }
    }
}