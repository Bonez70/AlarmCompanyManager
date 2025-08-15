// ViewModels/SettingsViewModel.cs
using System.Collections.ObjectModel;
using System.Windows.Input;
using AlarmCompanyManager.Models;
using AlarmCompanyManager.Services;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        // Collections for all lookup tables
        private ObservableCollection<CustomerType> _customerTypes = new();
        private ObservableCollection<ContactType> _contactTypes = new();
        private ObservableCollection<PanelType> _panelTypes = new();
        private ObservableCollection<MonitoringType> _monitoringTypes = new();
        private ObservableCollection<DeviceType> _deviceTypes = new();
        private ObservableCollection<CommunicatorType> _communicatorTypes = new();
        private ObservableCollection<WorkOrderType> _workOrderTypes = new();
        private ObservableCollection<WorkOrderCategory> _workOrderCategories = new();
        private ObservableCollection<WorkOrderStatus> _workOrderStatuses = new();
        private ObservableCollection<Technician> _technicians = new();
        private ObservableCollection<Communicator> _communicators = new();

        // Current items being edited
        private CustomerType _currentCustomerType = new();
        private ContactType _currentContactType = new();
        private PanelType _currentPanelType = new();
        private MonitoringType _currentMonitoringType = new();
        private DeviceType _currentDeviceType = new();
        private CommunicatorType _currentCommunicatorType = new();
        private WorkOrderType _currentWorkOrderType = new();
        private WorkOrderCategory _currentWorkOrderCategory = new();
        private WorkOrderStatus _currentWorkOrderStatus = new();
        private Technician _currentTechnician = new();
        private Communicator _currentCommunicator = new();

        // Selected items
        private CustomerType? _selectedCustomerType;
        private ContactType? _selectedContactType;
        private PanelType? _selectedPanelType;
        private MonitoringType? _selectedMonitoringType;
        private DeviceType? _selectedDeviceType;
        private CommunicatorType? _selectedCommunicatorType;
        private WorkOrderType? _selectedWorkOrderType;
        private WorkOrderCategory? _selectedWorkOrderCategory;
        private WorkOrderStatus? _selectedWorkOrderStatus;
        private Technician? _selectedTechnician;
        private Communicator? _selectedCommunicator;

        // Edit modes
        private bool _isEditingCustomerType = false;
        private bool _isEditingContactType = false;
        private bool _isEditingPanelType = false;
        private bool _isEditingMonitoringType = false;
        private bool _isEditingDeviceType = false;
        private bool _isEditingCommunicatorType = false;
        private bool _isEditingWorkOrderType = false;
        private bool _isEditingWorkOrderCategory = false;
        private bool _isEditingWorkOrderStatus = false;
        private bool _isEditingTechnician = false;
        private bool _isEditingCommunicator = false;

        // Active tab
        private int _selectedTabIndex = 0;

        public SettingsViewModel(
            ISettingsService settingsService,
            IDialogService dialogService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            InitializeCommands();
            _ = InitializeAsync();
        }

        #region Properties

        // Collections
        public ObservableCollection<CustomerType> CustomerTypes
        {
            get => _customerTypes;
            set => SetProperty(ref _customerTypes, value);
        }

        public ObservableCollection<ContactType> ContactTypes
        {
            get => _contactTypes;
            set => SetProperty(ref _contactTypes, value);
        }

        public ObservableCollection<PanelType> PanelTypes
        {
            get => _panelTypes;
            set => SetProperty(ref _panelTypes, value);
        }

        public ObservableCollection<MonitoringType> MonitoringTypes
        {
            get => _monitoringTypes;
            set => SetProperty(ref _monitoringTypes, value);
        }

        public ObservableCollection<DeviceType> DeviceTypes
        {
            get => _deviceTypes;
            set => SetProperty(ref _deviceTypes, value);
        }

        public ObservableCollection<CommunicatorType> CommunicatorTypes
        {
            get => _communicatorTypes;
            set => SetProperty(ref _communicatorTypes, value);
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

        public ObservableCollection<Communicator> Communicators
        {
            get => _communicators;
            set => SetProperty(ref _communicators, value);
        }

        // Current items being edited
        public CustomerType CurrentCustomerType
        {
            get => _currentCustomerType;
            set => SetProperty(ref _currentCustomerType, value);
        }

        public ContactType CurrentContactType
        {
            get => _currentContactType;
            set => SetProperty(ref _currentContactType, value);
        }

        public PanelType CurrentPanelType
        {
            get => _currentPanelType;
            set => SetProperty(ref _currentPanelType, value);
        }

        public MonitoringType CurrentMonitoringType
        {
            get => _currentMonitoringType;
            set => SetProperty(ref _currentMonitoringType, value);
        }

        public DeviceType CurrentDeviceType
        {
            get => _currentDeviceType;
            set => SetProperty(ref _currentDeviceType, value);
        }

        public CommunicatorType CurrentCommunicatorType
        {
            get => _currentCommunicatorType;
            set => SetProperty(ref _currentCommunicatorType, value);
        }

        public WorkOrderType CurrentWorkOrderType
        {
            get => _currentWorkOrderType;
            set => SetProperty(ref _currentWorkOrderType, value);
        }

        public WorkOrderCategory CurrentWorkOrderCategory
        {
            get => _currentWorkOrderCategory;
            set => SetProperty(ref _currentWorkOrderCategory, value);
        }

        public WorkOrderStatus CurrentWorkOrderStatus
        {
            get => _currentWorkOrderStatus;
            set => SetProperty(ref _currentWorkOrderStatus, value);
        }

        public Technician CurrentTechnician
        {
            get => _currentTechnician;
            set => SetProperty(ref _currentTechnician, value);
        }

        public Communicator CurrentCommunicator
        {
            get => _currentCommunicator;
            set => SetProperty(ref _currentCommunicator, value);
        }

        // Selected items
        public CustomerType? SelectedCustomerType
        {
            get => _selectedCustomerType;
            set => SetProperty(ref _selectedCustomerType, value);
        }

        public ContactType? SelectedContactType
        {
            get => _selectedContactType;
            set => SetProperty(ref _selectedContactType, value);
        }

        public PanelType? SelectedPanelType
        {
            get => _selectedPanelType;
            set => SetProperty(ref _selectedPanelType, value);
        }

        public MonitoringType? SelectedMonitoringType
        {
            get => _selectedMonitoringType;
            set => SetProperty(ref _selectedMonitoringType, value);
        }

        public DeviceType? SelectedDeviceType
        {
            get => _selectedDeviceType;
            set => SetProperty(ref _selectedDeviceType, value);
        }

        public CommunicatorType? SelectedCommunicatorType
        {
            get => _selectedCommunicatorType;
            set => SetProperty(ref _selectedCommunicatorType, value);
        }

        public WorkOrderType? SelectedWorkOrderType
        {
            get => _selectedWorkOrderType;
            set => SetProperty(ref _selectedWorkOrderType, value);
        }

        public WorkOrderCategory? SelectedWorkOrderCategory
        {
            get => _selectedWorkOrderCategory;
            set => SetProperty(ref _selectedWorkOrderCategory, value);
        }

        public WorkOrderStatus? SelectedWorkOrderStatus
        {
            get => _selectedWorkOrderStatus;
            set => SetProperty(ref _selectedWorkOrderStatus, value);
        }

        public Technician? SelectedTechnician
        {
            get => _selectedTechnician;
            set => SetProperty(ref _selectedTechnician, value);
        }

        public Communicator? SelectedCommunicator
        {
            get => _selectedCommunicator;
            set => SetProperty(ref _selectedCommunicator, value);
        }

        // Edit modes
        public bool IsEditingCustomerType
        {
            get => _isEditingCustomerType;
            set => SetProperty(ref _isEditingCustomerType, value);
        }

        public bool IsEditingContactType
        {
            get => _isEditingContactType;
            set => SetProperty(ref _isEditingContactType, value);
        }

        public bool IsEditingPanelType
        {
            get => _isEditingPanelType;
            set => SetProperty(ref _isEditingPanelType, value);
        }

        public bool IsEditingMonitoringType
        {
            get => _isEditingMonitoringType;
            set => SetProperty(ref _isEditingMonitoringType, value);
        }

        public bool IsEditingDeviceType
        {
            get => _isEditingDeviceType;
            set => SetProperty(ref _isEditingDeviceType, value);
        }

        public bool IsEditingCommunicatorType
        {
            get => _isEditingCommunicatorType;
            set => SetProperty(ref _isEditingCommunicatorType, value);
        }

        public bool IsEditingWorkOrderType
        {
            get => _isEditingWorkOrderType;
            set => SetProperty(ref _isEditingWorkOrderType, value);
        }

        public bool IsEditingWorkOrderCategory
        {
            get => _isEditingWorkOrderCategory;
            set => SetProperty(ref _isEditingWorkOrderCategory, value);
        }

        public bool IsEditingWorkOrderStatus
        {
            get => _isEditingWorkOrderStatus;
            set => SetProperty(ref _isEditingWorkOrderStatus, value);
        }

        public bool IsEditingTechnician
        {
            get => _isEditingTechnician;
            set => SetProperty(ref _isEditingTechnician, value);
        }

        public bool IsEditingCommunicator
        {
            get => _isEditingCommunicator;
            set => SetProperty(ref _isEditingCommunicator, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        #endregion

        #region Commands

        // Customer Type Commands
        public ICommand AddCustomerTypeCommand { get; private set; } = null!;
        public ICommand EditCustomerTypeCommand { get; private set; } = null!;
        public ICommand SaveCustomerTypeCommand { get; private set; } = null!;
        public ICommand CancelCustomerTypeCommand { get; private set; } = null!;
        public ICommand DeleteCustomerTypeCommand { get; private set; } = null!;

        // Contact Type Commands
        public ICommand AddContactTypeCommand { get; private set; } = null!;
        public ICommand EditContactTypeCommand { get; private set; } = null!;
        public ICommand SaveContactTypeCommand { get; private set; } = null!;
        public ICommand CancelContactTypeCommand { get; private set; } = null!;
        public ICommand DeleteContactTypeCommand { get; private set; } = null!;

        // Panel Type Commands
        public ICommand AddPanelTypeCommand { get; private set; } = null!;
        public ICommand EditPanelTypeCommand { get; private set; } = null!;
        public ICommand SavePanelTypeCommand { get; private set; } = null!;
        public ICommand CancelPanelTypeCommand { get; private set; } = null!;
        public ICommand DeletePanelTypeCommand { get; private set; } = null!;

        // Monitoring Type Commands
        public ICommand AddMonitoringTypeCommand { get; private set; } = null!;
        public ICommand EditMonitoringTypeCommand { get; private set; } = null!;
        public ICommand SaveMonitoringTypeCommand { get; private set; } = null!;
        public ICommand CancelMonitoringTypeCommand { get; private set; } = null!;
        public ICommand DeleteMonitoringTypeCommand { get; private set; } = null!;

        // Device Type Commands
        public ICommand AddDeviceTypeCommand { get; private set; } = null!;
        public ICommand EditDeviceTypeCommand { get; private set; } = null!;
        public ICommand SaveDeviceTypeCommand { get; private set; } = null!;
        public ICommand CancelDeviceTypeCommand { get; private set; } = null!;
        public ICommand DeleteDeviceTypeCommand { get; private set; } = null!;

        // Communicator Type Commands
        public ICommand AddCommunicatorTypeCommand { get; private set; } = null!;
        public ICommand EditCommunicatorTypeCommand { get; private set; } = null!;
        public ICommand SaveCommunicatorTypeCommand { get; private set; } = null!;
        public ICommand CancelCommunicatorTypeCommand { get; private set; } = null!;
        public ICommand DeleteCommunicatorTypeCommand { get; private set; } = null!;

        // Work Order Type Commands
        public ICommand AddWorkOrderTypeCommand { get; private set; } = null!;
        public ICommand EditWorkOrderTypeCommand { get; private set; } = null!;
        public ICommand SaveWorkOrderTypeCommand { get; private set; } = null!;
        public ICommand CancelWorkOrderTypeCommand { get; private set; } = null!;
        public ICommand DeleteWorkOrderTypeCommand { get; private set; } = null!;

        // Work Order Category Commands
        public ICommand AddWorkOrderCategoryCommand { get; private set; } = null!;
        public ICommand EditWorkOrderCategoryCommand { get; private set; } = null!;
        public ICommand SaveWorkOrderCategoryCommand { get; private set; } = null!;
        public ICommand CancelWorkOrderCategoryCommand { get; private set; } = null!;
        public ICommand DeleteWorkOrderCategoryCommand { get; private set; } = null!;

        // Work Order Status Commands
        public ICommand AddWorkOrderStatusCommand { get; private set; } = null!;
        public ICommand EditWorkOrderStatusCommand { get; private set; } = null!;
        public ICommand SaveWorkOrderStatusCommand { get; private set; } = null!;
        public ICommand CancelWorkOrderStatusCommand { get; private set; } = null!;
        public ICommand DeleteWorkOrderStatusCommand { get; private set; } = null!;

        // Technician Commands
        public ICommand AddTechnicianCommand { get; private set; } = null!;
        public ICommand EditTechnicianCommand { get; private set; } = null!;
        public ICommand SaveTechnicianCommand { get; private set; } = null!;
        public ICommand CancelTechnicianCommand { get; private set; } = null!;
        public ICommand DeleteTechnicianCommand { get; private set; } = null!;

        // Communicator Commands
        public ICommand AddCommunicatorCommand { get; private set; } = null!;
        public ICommand EditCommunicatorCommand { get; private set; } = null!;
        public ICommand SaveCommunicatorCommand { get; private set; } = null!;
        public ICommand CancelCommunicatorCommand { get; private set; } = null!;
        public ICommand DeleteCommunicatorCommand { get; private set; } = null!;

        // General Commands
        public ICommand RefreshCommand { get; private set; } = null!;

        #endregion

        #region Initialization

        private void InitializeCommands()
        {
            // Customer Type Commands
            AddCustomerTypeCommand = new RelayCommand(() => StartAddingCustomerType());
            EditCustomerTypeCommand = new RelayCommand(() => StartEditingCustomerType());
            SaveCustomerTypeCommand = new AsyncRelayCommand(SaveCustomerTypeAsync);
            CancelCustomerTypeCommand = new RelayCommand(() => CancelEditingCustomerType());
            DeleteCustomerTypeCommand = new AsyncRelayCommand(DeleteCustomerTypeAsync);

            // Contact Type Commands
            AddContactTypeCommand = new RelayCommand(() => StartAddingContactType());
            EditContactTypeCommand = new RelayCommand(() => StartEditingContactType());
            SaveContactTypeCommand = new AsyncRelayCommand(SaveContactTypeAsync);
            CancelContactTypeCommand = new RelayCommand(() => CancelEditingContactType());
            DeleteContactTypeCommand = new AsyncRelayCommand(DeleteContactTypeAsync);

            // Panel Type Commands
            AddPanelTypeCommand = new RelayCommand(() => StartAddingPanelType());
            EditPanelTypeCommand = new RelayCommand(() => StartEditingPanelType());
            SavePanelTypeCommand = new AsyncRelayCommand(SavePanelTypeAsync);
            CancelPanelTypeCommand = new RelayCommand(() => CancelEditingPanelType());
            DeletePanelTypeCommand = new AsyncRelayCommand(DeletePanelTypeAsync);

            // Monitoring Type Commands
            AddMonitoringTypeCommand = new RelayCommand(() => StartAddingMonitoringType());
            EditMonitoringTypeCommand = new RelayCommand(() => StartEditingMonitoringType());
            SaveMonitoringTypeCommand = new AsyncRelayCommand(SaveMonitoringTypeAsync);
            CancelMonitoringTypeCommand = new RelayCommand(() => CancelEditingMonitoringType());
            DeleteMonitoringTypeCommand = new AsyncRelayCommand(DeleteMonitoringTypeAsync);

            // Device Type Commands
            AddDeviceTypeCommand = new RelayCommand(() => StartAddingDeviceType());
            EditDeviceTypeCommand = new RelayCommand(() => StartEditingDeviceType());
            SaveDeviceTypeCommand = new AsyncRelayCommand(SaveDeviceTypeAsync);
            CancelDeviceTypeCommand = new RelayCommand(() => CancelEditingDeviceType());
            DeleteDeviceTypeCommand = new AsyncRelayCommand(DeleteDeviceTypeAsync);

            // Communicator Type Commands
            AddCommunicatorTypeCommand = new RelayCommand(() => StartAddingCommunicatorType());
            EditCommunicatorTypeCommand = new RelayCommand(() => StartEditingCommunicatorType());
            SaveCommunicatorTypeCommand = new AsyncRelayCommand(SaveCommunicatorTypeAsync);
            CancelCommunicatorTypeCommand = new RelayCommand(() => CancelEditingCommunicatorType());
            DeleteCommunicatorTypeCommand = new AsyncRelayCommand(DeleteCommunicatorTypeAsync);

            // Work Order Type Commands
            AddWorkOrderTypeCommand = new RelayCommand(() => StartAddingWorkOrderType());
            EditWorkOrderTypeCommand = new RelayCommand(() => StartEditingWorkOrderType());
            SaveWorkOrderTypeCommand = new AsyncRelayCommand(SaveWorkOrderTypeAsync);
            CancelWorkOrderTypeCommand = new RelayCommand(() => CancelEditingWorkOrderType());
            DeleteWorkOrderTypeCommand = new AsyncRelayCommand(DeleteWorkOrderTypeAsync);

            // Work Order Category Commands
            AddWorkOrderCategoryCommand = new RelayCommand(() => StartAddingWorkOrderCategory());
            EditWorkOrderCategoryCommand = new RelayCommand(() => StartEditingWorkOrderCategory());
            SaveWorkOrderCategoryCommand = new AsyncRelayCommand(SaveWorkOrderCategoryAsync);
            CancelWorkOrderCategoryCommand = new RelayCommand(() => CancelEditingWorkOrderCategory());
            DeleteWorkOrderCategoryCommand = new AsyncRelayCommand(DeleteWorkOrderCategoryAsync);

            // Work Order Status Commands
            AddWorkOrderStatusCommand = new RelayCommand(() => StartAddingWorkOrderStatus());
            EditWorkOrderStatusCommand = new RelayCommand(() => StartEditingWorkOrderStatus());
            SaveWorkOrderStatusCommand = new AsyncRelayCommand(SaveWorkOrderStatusAsync);
            CancelWorkOrderStatusCommand = new RelayCommand(() => CancelEditingWorkOrderStatus());
            DeleteWorkOrderStatusCommand = new AsyncRelayCommand(DeleteWorkOrderStatusAsync);

            // Technician Commands
            AddTechnicianCommand = new RelayCommand(() => StartAddingTechnician());
            EditTechnicianCommand = new RelayCommand(() => StartEditingTechnician());
            SaveTechnicianCommand = new AsyncRelayCommand(SaveTechnicianAsync);
            CancelTechnicianCommand = new RelayCommand(() => CancelEditingTechnician());
            DeleteTechnicianCommand = new AsyncRelayCommand(DeleteTechnicianAsync);

            // Communicator Commands
            AddCommunicatorCommand = new RelayCommand(() => StartAddingCommunicator());
            EditCommunicatorCommand = new RelayCommand(() => StartEditingCommunicator());
            SaveCommunicatorCommand = new AsyncRelayCommand(SaveCommunicatorAsync);
            CancelCommunicatorCommand = new RelayCommand(() => CancelEditingCommunicator());
            DeleteCommunicatorCommand = new AsyncRelayCommand(DeleteCommunicatorAsync);

            // General Commands
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        private async Task InitializeAsync()
        {
            try
            {
                await LoadAllDataAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error initializing SettingsViewModel");
                _dialogService.ShowMessage($"Error initializing settings: {ex.Message}",
                    "Initialization Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task LoadAllDataAsync()
        {
            try
            {
                IsBusy = true;
                BusyMessage = "Loading settings data...";

                var tasks = new[]
                {
                    LoadCustomerTypesAsync(),
                    LoadContactTypesAsync(),
                    LoadPanelTypesAsync(),
                    LoadMonitoringTypesAsync(),
                    LoadDeviceTypesAsync(),
                    LoadCommunicatorTypesAsync(),
                    LoadWorkOrderTypesAsync(),
                    LoadWorkOrderCategoriesAsync(),
                    LoadWorkOrderStatusesAsync(),
                    LoadTechniciansAsync(),
                    LoadCommunicatorsAsync()
                };

                await Task.WhenAll(tasks);

                Logger.LogInfo("All settings data loaded successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading settings data");
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Data Loading Methods

        private async Task LoadCustomerTypesAsync()
        {
            var data = await _settingsService.GetCustomerTypesAsync();
            CustomerTypes = data.ToObservableCollection();
        }

        private async Task LoadContactTypesAsync()
        {
            var data = await _settingsService.GetContactTypesAsync();
            ContactTypes = data.ToObservableCollection();
        }

        private async Task LoadPanelTypesAsync()
        {
            var data = await _settingsService.GetPanelTypesAsync();
            PanelTypes = data.ToObservableCollection();
        }

        private async Task LoadMonitoringTypesAsync()
        {
            var data = await _settingsService.GetMonitoringTypesAsync();
            MonitoringTypes = data.ToObservableCollection();
        }

        private async Task LoadDeviceTypesAsync()
        {
            var data = await _settingsService.GetDeviceTypesAsync();
            DeviceTypes = data.ToObservableCollection();
        }

        private async Task LoadCommunicatorTypesAsync()
        {
            var data = await _settingsService.GetCommunicatorTypesAsync();
            CommunicatorTypes = data.ToObservableCollection();
        }

        private async Task LoadWorkOrderTypesAsync()
        {
            var data = await _settingsService.GetWorkOrderTypesAsync();
            WorkOrderTypes = data.ToObservableCollection();
        }

        private async Task LoadWorkOrderCategoriesAsync()
        {
            var data = await _settingsService.GetWorkOrderCategoriesAsync();
            WorkOrderCategories = data.ToObservableCollection();
        }

        private async Task LoadWorkOrderStatusesAsync()
        {
            var data = await _settingsService.GetWorkOrderStatusesAsync();
            WorkOrderStatuses = data.ToObservableCollection();
        }

        private async Task LoadTechniciansAsync()
        {
            var data = await _settingsService.GetTechniciansAsync();
            Technicians = data.ToObservableCollection();
        }

        private async Task LoadCommunicatorsAsync()
        {
            var data = await _settingsService.GetCommunicatorsAsync();
            Communicators = data.ToObservableCollection();
        }

        #endregion

        #region Customer Type Operations

        private void StartAddingCustomerType()
        {
            CurrentCustomerType = new CustomerType { IsActive = true };
            IsEditingCustomerType = true;
        }

        private void StartEditingCustomerType()
        {
            if (SelectedCustomerType != null)
            {
                CurrentCustomerType = new CustomerType
                {
                    CustomerTypeId = SelectedCustomerType.CustomerTypeId,
                    TypeName = SelectedCustomerType.TypeName,
                    Description = SelectedCustomerType.Description,
                    IsActive = SelectedCustomerType.IsActive
                };
                IsEditingCustomerType = true;
            }
        }

        private async Task SaveCustomerTypeAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentCustomerType.TypeName))
                {
                    _dialogService.ShowMessage("Type name is required.", "Validation Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                CustomerType savedItem;
                if (CurrentCustomerType.CustomerTypeId == 0)
                {
                    savedItem = await _settingsService.AddCustomerTypeAsync(CurrentCustomerType);
                    CustomerTypes.Add(savedItem);
                }
                else
                {
                    savedItem = await _settingsService.UpdateCustomerTypeAsync(CurrentCustomerType);
                    var existing = CustomerTypes.FirstOrDefault(ct => ct.CustomerTypeId == savedItem.CustomerTypeId);
                    if (existing != null)
                    {
                        var index = CustomerTypes.IndexOf(existing);
                        CustomerTypes[index] = savedItem;
                    }
                }

                IsEditingCustomerType = false;
                SelectedCustomerType = savedItem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving customer type");
                _dialogService.ShowMessage($"Error saving customer type: {ex.Message}", "Save Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CancelEditingCustomerType()
        {
            IsEditingCustomerType = false;
            CurrentCustomerType = new CustomerType();
        }

        private async Task DeleteCustomerTypeAsync()
        {
            try
            {
                if (SelectedCustomerType == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete '{SelectedCustomerType.TypeName}'?",
                    "Delete Customer Type");

                if (!result) return;

                var success = await _settingsService.DeleteCustomerTypeAsync(SelectedCustomerType.CustomerTypeId);
                if (success)
                {
                    CustomerTypes.Remove(SelectedCustomerType);
                    SelectedCustomerType = null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting customer type");
                _dialogService.ShowMessage($"Error deleting customer type: {ex.Message}", "Delete Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Contact Type Operations

        private void StartAddingContactType()
        {
            CurrentContactType = new ContactType { IsActive = true };
            IsEditingContactType = true;
        }

        private void StartEditingContactType()
        {
            if (SelectedContactType != null)
            {
                CurrentContactType = new ContactType
                {
                    ContactTypeId = SelectedContactType.ContactTypeId,
                    TypeName = SelectedContactType.TypeName,
                    Description = SelectedContactType.Description,
                    IsActive = SelectedContactType.IsActive
                };
                IsEditingContactType = true;
            }
        }

        private async Task SaveContactTypeAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentContactType.TypeName))
                {
                    _dialogService.ShowMessage("Type name is required.", "Validation Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                ContactType savedItem;
                if (CurrentContactType.ContactTypeId == 0)
                {
                    savedItem = await _settingsService.AddContactTypeAsync(CurrentContactType);
                    ContactTypes.Add(savedItem);
                }
                else
                {
                    savedItem = await _settingsService.UpdateContactTypeAsync(CurrentContactType);
                    var existing = ContactTypes.FirstOrDefault(ct => ct.ContactTypeId == savedItem.ContactTypeId);
                    if (existing != null)
                    {
                        var index = ContactTypes.IndexOf(existing);
                        ContactTypes[index] = savedItem;
                    }
                }

                IsEditingContactType = false;
                SelectedContactType = savedItem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving contact type");
                _dialogService.ShowMessage($"Error saving contact type: {ex.Message}", "Save Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CancelEditingContactType()
        {
            IsEditingContactType = false;
            CurrentContactType = new ContactType();
        }

        private async Task DeleteContactTypeAsync()
        {
            try
            {
                if (SelectedContactType == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete '{SelectedContactType.TypeName}'?",
                    "Delete Contact Type");

                if (!result) return;

                var success = await _settingsService.DeleteContactTypeAsync(SelectedContactType.ContactTypeId);
                if (success)
                {
                    ContactTypes.Remove(SelectedContactType);
                    SelectedContactType = null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting contact type");
                _dialogService.ShowMessage($"Error deleting contact type: {ex.Message}", "Delete Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Panel Type Operations

        private void StartAddingPanelType()
        {
            CurrentPanelType = new PanelType { IsActive = true };
            IsEditingPanelType = true;
        }

        private void StartEditingPanelType()
        {
            if (SelectedPanelType != null)
            {
                CurrentPanelType = new PanelType
                {
                    PanelTypeId = SelectedPanelType.PanelTypeId,
                    Manufacturer = SelectedPanelType.Manufacturer,
                    ModelNumber = SelectedPanelType.ModelNumber,
                    Description = SelectedPanelType.Description,
                    IsActive = SelectedPanelType.IsActive
                };
                IsEditingPanelType = true;
            }
        }

        private async Task SavePanelTypeAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentPanelType.Manufacturer) ||
                    string.IsNullOrWhiteSpace(CurrentPanelType.ModelNumber))
                {
                    _dialogService.ShowMessage("Manufacturer and Model Number are required.", "Validation Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                PanelType savedItem;
                if (CurrentPanelType.PanelTypeId == 0)
                {
                    savedItem = await _settingsService.AddPanelTypeAsync(CurrentPanelType);
                    PanelTypes.Add(savedItem);
                }
                else
                {
                    savedItem = await _settingsService.UpdatePanelTypeAsync(CurrentPanelType);
                    var existing = PanelTypes.FirstOrDefault(pt => pt.PanelTypeId == savedItem.PanelTypeId);
                    if (existing != null)
                    {
                        var index = PanelTypes.IndexOf(existing);
                        PanelTypes[index] = savedItem;
                    }
                }

                IsEditingPanelType = false;
                SelectedPanelType = savedItem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving panel type");
                _dialogService.ShowMessage($"Error saving panel type: {ex.Message}", "Save Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CancelEditingPanelType()
        {
            IsEditingPanelType = false;
            CurrentPanelType = new PanelType();
        }

        private async Task DeletePanelTypeAsync()
        {
            try
            {
                if (SelectedPanelType == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete '{SelectedPanelType.Manufacturer} {SelectedPanelType.ModelNumber}'?",
                    "Delete Panel Type");

                if (!result) return;

                var success = await _settingsService.DeletePanelTypeAsync(SelectedPanelType.PanelTypeId);
                if (success)
                {
                    PanelTypes.Remove(SelectedPanelType);
                    SelectedPanelType = null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting panel type");
                _dialogService.ShowMessage($"Error deleting panel type: {ex.Message}", "Delete Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Technician Operations

        private void StartAddingTechnician()
        {
            CurrentTechnician = new Technician
            {
                IsActive = true,
                CreatedDate = DateTime.Now,
                HireDate = DateTime.Today
            };
            IsEditingTechnician = true;
        }

        private void StartEditingTechnician()
        {
            if (SelectedTechnician != null)
            {
                CurrentTechnician = new Technician
                {
                    TechnicianId = SelectedTechnician.TechnicianId,
                    FirstName = SelectedTechnician.FirstName,
                    LastName = SelectedTechnician.LastName,
                    EmailAddress = SelectedTechnician.EmailAddress,
                    PhoneNumber = SelectedTechnician.PhoneNumber,
                    CellPhone = SelectedTechnician.CellPhone,
                    EmployeeNumber = SelectedTechnician.EmployeeNumber,
                    HireDate = SelectedTechnician.HireDate,
                    Specializations = SelectedTechnician.Specializations,
                    Certifications = SelectedTechnician.Certifications,
                    IsActive = SelectedTechnician.IsActive,
                    CreatedDate = SelectedTechnician.CreatedDate
                };
                IsEditingTechnician = true;
            }
        }

        private async Task SaveTechnicianAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentTechnician.FirstName) ||
                    string.IsNullOrWhiteSpace(CurrentTechnician.LastName))
                {
                    _dialogService.ShowMessage("First Name and Last Name are required.", "Validation Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                // Validate email if provided
                if (!string.IsNullOrWhiteSpace(CurrentTechnician.EmailAddress) &&
                    !ValidationHelper.IsValidEmail(CurrentTechnician.EmailAddress))
                {
                    _dialogService.ShowMessage("Please enter a valid email address.", "Validation Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                Technician savedItem;
                if (CurrentTechnician.TechnicianId == 0)
                {
                    savedItem = await _settingsService.AddTechnicianAsync(CurrentTechnician);
                    Technicians.Add(savedItem);
                }
                else
                {
                    savedItem = await _settingsService.UpdateTechnicianAsync(CurrentTechnician);
                    var existing = Technicians.FirstOrDefault(t => t.TechnicianId == savedItem.TechnicianId);
                    if (existing != null)
                    {
                        var index = Technicians.IndexOf(existing);
                        Technicians[index] = savedItem;
                    }
                }

                IsEditingTechnician = false;
                SelectedTechnician = savedItem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving technician");
                _dialogService.ShowMessage($"Error saving technician: {ex.Message}", "Save Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CancelEditingTechnician()
        {
            IsEditingTechnician = false;
            CurrentTechnician = new Technician();
        }

        private async Task DeleteTechnicianAsync()
        {
            try
            {
                if (SelectedTechnician == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete technician '{SelectedTechnician.FullName}'?",
                    "Delete Technician");

                if (!result) return;

                var success = await _settingsService.DeleteTechnicianAsync(SelectedTechnician.TechnicianId);
                if (success)
                {
                    Technicians.Remove(SelectedTechnician);
                    SelectedTechnician = null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting technician");
                _dialogService.ShowMessage($"Error deleting technician: {ex.Message}", "Delete Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Generic CRUD Operations for Simple Types

        // These methods handle the simpler lookup types that only have TypeName and Description
        private async Task SaveSimpleTypeAsync<T>(
            T currentItem,
            Func<T, Task<T>> addMethod,
            Func<T, Task<T>> updateMethod,
            ObservableCollection<T> collection,
            Action<bool> setEditMode,
            Action<T> setSelected,
            Func<T, int> getId,
            Func<T, string> getName)
            where T : class, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(getName(currentItem)))
                {
                    _dialogService.ShowMessage("Name is required.", "Validation Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                T savedItem;
                if (getId(currentItem) == 0)
                {
                    savedItem = await addMethod(currentItem);
                    collection.Add(savedItem);
                }
                else
                {
                    savedItem = await updateMethod(currentItem);
                    var existing = collection.FirstOrDefault(item => getId(item) == getId(savedItem));
                    if (existing != null)
                    {
                        var index = collection.IndexOf(existing);
                        collection[index] = savedItem;
                    }
                }

                setEditMode(false);
                setSelected(savedItem);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error saving {typeof(T).Name}");
                _dialogService.ShowMessage($"Error saving {typeof(T).Name}: {ex.Message}", "Save Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task DeleteSimpleTypeAsync<T>(
            T selectedItem,
            Func<int, Task<bool>> deleteMethod,
            ObservableCollection<T> collection,
            Action<T?> setSelected,
            Func<T, int> getId,
            Func<T, string> getName,
            string typeName)
            where T : class
        {
            try
            {
                if (selectedItem == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete '{getName(selectedItem)}'?",
                    $"Delete {typeName}");

                if (!result) return;

                var success = await deleteMethod(getId(selectedItem));
                if (success)
                {
                    collection.Remove(selectedItem);
                    setSelected(null);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting {typeName}");
                _dialogService.ShowMessage($"Error deleting {typeName}: {ex.Message}", "Delete Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Monitoring Type Operations

        private void StartAddingMonitoringType()
        {
            CurrentMonitoringType = new MonitoringType { IsActive = true };
            IsEditingMonitoringType = true;
        }

        private void StartEditingMonitoringType()
        {
            if (SelectedMonitoringType != null)
            {
                CurrentMonitoringType = new MonitoringType
                {
                    MonitoringTypeId = SelectedMonitoringType.MonitoringTypeId,
                    TypeName = SelectedMonitoringType.TypeName,
                    Description = SelectedMonitoringType.Description,
                    IsActive = SelectedMonitoringType.IsActive
                };
                IsEditingMonitoringType = true;
            }
        }

        private async Task SaveMonitoringTypeAsync()
        {
            await SaveSimpleTypeAsync(
                CurrentMonitoringType,
                _settingsService.AddMonitoringTypeAsync,
                _settingsService.UpdateMonitoringTypeAsync,
                MonitoringTypes,
                (editing) => IsEditingMonitoringType = editing,
                (selected) => SelectedMonitoringType = selected,
                (mt) => mt.MonitoringTypeId,
                (mt) => mt.TypeName
            );
        }

        private void CancelEditingMonitoringType()
        {
            IsEditingMonitoringType = false;
            CurrentMonitoringType = new MonitoringType();
        }

        private async Task DeleteMonitoringTypeAsync()
        {
            await DeleteSimpleTypeAsync(
                SelectedMonitoringType,
                _settingsService.DeleteMonitoringTypeAsync,
                MonitoringTypes,
                (selected) => SelectedMonitoringType = selected,
                (mt) => mt.MonitoringTypeId,
                (mt) => mt.TypeName,
                "Monitoring Type"
            );
        }

        #endregion

        #region Device Type Operations

        private void StartAddingDeviceType()
        {
            CurrentDeviceType = new DeviceType { IsActive = true };
            IsEditingDeviceType = true;
        }

        private void StartEditingDeviceType()
        {
            if (SelectedDeviceType != null)
            {
                CurrentDeviceType = new DeviceType
                {
                    DeviceTypeId = SelectedDeviceType.DeviceTypeId,
                    TypeName = SelectedDeviceType.TypeName,
                    Description = SelectedDeviceType.Description,
                    IsActive = SelectedDeviceType.IsActive
                };
                IsEditingDeviceType = true;
            }
        }

        private async Task SaveDeviceTypeAsync()
        {
            await SaveSimpleTypeAsync(
                CurrentDeviceType,
                _settingsService.AddDeviceTypeAsync,
                _settingsService.UpdateDeviceTypeAsync,
                DeviceTypes,
                (editing) => IsEditingDeviceType = editing,
                (selected) => SelectedDeviceType = selected,
                (dt) => dt.DeviceTypeId,
                (dt) => dt.TypeName
            );
        }

        private void CancelEditingDeviceType()
        {
            IsEditingDeviceType = false;
            CurrentDeviceType = new DeviceType();
        }

        private async Task DeleteDeviceTypeAsync()
        {
            await DeleteSimpleTypeAsync(
                SelectedDeviceType,
                _settingsService.DeleteDeviceTypeAsync,
                DeviceTypes,
                (selected) => SelectedDeviceType = selected,
                (dt) => dt.DeviceTypeId,
                (dt) => dt.TypeName,
                "Device Type"
            );
        }

        #endregion

        #region Communicator Type Operations

        private void StartAddingCommunicatorType()
        {
            CurrentCommunicatorType = new CommunicatorType { IsActive = true };
            IsEditingCommunicatorType = true;
        }

        private void StartEditingCommunicatorType()
        {
            if (SelectedCommunicatorType != null)
            {
                CurrentCommunicatorType = new CommunicatorType
                {
                    CommunicatorTypeId = SelectedCommunicatorType.CommunicatorTypeId,
                    TypeName = SelectedCommunicatorType.TypeName,
                    Description = SelectedCommunicatorType.Description,
                    IsActive = SelectedCommunicatorType.IsActive
                };
                IsEditingCommunicatorType = true;
            }
        }

        private async Task SaveCommunicatorTypeAsync()
        {
            await SaveSimpleTypeAsync(
                CurrentCommunicatorType,
                _settingsService.AddCommunicatorTypeAsync,
                _settingsService.UpdateCommunicatorTypeAsync,
                CommunicatorTypes,
                (editing) => IsEditingCommunicatorType = editing,
                (selected) => SelectedCommunicatorType = selected,
                (ct) => ct.CommunicatorTypeId,
                (ct) => ct.TypeName
            );
        }

        private void CancelEditingCommunicatorType()
        {
            IsEditingCommunicatorType = false;
            CurrentCommunicatorType = new CommunicatorType();
        }

        private async Task DeleteCommunicatorTypeAsync()
        {
            await DeleteSimpleTypeAsync(
                SelectedCommunicatorType,
                _settingsService.DeleteCommunicatorTypeAsync,
                CommunicatorTypes,
                (selected) => SelectedCommunicatorType = selected,
                (ct) => ct.CommunicatorTypeId,
                (ct) => ct.TypeName,
                "Communicator Type"
            );
        }

        #endregion

        #region Work Order Type Operations

        private void StartAddingWorkOrderType()
        {
            CurrentWorkOrderType = new WorkOrderType { IsActive = true };
            IsEditingWorkOrderType = true;
        }

        private void StartEditingWorkOrderType()
        {
            if (SelectedWorkOrderType != null)
            {
                CurrentWorkOrderType = new WorkOrderType
                {
                    WorkOrderTypeId = SelectedWorkOrderType.WorkOrderTypeId,
                    TypeName = SelectedWorkOrderType.TypeName,
                    Description = SelectedWorkOrderType.Description,
                    IsActive = SelectedWorkOrderType.IsActive
                };
                IsEditingWorkOrderType = true;
            }
        }

        private async Task SaveWorkOrderTypeAsync()
        {
            await SaveSimpleTypeAsync(
                CurrentWorkOrderType,
                _settingsService.AddWorkOrderTypeAsync,
                _settingsService.UpdateWorkOrderTypeAsync,
                WorkOrderTypes,
                (editing) => IsEditingWorkOrderType = editing,
                (selected) => SelectedWorkOrderType = selected,
                (wot) => wot.WorkOrderTypeId,
                (wot) => wot.TypeName
            );
        }

        private void CancelEditingWorkOrderType()
        {
            IsEditingWorkOrderType = false;
            CurrentWorkOrderType = new WorkOrderType();
        }

        private async Task DeleteWorkOrderTypeAsync()
        {
            await DeleteSimpleTypeAsync(
                SelectedWorkOrderType,
                _settingsService.DeleteWorkOrderTypeAsync,
                WorkOrderTypes,
                (selected) => SelectedWorkOrderType = selected,
                (wot) => wot.WorkOrderTypeId,
                (wot) => wot.TypeName,
                "Work Order Type"
            );
        }

        #endregion

        #region Work Order Category Operations

        private void StartAddingWorkOrderCategory()
        {
            CurrentWorkOrderCategory = new WorkOrderCategory { IsActive = true };
            IsEditingWorkOrderCategory = true;
        }

        private void StartEditingWorkOrderCategory()
        {
            if (SelectedWorkOrderCategory != null)
            {
                CurrentWorkOrderCategory = new WorkOrderCategory
                {
                    CategoryId = SelectedWorkOrderCategory.CategoryId,
                    CategoryName = SelectedWorkOrderCategory.CategoryName,
                    Description = SelectedWorkOrderCategory.Description,
                    IsActive = SelectedWorkOrderCategory.IsActive
                };
                IsEditingWorkOrderCategory = true;
            }
        }

        private async Task SaveWorkOrderCategoryAsync()
        {
            await SaveSimpleTypeAsync(
                CurrentWorkOrderCategory,
                _settingsService.AddWorkOrderCategoryAsync,
                _settingsService.UpdateWorkOrderCategoryAsync,
                WorkOrderCategories,
                (editing) => IsEditingWorkOrderCategory = editing,
                (selected) => SelectedWorkOrderCategory = selected,
                (woc) => woc.CategoryId,
                (woc) => woc.CategoryName
            );
        }

        private void CancelEditingWorkOrderCategory()
        {
            IsEditingWorkOrderCategory = false;
            CurrentWorkOrderCategory = new WorkOrderCategory();
        }

        private async Task DeleteWorkOrderCategoryAsync()
        {
            await DeleteSimpleTypeAsync(
                SelectedWorkOrderCategory,
                _settingsService.DeleteWorkOrderCategoryAsync,
                WorkOrderCategories,
                (selected) => SelectedWorkOrderCategory = selected,
                (woc) => woc.CategoryId,
                (woc) => woc.CategoryName,
                "Work Order Category"
            );
        }

        #endregion

        #region Work Order Status Operations

        private void StartAddingWorkOrderStatus()
        {
            CurrentWorkOrderStatus = new WorkOrderStatus
            {
                IsActive = true,
                SortOrder = WorkOrderStatuses.Count + 1,
                ColorCode = "#2196F3"
            };
            IsEditingWorkOrderStatus = true;
        }

        private void StartEditingWorkOrderStatus()
        {
            if (SelectedWorkOrderStatus != null)
            {
                CurrentWorkOrderStatus = new WorkOrderStatus
                {
                    StatusId = SelectedWorkOrderStatus.StatusId,
                    StatusName = SelectedWorkOrderStatus.StatusName,
                    Description = SelectedWorkOrderStatus.Description,
                    ColorCode = SelectedWorkOrderStatus.ColorCode,
                    SortOrder = SelectedWorkOrderStatus.SortOrder,
                    IsActive = SelectedWorkOrderStatus.IsActive
                };
                IsEditingWorkOrderStatus = true;
            }
        }

        private async Task SaveWorkOrderStatusAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentWorkOrderStatus.StatusName))
                {
                    _dialogService.ShowMessage("Status name is required.", "Validation Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                WorkOrderStatus savedItem;
                if (CurrentWorkOrderStatus.StatusId == 0)
                {
                    savedItem = await _settingsService.AddWorkOrderStatusAsync(CurrentWorkOrderStatus);
                    WorkOrderStatuses.Add(savedItem);
                }
                else
                {
                    savedItem = await _settingsService.UpdateWorkOrderStatusAsync(CurrentWorkOrderStatus);
                    var existing = WorkOrderStatuses.FirstOrDefault(wos => wos.StatusId == savedItem.StatusId);
                    if (existing != null)
                    {
                        var index = WorkOrderStatuses.IndexOf(existing);
                        WorkOrderStatuses[index] = savedItem;
                    }
                }

                IsEditingWorkOrderStatus = false;
                SelectedWorkOrderStatus = savedItem;

                // Re-sort the collection by SortOrder
                var sortedStatuses = WorkOrderStatuses.OrderBy(wos => wos.SortOrder).ToList();
                WorkOrderStatuses.Clear();
                foreach (var status in sortedStatuses)
                {
                    WorkOrderStatuses.Add(status);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving work order status");
                _dialogService.ShowMessage($"Error saving work order status: {ex.Message}", "Save Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CancelEditingWorkOrderStatus()
        {
            IsEditingWorkOrderStatus = false;
            CurrentWorkOrderStatus = new WorkOrderStatus();
        }

        private async Task DeleteWorkOrderStatusAsync()
        {
            await DeleteSimpleTypeAsync(
                SelectedWorkOrderStatus,
                _settingsService.DeleteWorkOrderStatusAsync,
                WorkOrderStatuses,
                (selected) => SelectedWorkOrderStatus = selected,
                (wos) => wos.StatusId,
                (wos) => wos.StatusName,
                "Work Order Status"
            );
        }

        #endregion

        #region Communicator Operations

        private void StartAddingCommunicator()
        {
            CurrentCommunicator = new Communicator
            {
                IsActive = true,
                CreatedDate = DateTime.Now,
                CommunicatorTypeId = CommunicatorTypes.FirstOrDefault()?.CommunicatorTypeId ?? 1
            };
            IsEditingCommunicator = true;
        }

        private void StartEditingCommunicator()
        {
            if (SelectedCommunicator != null)
            {
                CurrentCommunicator = new Communicator
                {
                    CommunicatorId = SelectedCommunicator.CommunicatorId,
                    CommunicatorTypeId = SelectedCommunicator.CommunicatorTypeId,
                    Manufacturer = SelectedCommunicator.Manufacturer,
                    ModelNumber = SelectedCommunicator.ModelNumber,
                    RadioId = SelectedCommunicator.RadioId,
                    IpAddress = SelectedCommunicator.IpAddress,
                    Gateway = SelectedCommunicator.Gateway,
                    Subnet = SelectedCommunicator.Subnet,
                    PhoneNumber1 = SelectedCommunicator.PhoneNumber1,
                    PhoneNumber2 = SelectedCommunicator.PhoneNumber2,
                    Notes = SelectedCommunicator.Notes,
                    IsActive = SelectedCommunicator.IsActive,
                    CreatedDate = SelectedCommunicator.CreatedDate
                };
                IsEditingCommunicator = true;
            }
        }

        private async Task SaveCommunicatorAsync()
        {
            try
            {
                if (CurrentCommunicator.CommunicatorTypeId <= 0)
                {
                    _dialogService.ShowMessage("Communicator type is required.", "Validation Error",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                Communicator savedItem;
                if (CurrentCommunicator.CommunicatorId == 0)
                {
                    savedItem = await _settingsService.AddCommunicatorAsync(CurrentCommunicator);
                    Communicators.Add(savedItem);
                }
                else
                {
                    savedItem = await _settingsService.UpdateCommunicatorAsync(CurrentCommunicator);
                    var existing = Communicators.FirstOrDefault(c => c.CommunicatorId == savedItem.CommunicatorId);
                    if (existing != null)
                    {
                        var index = Communicators.IndexOf(existing);
                        Communicators[index] = savedItem;
                    }
                }

                IsEditingCommunicator = false;
                SelectedCommunicator = savedItem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving communicator");
                _dialogService.ShowMessage($"Error saving communicator: {ex.Message}", "Save Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CancelEditingCommunicator()
        {
            IsEditingCommunicator = false;
            CurrentCommunicator = new Communicator();
        }

        private async Task DeleteCommunicatorAsync()
        {
            try
            {
                if (SelectedCommunicator == null) return;

                var displayName = !string.IsNullOrWhiteSpace(SelectedCommunicator.Manufacturer) &&
                                 !string.IsNullOrWhiteSpace(SelectedCommunicator.ModelNumber)
                    ? $"{SelectedCommunicator.Manufacturer} {SelectedCommunicator.ModelNumber}"
                    : "this communicator";

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete {displayName}?",
                    "Delete Communicator");

                if (!result) return;

                var success = await _settingsService.DeleteCommunicatorAsync(SelectedCommunicator.CommunicatorId);
                if (success)
                {
                    Communicators.Remove(SelectedCommunicator);
                    SelectedCommunicator = null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting communicator");
                _dialogService.ShowMessage($"Error deleting communicator: {ex.Message}", "Delete Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Refresh

        public async Task RefreshAsync()
        {
            try
            {
                await LoadAllDataAsync();
                Logger.LogInfo("Settings view refreshed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error refreshing settings view");
                _dialogService.ShowMessage($"Error refreshing data: {ex.Message}",
                    "Refresh Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        public string GetCommunicatorDisplayName(Communicator communicator)
        {
            if (communicator == null) return "Unknown";

            var typeName = CommunicatorTypes.FirstOrDefault(ct => ct.CommunicatorTypeId == communicator.CommunicatorTypeId)?.TypeName ?? "Unknown Type";

            if (!string.IsNullOrWhiteSpace(communicator.Manufacturer) && !string.IsNullOrWhiteSpace(communicator.ModelNumber))
            {
                return $"{typeName} - {communicator.Manufacturer} {communicator.ModelNumber}";
            }

            return typeName;
        }

        public void NavigateToTab(int tabIndex)
        {
            SelectedTabIndex = tabIndex;
        }

        #endregion
    }
}