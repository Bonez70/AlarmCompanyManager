using System.Windows;

namespace AlarmCompanyManager.Utilities
{
    public interface IDialogService
    {
        MessageBoxResult ShowMessage(string message, string title = "Information", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information);
        bool ShowConfirmation(string message, string title = "Confirmation");
        string? ShowSaveFileDialog(string filter = "All files (*.*)|*.*", string defaultExt = "");
        string? ShowOpenFileDialog(string filter = "All files (*.*)|*.*");
    }

    public class DialogService : IDialogService
    {
        public MessageBoxResult ShowMessage(string message, string title = "Information", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            return MessageBox.Show(message, title, button, icon);
        }

        public bool ShowConfirmation(string message, string title = "Confirmation")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public string? ShowSaveFileDialog(string filter = "All files (*.*)|*.*", string defaultExt = "")
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter,
                DefaultExt = defaultExt
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string? ShowOpenFileDialog(string filter = "All files (*.*)|*.*")
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}