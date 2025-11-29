using System.ComponentModel.DataAnnotations;

namespace UEFASwissFormatSelector.Models
{
    public class Identifiable
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Key]
        public Guid Id { get; set; }
        public Identifiable()
        {
            Id = Guid.NewGuid();
        }
    }
}
