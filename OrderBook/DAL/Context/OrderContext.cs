namespace OrderBook.DAL.Context
{
    using Models;
    using System.Data.Entity;

    public class OrderContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
    }
}