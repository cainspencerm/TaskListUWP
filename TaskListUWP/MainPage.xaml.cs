using Persistance.Converters;
using Persistance.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TaskList.Dialogs;
using TaskList.ViewModels;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TaskList
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public TaskListDB CurrentListName => (DataContext as MainViewModel)?.CurrentList;

        private int _SearchedItemId;

        private enum SortOrder { Priority, Time };
        private SortOrder _Sort;

        public event PropertyChangedEventHandler PropertyChanged;

        // TODO Set up for personalization.
        public Color Accent_Color => Colors.SlateBlue;

        public MainPage()
        {
            InitializeComponent();

            DataContext = null;
            try
            {
                DataContext = new MainViewModel();
            }
            catch (Exception e)
            {
                DataContext = null;

                ErrorDialog diag = new ErrorDialog("Load Data Error", e.Message);
                _ = diag.ShowAsync();
            }

            if (DataContext == null)
            {
                return;
            } (DataContext as MainViewModel).PropertyChanged += RenderCalendar;

            (DataContext as MainViewModel).StartDate = DateTime.Today.AddDays(0);
            (DataContext as MainViewModel).EndDate = DateTime.Today.AddDays(14);

            _SearchedItemId = 0;
            _Sort = SortOrder.Time;

            // Initialize first calendar render.
            (DataContext as MainViewModel).NotifyPropertyChanged();
            (DataContext as MainViewModel).UnsavedData = false; // Revert "unsaved data" change.
        }

        private async void RenderCalendar(object sender, PropertyChangedEventArgs e)
        {
            if ((DataContext as MainViewModel).ItemList == null)
            {
                List<TaskListDB> names = (DataContext as MainViewModel)?.GetLists();
                if (names == null || names.Count == 0)
                {
                    CreateListDialog diag = new CreateListDialog();
                    ContentDialogResult result = await diag.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        string name = diag.DataContext as string;

                        TaskListDB created = (DataContext as MainViewModel).CreateList(name);
                        if (created != null)
                        {
                            TaskListDB changed = (DataContext as MainViewModel).ChangeList(created);
                            if (changed != null)
                            {
                                (DataContext as MainViewModel).CurrentList = changed;
                                NotifyPropertyChanged("CurrentList");
                            }
                            else
                            {
                                ErrorDialog errDiag = new ErrorDialog("Change List Error", "Could not change list.");
                                _ = await errDiag.ShowAsync();
                            }
                        }
                    }
                    else
                    {
                        // TODO Disable adding items to list.
                    }
                }
                else
                {
                    LoadListDialog diag = new LoadListDialog(secondaryClick: false);
                    ContentDialogResult result = await diag.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        TaskListDB list = diag.DataContext as TaskListDB;

                        TaskListDB changed = (DataContext as MainViewModel).ChangeList(list);
                        if (changed == null)
                        {
                            ErrorDialog errDiag = new ErrorDialog("Change List Error", "Could not change list.");
                            _ = await errDiag.ShowAsync();
                        }
                    }
                }
            }

            // Refresh list name.
            ListNameTextBlock.Text = (DataContext as MainViewModel)?.CurrentList.Name;

            // Initialize render list for date sorting.
            List<List<Item>> sortingLists = new List<List<Item>>();
            double numDays = ((DataContext as MainViewModel).EndDate.Date - (DataContext as MainViewModel).StartDate.Date).TotalDays;
            for (int i = 0; i < (int)numDays; i++)
            {
                sortingLists.Add(new List<Item>());
            }

            // Add items to list.
            if ((DataContext as MainViewModel).ItemList != null)
            {
                foreach (Item item in (DataContext as MainViewModel).ItemList)
                {
                    int day = 0;
                    if (item is Task)
                    {
                        day = (int)((item as Task).Deadline.Date - (DataContext as MainViewModel).StartDate.Date).TotalDays;
                        sortingLists[day].Add(item);
                    }
                    else if (item is Appointment)
                    {
                        day = (int)((item as Appointment).Start.Date - (DataContext as MainViewModel).StartDate.Date).TotalDays;
                        sortingLists[day].Add(item);
                    }
                }
            }

            // Render days.
            int childPos = 0;
            for (DateTime date = (DataContext as MainViewModel).StartDate; date < (DataContext as MainViewModel).EndDate; date = date.AddDays(1))
            {
                if (!(CalendarItemsStackPanel.Children[childPos] is Grid grid))
                {
                    break;
                }
                TextBlock textBlock = grid.Children[0] as TextBlock;
                textBlock.Text = $"{date:MMM} - {date.Day:00}";
                textBlock.Style = Application.Current.Resources["SubheaderTextBlockStyle"] as Style;

                // Clear previous tiles.
                ScrollViewer scrollViewer = grid.Children[1] as ScrollViewer;
                StackPanel itemTileStackPanel = scrollViewer.Content as StackPanel;
                itemTileStackPanel.Children.Clear();

                // Sort list.
                List<Item> list = _Sort == SortOrder.Time
                    ? sortingLists[childPos].OrderBy(x => x is Task ? (x as Task).Deadline : (x as Appointment).Start).ThenBy(x => x.Name).ToList()
                    : sortingLists[childPos].OrderBy(x => x.Priority).ThenBy(x => x.Name).Reverse().ToList();

                // Add new tiles.
                Item lastItem = new Task() { Deadline = DateTime.Now };
                for (int i = 0; i < list.Count + 1; i++)
                {
                    Item item = i < list.Count ? list[i] : null;

                    DateTime lastdt = DateTime.Now;
                    if (lastItem is Task)
                    {
                        lastdt = (lastItem as Task).Deadline;
                    }
                    else if (item is Appointment)
                    {
                        lastdt = (lastItem as Appointment).Start;
                    }

                    DateTime dt = DateTime.Now;
                    if (item is Task)
                    {
                        dt = (item as Task).Deadline;
                    }
                    else if (item is Appointment)
                    {
                        dt = (item as Appointment).Start;
                    }

                    if (date == DateTime.Now.Date &&
                        _Sort == SortOrder.Time &&
                        ((item == null &&
                          lastdt.TimeOfDay < DateTime.Now.TimeOfDay) ||
                         (item != null &&
                          dt > DateTime.Now &&
                          lastdt <= DateTime.Now)
                         ))
                    {
                        StackPanel timeStackPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Width = 400,
                            Height = 30,
                            Spacing = 10
                        };

                        TextBlock timeTextBlock = new TextBlock
                        {
                            Text = DateTime.Now.ToString("t"),
                            VerticalAlignment = VerticalAlignment.Center,
                            Foreground = new SolidColorBrush(Colors.WhiteSmoke)
                        };
                        timeStackPanel.Children.Add(timeTextBlock);

                        Rectangle rect = new Rectangle
                        {
                            Height = 2,
                            Width = 330,
                            Fill = new SolidColorBrush(Colors.WhiteSmoke)
                        };
                        timeStackPanel.Children.Add(rect);

                        itemTileStackPanel.Children.Add(timeStackPanel);
                    }

                    if (item == null || dt < (DataContext as MainViewModel).StartDate || dt >= (DataContext as MainViewModel).EndDate)
                    {
                        continue;
                    }

                    ItemTile itemTile = new ItemTile(ref item)
                    {
                        Background = new SolidColorBrush(Accent_Color)
                    };

                    itemTileStackPanel.Children.Add(itemTile);

                    lastItem = item;
                }

                childPos++;
            }

            // Pretty sure this has potential to bug.
            if (_SearchedItemId != 0)
            {
                if (!(CalendarItemsStackPanel.Children[0] is Grid grid))
                {
                    return;
                }

                // Clear previous tiles.
                ScrollViewer scrollViewer = grid.Children[1] as ScrollViewer;
                StackPanel itemTileStackPanel = scrollViewer.Content as StackPanel;
                (itemTileStackPanel.Children.First(item => ((item as ItemTile).DataContext as Item).Id == _SearchedItemId) as ItemTile).IsExpanded = true;
                _SearchedItemId = 0;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            TaskDialog diag = new TaskDialog();
            ContentDialogResult result = await diag.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                (DataContext as MainViewModel).AddItem(diag.DataContext as Task);
            }
        }

        private async void AddAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            AppointmentDialog diag = new AppointmentDialog();
            ContentDialogResult result = await diag.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                (DataContext as MainViewModel).AddItem(diag.DataContext as Appointment);
            }
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            float dayWidth = CalendarItemsStackPanel.Children[0].ActualSize.X + 10;
            if ((sender as ScrollViewer).ScrollableWidth == (sender as ScrollViewer).HorizontalOffset)
            {
                var numDays = 7;
                (DataContext as MainViewModel).StartDate = (DataContext as MainViewModel).StartDate.AddDays(numDays);
                (DataContext as MainViewModel).EndDate = (DataContext as MainViewModel).EndDate.AddDays(numDays);

                _ = (sender as ScrollViewer).ChangeView(CalendarItemsScrollViewer.HorizontalOffset - (dayWidth * numDays), null, null, true);

                (DataContext as MainViewModel).LoadData();
            }

            else if ((sender as ScrollViewer).HorizontalOffset == 0)
            {
                int numDays = -7;
                (DataContext as MainViewModel).StartDate = (DataContext as MainViewModel).StartDate.AddDays(numDays);
                (DataContext as MainViewModel).EndDate = (DataContext as MainViewModel).EndDate.AddDays(numDays);

                _ = (sender as ScrollViewer).ChangeView(CalendarItemsScrollViewer.HorizontalOffset - (dayWidth * numDays), null, null, true);

                (DataContext as MainViewModel).LoadData();
            }
        }

        private async void CreateList_Click(object sender, RoutedEventArgs e)
        {
            CreateListDialog createDiag = new CreateListDialog();
            ContentDialogResult result = await createDiag.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (!(createDiag.DataContext is string name) || name == "")
                {
                    return;
                }

                TaskListDB created = (DataContext as MainViewModel).CreateList(name);
                if (created == null)
                {
                    ErrorDialog diag = new ErrorDialog("Create List Error", "Could not create list.");
                    _ = await diag.ShowAsync();
                    return;
                }

                if ((DataContext as MainViewModel).CurrentList.ToString() == "")
                {
                    TaskListDB changed = (DataContext as MainViewModel).ChangeList(created);
                    if (changed == null)
                    {
                        ErrorDialog diag = new ErrorDialog("Change List Error", "Could not change list.");
                        _ = await diag.ShowAsync();
                    }
                }
            }
        }

        private async void ChangeList_Click(object sender, RoutedEventArgs e)
        {
            LoadListDialog loadDiag = new LoadListDialog();
            ContentDialogResult result = await loadDiag.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (!(loadDiag.DataContext is TaskListDB list))
                {
                    return;
                }

                TaskListDB changed = (DataContext as MainViewModel).ChangeList(list);
                if (changed == null)
                {
                    ErrorDialog diag = new ErrorDialog("Change List Error", "Could not change list.");
                    _ = await diag.ShowAsync();
                }
            }
        }

        private async void DeleteList_Click(object sender, RoutedEventArgs e)
        {
            DeleteListDialog deleteDiag = new DeleteListDialog();
            ContentDialogResult result = await deleteDiag.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (!(deleteDiag.DataContext is TaskListDB list))
                {
                    return;
                }

                bool deleted = (DataContext as MainViewModel).DeleteList(list.Id);
                if (!deleted)
                {
                    ErrorDialog diag = new ErrorDialog("Delete List Error", "Could not delete list.");
                    _ = await diag.ShowAsync();
                }
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as Button).Content as string) == "Priority")
            {
                _Sort = SortOrder.Priority;
                (sender as Button).Content = "Time";
            }
            else
            {
                _Sort = SortOrder.Time;
                (sender as Button).Content = "Priority";
            }

            (DataContext as MainViewModel).NotifyPropertyChanged();
        }

        // Handle text change and present suitable items
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text == "" || sender.Text.Trim() == "")
            {
                sender.ItemsSource = new List<Item>();
                return;
            }

            List<Item> suitableItems = (DataContext as MainViewModel).Search(sender.Text);

            sender.ItemsSource = suitableItems;
        }

        // Handle user selecting an item, in our case just output the selected item.
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            _SearchedItemId = args.SelectedItem is Item ? (args.SelectedItem as Item).Id : 0;
            if (_SearchedItemId == 0) return;

            sender.Text = args.SelectedItem.ToString();

            DateTime date = DateTime.Now;
            if (args.SelectedItem is Task)
            {
                date = (args.SelectedItem as Task).Deadline.Date;
            }
            else if (args.SelectedItem is Appointment)
            {
                date = (args.SelectedItem as Appointment).Start.Date;
            }

            (DataContext as MainViewModel).StartDate = date;
            (DataContext as MainViewModel).EndDate = date.AddDays(14);

            (DataContext as MainViewModel).NotifyPropertyChanged();
        }
    }
}