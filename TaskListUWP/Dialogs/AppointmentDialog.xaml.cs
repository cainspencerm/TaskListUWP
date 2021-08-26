using Persistance.Converters;
using Persistance.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList.Dialogs
{
    public sealed partial class AppointmentDialog : ContentDialog
    {
        public AppointmentDialog()
        {
            InitializeComponent();
            DataContext = new Appointment();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            (DataContext as Appointment).Name = NameTextBox.Text;
            (DataContext as Appointment).Description = DescriptionTextBox.Text;

            var date = StartDatePicker.Date.DateTime;
            var time = StartTimePicker.Time;
            (DataContext as Appointment).Start = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);

            date = StartDatePicker.Date.DateTime;
            time = StartTimePicker.Time;
            (DataContext as Appointment).Stop = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);

            var attendeeNames = AttendeesTextBox.Text.Split(", ").ToList();
            for (int i = 0; i < attendeeNames.Count(); i++)
            {
                var trim = attendeeNames[i].Trim();
                if (trim != "")
                {
                    (DataContext as Appointment).Attendees.Add(new AttendeeDB() { FirstName = attendeeNames[i] });
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}