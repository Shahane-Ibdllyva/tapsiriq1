using System;

namespace Domain.Models.View
{
    public class StudentExamReport
    {
        public int ExamId { get; set; }
        public string StudentFullName { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public string TeacherFullName { get; set; } = null!;
        public DateTime ExamDate { get; set; }
        public int Grade { get; set; }
    }
}