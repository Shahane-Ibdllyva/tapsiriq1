using System;
using Domain.Models; 

namespace tapsiriq1.Domain.Entities
{
    public class AppFile : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public byte[] Content { get; set; } = null!;     
        public string OriginalName { get; set; } = null!; 
        public string Extension { get; set; } = null!;    
        public string ContentType { get; set; } = null!;  
        public long Size { get; set; }                   
    }
}