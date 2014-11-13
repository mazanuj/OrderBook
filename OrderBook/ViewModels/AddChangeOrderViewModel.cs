namespace OrderBook.ViewModels
{
    using Caliburn.Micro;
    using OrderBook.DAL.BusinessModels;
    using OrderBook.DAL.Context;
    using OrderBook.DAL.Entities;
    using OrderBook.DAL.Mappers;
    using System;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Windows;

    [Export(typeof(AddChangeOrderViewModel))]
    public class AddChangeOrderViewModel : Screen
    {
        private readonly OrderMapper mapper = new OrderMapper();
        private readonly bool isInEditMode;
        private readonly OrderBusinessModel currentOrderBusModel;

        public string Details { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public bool IsOkay { get; set; }

        [ImportingConstructor]
        public AddChangeOrderViewModel(OrderBusinessModel currentOrderBusModel)
        {
            if (currentOrderBusModel == null) return;
            this.isInEditMode = true;

            this.currentOrderBusModel = currentOrderBusModel;

            Details = this.currentOrderBusModel.Details;
            Name = this.currentOrderBusModel.Name;
            Phone = this.currentOrderBusModel.Phone;
        }

        public void Save()
        {
            if (!CheckIfFieldsAreFilled())
            {
                MessageBox.Show("Not all fields are filled");
                return;
            }

            if (this.isInEditMode)
            {
                ChangeCurrentOrder();
            }
            else
            {
                using (var db = new OrderContext())
                {
                    var orderBusModel = new OrderBusinessModel
                    {
                        Id = Guid.NewGuid(),
                        Details = this.Details,
                        Name = this.Name,
                        Phone = this.Phone,
                        Status = Status.Neutral
                    };

                    db.Orders.Add(mapper.Map(orderBusModel));
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
        }

        private void ChangeCurrentOrder()
        {
            using (var db = new OrderContext())
            {
                var orderBusModel = new OrderBusinessModel
                {
                    Id = currentOrderBusModel.Id,
                    Details = this.Details,
                    Name = this.Name,
                    Phone = this.Phone,
                    Status = currentOrderBusModel.Status
                };

                db.Entry(mapper.Map(orderBusModel)).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
