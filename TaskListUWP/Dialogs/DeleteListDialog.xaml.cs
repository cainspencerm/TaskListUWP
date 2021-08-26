using Persistance.Converters;
using System.Collections.ObjectModel;
using TaskList.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList.Dialogs
{
    public sealed partial class DeleteListDialog : ContentDialog
    {
        private ObservableCollection<TaskListDB> Lists { get; set; }
        public DeleteListDialog()
        {
            InitializeComponent();

            Lists = new ObservableCollection<TaskListDB>();

            var mainViewModel = ((Window.Current.Content as Frame).Content as MainPage).DataContext as MainViewModel;
            foreach (var list in mainViewModel.GetLists())
            {
                Lists.Add(list);
            }

            ListNameListView.ItemsSource = Lists;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            DataContext = ListNameListView.SelectedItem as TaskListDB;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
