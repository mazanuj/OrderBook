using OrderBook.DAL.BusinessModels;
using OrderBook.DAL.Entities;

namespace OrderBook.DAL.Mappers
{
    internal class SettingsMapper : IMapper<Setting, SettingsBusinessModel>
    {
        public Setting Map(SettingsBusinessModel source)
        {
            if (source == null)
            {
                return null;
            }

            var destination = new Setting
            {
                Id = source.Id,
                Height = source.Height,
                Left = source.Left,
                Top = source.Top,
                Width = source.Width,
                DetailsWidth = source.DetailsWidth,
                NameWidth = source.NameWidth,
                PhoneWidth = source.PhoneWidth,
                DateWidth = source.DateWidth
            };

            return destination;
        }

        public SettingsBusinessModel Map(Setting source)
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
                NameWidth = source.NameWidth,
                DateWidth = source.DateWidth
            };

            return destination;
        }
    }
}