// ViewModels/MainViewModel.cs
using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmCompanyManager.Services;
using AlarmCompanyManager.Utilities;
using AlarmCompanyManager.Views.Dashboard;
using AlarmCompanyManager.Views.Customers;
using Microsoft.Extensions.DependencyInjection;

namespace AlarmCompanyManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICustomerService _customerService;
        private readonly IWorkOrderService _workOrderService;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        private object? _currentView;
        private string _currentViewTitle = "Dashboard";
        private bool _isMenuOpen = true;
        private string _statusMessage = "Ready";
        private bool _databaseConnectionStatus = true;
        private string _databaseStatus = "Connected";
        private int _notificationCount = 0;

        public MainViewModel(
            IServiceProvider serviceProvider,
            ICustomerService customerService,
            IWorkOrderService workOrderService,
            ISettingsService settingsService,
            IDialogService dialogService)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _workOrderService = workOrderService ?? throw new ArgumentNullException(nameof(workOrderService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            InitializeCommands();
            InitializeViews();
        }

        #region Properties

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set => SetProperty(ref _currentViewTitle, value);
        }

        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set => SetProperty(ref _isMenuOpen, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool DatabaseConnectionStatus
        {
            get => _databaseConnectionStatus;
            set => SetProperty(ref _databaseConnectionStatus, value);
        }

        public string DatabaseStatus
        {
            get => _databaseStatus;
            set => SetProperty(ref _databaseStatus, value);
        }

        public int NotificationCount
        {
            get => _notificationCount;
            set => SetProperty(ref _notificationCount, value);
        }

        #endregion

        #region Commands

        public ICommand NavigateCommand { get; private set; } = null!;
        public ICommand SearchCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand ExitCommand { get; private set; } = null!;

        #endregion

        #region Initialization

        private void InitializeCommands()
        {
            NavigateCommand = new RelayCommand<string>(async parameter => await NavigateToAsync(parameter));
            SearchCommand = new RelayCommand<string>(async parameter => await PerformSearchAsync(parameter));
            RefreshCommand = new AsyncRelayCommand(RefreshCurrentViewAsync);
            ExitCommand = new RelayCommand(ExitApplication);
        }

        private void InitializeViews()
        {
            // Start with Dashboard
            NavigateToAsync("Dashboard");
        }

        public async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                BusyMessage = "Initializing application...";

                Logger.LogInfo("Initializing MainViewModel");

                // Check database connection
                await CheckDatabaseConnectionAsync();

                // Load notifications
                await LoadNotificationsAsync();

                // Navigate to dashboard
                await NavigateToAsync("Dashboard");

                StatusMessage = "Application ready";
                Logger.LogInfo("MainViewModel initialization completed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error initializing MainViewModel");
                StatusMessage = "Error initializing application";
                _dialogService.ShowMessage($"Error initializing application: {ex.Message}",
                    "Initialization Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Navigation

        private async Task NavigateToAsync(string? viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName)) return;

            try
            {
                Logger.LogInfo($"Navigating to: {viewName}");

                object? newView = null;
                string newTitle = viewName;

                switch (viewName.ToLower())
                {
                    case "dashboard":
                        newView = CreateDashboardView();
                        newTitle = "Dashboard";
                        break;

                    case "customerlist":
                        newView = await CreateCustomerListViewAsync();
                        newTitle = "Customers";
                        break;

                    case "addcustomer":
                        newView = CreateAddCustomerView();
                        newTitle = "Add Customer";
                        break;

                    case "workorderlist":
                        newView = await CreateWorkOrderListViewAsync();
                        newTitle = "Work Orders";
                        break;

                    case "addworkorder":
                        newView = CreateAddWorkOrderView();
                        newTitle = "Create Work Order";
                        break;

                    case "calendar":
                        newView = await CreateCalendarViewAsync();
                        newTitle = "Calendar";
                        break;

                    case "customerreports":
                        newView = CreateCustomerReportsView();
                        newTitle = "Customer Reports";
                        break;

                    case "workorderreports":
                        newView = CreateWorkOrderReportsView();
                        newTitle = "Work Order Reports";
                        break;

                    case "customreports":
                        newView = CreateCustomReportsView();
                        newTitle = "Custom Reports";
                        break;

                    case "settings":
                        newView = CreateSettingsView();
                        newTitle = "Settings";
                        break;

                    default:
                        Logger.LogWarning($"Unknown view name: {viewName}");
                        newView = CreateDashboardView();
                        newTitle = "Dashboard";
                        break;
                }

                if (newView != null)
                {
                    CurrentView = newView;
                    CurrentViewTitle = newTitle;
                    StatusMessage = $"Navigated to {newTitle}";

                    // Close menu on navigation (for mobile-like experience)
                    if (System.Windows.SystemParameters.PrimaryScreenWidth < 1200)
                    {
                        IsMenuOpen = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error navigating to {viewName}");
                StatusMessage = $"Error navigating to {viewName}";
                _dialogService.ShowMessage($"Error navigating to {viewName}: {ex.Message}",
                    "Navigation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region View Creation Methods

        private object CreateDashboardView()
        {
            var viewModel = _serviceProvider.GetRequiredService<DashboardViewModel>();
            return new DashboardView { DataContext = viewModel };
        }

        private async Task<object> CreateCustomerListViewAsync()
        {
            var viewModel = _serviceProvider.GetRequiredService<CustomerViewModel>();
            await viewModel.LoadCustomersAsync();
            return new CustomerListView { DataContext = viewModel };
        }

        private object CreateAddCustomerView()
        {
            var viewModel = _serviceProvider.GetRequiredService<CustomerViewModel>();
            viewModel.PrepareForNewCustomer();
            return new CustomerDetailsView { DataContext = viewModel };
        }

        private async Task<object> CreateWorkOrderListViewAsync()
        {
            // TODO: Create WorkOrderListView
            return new System.Windows.Controls.TextBlock
            {
                Text = "Work Order List - Coming Soon",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 24,
                Margin = new System.Windows.Thickness(32)
            };
        }

        private object CreateAddWorkOrderView()
        {
            // TODO: Create WorkOrderDetailsView  
            return new System.Windows.Controls.TextBlock
            {
                Text = "Add Work Order - Coming Soon",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 24,
                Margin = new System.Windows.Thickness(32)
            };
        }

        private async Task<object> CreateCalendarViewAsync()
        {
            // TODO: Create CalendarView
            return new System.Windows.Controls.TextBlock
            {
                Text = "Calendar View - Coming Soon",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 24,
                Margin = new System.Windows.Thickness(32)
            };
        }

        private object CreateCustomerReportsView()
        {
            // TODO: Implement CustomerReportsView
            return new System.Windows.Controls.TextBlock
            {
                Text = "Customer Reports - Coming Soon",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 24
            };
        }

        private object CreateWorkOrderReportsView()
        {
            // TODO: Implement WorkOrderReportsView
            return new System.Windows.Controls.TextBlock
            {
                Text = "Work Order Reports - Coming Soon",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 24
            };
        }

        private object CreateCustomReportsView()
        {
            // TODO: Implement CustomReportsView
            return new System.Windows.Controls.TextBlock
            {
                Text = "Custom Reports - Coming Soon",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 24
            };
        }

        private object CreateSettingsView()
        {
            // TODO: Create SettingsView
            return new System.Windows.Controls.TextBlock
            {
                Text = "Settings - Coming Soon",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 24,
                Margin = new System.Windows.Thickness(32)
            };
        }

        #endregion

        #region Menu and Search

        public void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
            Logger.LogInfo($"Menu toggled: {IsMenuOpen}");
        }

        public void PerformSearch(string searchTerm)
        {
            _ = PerformSearchAsync(searchTerm);
        }

        private async Task PerformSearchAsync(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return;

            try
            {
                Logger.LogInfo($"Performing search: {searchTerm}");
                StatusMessage = $"Searching for: {searchTerm}";

                // Determine what to search based on current view
                if (CurrentViewTitle.Contains("Customer"))
                {
                    // Navigate to customer list with search
                    var viewModel = _serviceProvider.GetRequiredService<CustomerViewModel>();
                    await viewModel.SearchCustomersAsync(searchTerm);
                    CurrentView = new CustomerListView { DataContext = viewModel };
                    CurrentViewTitle = "Customer Search Results";
                }
                else if (CurrentViewTitle.Contains("Work Order"))
                {
                    // TODO: Navigate to work order list with search when WorkOrderListView is created
                    CurrentView = new System.Windows.Controls.TextBlock
                    {
                        Text = $"Work Order Search for '{searchTerm}' - Coming Soon",
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        FontSize = 24,
                        Margin = new System.Windows.Thickness(32)
                    };
                    CurrentViewTitle = "Work Order Search Results";
                }
                else
                {
                    // Global search - search both customers and work orders
                    await PerformGlobalSearchAsync(searchTerm);
                }

                StatusMessage = $"Search completed for: {searchTerm}";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error performing search: {searchTerm}");
                StatusMessage = "Search failed";
                _dialogService.ShowMessage($"Error performing search: {ex.Message}",
                    "Search Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task PerformGlobalSearchAsync(string searchTerm)
        {
            // TODO: Implement global search view that shows results from multiple entities
            var customerViewModel = _serviceProvider.GetRequiredService<CustomerViewModel>();
            await customerViewModel.SearchCustomersAsync(searchTerm);
            CurrentView = new CustomerListView { DataContext = customerViewModel };
            CurrentViewTitle = "Global Search Results";
        }

        #endregion

        #region Database and Notifications

        private async Task CheckDatabaseConnectionAsync()
        {
            try
            {
                // Try to get a simple count to test connection
                var customers = await _customerService.GetAllCustomersAsync();
                DatabaseConnectionStatus = true;
                DatabaseStatus = "Connected";
                Logger.LogInfo("Database connection verified");
            }
            catch (Exception ex)
            {
                DatabaseConnectionStatus = false;
                DatabaseStatus = "Disconnected";
                Logger.LogError(ex, "Database connection failed");
            }
        }

        private async Task LoadNotificationsAsync()
        {
            try
            {
                // TODO: Implement notification system
                // For now, just count overdue work orders
                var workOrders = await _workOrderService.GetAllWorkOrdersAsync();
                var overdueCount = workOrders.Count(wo =>
                    wo.ScheduledDate.HasValue &&
                    wo.ScheduledDate.Value < DateTime.Today &&
                    wo.StatusId != (int)Models.WorkOrderStatusEnum.Completed &&
                    wo.StatusId != (int)Models.WorkOrderStatusEnum.Canceled);

                NotificationCount = overdueCount;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading notifications");
                NotificationCount = 0;
            }
        }

        #endregion

        #region Refresh and Cleanup

        private async Task RefreshCurrentViewAsync()
        {
            try
            {
                IsBusy = true;
                BusyMessage = "Refreshing...";

                await CheckDatabaseConnectionAsync();
                await LoadNotificationsAsync();

                // Refresh current view if it has a refresh method
                if (CurrentView is System.Windows.FrameworkElement element &&
                    element.DataContext is ViewModelBase viewModel)
                {
                    // If the view model has a refresh method, call it
                    var refreshMethod = viewModel.GetType().GetMethod("RefreshAsync");
                    if (refreshMethod != null)
                    {
                        var task = refreshMethod.Invoke(viewModel, null) as Task;
                        if (task != null)
                        {
                            await task;
                        }
                    }
                }

                StatusMessage = "Refreshed successfully";
                Logger.LogInfo("Current view refreshed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error refreshing current view");
                StatusMessage = "Refresh failed";
                _dialogService.ShowMessage($"Error refreshing: {ex.Message}",
                    "Refresh Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Cleanup()
        {
            try
            {
                Logger.LogInfo("Cleaning up MainViewModel");

                // Cleanup current view if needed
                if (CurrentView is IDisposable disposableView)
                {
                    disposableView.Dispose();
                }

                StatusMessage = "Application closing...";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during MainViewModel cleanup");
            }
        }

        private void ExitApplication()
        {
            try
            {
                var result = _dialogService.ShowConfirmation(
                    "Are you sure you want to exit the application?",
                    "Exit Application");

                if (result)
                {
                    Logger.LogInfo("User confirmed application exit");
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error exiting application");
            }
        }

        #endregion
    }
}