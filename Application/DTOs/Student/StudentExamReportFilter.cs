namespace Application.DTOs.Student
{
    public class StudentExamReportFilter
    {
        public string? StudentFullName { get; set; }
        public string? SubjectName { get; set; }
        public string? TeacherFullName { get; set; }
        public DateTime? ExamDateMin { get; set; }
        public DateTime? ExamDateMax { get; set; }
        public int? GradeMin { get; set; }
        public int? GradeMax { get; set; }
    }
}
