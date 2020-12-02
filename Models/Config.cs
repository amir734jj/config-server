using System;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Interfaces;

namespace Models
{
    public class Config : IEntity
    {
        public int Id { get; set; }
        
        [Column(TypeName = "VARCHAR(256)")]
        public string AuthKey { get; set; }
        
        [Column(TypeName = "text")]
        public string Value { get; set; }
        
        public DateTimeOffset LastAccessTime { get; set; }
    }
}