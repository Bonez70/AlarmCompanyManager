// ViewModels/CustomerViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using AlarmCompanyManager.Models;
using AlarmCompanyManager.Services;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager.ViewModels
{
    public class CustomerViewModel : ViewModelBase
    {
        private readonly ICustomerService _customerService;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<Customer> _customers = new();
        private ObservableCollection<CustomerType> _customerTypes = new();
        private ObservableCollection<ContactType> _contactTypes = new();
        private Customer? _selectedCustomer;
        private Customer _currentCustomer = new();
        private string _searchText = string.Empty;
        private bool _isEditMode = false;
        private bool _isNewCustomer = false;

        public CustomerViewModel(
            ICustomerService customerService,
            ISettingsService settingsService,
            IDialogService dialogService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            InitializeCommands();
            _ = InitializeAsync();
        }

        #region Properties

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

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

        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value, () => OnSelectedCustomerChanged());
        }

        public Customer CurrentCustomer
        {
            get => _currentCustomer;
            set => SetProperty(ref _currentCustomer, value);
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

        public bool IsNewCustomer
        {
            get => _isNewCustomer;
            set => SetProperty(ref _isNewCustomer, value, () => OnPropertyChanged(nameof(CanEdit)));
        }

        public bool CanEdit => IsEditMode || IsNewCustomer;
        public bool CanSave => CanEdit && IsCurrentCustomerValid();
        public bool CanDelete => SelectedCustomer != null && !IsNewCustomer && !IsEditMode;

        #endregion

        #region Commands

        public ICommand LoadCustomersCommand { get; private set; } = null!;
        public ICommand SearchCommand { get; private set; } = null!;
        public ICommand AddCustomerCommand { get; private set; } = null!;
        public ICommand EditCustomerCommand { get; private set; } = null!;
        public ICommand SaveCustomerCommand { get; private set; } = null!;
        public ICommand CancelEditCommand { get; private set; } = null!;
        public ICommand DeleteCustomerCommand { get; private set; } = null!;
        public ICommand ViewCustomerDetailsCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;

        #endregion

        #region Initialization

        private void InitializeCommands()
        {
            LoadCustomersCommand = new AsyncRelayCommand(LoadCustomersAsync);
            SearchCommand = new AsyncRelayCommand<string>(SearchCustomersAsync);
            AddCustomerCommand = new RelayCommand(PrepareForNewCustomer);
            EditCustomerCommand = new RelayCommand(StartEditingCustomer, () => SelectedCustomer != null);
            SaveCustomerCommand = new AsyncRelayCommand(SaveCustomerAsync, () => CanSave);
            CancelEditCommand = new RelayCommand(CancelEditing);
            DeleteCustomerCommand = new AsyncRelayCommand(DeleteCustomerAsync, () => CanDelete);
            ViewCustomerDetailsCommand = new RelayCommand<Customer>(ViewCustomerDetails);
            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        private async Task InitializeAsync()
        {
            try
            {
                await LoadLookupDataAsync();
                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error initializing CustomerViewModel");
                _dialogService.ShowMessage($"Error initializing customer view: {ex.Message}",
                    "Initialization Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task LoadLookupDataAsync()
        {
            try
            {
                var customerTypesTask = _settingsService.GetCustomerTypesAsync();
                var contactTypesTask = _settingsService.GetContactTypesAsync();

                await Task.WhenAll(customerTypesTask, contactTypesTask);

                CustomerTypes = (await customerTypesTask).ToObservableCollection();
                ContactTypes = (await contactTypesTask).ToObservableCollection();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading lookup data");
                throw;
            }
        }

        #endregion

        #region Customer Operations

        public async Task LoadCustomersAsync()
        {
            try
            {
                IsBusy = true;
                BusyMessage = "Loading customers...";

                Logger.LogInfo("Loading customers");

                var customers = await _customerService.GetAllCustomersAsync();
                Customers = customers.ToObservableCollection();

                Logger.LogInfo($"Loaded {Customers.Count} customers");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading customers");
                _dialogService.ShowMessage($"Error loading customers: {ex.Message}",
                    "Load Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SearchCustomersAsync(string? searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    await LoadCustomersAsync();
                    return;
                }

                IsBusy = true;
                BusyMessage = $"Searching for: {searchTerm}";

                Logger.LogInfo($"Searching customers: {searchTerm}");

                var customers = await _customerService.SearchCustomersAsync(searchTerm);
                Customers = customers.ToObservableCollection();

                Logger.LogInfo($"Found {Customers.Count} customers matching '{searchTerm}'");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error searching customers: {searchTerm}");
                _dialogService.ShowMessage($"Error searching customers: {ex.Message}",
                    "Search Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void PrepareForNewCustomer()
        {
            try
            {
                Logger.LogInfo("Preparing for new customer");

                CurrentCustomer = new Customer
                {
                    CustomerTypeId = CustomerTypes.FirstOrDefault()?.CustomerTypeId ?? 1,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                IsNewCustomer = true;
                IsEditMode = false;
                SelectedCustomer = null;

                Logger.LogInfo("Ready for new customer entry");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error preparing for new customer");
                _dialogService.ShowMessage($"Error preparing new customer form: {ex.Message}",
                    "Preparation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void StartEditingCustomer()
        {
            try
            {
                if (SelectedCustomer == null) return;

                Logger.LogInfo($"Starting edit for customer: {SelectedCustomer.CustomerId}");

                // Create a copy for editing
                CurrentCustomer = new Customer
                {
                    CustomerId = SelectedCustomer.CustomerId,
                    CompanyName = SelectedCustomer.CompanyName,
                    FirstName = SelectedCustomer.FirstName,
                    LastName = SelectedCustomer.LastName,
                    Street = SelectedCustomer.Street,
                    City = SelectedCustomer.City,
                    State = SelectedCustomer.State,
                    ZipCode = SelectedCustomer.ZipCode,
                    County = SelectedCustomer.County,
                    EmailAddress = SelectedCustomer.EmailAddress,
                    HomePhone = SelectedCustomer.HomePhone,
                    BusinessPhone = SelectedCustomer.BusinessPhone,
                    CellPhone = SelectedCustomer.CellPhone,
                    CustomerTypeId = SelectedCustomer.CustomerTypeId,
                    LinkedCustomerId = SelectedCustomer.LinkedCustomerId,
                    CreatedDate = SelectedCustomer.CreatedDate,
                    ModifiedDate = SelectedCustomer.ModifiedDate,
                    IsActive = SelectedCustomer.IsActive
                };

                IsEditMode = true;
                IsNewCustomer = false;

                Logger.LogInfo("Customer edit mode activated");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error starting customer edit");
                _dialogService.ShowMessage($"Error starting edit: {ex.Message}",
                    "Edit Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task SaveCustomerAsync()
        {
            try
            {
                if (!IsCurrentCustomerValid())
                {
                    _dialogService.ShowMessage("Please correct the validation errors before saving.",
                        "Validation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }

                IsBusy = true;
                BusyMessage = IsNewCustomer ? "Creating customer..." : "Updating customer...";

                Logger.LogInfo($"{(IsNewCustomer ? "Creating" : "Updating")} customer: {CurrentCustomer.FirstName} {CurrentCustomer.LastName}");

                Customer savedCustomer;

                if (IsNewCustomer)
                {
                    savedCustomer = await _customerService.CreateCustomerAsync(CurrentCustomer);
                    Customers.Add(savedCustomer);
                    Logger.LogInfo($"Customer created with ID: {savedCustomer.CustomerId}");
                }
                else
                {
                    savedCustomer = await _customerService.UpdateCustomerAsync(CurrentCustomer);

                    // Update the customer in the collection
                    var existingCustomer = Customers.FirstOrDefault(c => c.CustomerId == savedCustomer.CustomerId);
                    if (existingCustomer != null)
                    {
                        var index = Customers.IndexOf(existingCustomer);
                        Customers[index] = savedCustomer;
                    }

                    Logger.LogInfo($"Customer updated: {savedCustomer.CustomerId}");
                }

                SelectedCustomer = savedCustomer;
                IsEditMode = false;
                IsNewCustomer = false;

                _dialogService.ShowMessage($"Customer {(IsNewCustomer ? "created" : "updated")} successfully!",
                    "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error saving customer: {CurrentCustomer.FirstName} {CurrentCustomer.LastName}");
                _dialogService.ShowMessage($"Error saving customer: {ex.Message}",
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
                Logger.LogInfo("Canceling customer edit");

                if (IsNewCustomer)
                {
                    CurrentCustomer = new Customer();
                }
                else if (SelectedCustomer != null)
                {
                    // Restore from selected customer
                    CurrentCustomer = SelectedCustomer;
                }

                IsEditMode = false;
                IsNewCustomer = false;

                Logger.LogInfo("Customer edit canceled");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error canceling customer edit");
            }
        }

        private async Task DeleteCustomerAsync()
        {
            try
            {
                if (SelectedCustomer == null) return;

                var result = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete customer '{SelectedCustomer.FirstName} {SelectedCustomer.LastName}'?",
                    "Delete Customer");

                if (!result) return;

                IsBusy = true;
                BusyMessage = "Deleting customer...";

                Logger.LogInfo($"Deleting customer: {SelectedCustomer.CustomerId}");

                var success = await _customerService.DeleteCustomerAsync(SelectedCustomer.CustomerId);

                if (success)
                {
                    Customers.Remove(SelectedCustomer);
                    SelectedCustomer = null;
                    CurrentCustomer = new Customer();

                    _dialogService.ShowMessage("Customer deleted successfully!",
                        "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                    Logger.LogInfo("Customer deleted successfully");
                }
                else
                {
                    _dialogService.ShowMessage("Customer could not be deleted.",
                        "Delete Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting customer: {SelectedCustomer?.CustomerId}");
                _dialogService.ShowMessage($"Error deleting customer: {ex.Message}",
                    "Delete Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ViewCustomerDetails(Customer? customer)
        {
            try
            {
                if (customer == null) return;

                Logger.LogInfo($"Viewing customer details: {customer.CustomerId}");
                SelectedCustomer = customer;
                CurrentCustomer = customer;
                IsEditMode = false;
                IsNewCustomer = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error viewing customer details: {customer?.CustomerId}");
            }
        }

        #endregion

        #region Validation

        private bool IsCurrentCustomerValid()
        {
            try
            {
                // Basic required field validation
                if (string.IsNullOrWhiteSpace(CurrentCustomer.FirstName))
                    return false;

                if (string.IsNullOrWhiteSpace(CurrentCustomer.LastName))
                    return false;

                if (string.IsNullOrWhiteSpace(CurrentCustomer.Street))
                    return false;

                if (string.IsNullOrWhiteSpace(CurrentCustomer.City))
                    return false;

                if (string.IsNullOrWhiteSpace(CurrentCustomer.State))
                    return false;

                if (string.IsNullOrWhiteSpace(CurrentCustomer.ZipCode))
                    return false;

                // Email validation if provided
                if (!string.IsNullOrWhiteSpace(CurrentCustomer.EmailAddress) &&
                    !ValidationHelper.IsValidEmail(CurrentCustomer.EmailAddress))
                    return false;

                // Phone validation if provided
                if (!string.IsNullOrWhiteSpace(CurrentCustomer.HomePhone) &&
                    !ValidationHelper.IsValidPhoneNumber(CurrentCustomer.HomePhone))
                    return false;

                if (!string.IsNullOrWhiteSpace(CurrentCustomer.BusinessPhone) &&
                    !ValidationHelper.IsValidPhoneNumber(CurrentCustomer.BusinessPhone))
                    return false;

                if (!string.IsNullOrWhiteSpace(CurrentCustomer.CellPhone) &&
                    !ValidationHelper.IsValidPhoneNumber(CurrentCustomer.CellPhone))
                    return false;

                // Zip code validation
                if (!ValidationHelper.IsValidZipCode(CurrentCustomer.ZipCode))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error validating customer");
                return false;
            }
        }

        #endregion

        #region Event Handlers

        private void OnSelectedCustomerChanged()
        {
            try
            {
                if (SelectedCustomer != null && !IsEditMode && !IsNewCustomer)
                {
                    CurrentCustomer = SelectedCustomer;
                    Logger.LogInfo($"Selected customer changed to: {SelectedCustomer.CustomerId}");
                }

                // Update command can execute states
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanSave));
                OnPropertyChanged(nameof(CanDelete));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error handling selected customer change");
            }
        }

        #endregion

        #region Refresh

        public async Task RefreshAsync()
        {
            try
            {
                await LoadLookupDataAsync();
                await LoadCustomersAsync();
                Logger.LogInfo("Customer view refreshed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error refreshing customer view");
                _dialogService.ShowMessage($"Error refreshing data: {ex.Message}",
                    "Refresh Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        public string GetCustomerDisplayName(Customer customer)
        {
            if (customer == null) return string.Empty;

            if (!string.IsNullOrWhiteSpace(customer.CompanyName))
            {
                return $"{customer.CompanyName} ({customer.FirstName} {customer.LastName})";
            }

            return $"{customer.FirstName} {customer.LastName}";
        }

        public string GetCustomerAddress(Customer customer)
        {
            if (customer == null) return string.Empty;

            return $"{customer.Street}, {customer.City}, {customer.State} {customer.ZipCode}";
        }

        public string GetCustomerTypeDisplayName(int customerTypeId)
        {
            var customerType = CustomerTypes.FirstOrDefault(ct => ct.CustomerTypeId == customerTypeId);
            return customerType?.TypeName ?? "Unknown";
        }

        public string FormatPhoneNumber(string? phoneNumber)
        {
            return ValidationHelper.FormatPhoneNumber(phoneNumber);
        }

        public void ClearSearch()
        {
            SearchText = string.Empty;
            _ = LoadCustomersAsync();
        }

        #endregion
    }
}