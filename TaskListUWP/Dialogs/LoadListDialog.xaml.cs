using Persistance.Converters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaskList.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList.Dialogs
{
    public sealed partial class LoadListDialog : ContentDialog
    {
        private ObservableCollection<TaskListDB> Lists { get; set; }
        public LoadListDialog(bool secondaryClick = true)
        {
            InitializeComponent();

            if (secondaryClick)
            {
                SecondaryButtonClick += ContentDialog_SecondaryButtonClick;
            } 
            else
            {
                // TODO Create a function to shake cancel button implying can't cancel.
            }

            Lists = new ObservableCollection<TaskListDB>();

            var mainViewModel = ((Window.Current.Content as Frame).Content as MainPage)?.DataContext as MainViewModel;
            var lists = mainViewModel?.GetLists();
            if (lists == null) return;

            foreach (var list in lists)
            {
                Lists.Add(list);
            }

            ListNameListView.ItemsSource = Lists;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var item = ListNameListView.SelectedItem;
            if (item != null)
            {
                DataContext = item as TaskListDB;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
