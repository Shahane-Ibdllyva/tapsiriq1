namespace Application.DTOs.Subject
{
    public class SubjectDto
    {
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int? ClassNumber { get; set; }
        public string TeacherName { get; set; } = null!;
        public string TeacherSurname { get; set; } = null!;
    }
}