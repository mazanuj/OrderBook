using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using OrderBook.DAL.BusinessModels;
using OrderBook.DAL.Context;
using OrderBook.DAL.Entities;
using OrderBook.DAL.Mappers;

namespace OrderBook.ViewModels
{
    using Caliburn.Micro;
    using System.ComponentModel.Composition;

    [Export(typeof (MainViewModel))]
    public class MainViewModel : PropertyChangedBase
    {
        private readonly IWindowManager windowManager;
        private string textBoxSearch;
        private double width = 800.0;
        private double height = 600.0;
        private double left, top;
        private int detailsWidth = 400;
        private int nameWidth = 100;
        private int phoneWidth = 100;
        private readonly OrderMapper orderMapper = new OrderMapper();
        private readonly SettingsMapper settingsMapper = new SettingsMapper();
        public ObservableCollection<OrderBusinessModel> OrderCollection { get; set; }

        public int PhoneWidth
        {
            get { return phoneWidth; }
            set
            {
                phoneWidth = value;
                NotifyOfPropertyChange(() => PhoneWidth);
            }
        }

        public int NameWidth
        {
            get { return nameWidth; }
            set
            {
                nameWidth = value;
                NotifyOfPropertyChange(() => NameWidth);
            }
        }

        public int DetailsWidth
        {
            get { return detailsWidth; }
            set
            {
                detailsWidth = value;
                NotifyOfPropertyChange(() => DetailsWidth);
            }
        }

        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }

        public double Left
        {
            get { return left; }
            set
            {
                left = value;
                NotifyOfPropertyChange(() => Left);
            }
        }

        public double Top
        {
            get { return top; }
            set
            {
                top = value;
                NotifyOfPropertyChange(() => Top);
            }
        }

        private OrderContext orderDb;
        private SettingsContext settingsDb;

        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            OrderCollection = new ObservableCollection<OrderBusinessModel>();
            AddItemsToCollection();
            LoadSavedSetings();
        }

        private void LoadSavedSetings()
        {
            using (settingsDb = new SettingsContext())
            {
                foreach (var setting in settingsDb.Settings)
                {
                    Width = setting.Width;
                    Height = setting.Height;
                    Top = setting.Top;
                    Left = setting.Left;
                    DetailsWidth = setting.DetailsWidth;
                    PhoneWidth = setting.PhoneWidth;
                    NameWidth = setting.NameWidth;
                }
            }
        }

        public string TextBoxSearch
        {
            get { return textBoxSearch; }
            set
            {
                textBoxSearch = value;
                Search(textBoxSearch);
                NotifyOfPropertyChange(() => TextBoxSearch);
            }
        }

        private void AddItemsToCollection()
        {
            OrderCollection.Clear();
            using (orderDb = new OrderContext())
            {
                var completed = new List<Order>();
                var neutral = new List<Order>();
                var uncompleted = new List<Order>();

                foreach (var order in orderDb.Orders)
                {
                    switch (order.Status)
                    {
                        case Status.Neutral:
                            neutral.Add(order);
                            break;
                        case Status.Completed:
                            completed.Add(order);
                            break;
                        case Status.Uncompleted:
                            uncompleted.Add(order);
                            break;
                    }
                }

                foreach (var order in uncompleted)
                    OrderCollection.Add(orderMapper.Map(order));
                foreach (var order in neutral)
                    OrderCollection.Add(orderMapper.Map(order));
                foreach (var order in completed)
                    OrderCollection.Add(orderMapper.Map(order));
            }
        }

        private void AddItemsToCollection(string query)
        {
            OrderCollection.Clear();
            using (orderDb = new OrderContext())
            {
                var completed = new List<Order>();
                var neutral = new List<Order>();
                var uncompleted = new List<Order>();

                foreach (var order in orderDb.Orders
                    .Where(x => x.Details.Contains(query) ||
                                x.Name.Contains(query) ||
                                x.Phone.Contains(query)))
                {
                    switch (order.Status)
                    {
                        case Status.Neutral:
                            neutral.Add(order);
                            break;
                        case Status.Completed:
                            completed.Add(order);
                            break;
                        case Status.Uncompleted:
                            uncompleted.Add(order);
                            break;
                    }
                }

                foreach (var order in uncompleted)
                    OrderCollection.Add(orderMapper.Map(order));
                foreach (var order in neutral)
                    OrderCollection.Add(orderMapper.Map(order));
                foreach (var order in completed)
                    OrderCollection.Add(orderMapper.Map(order));
            }
        }

        public void AddNewItem()
        {
            ShowConfirmationItemDialog(null);
        }

        public void RemoveItem(OrderBusinessModel orderBusModel)
        {
            using (orderDb = new OrderContext())
            {
                orderDb.Entry(orderMapper.Map(orderBusModel)).State = EntityState.Deleted;
                orderDb.SaveChanges();
            }

            AddItemsToCollection();
        }

        public void ChangeItem(OrderBusinessModel orderBusModel)
        {
            ShowConfirmationItemDialog(orderBusModel);
        }

        public void SetToCompleted(OrderBusinessModel orderBusModel)
        {
            ChangeOrderStatus(orderBusModel, Status.Completed);
        }

        public void SetToUncompleted(OrderBusinessModel orderBusModel)
        {
            ChangeOrderStatus(orderBusModel, Status.Uncompleted);
        }

        private void ChangeOrderStatus(OrderBusinessModel orderBusModel, Status status)
        {
            using (orderDb = new OrderContext())
            {
                orderBusModel.Status = status;
                var order = orderMapper.Map(orderBusModel);
                orderDb.Entry(order).State = EntityState.Modified;
                orderDb.SaveChanges();
            }
            AddItemsToCollection();
        }

        private void ShowConfirmationItemDialog(OrderBusinessModel currentOrderBusModel)
        {
            var confirmationViewModel = new AddChangeOrderViewModel(currentOrderBusModel);
            windowManager.ShowDialog(confirmationViewModel);

            if (confirmationViewModel.IsOkay)
                AddItemsToCollection();
        }

        private void Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText) ||
                string.IsNullOrWhiteSpace(searchText))
            {
                AddItemsToCollection();
                return;
            }

            AddItemsToCollection(searchText);
        }

        public void OnClose()
        {
            using (settingsDb = new SettingsContext())
            {
                if (settingsDb.Settings.Count() != 0)
                    foreach (var setting in settingsDb.Settings)
                        settingsDb.Settings.Remove(setting);

                var settingsBusModel = new SettingsBusinessModel
                {
                    DetailsWidth = DetailsWidth,
                    PhoneWidth = PhoneWidth,
                    NameWidth = NameWidth,
                    /* = */
                    /* = */
                    Height = Height,
                    Width = Width,
                    Left = Left,
                    Top = Top,
                    Id = Guid.NewGuid()
                };

                settingsDb.Settings.Add(settingsMapper.Map(settingsBusModel));
                settingsDb.SaveChanges();
            }
        }
    }
}