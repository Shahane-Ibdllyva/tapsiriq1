namespace Application.DTOs.Student
{
    public class CreateStudentDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int? ClassNumber { get; set; }
    }
}