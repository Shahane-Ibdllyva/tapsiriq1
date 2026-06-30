using System.Collections.Generic;

namespace Domain.Models
{
    public class Subject : BaseEntity
    {
        public int SubjectId { get; set; }

        public string? SubjectCode { get; set; }

        public string SubjectName { get; set; } = null!;

        public int ClassNumber { get; set; }

        public string TeacherName { get; set; } = null!;

        public string TeacherSurname { get; set; } = null!;

        // BU SƏTİR MÜTLƏQ OLMALIDIR (İmtahanlar ilə əlaqə):
        //public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}