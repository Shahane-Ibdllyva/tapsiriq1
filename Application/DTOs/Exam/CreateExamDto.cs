namespace Application.DTOs.Exam
{
    public class CreateExamDto
    {
        public int SubjectId { get; set; }
        public int StudentId { get; set; } 
        public DateTime ExamDate { get; set; }
        public int? Grade { get; set; }
    }
}