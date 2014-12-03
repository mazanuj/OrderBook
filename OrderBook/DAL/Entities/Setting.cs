using System;
using System.ComponentModel.DataAnnotations;

namespace OrderBook.DAL.Entities
{
    public class Setting
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Double Width { get; set; }

        [Required]
        public Double Height { get; set; }

        [Required]
        public Double Top { get; set; }

        [Required]
        public Double Left { get; set; }

        [Required]
        public Int32 PhoneWidth { get; set; }

        [Required]
        public Int32 NameWidth { get; set; }

        [Required]
        public Int32 DetailsWidth { get; set; }

        [Required]
        public Int32 DateWidth { get; set; }
    }
}