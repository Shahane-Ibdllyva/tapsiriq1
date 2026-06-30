namespace Application.DTOs.Exam
{
    public class UpdateExamDto
    {
        public int ExamId { get; set; }
        public DateTime ExamDate { get; set; }
        public int? Grade { get; set; }
        public int? StudentId { get; set; }
        public int? SubjectId { get; set; }
    }
}