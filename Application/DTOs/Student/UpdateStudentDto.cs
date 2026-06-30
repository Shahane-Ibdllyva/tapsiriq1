namespace Application.DTOs.Student
{
    public class UpdateStudentDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int? ClassNumber { get; set; }
    }
}