using System.Collections.ObjectModel;
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
        private readonly bool isInEditMode;
        private string details;
        private string name;
        private string phone;
        private readonly OrderMapper mapper = new OrderMapper();
        private readonly OrderBusinessModel currentOrderBusModel;
        private ObservableCollection<OrderBusinessModel> businessModels = new ObservableCollection<OrderBusinessModel>();

        public string Details { get; set; }
        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsOkay { get; set; }

        [ImportingConstructor]
        public AddChangeOrderViewModel(OrderBusinessModel currentOrderBusModel,
            ObservableCollection<OrderBusinessModel> busModel)
        {
            businessModels = busModel;
            base.DisplayName = "Добавление (Редактирование) записи";

            if (currentOrderBusModel == null)
            {
                base.DisplayName = "Добавление записи";
                return;
            }

            base.DisplayName = "Редактирование записи";
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
                businessModels.Insert(0, new OrderBusinessModel
                {
                    Id = Guid.NewGuid(),
                    Details = Details,
                    Name = Name,
                    Phone = Phone, //"38" + Regex.Replace(Phone, @"(^\s*\+?(38)?)?(\(|\)|\s|\-)?", string.Empty),
                    Status = Status.Neutral,
                    Date = DateTime.Now
                });

                //using (var db = new OrderContext())
                //{
                //    var orderBusModel = new OrderBusinessModel
                //    {
                //        Id = Guid.NewGuid(),
                //        Date = DateTime.Now,
                //        Details = Details,
                //        Name = Name,
                //        Phone = Phone, //"38" + Regex.Replace(Phone, @"(^\s*\+?(38)?)?(\(|\)|\s|\-)?", string.Empty),
                //        Status = Status.Neutral
                //    };

                //    db.Orders.Add(mapper.Map(orderBusModel));
                //    db.SaveChanges();
                //}
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
            return true;
            //return !string.IsNullOrEmpty(Details) && !string.IsNullOrEmpty(Name) &&
            //       !string.IsNullOrEmpty(Phone) && Regex.IsMatch(Phone, @"^(\d|\-|\(|\)|\+|\s)*$");
        }

        private void ChangeCurrentOrder()
        {
            //businessModels[businessModels.IndexOf(currentOrderBusModel)] = new OrderBusinessModel
            //{
            //    Id = currentOrderBusModel.Id,
            //    Details = Details,
            //    Name = Name,
            //    Phone = Phone,
            //    Status = currentOrderBusModel.Status,
            //    Date = currentOrderBusModel.Date
            //};

            businessModels.Insert(businessModels.IndexOf(currentOrderBusModel), new OrderBusinessModel
            {
                Id = Guid.NewGuid(),
                Details = Details,
                Name = Name,
                Phone = Phone, //"38" + Regex.Replace(Phone, @"(^\s*\+?(38)?)?(\(|\)|\s|\-)?", string.Empty),
                Status = currentOrderBusModel.Status,
                Date = currentOrderBusModel.Date
            });
        }
    }
}