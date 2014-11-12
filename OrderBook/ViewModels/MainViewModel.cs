namespace OrderBook.ViewModels
{
    using Caliburn.Micro;
    using OrderBook.DAL.Context;
    using OrderBook.DAL.Models;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    [Export(typeof(MainViewModel))]
    public class MainViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;

        public ObservableCollection<Order> OrderCollection { get; set; }

        private OrderContext db;

        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;

            using (db = new OrderContext())
            {
                OrderCollection = new ObservableCollection<Order>();

                foreach (var order in db.Orders)
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
            OrderCollection.Clear();

            using (db = new OrderContext())
            {
                foreach (var order in this.db.Orders)
                {
                    OrderCollection.Add(order);
                }
            }
        }
    }
}
