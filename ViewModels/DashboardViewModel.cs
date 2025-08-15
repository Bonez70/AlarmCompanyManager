// ViewModels/DashboardViewModel.cs
using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmCompanyManager.Models;
using AlarmCompanyManager.Services;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly IWorkOrderService _workOrderService;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        private int _totalCustomers;
        private int _totalWorkOrders;
        private int _pendingWorkOrders;
        private int _overdueWorkOrders;
        private int _completedThisMonth;
        private int _activeSecuritySystems;
        private decimal _monthlyRevenue;
        private int _newCustomersThisMonth;

        private ObservableCollection<WorkOrder> _recentWorkOrders = new();
        private ObservableCollection<WorkOrder> _upcomingWorkOrders = new();
        private ObservableCollection<Customer> _recentCustomers = new();

        public DashboardViewModel(
            ICustomerService customerService,
            IWorkOrderService workOrderService,
            ISettingsService settingsService,
            IDialogService dialogService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _workOrderService = workOrderService ?? throw new ArgumentNullException(nameof(workOrderService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            InitializeCommands();
            _ = LoadDashboardDataAsync();
        }

        #region Properties

        public int TotalCustomers
        {
            get => _totalCustomers;
            set => SetProperty(ref _totalCustomers, value);
        }

        public int TotalWorkOrders
        {
            get => _totalWorkOrders;
            set => SetProperty(ref _totalWorkOrders, value);
        }

        public int PendingWorkOrders
        {
            get => _pendingWorkOrders;
            set => SetProperty(ref _pendingWorkOrders, value);
        }

        public int OverdueWorkOrders
        {
            get => _overdueWorkOrders;
            set => SetProperty(ref _overdueWorkOrders, value);
        }

        public int CompletedThisMonth
        {
            get => _completedThisMonth;
            set => SetProperty(ref _completedThisMonth, value);
        }

        public int ActiveSecuritySystems
        {
            get => _activeSecuritySystems;
            set => SetProperty(ref _activeSecuritySystems, value);
        }

        public decimal MonthlyRevenue
        {
            get => _monthlyRevenue;
            set => SetProperty(ref _monthlyRevenue, value);
        }

        public int NewCustomersThisMonth
        {
            get => _newCustomersThisMonth;
            set => SetProperty(ref _newCustomersThisMonth, value);
        }

        public ObservableCollection<WorkOrder> RecentWorkOrders
        {
            get => _recentWorkOrders;
            set => SetProperty(ref _recentWorkOrders, value);
        }

        public ObservableCollection<WorkOrder> UpcomingWorkOrders
        {
            get => _upcomingWorkOrders;
            set => SetProperty(ref _upcomingWorkOrders, value);
        }

        public ObservableCollection<Customer> RecentCustomers
        {
            get => _recentCustomers;
            set => SetProperty(ref _recentCustomers, value);
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand ViewAllCustomersCommand { get; private set; } = null!;
        public ICommand ViewAllWorkOrdersCommand { get; private set; } = null!;
        public ICommand CreateWorkOrderCommand { get; private set; } = null!;
        public ICommand AddCustomerCommand { get; private set; } = null!;

        #endregion

        #region Initialization

        private void InitializeCommands()
        {
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            ViewAllCustomersCommand = new RelayCommand(() => NavigateToCustomers());
            ViewAllWorkOrdersCommand = new RelayCommand(() => NavigateToWorkOrders());
            CreateWorkOrderCommand = new RelayCommand(() => NavigateToCreateWorkOrder());
            AddCustomerCommand = new RelayCommand(() => NavigateToAddCustomer());
        }

        public async Task LoadDashboardDataAsync()
        {
            try
            {
                IsBusy = true;
                BusyMessage = "Loading dashboard data...";

                Logger.LogInfo("Loading dashboard data");

                // Load all data in parallel for better performance
                var tasks = new[]
                {
                    LoadCustomerMetricsAsync(),
                    LoadWorkOrderMetricsAsync(),
                    LoadRecentDataAsync()
                };

                await Task.WhenAll(tasks);

                Logger.LogInfo("Dashboard data loaded successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading dashboard data");
                _dialogService.ShowMessage($"Error loading dashboard data: {ex.Message}",
                    "Dashboard Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Data Loading

        private async Task LoadCustomerMetricsAsync()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                TotalCustomers = customers.Count();

                // Count customers added this month
                var thisMonth = DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1); // First day of current month
                NewCustomersThisMonth = customers.Count(c => c.CreatedDate >= thisMonth);

                // Count active security systems
                var customersWithSystems = customers.Where(c => c.SecuritySystems.Any(ss => ss.IsActive));
                ActiveSecuritySystems = customersWithSystems.Sum(c => c.SecuritySystems.Count(ss => ss.IsActive));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading customer metrics");
                throw;
            }
        }

        private async Task LoadWorkOrderMetricsAsync()
        {
            try
            {
                var workOrders = await _workOrderService.GetAllWorkOrdersAsync();
                TotalWorkOrders = workOrders.Count();

                // Count pending work orders
                PendingWorkOrders = workOrders.Count(wo =>
                    wo.StatusId == (int)WorkOrderStatusEnum.Unscheduled ||
                    wo.StatusId == (int)WorkOrderStatusEnum.Scheduled ||
                    wo.StatusId == (int)WorkOrderStatusEnum.InProgress ||
                    wo.StatusId == (int)WorkOrderStatusEnum.Pending);

                // Count overdue work orders
                OverdueWorkOrders = workOrders.Count(wo =>
                    wo.ScheduledDate.HasValue &&
                    wo.ScheduledDate.Value < DateTime.Today &&
                    wo.StatusId != (int)WorkOrderStatusEnum.Completed &&
                    wo.StatusId != (int)WorkOrderStatusEnum.Canceled);

                // Count completed this month
                var thisMonth = DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1);
                CompletedThisMonth = workOrders.Count(wo =>
                    wo.StatusId == (int)WorkOrderStatusEnum.Completed &&
                    wo.CompletedDate.HasValue &&
                    wo.CompletedDate.Value >= thisMonth);

                // Calculate monthly revenue (from completed work orders this month)
                MonthlyRevenue = workOrders
                    .Where(wo => wo.StatusId == (int)WorkOrderStatusEnum.Completed &&
                                wo.CompletedDate.HasValue &&
                                wo.CompletedDate.Value >= thisMonth &&
                                wo.ActualCost.HasValue)
                    .Sum(wo => wo.ActualCost.Value);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading work order metrics");
                throw;
            }
        }

        private async Task LoadRecentDataAsync()
        {
            try
            {
                // Load recent work orders
                var recentWorkOrders = await _workOrderService.GetAllWorkOrdersAsync();
                RecentWorkOrders = recentWorkOrders
                    .OrderByDescending(wo => wo.CreatedDate)
                    .Take(5)
                    .ToObservableCollection();

                // Load upcoming work orders (next 7 days)
                var nextWeek = DateTime.Today.AddDays(7);
                var upcomingWorkOrders = recentWorkOrders
                    .Where(wo => wo.ScheduledDate.HasValue &&
                                wo.ScheduledDate.Value >= DateTime.Today &&
                                wo.ScheduledDate.Value <= nextWeek &&
                                wo.StatusId != (int)WorkOrderStatusEnum.Completed &&
                                wo.StatusId != (int)WorkOrderStatusEnum.Canceled)
                    .OrderBy(wo => wo.ScheduledDate)
                    .ThenBy(wo => wo.ScheduledStartTime)
                    .Take(5);

                UpcomingWorkOrders = upcomingWorkOrders.ToObservableCollection();

                // Load recent customers
                var recentCustomers = await _customerService.GetAllCustomersAsync();
                RecentCustomers = recentCustomers
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(5)
                    .ToObservableCollection();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading recent data");
                throw;
            }
        }

        #endregion

        #region Refresh

        public async Task RefreshAsync()
        {
            await LoadDashboardDataAsync();
        }

        #endregion

        #region Navigation

        private void NavigateToCustomers()
        {
            // This would be handled by the main view model
            Logger.LogInfo("Navigating to customers from dashboard");
            // In a real implementation, you might use a navigation service or messaging
        }

        private void NavigateToWorkOrders()
        {
            Logger.LogInfo("Navigating to work orders from dashboard");
        }

        private void NavigateToCreateWorkOrder()
        {
            Logger.LogInfo("Navigating to create work order from dashboard");
        }

        private void NavigateToAddCustomer()
        {
            Logger.LogInfo("Navigating to add customer from dashboard");
        }

        #endregion

        #region Helper Methods

        public string GetWorkOrderStatusColor(int statusId)
        {
            return statusId switch
            {
                (int)WorkOrderStatusEnum.Unscheduled => "#FF6B6B",
                (int)WorkOrderStatusEnum.Scheduled => "#4ECDC4",
                (int)WorkOrderStatusEnum.InProgress => "#45B7D1",
                (int)WorkOrderStatusEnum.Pending => "#FFA726",
                (int)WorkOrderStatusEnum.Canceled => "#78909C",
                (int)WorkOrderStatusEnum.Completed => "#66BB6A",
                _ => "#9E9E9E"
            };
        }

        public string GetCustomerTypeColor(int customerTypeId)
        {
            return customerTypeId switch
            {
                (int)CustomerTypeEnum.Residential => "#2196F3",
                (int)CustomerTypeEnum.Commercial => "#FF9800",
                (int)CustomerTypeEnum.Government => "#4CAF50",
                (int)CustomerTypeEnum.Education => "#9C27B0",
                _ => "#9E9E9E"
            };
        }

        public string FormatCurrency(decimal amount)
        {
            return amount.ToString("C2");
        }

        public string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} day(s) ago";
            else if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} hour(s) ago";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
            else
                return "Just now";
        }

        #endregion
    }
}