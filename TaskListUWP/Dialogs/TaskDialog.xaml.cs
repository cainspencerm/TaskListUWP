using Persistance.Models;
using System;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList.Dialogs
{
    public sealed partial class TaskDialog : ContentDialog
    {
        public TaskDialog()
        {
            this.InitializeComponent();
            DataContext = new Task();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            (DataContext as Task).Name = NameTextBox.Text;
            (DataContext as Task).Description = DescriptionTextBox.Text;

            var date = DeadlineDatePicker.Date.DateTime;
            var time = DeadlineTimePicker.Time;
            (DataContext as Task).Deadline = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}