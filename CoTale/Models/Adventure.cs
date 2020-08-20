using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace CoTale.Models
{
    public class Adventure
    {
        public int ID { get; set; }

        
        [MaxLength(118)]
        [MinLength(1)]
        [Required]
        public string Title { get; set; }
        //public DateTime ReleaseDate { get; set; }
        public int ParentId { get; set; }
        public int Price { get; set; }
        public int Order { get; set; }
    }
    public class AdventureDBContext : DbContext
    {
        public DbSet<Adventure> Adventures { get; set; }
    }
}