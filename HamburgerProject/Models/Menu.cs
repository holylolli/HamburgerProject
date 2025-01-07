using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HamburgerProject.Models
{
    public class Menu
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(250)]
        [NotMapped]
        public IFormFile Photo { get; set; }

        public string? ImagePath { get; set; }        

        public ICollection<Order> Orders { get; set; }         

    }
}
