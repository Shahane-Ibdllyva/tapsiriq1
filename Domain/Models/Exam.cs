namespace Domain.Models
{
    public class Exam : BaseEntity
    {
        public int ExamId { get; set; }

        public int SubjectId { get; set; }

        public int StudentId { get; set; }

        public DateTime ExamDate { get; set; }

        public int? Grade { get; set; }

        // Navigation properties
        public Subject Subject { get; set; } = null!;

        public Student Student { get; set; } = null!;
    }
}