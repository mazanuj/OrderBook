namespace OrderBook.DAL.Context
{
    using System.Data.Entity;
    using Entities;

    public class OrderContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
    }
}