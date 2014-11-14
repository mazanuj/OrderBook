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
        private readonly OrderMapper mapper = new OrderMapper();
        private readonly IWindowManager windowManager;
        private string textBoxSearch;
        public ObservableCollection<OrderBusinessModel> OrderCollection { get; set; }
        private OrderContext db;

        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            OrderCollection = new ObservableCollection<OrderBusinessModel>();
            AddItemsToCollection();
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
            using (db = new OrderContext())
            {
                foreach (var order in db.Orders)
                {
                    OrderCollection.Add(mapper.Map(order));
                }
            }
        }

        private void AddItemsToCollection(string query)
        {
            OrderCollection.Clear();
            using (db = new OrderContext())
            {
                foreach (var order in db.Orders
                    .Where(x => x.Details.Contains(query) ||
                                x.Name.Contains(query) ||
                                x.Phone.Contains(query)))
                {
                    OrderCollection.Add(mapper.Map(order));
                }
            }
        }

        public void AddNewItem()
        {
            ShowConfirmationItemDialog(null);
        }

        public void RemoveItem(OrderBusinessModel orderBusModel)
        {
            using (db = new OrderContext())
            {
                var order = mapper.Map(orderBusModel);
                db.Entry(order).State = EntityState.Deleted;
                db.SaveChanges();
            }

            RefreshList();
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
            using (db = new OrderContext())
            {
                orderBusModel.Status = status;
                var order = mapper.Map(orderBusModel);
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private void ShowConfirmationItemDialog(OrderBusinessModel currentOrderBusModel)
        {
            var confirmationViewModel = new AddChangeOrderViewModel(currentOrderBusModel);
            windowManager.ShowDialog(confirmationViewModel);

            if (confirmationViewModel.IsOkay)
            {
                RefreshList();
            }
        }

        private void RefreshList()
        {
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
    }
}