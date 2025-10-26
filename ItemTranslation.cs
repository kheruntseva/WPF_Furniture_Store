using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mywpf
{
    public class ItemTranslation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid RectangleItemId { get; set; }

        [Required, MaxLength(2)]
        public string LanguageCode { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        [MaxLength(50)]
        public string Availability { get; set; }

        [ForeignKey("RectangleItemId")]
        public RectangleItem RectangleItem { get; set; }
    }
}