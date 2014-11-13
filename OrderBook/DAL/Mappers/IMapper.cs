namespace OrderBook.DAL.Mappers
{
    public interface IMapper<TSource, TDestination>
    {
        TSource Map(TDestination source);

        TDestination Map(TSource source);
    }
}
