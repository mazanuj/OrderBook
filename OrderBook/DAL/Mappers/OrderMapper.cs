namespace OrderBook.DAL.Mappers
{
    using BusinessModels;
    using Entities;

    public class OrderMapper : IMapper<Order, OrderBusinessModel>
    {
        public Order Map(OrderBusinessModel source)
        {
            if (source == null)
            {
                return null;
            }

            return new Order
            {
                Id = source.Id,
                Details = string.IsNullOrEmpty(source.Details) ? "<>" : source.Details,
                Name = string.IsNullOrEmpty(source.Name) ? "<>" : source.Name,
                Phone = string.IsNullOrEmpty(source.Phone) ? "<>" : source.Phone,
                Status = source.Status,
                Date = source.Date
            };
        }

        public OrderBusinessModel Map(Order source)
        {
            if (source == null)
            {
                return null;
            }

            return new OrderBusinessModel
            {
                Id = source.Id,
                Details = source.Details == "<>" ? string.Empty : source.Details,
                Name = source.Name == "<>" ? string.Empty : source.Name,
                Phone = source.Phone == "<>" ? string.Empty : source.Phone,
                Status = source.Status,
                Date = source.Date
            };
        }
    }
}