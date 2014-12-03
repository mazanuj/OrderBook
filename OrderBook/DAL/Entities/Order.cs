namespace OrderBook.DAL.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Order
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Details { get; set; }

        [Required]
        public String Name { get; set; }

        [Required]
        public String Phone { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}