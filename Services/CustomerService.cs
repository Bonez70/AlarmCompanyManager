using Microsoft.EntityFrameworkCore;
using AlarmCompanyManager.Data;
using AlarmCompanyManager.Models;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AlarmCompanyContext _context;

        public CustomerService(AlarmCompanyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            try
            {
                Logger.LogInfo("Retrieving all customers");
                return await _context.Customers
                    .Include(c => c.CustomerType)
                    .Include(c => c.LinkedCustomer)
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving all customers");
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                Logger.LogInfo($"Retrieving customer with ID: {customerId}");
                return await _context.Customers
                    .Include(c => c.CustomerType)
                    .Include(c => c.LinkedCustomer)
                    .Include(c => c.Contacts.Where(contact => contact.IsActive))
                        .ThenInclude(contact => contact.ContactType)
                    .Include(c => c.SecuritySystems.Where(ss => ss.IsActive))
                        .ThenInclude(ss => ss.PanelType)
                    .Include(c => c.SecuritySystems.Where(ss => ss.IsActive))
                        .ThenInclude(ss => ss.MonitoringType)
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving customer with ID: {customerId}");
                throw;
            }
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            try
            {
                Logger.LogInfo($"Creating new customer: {customer.FirstName} {customer.LastName}");

                customer.CreatedDate = DateTime.Now;
                customer.IsActive = true;

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                Logger.LogInfo($"Customer created successfully with ID: {customer.CustomerId}");
                return customer;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating customer: {customer.FirstName} {customer.LastName}");
                throw;
            }
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                Logger.LogInfo($"Updating customer with ID: {customer.CustomerId}");

                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);

                if (existingCustomer == null)
                {
                    throw new InvalidOperationException($"Customer with ID {customer.CustomerId} not found");
                }

                // Update properties
                existingCustomer.CompanyName = customer.CompanyName;
                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.Street = customer.Street;
                existingCustomer.City = customer.City;
                existingCustomer.State = customer.State;
                existingCustomer.ZipCode = customer.ZipCode;
                existingCustomer.County = customer.County;
                existingCustomer.EmailAddress = customer.EmailAddress;
                existingCustomer.HomePhone = customer.HomePhone;
                existingCustomer.BusinessPhone = customer.BusinessPhone;
                existingCustomer.CellPhone = customer.CellPhone;
                existingCustomer.CustomerTypeId = customer.CustomerTypeId;
                existingCustomer.LinkedCustomerId = customer.LinkedCustomerId;
                existingCustomer.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                Logger.LogInfo($"Customer updated successfully: {customer.CustomerId}");
                return existingCustomer;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating customer: {customer.CustomerId}");
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                Logger.LogInfo($"Deleting customer with ID: {customerId}");

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (customer == null)
                {
                    Logger.LogWarning($"Customer with ID {customerId} not found for deletion");
                    return false;
                }

                // Soft delete - set IsActive to false
                customer.IsActive = false;
                customer.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                Logger.LogInfo($"Customer deleted successfully: {customerId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting customer: {customerId}");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                Logger.LogInfo($"Searching customers with term: {searchTerm}");

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllCustomersAsync();
                }

                var term = searchTerm.ToLower().Trim();

                return await _context.Customers
                    .Include(c => c.CustomerType)
                    .Include(c => c.LinkedCustomer)
                    .Where(c => c.IsActive && (
                        c.FirstName.ToLower().Contains(term) ||
                        c.LastName.ToLower().Contains(term) ||
                        (c.CompanyName != null && c.CompanyName.ToLower().Contains(term)) ||
                        (c.EmailAddress != null && c.EmailAddress.ToLower().Contains(term)) ||
                        c.Street.ToLower().Contains(term) ||
                        c.City.ToLower().Contains(term) ||
                        c.ZipCode.Contains(term) ||
                        (c.HomePhone != null && c.HomePhone.Contains(term)) ||
                        (c.BusinessPhone != null && c.BusinessPhone.Contains(term)) ||
                        (c.CellPhone != null && c.CellPhone.Contains(term))
                    ))
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error searching customers with term: {searchTerm}");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByTypeAsync(int customerTypeId)
        {
            try
            {
                Logger.LogInfo($"Retrieving customers by type ID: {customerTypeId}");

                return await _context.Customers
                    .Include(c => c.CustomerType)
                    .Include(c => c.LinkedCustomer)
                    .Where(c => c.IsActive && c.CustomerTypeId == customerTypeId)
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving customers by type: {customerTypeId}");
                throw;
            }
        }

        #region Contact Methods

        public async Task<IEnumerable<Contact>> GetCustomerContactsAsync(int customerId)
        {
            try
            {
                Logger.LogInfo($"Retrieving contacts for customer ID: {customerId}");

                return await _context.Contacts
                    .Include(c => c.ContactType)
                    .Where(c => c.CustomerId == customerId && c.IsActive)
                    .OrderBy(c => c.ContactType.TypeName)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving contacts for customer: {customerId}");
                throw;
            }
        }

        public async Task<Contact> AddContactAsync(Contact contact)
        {
            try
            {
                Logger.LogInfo($"Adding contact for customer ID: {contact.CustomerId}");

                contact.CreatedDate = DateTime.Now;
                contact.IsActive = true;

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                // Load the contact type for return
                await _context.Entry(contact)
                    .Reference(c => c.ContactType)
                    .LoadAsync();

                Logger.LogInfo($"Contact added successfully with ID: {contact.ContactId}");
                return contact;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding contact for customer: {contact.CustomerId}");
                throw;
            }
        }

        public async Task<Contact> UpdateContactAsync(Contact contact)
        {
            try
            {
                Logger.LogInfo($"Updating contact with ID: {contact.ContactId}");

                var existingContact = await _context.Contacts
                    .FirstOrDefaultAsync(c => c.ContactId == contact.ContactId);

                if (existingContact == null)
                {
                    throw new InvalidOperationException($"Contact with ID {contact.ContactId} not found");
                }

                existingContact.Name = contact.Name;
                existingContact.HomePhone = contact.HomePhone;
                existingContact.BusinessPhone = contact.BusinessPhone;
                existingContact.CellPhone = contact.CellPhone;
                existingContact.EmailAddress = contact.EmailAddress;
                existingContact.ContactTypeId = contact.ContactTypeId;

                await _context.SaveChangesAsync();

                // Load the contact type for return
                await _context.Entry(existingContact)
                    .Reference(c => c.ContactType)
                    .LoadAsync();

                Logger.LogInfo($"Contact updated successfully: {contact.ContactId}");
                return existingContact;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating contact: {contact.ContactId}");
                throw;
            }
        }

        public async Task<bool> DeleteContactAsync(int contactId)
        {
            try
            {
                Logger.LogInfo($"Deleting contact with ID: {contactId}");

                var contact = await _context.Contacts
                    .FirstOrDefaultAsync(c => c.ContactId == contactId);

                if (contact == null)
                {
                    Logger.LogWarning($"Contact with ID {contactId} not found for deletion");
                    return false;
                }

                // Soft delete
                contact.IsActive = false;
                await _context.SaveChangesAsync();

                Logger.LogInfo($"Contact deleted successfully: {contactId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting contact: {contactId}");
                throw;
            }
        }

        #endregion

        #region Security System Methods

        public async Task<IEnumerable<SecuritySystem>> GetCustomerSecuritySystemsAsync(int customerId)
        {
            try
            {
                Logger.LogInfo($"Retrieving security systems for customer ID: {customerId}");

                return await _context.SecuritySystems
                    .Include(ss => ss.PanelType)
                    .Include(ss => ss.MonitoringType)
                    .Include(ss => ss.PrimaryCommunicator)
                        .ThenInclude(c => c.CommunicatorType)
                    .Include(ss => ss.SecondaryCommunicator)
                        .ThenInclude(c => c.CommunicatorType)
                    .Include(ss => ss.Zones.Where(z => z.IsActive))
                        .ThenInclude(z => z.DeviceType)
                    .Include(ss => ss.CallList.Where(cl => cl.IsActive))
                    .Where(ss => ss.CustomerId == customerId && ss.IsActive)
                    .OrderBy(ss => ss.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving security systems for customer: {customerId}");
                throw;
            }
        }

        public async Task<SecuritySystem> AddSecuritySystemAsync(SecuritySystem securitySystem)
        {
            try
            {
                Logger.LogInfo($"Adding security system for customer ID: {securitySystem.CustomerId}");

                securitySystem.CreatedDate = DateTime.Now;
                securitySystem.IsActive = true;

                _context.SecuritySystems.Add(securitySystem);
                await _context.SaveChangesAsync();

                // Load related entities for return
                await LoadSecuritySystemRelatedEntities(securitySystem);

                Logger.LogInfo($"Security system added successfully with ID: {securitySystem.SecuritySystemId}");
                return securitySystem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding security system for customer: {securitySystem.CustomerId}");
                throw;
            }
        }

        public async Task<SecuritySystem> UpdateSecuritySystemAsync(SecuritySystem securitySystem)
        {
            try
            {
                Logger.LogInfo($"Updating security system with ID: {securitySystem.SecuritySystemId}");

                var existingSystem = await _context.SecuritySystems
                    .FirstOrDefaultAsync(ss => ss.SecuritySystemId == securitySystem.SecuritySystemId);

                if (existingSystem == null)
                {
                    throw new InvalidOperationException($"Security system with ID {securitySystem.SecuritySystemId} not found");
                }

                // Update properties
                existingSystem.CentralStationNumber = securitySystem.CentralStationNumber;
                existingSystem.PanelTypeId = securitySystem.PanelTypeId;
                existingSystem.MonitoringTypeId = securitySystem.MonitoringTypeId;
                existingSystem.MonitoringStartDate = securitySystem.MonitoringStartDate;
                existingSystem.InstalledDate = securitySystem.InstalledDate;
                existingSystem.MasterSecurityCode = securitySystem.MasterSecurityCode;
                existingSystem.CodeWord = securitySystem.CodeWord;
                existingSystem.PolicePhone = securitySystem.PolicePhone;
                existingSystem.FireDeptPhone = securitySystem.FireDeptPhone;
                existingSystem.AmbulancePhone = securitySystem.AmbulancePhone;
                existingSystem.CityPermitNumber = securitySystem.CityPermitNumber;
                existingSystem.PermitDueDate = securitySystem.PermitDueDate;
                existingSystem.AuthorityNotes = securitySystem.AuthorityNotes;
                existingSystem.PrimaryCommunicatorId = securitySystem.PrimaryCommunicatorId;
                existingSystem.SecondaryCommunicatorId = securitySystem.SecondaryCommunicatorId;
                existingSystem.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // Load related entities for return
                await LoadSecuritySystemRelatedEntities(existingSystem);

                Logger.LogInfo($"Security system updated successfully: {securitySystem.SecuritySystemId}");
                return existingSystem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating security system: {securitySystem.SecuritySystemId}");
                throw;
            }
        }

        public async Task<bool> DeleteSecuritySystemAsync(int securitySystemId)
        {
            try
            {
                Logger.LogInfo($"Deleting security system with ID: {securitySystemId}");

                var securitySystem = await _context.SecuritySystems
                    .FirstOrDefaultAsync(ss => ss.SecuritySystemId == securitySystemId);

                if (securitySystem == null)
                {
                    Logger.LogWarning($"Security system with ID {securitySystemId} not found for deletion");
                    return false;
                }

                // Soft delete
                securitySystem.IsActive = false;
                securitySystem.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                Logger.LogInfo($"Security system deleted successfully: {securitySystemId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting security system: {securitySystemId}");
                throw;
            }
        }

        private async Task LoadSecuritySystemRelatedEntities(SecuritySystem securitySystem)
        {
            await _context.Entry(securitySystem)
                .Reference(ss => ss.PanelType)
                .LoadAsync();

            await _context.Entry(securitySystem)
                .Reference(ss => ss.MonitoringType)
                .LoadAsync();

            if (securitySystem.PrimaryCommunicatorId.HasValue)
            {
                await _context.Entry(securitySystem)
                    .Reference(ss => ss.PrimaryCommunicator)
                    .LoadAsync();
            }

            if (securitySystem.SecondaryCommunicatorId.HasValue)
            {
                await _context.Entry(securitySystem)
                    .Reference(ss => ss.SecondaryCommunicator)
                    .LoadAsync();
            }
        }

        #endregion
    }
}