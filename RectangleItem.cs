using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mywpf
{
    public class RectangleItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Color { get; set; }

        public string Image { get; set; }

        public virtual ICollection<ItemTranslation> ItemTranslations { get; set; } = new List<ItemTranslation>();

        [NotMapped]
        public string Name { get; set; }

        [NotMapped]
        public string Description { get; set; }

        [NotMapped]
        public string Category { get; set; }

        [NotMapped]
        public string Availability { get; set; }

        public RectangleItem() { }

        public RectangleItem(RectangleItem other)
        {
            Id = other.Id;
            Price = other.Price;
            Color = other.Color;
            Image = other.Image;
            Name = other.Name;
            Description = other.Description;
            Category = other.Category;
            Availability = other.Availability;
            ItemTranslations = other.ItemTranslations;
        }
    }
}