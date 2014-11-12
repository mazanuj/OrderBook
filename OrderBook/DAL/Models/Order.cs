namespace OrderBook.DAL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        //[Required]
        //[StringLength(100)]
        public String Details { get; set; }

        [Required]
        [StringLength(100)]
        public String Name { get; set; }

        [Required]
        [StringLength(30)]
        public String Phone { get; set; }

        [Required]
        public Status Status { get; set; }
    }
}
