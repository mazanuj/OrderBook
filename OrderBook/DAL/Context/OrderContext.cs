namespace OrderBook.DAL.Context
{
    using OrderBook.DAL.Models;
    using System.Data.Entity;

    public class OrderContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
    }
}
