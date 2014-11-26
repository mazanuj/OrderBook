using System;

namespace OrderBook.DAL.BusinessModels
{
    public class SettingsBusinessModel
    {
        public Guid Id { get; set; }
        public Double Width { get; set; }
        public Double Height { get; set; }
        public Double Top { get; set; }
        public Double Left { get; set; }
        public Int32 PhoneWidth { get; set; }
        public Int32 NameWidth { get; set; }
        public Int32 DetailsWidth { get; set; }
    }
}