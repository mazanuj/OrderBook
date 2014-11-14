namespace OrderBook.DAL.Mappers
{
    using OrderBook.DAL.BusinessModels;
    using OrderBook.DAL.Entities;

    public class OrderMapper : IMapper<Order, OrderBusinessModel>
    {
        public Order Map(OrderBusinessModel source)
        {
            if (source == null)
            {
                return null;
            }

            var destination = new Order 
            { 
                Id = source.Id,
                Details = source.Details,
                Name = source.Name,
                Phone = source.Phone,
                Status = source.Status
            };

            return destination;
        }

        public OrderBusinessModel Map(Order source)
        {
            if (source == null)
            {
                return null;
            }
            
            var destination = new OrderBusinessModel
            {
                Id = source.Id,
                Details = source.Details,
                Name = source.Name,
                Phone = source.Phone,
                Status = source.Status
            };
            
            return destination;
        }
    }
}
