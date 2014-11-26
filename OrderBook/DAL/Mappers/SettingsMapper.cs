using OrderBook.DAL.BusinessModels;
using OrderBook.DAL.Entities;

namespace OrderBook.DAL.Mappers
{
    internal class SettingsMapper : IMapper<Settings, SettingsBusinessModel>
    {
        public Settings Map(SettingsBusinessModel source)
        {
            if (source == null)
            {
                return null;
            }

            var destination = new Settings
            {
                Id = source.Id,
                Height = source.Height,
                Left = source.Left,
                Top = source.Top,
                Width = source.Width,
                DetailsWidth = source.DetailsWidth,
                NameWidth = source.NameWidth,
                PhoneWidth = source.PhoneWidth
            };

            return destination;
        }

        public SettingsBusinessModel Map(Settings source)
        {
            if (source == null)
            {
                return null;
            }

            var destination = new SettingsBusinessModel
            {
                Id = source.Id,
                Height = source.Height,
                Left = source.Left,
                Top = source.Top,
                Width = source.Width,
                PhoneWidth = source.PhoneWidth,
                DetailsWidth = source.DetailsWidth,
                NameWidth = source.NameWidth
            };

            return destination;
        }
    }
}