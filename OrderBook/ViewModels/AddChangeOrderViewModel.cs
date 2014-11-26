using System.Text.RegularExpressions;

namespace OrderBook.ViewModels
{
    using Caliburn.Micro;
    using DAL.BusinessModels;
    using DAL.Context;
    using DAL.Entities;
    using DAL.Mappers;
    using System;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Windows;

    [Export(typeof (AddChangeOrderViewModel))]
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
            base.DisplayName = "Add/change order";

            if (currentOrderBusModel == null)
            {
                base.DisplayName = "Add new order";
                return;
            }
            base.DisplayName = "Change order";
            isInEditMode = true;

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

            if (isInEditMode)
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
                        Details = Details,
                        Name = Name,
                        Phone = "38" + Regex.Replace(Phone, @"(^\s*\+?(38)?)?(\(|\)|\s|\-)?", string.Empty),
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
                    Details = Details,
                    Name = Name,
                    Phone = "38" + Regex.Replace(Phone, @"(^\s*\+?(38)?)?(\(|\)|\s|\-)?", string.Empty),
                    Status = currentOrderBusModel.Status
                };

                db.Entry(mapper.Map(orderBusModel)).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}