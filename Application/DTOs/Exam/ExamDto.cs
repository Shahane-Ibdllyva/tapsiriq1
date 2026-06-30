namespace Application.DTOs.Exam
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public int StudentNumber { get; set; }
        public DateTime ExamDate { get; set; }
        public int? Grade { get; set; }

        public string SubjectName { get; set; } = null!;
    }
}