using Persistance.Converters;
using Persistance.Models;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList.Dialogs
{
    public sealed partial class EditItemDialog : ContentDialog
    {
        public EditItemDialog(Item item)
        {
            InitializeComponent();
            DataContext = item;

            EditItemStackPanel.Children.Clear();

            TextBox nameTextBox = new TextBox
            {
                PlaceholderText = "Name",
                Text = (DataContext as Item).Name,
                Width = 350
            };
            EditItemStackPanel.Children.Add(nameTextBox);

            TextBox descriptionTextBox = new TextBox
            {
                Width = 350,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                IsSpellCheckEnabled = true,
                PlaceholderText = "Description",
                Text = (DataContext as Item).Description
            };
            EditItemStackPanel.Children.Add(descriptionTextBox);

            if (DataContext is Task)
            {
                DatePicker deadlineDatePicker = new DatePicker
                {
                    Date = (DataContext as Task).Deadline.Date,
                    Width = 350
                };
                EditItemStackPanel.Children.Add(deadlineDatePicker);

                TimePicker deadlineTimePicker = new TimePicker
                {
                    Time = (DataContext as Task).Deadline.TimeOfDay,
                    Width = 350
                };
                EditItemStackPanel.Children.Add(deadlineTimePicker);

                CheckBox isCompleteCheckBox = new CheckBox
                {
                    Content = "Complete",
                    IsChecked = (DataContext as Task).IsComplete
                };
                EditItemStackPanel.Children.Add(isCompleteCheckBox);
            }
            else
            {
                DatePicker startDatePicker = new DatePicker
                {
                    Date = (DataContext as Appointment).Start.Date,
                    Width = 350
                };
                EditItemStackPanel.Children.Add(startDatePicker);

                TimePicker startTimePicker = new TimePicker
                {
                    Time = (DataContext as Appointment).Start.TimeOfDay,
                    Width = 350
                };
                EditItemStackPanel.Children.Add(startTimePicker);

                TextBlock textBlock = new TextBlock
                {
                    Text = "to",
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                EditItemStackPanel.Children.Add(textBlock);

                DatePicker stopDatePicker = new DatePicker
                {
                    Date = (DataContext as Appointment).Stop.Date,
                    Width = 350
                };
                EditItemStackPanel.Children.Add(stopDatePicker);

                TimePicker stopTimePicker = new TimePicker
                {
                    Time = (DataContext as Appointment).Stop.TimeOfDay,
                    Width = 350
                };
                EditItemStackPanel.Children.Add(stopTimePicker);

                VariableSizedWrapGrid attendeesWrapGrid = new VariableSizedWrapGrid
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    ItemHeight = 40,
                    Width = 350
                };

                // TODO Figure out how to stop setting the width of each button.

                TextBlock attendeesTextBlock = new TextBlock() { Text = "Attendees:", VerticalAlignment = VerticalAlignment.Center };
                attendeesWrapGrid.Children.Add(attendeesTextBlock);

                foreach (AttendeeDB attendee in (DataContext as Appointment).Attendees)
                {
                    string trim = attendee.FirstName.Trim();
                    if (trim == "")
                    {
                        continue;
                    }

                    Button button = new Button
                    {
                        Content = attendee,
                        DataContext = item,
                        Margin = new Thickness(5, 5, 5, 5)
                    };
                    button.Click += AttendeeButtonClick;
                    attendeesWrapGrid.Children.Add(button);
                }

                EditItemStackPanel.Children.Add(attendeesWrapGrid);

                TextBox attendeesTextBox = new TextBox
                {
                    Width = 350,
                    PlaceholderText = "John, Sally, Adrien...",
                    Text = string.Join(", ", (DataContext as Appointment).Attendees)
                };
                attendeesTextBox.TextChanged += AttendeesTextChanged;
                EditItemStackPanel.Children.Add(attendeesTextBox);
            }
        }

        private void AttendeesTextChanged(object sender, TextChangedEventArgs e)
        {
            Appointment item = DataContext as Appointment;
            item.Attendees = (sender as TextBox).Text.Split(", ").Select(a => new AttendeeDB() { FirstName = a }).ToList();
            for (int i = 0; i < item.Attendees.Count; i++)
            {
                string trim = item.Attendees[i].FirstName.Trim();
                if (trim == "")
                {
                    item.Attendees.RemoveAt(i);
                    i--;
                }
            }

            (EditItemStackPanel.Children[7] as VariableSizedWrapGrid).Children.Clear();

            TextBlock attendeesTextBlock = new TextBlock() { Text = "Attendees:", VerticalAlignment = VerticalAlignment.Center };
            (EditItemStackPanel.Children[7] as VariableSizedWrapGrid).Children.Add(attendeesTextBlock);

            foreach (AttendeeDB attendee in item.Attendees)
            {
                Button button = new Button
                {
                    Content = attendee,
                    DataContext = item,
                    Margin = new Thickness(5, 5, 5, 5)
                };
                button.Click += AttendeeButtonClick;
                (EditItemStackPanel.Children[7] as VariableSizedWrapGrid).Children.Add(button);
            }
        }

        private void AttendeeButtonClick(object sender, RoutedEventArgs e)
        {
            // Incorrect usage of action. Implement correct action.
            // ((sender as Button).Parent as VariableSizedWrapGrid).Children.Remove(sender as Button);
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            (DataContext as Item).Name = (EditItemStackPanel.Children[0] as TextBox).Text;
            (DataContext as Item).Description = (EditItemStackPanel.Children[1] as TextBox).Text;

            if (DataContext is Task)
            {
                (DataContext as Task).Deadline = (EditItemStackPanel.Children[2] as DatePicker).Date.DateTime;
                (DataContext as Task).Deadline = (DataContext as Task).Deadline.Add((EditItemStackPanel.Children[3] as TimePicker).Time);
                (DataContext as Task).IsComplete = (EditItemStackPanel.Children[4] as CheckBox).IsChecked.Value;
            }
            else
            {
                (DataContext as Appointment).Start = (EditItemStackPanel.Children[2] as DatePicker).Date.DateTime;
                (DataContext as Appointment).Start = (DataContext as Appointment).Start.Add((EditItemStackPanel.Children[3] as TimePicker).Time);

                (DataContext as Appointment).Stop = (EditItemStackPanel.Children[5] as DatePicker).Date.DateTime;
                (DataContext as Appointment).Stop = (DataContext as Appointment).Stop.Add((EditItemStackPanel.Children[6] as TimePicker).Time);

                (DataContext as Appointment).Attendees.Clear();
                var attendees = (EditItemStackPanel.Children[7] as VariableSizedWrapGrid).Children.Select(item => (item as Button)?.Content as AttendeeDB).ToList();
                foreach (var attendee in attendees)
                {
                    if (attendee is null)
                    {
                        continue;
                    }

                    (DataContext as Appointment).Attendees.Add(attendee);
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
