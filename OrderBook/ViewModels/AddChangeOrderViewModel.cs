namespace OrderBook.ViewModels
{
    using System;

    using Caliburn.Micro;
    using OrderBook.DAL.Context;
    using OrderBook.DAL.Models;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;

    [Export(typeof(AddChangeOrderViewModel))]
    public class AddChangeOrderViewModel : Screen
    {
        private readonly bool _isInEditMode;
        private readonly Order _currentOrder;

        public string Details { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        //public Status Status { get; set; }

        public bool IsOkay { get; set; }

        [ImportingConstructor]
        public AddChangeOrderViewModel(Order currentOrder)
        {
            if (currentOrder == null) return;
            _isInEditMode = true;

            _currentOrder = currentOrder;

            Details = currentOrder.Details;
            Name = currentOrder.Name;
            Phone = currentOrder.Phone;
            //Status = currentOrder.Status;
        }

        public void Save()
        {
            if (!CheckIfFieldsAreFilled())
            {
                MessageBox.Show("Not all fields are filled");
                return;
            }

            if (_isInEditMode)
            {
                ChangeCurrentOrder();
            }
            else
            {
                using (var db = new OrderContext())
                {
                    db.Orders.Add(
                        new Order
                            {
                                Id = Guid.NewGuid(),
                                Details = this.Details,
                                Name = this.Name,
                                Phone = this.Phone,
                                Status = Status.Neutral
                            });
                    db.SaveChanges();
                }
            }

            IsOkay = true;
            TryClose();

        }

        public void Cancel()
        {
            TryClose();
        }

        private bool CheckIfFieldsAreFilled()
        {
            return !string.IsNullOrEmpty(Details) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Phone);
            //&& !string.IsNullOrEmpty(Status);
        }

        private void ChangeCurrentOrder()
        {
            using (var db = new OrderContext())
            {
                var order = db.Orders.SingleOrDefault(x => x.Id == this._currentOrder.Id);
                if (order != null)
                {
                    order.Details = Details;
                    order.Name = Name;
                    order.Phone = Phone;
                    //order.Status = Status;
                }
                db.SaveChanges();
            }
        }
    }
}
