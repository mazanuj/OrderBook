namespace OrderBook.ViewModels
{
    using Caliburn.Micro;
    using OrderBook.DAL.BusinessModels;
    using OrderBook.DAL.Context;
    using OrderBook.DAL.Entities;
    using OrderBook.DAL.Mappers;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Data.Entity;

    [Export(typeof(MainViewModel))]
    public class MainViewModel : PropertyChangedBase
    {
        private readonly OrderMapper mapper = new OrderMapper();
        private readonly IWindowManager windowManager;

        public ObservableCollection<OrderBusinessModel> OrderCollection { get; set; }

        private OrderContext db;

        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;

            using (db = new OrderContext())
            {
                OrderCollection = new ObservableCollection<OrderBusinessModel>();

                foreach (var order in db.Orders)
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
                var order = this.mapper.Map(orderBusModel);
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private void ShowConfirmationItemDialog(OrderBusinessModel currentOrderBusModel)
        {
            var confirmationViewModel = new AddChangeOrderViewModel(currentOrderBusModel);

            this.windowManager.ShowDialog(confirmationViewModel);

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
                    OrderCollection.Add(mapper.Map(order));
                }
            }
        }
    }
}
