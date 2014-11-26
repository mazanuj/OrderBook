using System.Data.Entity;
using OrderBook.DAL.Entities;

namespace OrderBook.DAL.Context
{
    public class SettingsContext : DbContext
    {
        public DbSet<Settings> Settings { get; set; }
    }
}