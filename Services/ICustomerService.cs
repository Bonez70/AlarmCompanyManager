using AlarmCompanyManager.Models;

namespace AlarmCompanyManager.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int customerId);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
        Task<IEnumerable<Customer>> GetCustomersByTypeAsync(int customerTypeId);

        // Contact methods
        Task<IEnumerable<Contact>> GetCustomerContactsAsync(int customerId);
        Task<Contact> AddContactAsync(Contact contact);
        Task<Contact> UpdateContactAsync(Contact contact);
        Task<bool> DeleteContactAsync(int contactId);

        // Security System methods
        Task<IEnumerable<SecuritySystem>> GetCustomerSecuritySystemsAsync(int customerId);
        Task<SecuritySystem> AddSecuritySystemAsync(SecuritySystem securitySystem);
        Task<SecuritySystem> UpdateSecuritySystemAsync(SecuritySystem securitySystem);
        Task<bool> DeleteSecuritySystemAsync(int securitySystemId);
    }
}