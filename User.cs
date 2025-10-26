using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static mywpf.AppDbContext;

namespace mywpf
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }
        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<RectangleItem> PurchasedProducts { get; set; } = new List<RectangleItem>();
        public virtual ICollection<OrderHistory> OrderHistory { get; set; } = new List<OrderHistory>();

        public User()
        {
            Id = Guid.NewGuid();
        }
    }

}

//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace mywpf
//{
//    public class User
//    {
//        [Key]
//        public Guid Id { get; set; } = Guid.NewGuid();

//        [Required]
//        [StringLength(50)]
//        public string Username { get; set; }

//        [Required]
//        [StringLength(100)]
//        public string Password { get; set; }
//        public bool IsAdmin { get; set; }

//        public virtual ICollection<RectangleItem> PurchasedProducts { get; set; } = new List<RectangleItem>();
//    }
//}