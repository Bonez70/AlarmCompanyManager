// MainWindow.xaml.cs
using System.Windows;
using AlarmCompanyManager.ViewModels;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            Logger.LogInfo("Main window initialized");

            // Handle window events
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogInfo("Main window loaded");

                // Initialize the view model
                if (DataContext is MainViewModel viewModel)
                {
                    await viewModel.InitializeAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during main window load");
                MessageBox.Show($"Error initializing application: {ex.Message}",
                    "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Logger.LogInfo("Main window closing");

                // Perform any cleanup here if needed
                if (DataContext is MainViewModel viewModel)
                {
                    // Save any pending changes or user preferences
                    viewModel.Cleanup();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during main window closing");
            }
        }

        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.ToggleMenu();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error toggling menu");
            }
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    if (DataContext is MainViewModel viewModel && SearchTextBox.Text.Length > 0)
                    {
                        viewModel.PerformSearch(SearchTextBox.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error performing search");
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            try
            {
                base.OnStateChanged(e);

                if (WindowState == WindowState.Minimized)
                {
                    Logger.LogInfo("Main window minimized");
                }
                else if (WindowState == WindowState.Maximized)
                {
                    Logger.LogInfo("Main window maximized");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error handling window state change");
            }
        }
    }
}