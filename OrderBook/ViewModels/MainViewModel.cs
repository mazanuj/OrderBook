namespace OrderBook.ViewModels
{
    using Caliburn.Micro;
    using OrderBook.DAL.Context;
    using OrderBook.DAL.Models;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    [Export(typeof (MainViewModel))]
    public class MainViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;
        private string textBoxSearch;
        private OrderContext db;
        public ObservableCollection<Order> OrderCollection { get; set; }

        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            OrderCollection = new ObservableCollection<Order>();
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
                    OrderCollection.Add(order);
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
                    OrderCollection.Add(order);
                }
            }
        }

        public void AddNewItem()
        {
            ShowConfirmationItemDialog(null);
        }

        public void RemoveItem(Order order)
        {
            using (db = new OrderContext())
            {
                var ord = db.Orders.SingleOrDefault(x => x.Id == order.Id);

                if (ord != null)
                {
                    db.Orders.Remove(ord);
                    db.SaveChanges();
                }
            }

            RefreshList();
        }

        public void ChangeItem(Order order)
        {
            ShowConfirmationItemDialog(order);
        }

        private void ShowConfirmationItemDialog(Order currentOrder)
        {
            var confirmationViewModel = new AddChangeOrderViewModel(currentOrder);

            _windowManager.ShowDialog(confirmationViewModel);

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