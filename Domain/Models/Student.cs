using System.Collections.Generic;

namespace Domain.Models
{
    public class Student : BaseEntity
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; } = null!;

        public string StudentSurname { get; set; } = null!;

        public int ClassNumber { get; set; }

        // Navigation
    }
}