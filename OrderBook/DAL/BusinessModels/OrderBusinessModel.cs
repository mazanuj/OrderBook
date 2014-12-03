namespace OrderBook.DAL.BusinessModels
{
    using System;
    using System.ComponentModel;
    using Entities;

    public class OrderBusinessModel : INotifyPropertyChanged
    {
        private Status status;

        public Guid Id { get; set; }

        public String Details { get; set; }

        public String Name { get; set; }

        public String Phone { get; set; }

        public DateTime Date { get; set; }

        public Status Status
        {
            get { return status; }
            set
            {
                if (status == value)
                    return;
                status = value;
                if (PropertyChanged != null)
                    SendPropertyChanged("Status");
            }
        }

        private void SendPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null) return;
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}