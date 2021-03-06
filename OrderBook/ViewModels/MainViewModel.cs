﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NPOI.XWPF.UserModel;
using OrderBook.DAL.BusinessModels;
using OrderBook.DAL.Context;
using OrderBook.DAL.Entities;
using OrderBook.DAL.Mappers;

namespace OrderBook.ViewModels
{
    using Caliburn.Micro;
    using System.ComponentModel.Composition;

    [Export(typeof (MainViewModel))]
    public class MainViewModel : PropertyChangedBase
    {
        private readonly IWindowManager windowManager;
        private string textBoxSearch;
        private double width = 800.0;
        private double height = 600.0;
        private double left, top;
        private int dateWidth = 150;
        private int detailsWidth = 400;
        private int nameWidth = 100;
        private int phoneWidth = 100;
        private int selectedNum;
        private bool canButtonPrev;
        private bool canButtonForv;

        private List<OrderBusinessModel> orderBusinessModels;
        private readonly OrderMapper orderMapper = new OrderMapper();
        private readonly SettingsMapper settingsMapper = new SettingsMapper();
        public ObservableCollection<OrderBusinessModel> OrderCollection { get; set; }

        private OrderBusinessModel selected;

        public OrderBusinessModel Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                NotifyOfPropertyChange(() => Selected);
            }
        }

        public int DateWidth
        {
            get { return dateWidth; }
            set
            {
                dateWidth = value;
                NotifyOfPropertyChange(() => DateWidth);
            }
        }

        public int PhoneWidth
        {
            get { return phoneWidth; }
            set
            {
                phoneWidth = value;
                NotifyOfPropertyChange(() => PhoneWidth);
            }
        }

        public int NameWidth
        {
            get { return nameWidth; }
            set
            {
                nameWidth = value;
                NotifyOfPropertyChange(() => NameWidth);
            }
        }

        public int DetailsWidth
        {
            get { return detailsWidth; }
            set
            {
                detailsWidth = value;
                NotifyOfPropertyChange(() => DetailsWidth);
            }
        }

        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }

        public double Left
        {
            get { return left; }
            set
            {
                left = value;
                NotifyOfPropertyChange(() => Left);
            }
        }

        public double Top
        {
            get { return top; }
            set
            {
                top = value;
                NotifyOfPropertyChange(() => Top);
            }
        }

        public bool CanButtonPrev
        {
            get { return canButtonPrev; }
            set
            {
                canButtonPrev = value;
                NotifyOfPropertyChange(() => CanButtonPrev);
            }
        }       

        public bool CanButtonForv
        {
            get { return canButtonForv; }
            set
            {
                canButtonForv = value;
                NotifyOfPropertyChange(() => CanButtonForv);
            }
        }

        private OrderContext orderDb;
        private SettingsContext settingsDb;
        //private ObservableCollection<OrderBusinessModel> originOrder = new ObservableCollection<OrderBusinessModel>();

        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {
            //TransferDb(" ", " ");
            //ReplaceCharacterInDb(" ", " ");
            this.windowManager = windowManager;
            OrderCollection = new ObservableCollection<OrderBusinessModel>();
            LoadSavedSetings();
            AddItemsToCollection();
        }

        private void ReplaceCharacterInDb(string s, string s1)
        {
            using (orderDb = new OrderContext())
            {
                var t = orderDb.Orders.ToList().Select(order => orderMapper.Map(order)).ToList();
                var b = t.Select(x => new OrderBusinessModel
                {
                    Details = x.Details.Replace(s, s1),
                    Phone = x.Phone.Replace(s, s1),
                    Name = x.Name.Replace(s, s1)
                });

                orderDb.Database.Delete();
                orderDb.Orders.AddRange(b.Select(x => orderMapper.Map(x)));
                orderDb.SaveChanges();
            }
        }

        private void TransferDb(string s, string s1)
        {
            using (var stream = File.OpenRead("1.docx"))
            {
                var doc = new XWPFDocument(stream);

                using (orderDb = new OrderContext())
                {
                    foreach (var table in doc.Tables)
                    {
                        for (var row = 0; row < table.NumberOfRows; row++)
                        {
                            var currRow = table.Rows[row];

                            var curCell = currRow.GetCell(0);
                            var details = new StringBuilder();

                            if (curCell.Paragraphs.Count == 1 && curCell.Paragraphs[0].ParagraphText == string.Empty)
                            {
                                details.Append("<>");
                            }
                            else
                            {
                                foreach (var paragraph in curCell.Paragraphs)
                                {
                                    details.AppendLine(paragraph.ParagraphText);
                                }
                            }

                            curCell = currRow.GetCell(1);
                            var name = new StringBuilder();

                            if (curCell.Paragraphs.Count == 1 && curCell.Paragraphs[0].ParagraphText == string.Empty)
                            {
                                name.Append("<>");
                            }
                            else
                            {
                                foreach (var paragraph in curCell.Paragraphs)
                                {
                                    name.AppendLine(paragraph.ParagraphText);
                                }
                            }

                            curCell = currRow.GetCell(2);
                            var phone = new StringBuilder();

                            if (curCell.Paragraphs.Count == 1 && curCell.Paragraphs[0].ParagraphText == string.Empty)
                            {
                                phone.Append("<>");
                            }
                            else
                            {
                                foreach (var paragraph in curCell.Paragraphs)
                                {
                                    phone.AppendLine(paragraph.ParagraphText);
                                }
                            }

                            var orderBusModel = new OrderBusinessModel
                            {
                                Id = Guid.NewGuid(),
                                Date = DateTime.Now,
                                Details = Regex.Replace(details.ToString(), @"(\r\n)+$", string.Empty).Replace(s, s1),
                                Name = Regex.Replace(name.ToString(), @"(\r\n)+$", string.Empty).Replace(s, s1),
                                Phone = Regex.Replace(phone.ToString(), @"(\r\n)+$", string.Empty).Replace(s, s1),
                                Status = Status.Completed
                            };
                            orderDb.Orders.Add(orderMapper.Map(orderBusModel));
                        }
                    }
                    try
                    {
                        orderDb.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationError in dbEx.EntityValidationErrors
                            .SelectMany(validationErrors => validationErrors.ValidationErrors))
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                validationError.ErrorMessage);
                        }
                    }
                }
            }
            ////////////////////////////////////////////////
        }

        private void LoadSavedSetings()
        {
            using (settingsDb = new SettingsContext())
            {
                foreach (var setting in settingsDb.Settings)
                {
                    Width = setting.Width;
                    Height = setting.Height;
                    Top = setting.Top;
                    Left = setting.Left;
                    DetailsWidth = setting.DetailsWidth;
                    PhoneWidth = setting.PhoneWidth;
                    NameWidth = setting.NameWidth;
                    DateWidth = setting.DateWidth;
                }
            }
        }

        public string TextBoxSearch
        {
            get { return textBoxSearch; }
            set
            {
                textBoxSearch = value;
                NotifyOfPropertyChange(() => TextBoxSearch);
                Task.WhenAny(Search());
            }
        }

        public async void ExecuteFilterView(KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;

            e.Handled = false;
            await Search();
        }

        public async Task ButtonForv()
        {
            selectedNum++;
            CanButtonPrev = true;

            if (orderBusinessModels.Count - 1 <= selectedNum)
                CanButtonForv = false;

            await Scroll();
        }

        public async Task ButtonPrev()
        {
            selectedNum--;
            CanButtonForv = true;

            if (selectedNum == 0)
                CanButtonPrev = false;
            
            await Scroll();
        }

        private void AddItemsToCollection()
        {
            OrderCollection.Clear();
            using (orderDb = new OrderContext())
            {
                foreach (var t in orderDb.Orders)
                {
                    OrderCollection.Add(orderMapper.Map(t));
                }
                //Search();
            }
        }

        public void AddNewItem()
        {
            ShowConfirmationItemDialog(null);
        }

        public async Task RemoveItem(OrderBusinessModel orderBusModel)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher
                    .BeginInvoke(
                        new System.Action(() => OrderCollection.Remove(orderBusModel)));
            });
            //Search();
        }

        public async Task ChangeItem(OrderBusinessModel orderBusModel)
        {
            ShowConfirmationItemDialog(orderBusModel);
            await RemoveItem(orderBusModel);
        }

        public void SetToCompleted(OrderBusinessModel orderBusModel)
        {
            ChangeOrderStatus(orderBusModel, Status.Completed);
        }

        public void SetToBusy(OrderBusinessModel orderBusModel)
        {
            ChangeOrderStatus(orderBusModel, Status.Busy);
        }

        public void SetToUncompleted(OrderBusinessModel orderBusModel)
        {
            ChangeOrderStatus(orderBusModel, Status.Uncompleted);
        }

        private void ChangeOrderStatus(OrderBusinessModel orderBusModel, Status status)
        {
            OrderCollection[OrderCollection.IndexOf(orderBusModel)].Status = status;
            //switch (status)
            //{
            //    case Status.Busy:
            //        originOrder.Move(index,0);
            //        break;
            //    case Status.Completed:
            //        var start = originOrder.Last(x => x.Status == Status.Neutral);
            //        var ind = originOrder.IndexOf(start);
            //        originOrder.Move(index, ind + 1);
            //        break;
            //}

            //Search();
            //using (orderDb = new OrderContext())
            //{
            //    orderBusModel.Status = status;
            //    var order = orderMapper.Map(orderBusModel);
            //    orderDb.Entry(order).State = EntityState.Modified;
            //    orderDb.SaveChanges();
            //}
            //AddItemsToCollection();
        }

        private void ShowConfirmationItemDialog(OrderBusinessModel currentOrderBusModel)
        {
            var confirmationViewModel = new AddChangeOrderViewModel(currentOrderBusModel, OrderCollection);
            windowManager.ShowDialog(confirmationViewModel);

            //if (confirmationViewModel.IsOkay)
            //    originOrder = new ObservableCollection<OrderBusinessModel>(OrderCollection);
        }

        public async Task Search()
        {
            await Task.Run(async () =>
            {
                var searchText = TextBoxSearch.ToLower();

                if (string.IsNullOrEmpty(searchText) ||
                    string.IsNullOrWhiteSpace(searchText) ||
                    searchText.Length == 1)
                {
                    CanButtonForv = false;
                    CanButtonPrev = false;
                    return;
                }

                orderBusinessModels = OrderCollection
                    .Where(x => (x.Details != null && x.Details.ToLower().Contains(searchText)) ||
                                (x.Name != null && x.Name.ToLower().Contains(searchText)) ||
                                (x.Phone != null && x.Phone.ToLower().Contains(searchText)))
                    .ToList();

                selectedNum = 0;
                CanButtonPrev = false;
                CanButtonForv = false;

                if (!orderBusinessModels.Any())
                    return;

                if (orderBusinessModels.Count > 1)
                    CanButtonForv = true;

                await Scroll();
                //await Sort(businessModels, searchText);
            });
        }

        private async Task Scroll()
        {
            if (orderBusinessModels == null || orderBusinessModels.Count == 0)
                return;
            await Task.Run(() =>
            {
                Application.Current.Dispatcher
                    .BeginInvoke(
                        new System.Action(
                            () =>
                            {
                                try
                                {
                                    Selected = orderBusinessModels[selectedNum];
                                }
                                catch (Exception ex)
                                {
                                }
                            }));
            });
        }

        public void ScrollIntoView(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            try
            {
                ((ListBox) sender).ScrollIntoView(e.AddedItems[0]);
            }
            catch (Exception)
            {

            }
        }

        public void OnClose()
        {
            using (settingsDb = new SettingsContext())
            {
                if (settingsDb.Settings.Count() != 0)
                    settingsDb.Database.Delete();

                var settingsBusModel = new SettingsBusinessModel
                {
                    DetailsWidth = DetailsWidth,
                    PhoneWidth = PhoneWidth,
                    NameWidth = NameWidth,
                    DateWidth = DateWidth,
                    Height = Height,
                    Width = Width,
                    Left = Left,
                    Top = Top,
                    Id = Guid.NewGuid()
                };

                settingsDb.Settings.Add(settingsMapper.Map(settingsBusModel));
                settingsDb.SaveChanges();
            }

            using (orderDb = new OrderContext())
            {
                orderDb.Database.Delete();
                orderDb.Orders.AddRange(OrderCollection.Select(x => orderMapper.Map(x)));
                orderDb.SaveChanges();
            }
        }

        private async Task Sort(IEnumerable<OrderBusinessModel> orderCollection, string query)
        {
            await Task.Run(async () =>
            {
                //OrderCollection.Clear();

                var completed = new List<OrderBusinessModel>();
                var neutral = new List<OrderBusinessModel>();
                var uncompleted = new List<OrderBusinessModel>();
                var busy = new List<OrderBusinessModel>();

                foreach (var order in orderCollection
                    .Where(x => x.Details.Contains(query) ||
                                x.Name.Contains(query) ||
                                x.Phone.Contains(query)))
                {
                    switch (order.Status)
                    {
                        case Status.Neutral:
                            neutral.Add(order);
                            break;
                        case Status.Completed:
                            completed.Add(order);
                            break;
                        case Status.Uncompleted:
                            uncompleted.Add(order);
                            break;
                        case Status.Busy:
                            busy.Add(order);
                            break;
                    }
                }

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher
                        .BeginInvoke(
                            new System.Action(
                                () =>
                                {
                                    OrderCollection.Clear();

                                    foreach (var order in busy)
                                        OrderCollection.Add(order);
                                    foreach (var order in uncompleted)
                                        OrderCollection.Add(order);
                                    foreach (var order in neutral)
                                        OrderCollection.Add(order);
                                    foreach (var order in completed)
                                        OrderCollection.Add(order);
                                }));
                });
            });
        }
    }
}