// ViewModels/WorkOrderViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using AlarmCompanyManager.Models;
using AlarmCompanyManager.Services;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager.ViewModels
{
    public class WorkOrderViewModel : ViewModelBase
    {
        private readonly IWorkOrderService _workOrderService;
        private readonly ICustomerService _customerService;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<WorkOrder> _workOrders = new();
        private ObservableCollection<Customer> _customers = new();
        private ObservableCollection<WorkOrderType> _workOrderTypes = new();
        private ObservableCollection<WorkOrderCategory> _workOrderCategories = new();
        private ObservableCollection<WorkOrderStatus> _workOrderStatuses = new();
        private ObservableCollection<Technician> _technicians = new();
        private ObservableCollection<WorkOrderItem> _workOrderItems = new();

        private WorkOrder? _selectedWorkOrder;
        private WorkOrder _currentWorkOrder = new();
        private WorkOrderItem _currentWorkOrderItem = new();
        private string _searchText = string.Empty;
        private bool _isEditMode = false;
        private bool _isNewWorkOrder = false;
        private DateTime _selectedDate = DateTime.Today;
        private int _selectedStatusFilter = 0; // 0 = All
        private int _selectedTechnicianFilter = 0; // 0 = All

        public WorkOrderViewModel(
            IWorkOrderService workOrderService,
            ICustomerService customerService,
            ISettingsService settingsService,
            IDialogService dialogService)
        {
            _workOrderService = workOrderService ?? throw new ArgumentNullException(nameof(workOrderService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            InitializeCommands();
            _ = InitializeAsync();
        }

        #region Properties

        public ObservableCollection<WorkOrder> WorkOrders
        {
            get => _workOrders;
            set => SetProperty(ref _workOrders, value);
        }

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public ObservableCollection<WorkOrderType> WorkOrderTypes
        {
            get => _workOrderTypes;
            set => SetProperty(ref _workOrderTypes, value);
        }

        public ObservableCollection<WorkOrderCategory> WorkOrderCategories
        {
            get => _workOrderCategories;
            set => SetProperty(ref _workOrderCategories, value);
        }

        public ObservableCollection<WorkOrderStatus> WorkOrderStatuses
        {
            get => _workOrderStatuses;
            set => SetProperty(ref _workOrderStatuses, value);
        }

        public ObservableCollection<Technician> Technicians
        {
            get => _technicians;
            set => SetProperty(ref _technicians, value);
        }

        public ObservableCollection<WorkOrderItem> WorkOrderItems
        {
            get => _workOrderItems;
            set => SetProperty(ref _workOrderItems, value);
        }

        public WorkOrder? SelectedWorkOrder
        {
            get => _selectedWorkOrder;
            set => SetProperty(ref _selectedWorkOrder, value, () => OnSelectedWorkOrderChanged());
        }

        public WorkOrder CurrentWorkOrder
        {
            get => _currentWorkOrder;
            set => SetProperty(ref _currentWorkOrder, value);
        }

        public WorkOrderItem CurrentWorkOrderItem
        {
            get => _currentWorkOrderItem;
            set => SetProperty(ref _currentWorkOrderItem, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value, () => OnPropertyChanged(nameof(CanEdit)));
        }

        public bool IsNewWorkOrder
        {
            get => _isNewWorkOrder;
            set => SetProperty(ref _isNewWorkOrder, value, () => OnPropertyChanged(nameof(CanEdit)));
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value, () => _ = LoadScheduledWorkOrdersAsync(value));
        }

        public int SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set => SetProperty(ref _selectedStatusFilter, value, () => _ = ApplyFiltersAsync());
        }

        public int SelectedTechnicianFilter
        {
            get => _selectedTechnicianFilter;
            set => SetProperty(ref _selectedTechnicianFilter, value, () => _ = ApplyFiltersAsync());
        }

        public bool CanEdit => IsEditMode || IsNewWorkOrder;
        public bool CanSave => CanEdit && IsCurrentWorkOrderValid();
        public bool CanDelete => SelectedWorkOrder != null && !IsNewWorkOrder && !IsEditMode;
        public bool CanAddItems => SelectedWorkOrder != null && CanEdit;

        #endregion

        #region Commands

        public ICommand LoadWorkOrdersCommand { get; private set; } = null!;
        public ICommand SearchCommand { get; private set; } = null!;
        public ICommand AddWorkOrderCommand { get; private set; } = null!;
        public ICommand EditWorkOrderCommand { get; private set; } = null!;
        public ICommand SaveWorkOrderCommand { get; private set; } = null!;
        public ICommand CancelEditCommand { get; private set; } = null!;
        public ICommand DeleteWorkOrderCommand { get; private set; } = null!;
        public ICommand ViewWorkOrderDetailsCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand PrintWorkOrderCommand { get; private set; } = null!;
        public ICommand CompleteWorkOrderCommand { get; private set; } = null!;
        public ICommand ScheduleWorkOrderCommand { get; private set; } = null!;

        // Work Order Items Commands
        public ICommand AddWorkOrderItemCommand { get; private set; } = null!;
        public ICommand EditWorkOrderItemCommand { get; private set; } = null!;
        public ICommand DeleteWorkOrderItemCommand { get; private set; } = null!;
        public ICommand SaveWorkOrderItemCommand { get; private set; } = null!;

        // Filter Commands
        public ICommand ClearFiltersCommand { get; private set; } = null!;
        public ICommand LoadScheduledCommand { get; private set; } = null!;

        #endregion

        #region Initialization

        private void InitializeCommands()
        {
            LoadWorkOrdersCommand = new AsyncRelayCommand(LoadWorkOrdersAsync);
            SearchCommand = new AsyncRelayCommand<string>(SearchWorkOrdersAsync);
            AddWorkOrderCommand = new RelayCommand(PrepareForNewWorkOrder);
            EditWorkOrderCommand = new RelayCommand(StartEditingWorkOrder, () => SelectedWorkOrder != null);
            SaveWorkOrderCommand = new AsyncRelayCommand(SaveWorkOrderAsync, () => CanSave);
            CancelEditCommand = new RelayCommand(CancelEditing);
            DeleteWorkOrderCommand = new AsyncRelayCommand(DeleteWorkOrderAsync, () => CanDelete);
            ViewWorkOrderDetailsCommand = new RelayCommand<WorkOrder>(ViewWorkOrderDetails);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            PrintWorkOrderCommand = new RelayCommand(PrintWorkOrder, () => SelectedWorkOrder != null);
            CompleteWorkOrderCommand = new AsyncRelayCommand(CompleteWorkOrderAsync, () => SelectedWorkOrder != null);
            ScheduleWorkOrderCommand = new RelayCommand(ScheduleWorkOrder, () => SelectedWorkOrder != null);

            AddWorkOrderItemCommand = new RelayCommand(PrepareForNewWorkOrderItem, () => CanAddItems);
            EditWorkOrderItemCommand = new RelayCommand<WorkOrderItem>(EditWorkOrderItem);
            DeleteWorkOrderItemCommand = new AsyncRelayCommand<WorkOrderItem>(DeleteWorkOrderItemAsync);
            SaveWorkOrderItemCommand = new AsyncRelayCommand(SaveWorkOrderItemAsync);

            ClearFiltersCommand = new RelayCommand(ClearFilters);
            LoadScheduledCommand = new AsyncRelayCommand<DateTime?>(LoadScheduledWorkOrdersAsync);
        }

        private async Task InitializeAsync()
        {
            try
            {
                await LoadLookupDataAsync();
                await LoadWorkOrdersAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error initializing WorkOrderViewModel");
                _dialogService.ShowMessage($"Error initializing work order view: {ex.Message}",
                    "Initialization Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task LoadLookupDataAsync()
        {
            try
            {
                var customersTask = _customerService.GetAllCustomersAsync();
                var workOrderTypesTask = _settingsService.GetWorkOrderTypesAsync();
                var workOrderCategoriesTask = _settingsService.GetWorkOrderCategoriesAsync();
                var workOrderStatusesTask = _settingsService.GetWorkOrderStatusesAsync();
                var techniciansTask = _settingsService.GetTechniciansAsync();

                await Task.WhenAll(customersTask, workOrderTypesTask, workOrderCategoriesTask,
                                 workOrderStatusesTask, techniciansTask);

                Customers = (await customersTask).ToObservableCollection();
                WorkOrderTypes = (await workOrderTypesTask).ToObservableCollection();
                WorkOrderCategories = (await workOrderCategoriesTask).ToObservableCollection();
                WorkOrderStatuses = (await workOrderStatusesTask).ToObservableCollection();
                Technicians = (await techniciansTask).ToObservableCollection();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading lookup data");
                throw;
            }
        }

        #endregion

        #region Work Order Operations

        public async Task LoadWorkOrdersAsync()
        {
            try
            {
                IsBusy = true;
                BusyMessage = "Loading work orders...";

                Logger.LogInfo("Loading work orders");

                var workOrders = await _workOrderService.GetAllWorkOrdersAsync();
                WorkOrders = workOrders.ToObservableCollection();

                Logger.LogInfo($"Loaded {WorkOrders.Count} work orders");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading work orders");
                _dialogService.ShowMessage($"Error loading work orders: {ex.Message}",
                    "Load Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SearchWorkOrdersAsync(string? searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    await LoadWorkOrdersAsync();
                    return;
                }

                IsBusy = true;
                BusyMessage = $"Searching for: {searchTerm}";

                Logger.LogInfo($"Searching work orders: {searchTerm}");

                var workOrders = await _workOrderService.SearchWorkOrdersAsync(searchTerm);
                WorkOrders = workOrders.ToObservableCollection();

                Logger.LogInfo($"Found {WorkOrders.Count} work orders matching '{searchTerm}'");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error searching work orders: {searchTerm}");
                _dialogService.ShowMessage($"Error searching work orders: {ex.Message}",
                    "Search Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void PrepareForNewWorkOrder()
        {
            try
            {
                Logger.LogInfo("Preparing for new work order");

                CurrentWorkOrder = new WorkOrder
                {
                    WorkOrderTypeId = WorkOrderTypes.FirstOrDefault()?.WorkOrderTypeId ?? 1,
                    CategoryId = WorkOrderCategories.FirstOrDefault()?.CategoryId ?? 1,
                    StatusId = (int)WorkOrderStatusEnum.Unscheduled,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Environment.UserName,
                    IsActive = true
                };

                WorkOrderItems.Clear();
                IsNewWorkOrder = true;
                IsEditMode = false;
                SelectedWorkOrder = null;

                Logger.LogInfo("Ready for new work order entry");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error preparing for new work order");
                _dialogService.ShowMessage($"Error preparing new work order form: {ex.Message}",
                    "Preparation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void StartEditingWorkOrder()
        {
            try
            {
                if (SelectedWorkOrder == null) return;

                Logger.LogInfo($"Starting edit for work order: {SelectedWorkOrder.WorkOrderId}");

                // Create a copy for editing
                CurrentWorkOrder = new WorkOrder
                {
                    WorkOrderId = SelectedWorkOrder.WorkOrderId,
                    WorkOrderNumber = SelectedWorkOrder.WorkOrderNumber,
                    CustomerId = SelectedWorkOrder.CustomerId,
                    Description = SelectedWorkOrder.Description,
                    WorkOrderTypeId = SelectedWorkOrder.WorkOrderTypeId,
                    CategoryId = SelectedWorkOrder.CategoryId,
                    StatusId = SelectedWorkOrder.StatusId,
                    TechnicianId = SelectedWorkOrder.TechnicianId,
                    ScheduledDate = SelectedWorkOrder.ScheduledDate,
                    ScheduledStartTime = SelectedWorkOrder.ScheduledStartTime,
                    EndTime = SelectedWorkOrder.EndTime,
                    EstimatedHours = SelectedWorkOrder.EstimatedHours,
                    ActualHours = SelectedWorkOrder.ActualHours,
                    CompletedDate = SelectedWorkOrder.CompletedDate,
                    Notes = SelectedWorkOrder.Notes,
                    EstimatedCost = SelectedWorkOrder.EstimatedCost,
                    ActualCost = SelectedWorkOrder.ActualCost,
                    CreatedDate = SelectedWorkOrder.CreatedDate,
                    CreatedBy = SelectedWorkOrder.CreatedBy,
                    IsActive = SelectedWorkOrder.IsActive
                };

                // Load work order items
                _ = LoadWorkOrderItemsAsync(SelectedWorkOrder.WorkOrderId);

                IsEditMode = true;
                IsNewWorkOrder = false;

                Logger.LogInfo("Work order edit mode activated");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error starting work order edit");
                _dialogService.ShowMessage($"Error starting edit: {ex.Message}",
                    "Edit Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task SaveWorkOrderAsync()
        {
            try
            {
                if (!IsCurrentWorkOrderValid())
                {
                    _dialogService.ShowMessage("Please correct the validation errors before saving.",
                        "Validation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                IsBusy = true;
                BusyMessage = IsNewWorkOrder ? "Creating work order..." : "Updating work order...";

                Logger.LogInfo($"{(IsNewWorkOrder ? "Creating" : "Updating")} work order: {CurrentWorkOrder.WorkOrderNumber}");

                WorkOrder savedWorkOrder;

                if (IsNewWorkOrder)
                {
                    savedWorkOrder = await _workOrderService.CreateWorkOrderAsync(CurrentWorkOrder);
                    WorkOrders.Insert(0, savedWorkOrder); // Add to top of list
                    Logger.LogInfo($"Work order created with ID: {savedWorkOrder.WorkOrderId}");
                }
                else
                {
                    savedWorkOrder = await _workOrderService.UpdateWorkOrderAsync(CurrentWorkOrder);

                    // Update the work order in the collection
                    var existingWorkOrder = WorkOrders.FirstOrDefault(wo => wo.WorkOrderId == savedWorkOrder.WorkOrderId);
                    if (existingWorkOrder != null)
                    {
                        var index = WorkOrders.IndexOf(existingWorkOrder);
                        WorkOrders[index] = savedWorkOrder;
                    }

                    Logger.LogInfo($"Work order updated: {savedWorkOrder.WorkOrderId}");
                }

                SelectedWorkOrder = savedWorkOrder;
                IsEditMode = false;
                IsNewWorkOrder = false;

                _dialogService.ShowMessage($"Work order {(IsNewWorkOrder ? "created" : "updated")} successfully!",
                    "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error saving work order: {CurrentWorkOrder.WorkOrderNumber}");
                _dialogService.ShowMessage($"Error saving work order: {ex.Message}",
                    "Save Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CancelEditing()
        {
            try
            {
                Logger.LogInfo("Canceling work order edit");

                if (IsNewWorkOrder)
                {
                    CurrentWorkOrder = new WorkOrder();
                    WorkOrderItems.Clear();
                }
                else if (SelectedWorkOrder != null)
                {
                    // Restore from selected work order
                    CurrentWorkOrder = SelectedWorkOrder;
                    _ = LoadWorkOrderItemsAsync(SelectedWorkOrder.WorkOrderId);
                }

                IsEditMode = false;
                IsNewWorkOrder = false;

                Logger.LogInfo("Work order edit canceled");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error canceling work order edit");
            }
        }

        private async Task DeleteWorkOrderAsync()
        {
            try
            {
                if (SelectedWorkOrder == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete work order '{SelectedWorkOrder.WorkOrderNumber}'?",
                    "Delete Work Order");

                if (!result) return;

                IsBusy = true;
                BusyMessage = "Deleting work order...";

                Logger.LogInfo($"Deleting work order: {SelectedWorkOrder.WorkOrderId}");

                var success = await _workOrderService.DeleteWorkOrderAsync(SelectedWorkOrder.WorkOrderId);

                if (success)
                {
                    WorkOrders.Remove(SelectedWorkOrder);
                    SelectedWorkOrder = null;
                    CurrentWorkOrder = new WorkOrder();
                    WorkOrderItems.Clear();

                    _dialogService.ShowMessage("Work order deleted successfully!",
                        "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                    Logger.LogInfo("Work order deleted successfully");
                }
                else
                {
                    _dialogService.ShowMessage("Work order could not be deleted.",
                        "Delete Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting work order: {SelectedWorkOrder?.WorkOrderId}");
                _dialogService.ShowMessage($"Error deleting work order: {ex.Message}",
                    "Delete Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ViewWorkOrderDetails(WorkOrder? workOrder)
        {
            try
            {
                if (workOrder == null) return;

                Logger.LogInfo($"Viewing work order details: {workOrder.WorkOrderId}");
                SelectedWorkOrder = workOrder;
                CurrentWorkOrder = workOrder;
                IsEditMode = false;
                IsNewWorkOrder = false;

                _ = LoadWorkOrderItemsAsync(workOrder.WorkOrderId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error viewing work order details: {workOrder?.WorkOrderId}");
            }
        }

        #endregion

        #region Work Order Items

        private async Task LoadWorkOrderItemsAsync(int workOrderId)
        {
            try
            {
                var items = await _workOrderService.GetWorkOrderItemsAsync(workOrderId);
                WorkOrderItems = items.ToObservableCollection();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error loading work order items: {workOrderId}");
            }
        }

        private void PrepareForNewWorkOrderItem()
        {
            try
            {
                if (SelectedWorkOrder == null) return;

                CurrentWorkOrderItem = new WorkOrderItem
                {
                    WorkOrderId = SelectedWorkOrder.WorkOrderId,
                    Quantity = 1,
                    UnitPrice = 0,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error preparing new work order item");
            }
        }

        private void EditWorkOrderItem(WorkOrderItem? item)
        {
            try
            {
                if (item == null) return;
                CurrentWorkOrderItem = item;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error editing work order item");
            }
        }

        private async Task SaveWorkOrderItemAsync()
        {
            try
            {
                if (CurrentWorkOrderItem.WorkOrderItemId == 0)
                {
                    // New item
                    var savedItem = await _workOrderService.AddWorkOrderItemAsync(CurrentWorkOrderItem);
                    WorkOrderItems.Add(savedItem);
                }
                else
                {
                    // Update existing
                    var savedItem = await _workOrderService.UpdateWorkOrderItemAsync(CurrentWorkOrderItem);
                    var existingItem = WorkOrderItems.FirstOrDefault(i => i.WorkOrderItemId == savedItem.WorkOrderItemId);
                    if (existingItem != null)
                    {
                        var index = WorkOrderItems.IndexOf(existingItem);
                        WorkOrderItems[index] = savedItem;
                    }
                }

                CurrentWorkOrderItem = new WorkOrderItem();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving work order item");
                _dialogService.ShowMessage($"Error saving work order item: {ex.Message}",
                    "Save Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task DeleteWorkOrderItemAsync(WorkOrderItem? item)
        {
            try
            {
                if (item == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete this item: '{item.Description}'?",
                    "Delete Item");

                if (!result) return;

                var success = await _workOrderService.DeleteWorkOrderItemAsync(item.WorkOrderItemId);
                if (success)
                {
                    WorkOrderItems.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting work order item");
                _dialogService.ShowMessage($"Error deleting work order item: {ex.Message}",
                    "Delete Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Filters and Calendar

        private async Task ApplyFiltersAsync()
        {
            try
            {
                var allWorkOrders = await _workOrderService.GetAllWorkOrdersAsync();
                var filteredWorkOrders = allWorkOrders.AsEnumerable();

                if (SelectedStatusFilter > 0)
                {
                    filteredWorkOrders = filteredWorkOrders.Where(wo => wo.StatusId == SelectedStatusFilter);
                }

                if (SelectedTechnicianFilter > 0)
                {
                    filteredWorkOrders = filteredWorkOrders.Where(wo => wo.TechnicianId == SelectedTechnicianFilter);
                }

                WorkOrders = filteredWorkOrders.ToObservableCollection();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error applying filters");
            }
        }

        public async Task LoadScheduledWorkOrdersAsync(DateTime? date = null)
        {
            try
            {
                var selectedDate = date ?? SelectedDate;
                IsBusy = true;
                BusyMessage = $"Loading scheduled work orders for {selectedDate:yyyy-MM-dd}...";

                var workOrders = await _workOrderService.GetScheduledWorkOrdersAsync(selectedDate);
                WorkOrders = workOrders.ToObservableCollection();

                Logger.LogInfo($"Loaded {WorkOrders.Count} scheduled work orders for {selectedDate:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading scheduled work orders");
                _dialogService.ShowMessage($"Error loading scheduled work orders: {ex.Message}",
                    "Load Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ClearFilters()
        {
            SelectedStatusFilter = 0;
            SelectedTechnicianFilter = 0;
            SearchText = string.Empty;
            _ = LoadWorkOrdersAsync();
        }

        #endregion

        #region Special Actions

        private async Task CompleteWorkOrderAsync()
        {
            try
            {
                if (SelectedWorkOrder == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Mark work order '{SelectedWorkOrder.WorkOrderNumber}' as completed?",
                    "Complete Work Order");

                if (!result) return;

                SelectedWorkOrder.StatusId = (int)WorkOrderStatusEnum.Completed;
                SelectedWorkOrder.CompletedDate = DateTime.Now;

                await _workOrderService.UpdateWorkOrderAsync(SelectedWorkOrder);

                _dialogService.ShowMessage("Work order marked as completed!",
                    "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error completing work order");
                _dialogService.ShowMessage($"Error completing work order: {ex.Message}",
                    "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ScheduleWorkOrder()
        {
            try
            {
                if (SelectedWorkOrder == null) return;

                // This would open a scheduling dialog
                Logger.LogInfo($"Scheduling work order: {SelectedWorkOrder.WorkOrderId}");
                _dialogService.ShowMessage("Scheduling functionality will be implemented in a future update.",
                    "Coming Soon", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error scheduling work order");
            }
        }

        private void PrintWorkOrder()
        {
            try
            {
                if (SelectedWorkOrder == null) return;

                // This would generate and print a work order report
                Logger.LogInfo($"Printing work order: {SelectedWorkOrder.WorkOrderId}");
                _dialogService.ShowMessage("Print functionality will be implemented in a future update.",
                    "Coming Soon", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error printing work order");
            }
        }

        #endregion

        #region Validation

        private bool IsCurrentWorkOrderValid()
        {
            try
            {
                // Required field validation
                if (CurrentWorkOrder.CustomerId <= 0)
                    return false;

                if (string.IsNullOrWhiteSpace(CurrentWorkOrder.Description))
                    return false;

                if (CurrentWorkOrder.WorkOrderTypeId <= 0)
                    return false;

                if (CurrentWorkOrder.CategoryId <= 0)
                    return false;

                if (CurrentWorkOrder.StatusId <= 0)
                    return false;

                // Date validation
                if (CurrentWorkOrder.ScheduledDate.HasValue && CurrentWorkOrder.ScheduledDate.Value < DateTime.Today)
                {
                    // Allow past dates only if status is completed
                    if (CurrentWorkOrder.StatusId != (int)WorkOrderStatusEnum.Completed)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error validating work order");
                return false;
            }
        }

        #endregion

        #region Event Handlers

        private void OnSelectedWorkOrderChanged()
        {
            try
            {
                if (SelectedWorkOrder != null && !IsEditMode && !IsNewWorkOrder)
                {
                    CurrentWorkOrder = SelectedWorkOrder;
                    _ = LoadWorkOrderItemsAsync(SelectedWorkOrder.WorkOrderId);
                    Logger.LogInfo($"Selected work order changed to: {SelectedWorkOrder.WorkOrderId}");
                }

                // Update command can execute states
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanSave));
                OnPropertyChanged(nameof(CanDelete));
                OnPropertyChanged(nameof(CanAddItems));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error handling selected work order change");
            }
        }

        #endregion

        #region Refresh

        public async Task RefreshAsync()
        {
            try
            {
                await LoadLookupDataAsync();
                await LoadWorkOrdersAsync();
                Logger.LogInfo("Work order view refreshed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error refreshing work order view");
                _dialogService.ShowMessage($"Error refreshing data: {ex.Message}",
                    "Refresh Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        public string GetWorkOrderDisplayName(WorkOrder workOrder)
        {
            if (workOrder == null) return string.Empty;
            return $"{workOrder.WorkOrderNumber} - {workOrder.Description}";
        }

        public string GetCustomerDisplayName(int customerId)
        {
            var customer = Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null) return "Unknown Customer";

            if (!string.IsNullOrWhiteSpace(customer.CompanyName))
            {
                return $"{customer.CompanyName} ({customer.FirstName} {customer.LastName})";
            }

            return $"{customer.FirstName} {customer.LastName}";
        }

        public string GetWorkOrderTypeDisplayName(int workOrderTypeId)
        {
            var workOrderType = WorkOrderTypes.FirstOrDefault(wot => wot.WorkOrderTypeId == workOrderTypeId);
            return workOrderType?.TypeName ?? "Unknown";
        }

        public string GetWorkOrderCategoryDisplayName(int categoryId)
        {
            var category = WorkOrderCategories.FirstOrDefault(woc => woc.CategoryId == categoryId);
            return category?.CategoryName ?? "Unknown";
        }

        public string GetWorkOrderStatusDisplayName(int statusId)
        {
            var status = WorkOrderStatuses.FirstOrDefault(wos => wos.StatusId == statusId);
            return status?.StatusName ?? "Unknown";
        }

        public string GetTechnicianDisplayName(int? technicianId)
        {
            if (!technicianId.HasValue) return "Unassigned";

            var technician = Technicians.FirstOrDefault(t => t.TechnicianId == technicianId.Value);
            return technician?.FullName ?? "Unknown Technician";
        }

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

        public string GetPriorityDisplayName(int priority)
        {
            return priority switch
            {
                1 => "Low",
                2 => "Normal",
                3 => "High",
                4 => "Critical",
                _ => "Normal"
            };
        }

        public string GetPriorityColor(int priority)
        {
            return priority switch
            {
                1 => "#4CAF50", // Green - Low
                2 => "#2196F3", // Blue - Normal
                3 => "#FF9800", // Orange - High
                4 => "#F44336", // Red - Critical
                _ => "#2196F3"  // Blue - Default
            };
        }

        public decimal GetWorkOrderItemsTotal()
        {
            return WorkOrderItems.Sum(item => item.TotalPrice);
        }

        public string FormatCurrency(decimal amount)
        {
            return amount.ToString("C2");
        }

        public string FormatDuration(decimal? hours)
        {
            if (!hours.HasValue) return "N/A";

            var totalMinutes = (int)(hours.Value * 60);
            var hrs = totalMinutes / 60;
            var mins = totalMinutes % 60;

            if (hrs > 0)
                return $"{hrs}h {mins}m";
            else
                return $"{mins}m";
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

        public bool IsOverdue(WorkOrder workOrder)
        {
            return workOrder.ScheduledDate.HasValue &&
                   workOrder.ScheduledDate.Value < DateTime.Today &&
                   workOrder.StatusId != (int)WorkOrderStatusEnum.Completed &&
                   workOrder.StatusId != (int)WorkOrderStatusEnum.Canceled;
        }

        public string GetWorkOrderAge(WorkOrder workOrder)
        {
            var age = DateTime.Now - workOrder.CreatedDate;

            if (age.TotalDays >= 1)
                return $"{(int)age.TotalDays} days old";
            else if (age.TotalHours >= 1)
                return $"{(int)age.TotalHours} hours old";
            else
                return "New";
        }

        public void ClearSearch()
        {
            SearchText = string.Empty;
            _ = LoadWorkOrdersAsync();
        }

        public async Task LoadWorkOrdersByCustomerAsync(int customerId)
        {
            try
            {
                IsBusy = true;
                BusyMessage = "Loading customer work orders...";

                var workOrders = await _workOrderService.GetWorkOrdersByCustomerAsync(customerId);
                WorkOrders = workOrders.ToObservableCollection();

                Logger.LogInfo($"Loaded {WorkOrders.Count} work orders for customer {customerId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error loading work orders for customer: {customerId}");
                _dialogService.ShowMessage($"Error loading customer work orders: {ex.Message}",
                    "Load Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadWorkOrdersByTechnicianAsync(int technicianId)
        {
            try
            {
                IsBusy = true;
                BusyMessage = "Loading technician work orders...";

                var workOrders = await _workOrderService.GetWorkOrdersByTechnicianAsync(technicianId);
                WorkOrders = workOrders.ToObservableCollection();

                Logger.LogInfo($"Loaded {WorkOrders.Count} work orders for technician {technicianId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error loading work orders for technician: {technicianId}");
                _dialogService.ShowMessage($"Error loading technician work orders: {ex.Message}",
                    "Load Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadWorkOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                IsBusy = true;
                BusyMessage = $"Loading work orders from {startDate:MM/dd/yyyy} to {endDate:MM/dd/yyyy}...";

                var workOrders = await _workOrderService.GetWorkOrdersByDateRangeAsync(startDate, endDate);
                WorkOrders = workOrders.ToObservableCollection();

                Logger.LogInfo($"Loaded {WorkOrders.Count} work orders for date range {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error loading work orders for date range: {startDate} to {endDate}");
                _dialogService.ShowMessage($"Error loading work orders for date range: {ex.Message}",
                    "Load Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void AssignTechnician(int technicianId)
        {
            try
            {
                if (SelectedWorkOrder == null) return;

                SelectedWorkOrder.TechnicianId = technicianId;
                _ = _workOrderService.UpdateWorkOrderAsync(SelectedWorkOrder);

                Logger.LogInfo($"Assigned technician {technicianId} to work order {SelectedWorkOrder.WorkOrderId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error assigning technician");
                _dialogService.ShowMessage($"Error assigning technician: {ex.Message}",
                    "Assignment Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public void UpdateStatus(int statusId)
        {
            try
            {
                if (SelectedWorkOrder == null) return;

                SelectedWorkOrder.StatusId = statusId;

                // Set completion date if status is completed
                if (statusId == (int)WorkOrderStatusEnum.Completed)
                {
                    SelectedWorkOrder.CompletedDate = DateTime.Now;
                }

                _ = _workOrderService.UpdateWorkOrderAsync(SelectedWorkOrder);

                Logger.LogInfo($"Updated status to {statusId} for work order {SelectedWorkOrder.WorkOrderId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating status");
                _dialogService.ShowMessage($"Error updating status: {ex.Message}",
                    "Status Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        public ObservableCollection<WorkOrder> GetOverdueWorkOrders()
        {
            return WorkOrders.Where(IsOverdue).ToObservableCollection();
        }

        public ObservableCollection<WorkOrder> GetTodaysWorkOrders()
        {
            return WorkOrders.Where(wo => wo.ScheduledDate.HasValue &&
                                         wo.ScheduledDate.Value.Date == DateTime.Today)
                             .ToObservableCollection();
        }

        public ObservableCollection<WorkOrder> GetUpcomingWorkOrders(int days = 7)
        {
            var endDate = DateTime.Today.AddDays(days);
            return WorkOrders.Where(wo => wo.ScheduledDate.HasValue &&
                                         wo.ScheduledDate.Value.Date > DateTime.Today &&
                                         wo.ScheduledDate.Value.Date <= endDate &&
                                         wo.StatusId != (int)WorkOrderStatusEnum.Completed &&
                                         wo.StatusId != (int)WorkOrderStatusEnum.Canceled)
                             .OrderBy(wo => wo.ScheduledDate)
                             .ToObservableCollection();
        }

        #endregion
    }
}