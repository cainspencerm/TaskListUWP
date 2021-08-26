using Persistance.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using TaskList.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Persistance.Converters;

namespace TaskList.ViewModels
{
    public class ItemTile : Expander
    {
        public ItemTile(ref Item item) : base()
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            DataContext = item;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Left;

            Header = new Grid();
            (Header as Grid).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
            (Header as Grid).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(160) });
            (Header as Grid).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60) });
            (Header as Grid).ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
            (Header as Grid).HorizontalAlignment = HorizontalAlignment.Left;
            (Header as Grid).VerticalAlignment = VerticalAlignment.Center;
            (Header as Grid).ColumnSpacing = 10;

            if (item is Task)
            {
                CheckBox checkBox = new CheckBox
                {
                    IsChecked = (item as Task).IsComplete,
                    Background = new SolidColorBrush(Colors.AliceBlue),
                    BorderBrush = new SolidColorBrush(Colors.AliceBlue),
                    FocusVisualPrimaryBrush = new SolidColorBrush(Colors.AliceBlue),
                    Foreground = new SolidColorBrush(Colors.AliceBlue),
                    DataContext = item.Id
                };
                checkBox.Checked += OnChecked;
                checkBox.Unchecked += OnUnchecked;
                checkBox.SetValue(Grid.ColumnProperty, 0);
                (Header as Grid).Children.Add(checkBox);
            }

            TextBlock nameTextBlock = new TextBlock
            {
                Text = item.Name,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            nameTextBlock.SetValue(Grid.ColumnProperty, 1);
            (Header as Grid).Children.Add(nameTextBlock);

            DateTime dt = DateTime.Now;
            dt = item is Task ? (item as Task).Deadline : (item as Appointment).Start;

            TextBlock timeTextBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Text = dt.ToString("t"),
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            timeTextBlock.SetValue(Grid.ColumnProperty, 2);
            (Header as Grid).Children.Add(timeTextBlock);

            string priority = "";
            for (int i = 0; i < item.Priority; i++)
            {
                priority += "!";
            }

            Button priorityButton = new Button
            {
                Content = priority,
                Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 159, G = 149, B = 223 }),
                Padding = new Thickness(-5, 0, -5, 0),
                Width = 30,
                Height = 30,
                MaxHeight = 30,
                VerticalAlignment = VerticalAlignment.Center
            };
            priorityButton.Click += PriorityButton_Click;
            priorityButton.DataContext = item.Id;
            priorityButton.SetValue(Grid.ColumnProperty, 3);
            (Header as Grid).Children.Add(priorityButton);

            Content = new StackPanel();
            (Content as StackPanel).HorizontalAlignment = HorizontalAlignment.Stretch;
            (Content as StackPanel).Orientation = Orientation.Vertical;
            (Content as StackPanel).Spacing = 10;

            TextBlock descriptionTextBlock = new TextBlock() { 
                Text = item.Description,
                TextWrapping = TextWrapping.WrapWholeWords,
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxLines = 5
            };
            (Content as StackPanel).Children.Add(descriptionTextBlock);

            if (item is Appointment)
            {
                VariableSizedWrapGrid attendeesWrapGrid = new VariableSizedWrapGrid
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    ItemHeight = 40
                };

                // TODO Figure out how to stop setting the width of each button.

                TextBlock attendeesTextBlock = new TextBlock() { Text = "Attendees:", VerticalAlignment = VerticalAlignment.Center };
                attendeesWrapGrid.Children.Add(attendeesTextBlock);

                foreach (AttendeeDB attendee in (item as Appointment).Attendees)
                {
                    Button button = new Button
                    {
                        Content = attendee,
                        Margin = new Thickness(5, 5, 5, 5)
                    };
                    attendeesWrapGrid.Children.Add(button);
                }

                (Content as StackPanel).Children.Add(attendeesWrapGrid);

                TextBlock timeframeTextBlock = new TextBlock
                {
                    Text = (item as Appointment).Start.ToString("t") + " - " + (item as Appointment).Stop.ToString("t"),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                (Content as StackPanel).Children.Add(timeframeTextBlock);
            }

            Rectangle rect = new Rectangle
            {
                Height = 2,
                Width = 380,
                Fill = new SolidColorBrush(Colors.WhiteSmoke),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            (Content as StackPanel).Children.Add(rect);

            StackPanel editDeleteStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 250
            };

            Button editButton = new Button
            {
                Content = "Edit",
                DataContext = item.Id,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            editButton.Click += EditButton_Click;
            editDeleteStackPanel.Children.Add(editButton);

            Button deleteButton = new Button
            {
                Content = "Delete",
                DataContext = item.Id,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            deleteButton.Click += DeleteButton_Click;
            editDeleteStackPanel.Children.Add(deleteButton);

            (Content as StackPanel).Children.Add(editDeleteStackPanel);
        }

        private void PriorityButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel mainViewModel = ((Window.Current.Content as Frame).Content as MainPage).DataContext as MainViewModel;
            string priority = (string)(sender as Button).Content;
            Item item = mainViewModel.ItemList.Find(x => x.Id == (int)(sender as Button).DataContext);

            if (item.Priority == 3)
            {
                priority = "";
                item.Priority = 0;
            }
            else
            {
                priority += "!";
                item.Priority += 1;
            }

            (sender as Button).Content = priority;
            mainViewModel.EditItem(item);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel mainViewModel = ((Window.Current.Content as Frame).Content as MainPage).DataContext as MainViewModel;
            Item item = mainViewModel.ItemList.Find(x => x.Id == (int)(sender as Button).DataContext);

            DeleteItemDialog diag = new DeleteItemDialog();
            ContentDialogResult result = await diag.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                mainViewModel.DeleteItem(item);
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel mainViewModel = ((Window.Current.Content as Frame).Content as MainPage).DataContext as MainViewModel;
            Item item = mainViewModel.ItemList.Find(x => x.Id == (int)(sender as Button).DataContext);

            EditItemDialog diag = new EditItemDialog(item);
            ContentDialogResult result = await diag.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Item editedItem = diag.DataContext as Item;
                mainViewModel.EditItem(editedItem);

                int pos = editedItem is Task ? 1 : 0;

                DateTime dt = DateTime.Now;
                dt = editedItem is Task ? (editedItem as Task).Deadline : (editedItem as Appointment).Start;

                ((Header as Grid).Children[pos] as TextBlock).Text = editedItem.Name;
                ((Header as Grid).Children[pos + 1] as TextBlock).Text = dt.ToString("t");

                ((Content as StackPanel).Children[0] as TextBlock).Text = editedItem.Description;

                if (editedItem is Task)
                {
                    ((Header as Grid).Children[0] as CheckBox).IsChecked = (editedItem as Task).IsComplete;
                }
                else
                {
                    ((Content as StackPanel).Children[1] as VariableSizedWrapGrid).Children.Clear();

                    TextBlock attendeesTextBlock = new TextBlock() { Text = "Attendees:", VerticalAlignment = VerticalAlignment.Center };
                    ((Content as StackPanel).Children[1] as VariableSizedWrapGrid).Children.Add(attendeesTextBlock);

                    foreach (AttendeeDB attendee in (editedItem as Appointment).Attendees)
                    {
                        Button button = new Button
                        {
                            Content = attendee,
                            Margin = new Thickness(5, 5, 5, 5)
                        };
                        ((Content as StackPanel).Children[1] as VariableSizedWrapGrid).Children.Add(button);
                    }

                    var text = (editedItem as Appointment).Start.ToString("t") + " - " + (editedItem as Appointment).Stop.ToString("t");
                    ((Content as StackPanel).Children[2] as TextBlock).Text = text;
                }
            }
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            MainViewModel mainViewModel = ((Window.Current.Content as Frame).Content as MainPage).DataContext as MainViewModel;
            Item item = mainViewModel.ItemList.Find(x => x.Id == (int)(sender as CheckBox).DataContext);
            (item as Task).IsComplete = false;
            mainViewModel.EditItem(item);
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            MainViewModel mainViewModel = ((Window.Current.Content as Frame).Content as MainPage).DataContext as MainViewModel;
            Item item = mainViewModel.ItemList.Find(x => x.Id == (int)(sender as CheckBox).DataContext);
            (item as Task).IsComplete = true;
            mainViewModel.EditItem(item);
        }
    }
}