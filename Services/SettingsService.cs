using Microsoft.EntityFrameworkCore;
using AlarmCompanyManager.Data;
using AlarmCompanyManager.Models;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AlarmCompanyContext _context;

        public SettingsService(AlarmCompanyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Customer Types

        public async Task<IEnumerable<CustomerType>> GetCustomerTypesAsync()
        {
            try
            {
                return await _context.CustomerTypes
                    .Where(ct => ct.IsActive)
                    .OrderBy(ct => ct.TypeName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving customer types");
                throw;
            }
        }

        public async Task<CustomerType> AddCustomerTypeAsync(CustomerType customerType)
        {
            try
            {
                customerType.IsActive = true;
                _context.CustomerTypes.Add(customerType);
                await _context.SaveChangesAsync();
                return customerType;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding customer type: {customerType.TypeName}");
                throw;
            }
        }

        public async Task<CustomerType> UpdateCustomerTypeAsync(CustomerType customerType)
        {
            try
            {
                var existing = await _context.CustomerTypes
                    .FirstOrDefaultAsync(ct => ct.CustomerTypeId == customerType.CustomerTypeId);

                if (existing == null)
                    throw new InvalidOperationException($"Customer type with ID {customerType.CustomerTypeId} not found");

                existing.TypeName = customerType.TypeName;
                existing.Description = customerType.Description;
                existing.IsActive = customerType.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating customer type: {customerType.CustomerTypeId}");
                throw;
            }
        }

        public async Task<bool> DeleteCustomerTypeAsync(int customerTypeId)
        {
            try
            {
                var customerType = await _context.CustomerTypes
                    .FirstOrDefaultAsync(ct => ct.CustomerTypeId == customerTypeId);

                if (customerType == null) return false;

                // Check if it's being used by any customers
                var isUsed = await _context.Customers
                    .AnyAsync(c => c.CustomerTypeId == customerTypeId && c.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete customer type because it is being used by customers");
                }

                customerType.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting customer type: {customerTypeId}");
                throw;
            }
        }

        #endregion

        #region Contact Types

        public async Task<IEnumerable<ContactType>> GetContactTypesAsync()
        {
            try
            {
                return await _context.ContactTypes
                    .Where(ct => ct.IsActive)
                    .OrderBy(ct => ct.TypeName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving contact types");
                throw;
            }
        }

        public async Task<ContactType> AddContactTypeAsync(ContactType contactType)
        {
            try
            {
                contactType.IsActive = true;
                _context.ContactTypes.Add(contactType);
                await _context.SaveChangesAsync();
                return contactType;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding contact type: {contactType.TypeName}");
                throw;
            }
        }

        public async Task<ContactType> UpdateContactTypeAsync(ContactType contactType)
        {
            try
            {
                var existing = await _context.ContactTypes
                    .FirstOrDefaultAsync(ct => ct.ContactTypeId == contactType.ContactTypeId);

                if (existing == null)
                    throw new InvalidOperationException($"Contact type with ID {contactType.ContactTypeId} not found");

                existing.TypeName = contactType.TypeName;
                existing.Description = contactType.Description;
                existing.IsActive = contactType.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating contact type: {contactType.ContactTypeId}");
                throw;
            }
        }

        public async Task<bool> DeleteContactTypeAsync(int contactTypeId)
        {
            try
            {
                var contactType = await _context.ContactTypes
                    .FirstOrDefaultAsync(ct => ct.ContactTypeId == contactTypeId);

                if (contactType == null) return false;

                // Check if it's being used
                var isUsed = await _context.Contacts
                    .AnyAsync(c => c.ContactTypeId == contactTypeId && c.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete contact type because it is being used by contacts");
                }

                contactType.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting contact type: {contactTypeId}");
                throw;
            }
        }

        #endregion

        #region Panel Types

        public async Task<IEnumerable<PanelType>> GetPanelTypesAsync()
        {
            try
            {
                return await _context.PanelTypes
                    .Where(pt => pt.IsActive)
                    .OrderBy(pt => pt.Manufacturer)
                    .ThenBy(pt => pt.ModelNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving panel types");
                throw;
            }
        }

        public async Task<PanelType> AddPanelTypeAsync(PanelType panelType)
        {
            try
            {
                panelType.IsActive = true;
                _context.PanelTypes.Add(panelType);
                await _context.SaveChangesAsync();
                return panelType;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding panel type: {panelType.Manufacturer} {panelType.ModelNumber}");
                throw;
            }
        }

        public async Task<PanelType> UpdatePanelTypeAsync(PanelType panelType)
        {
            try
            {
                var existing = await _context.PanelTypes
                    .FirstOrDefaultAsync(pt => pt.PanelTypeId == panelType.PanelTypeId);

                if (existing == null)
                    throw new InvalidOperationException($"Panel type with ID {panelType.PanelTypeId} not found");

                existing.Manufacturer = panelType.Manufacturer;
                existing.ModelNumber = panelType.ModelNumber;
                existing.Description = panelType.Description;
                existing.IsActive = panelType.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating panel type: {panelType.PanelTypeId}");
                throw;
            }
        }

        public async Task<bool> DeletePanelTypeAsync(int panelTypeId)
        {
            try
            {
                var panelType = await _context.PanelTypes
                    .FirstOrDefaultAsync(pt => pt.PanelTypeId == panelTypeId);

                if (panelType == null) return false;

                // Check if it's being used
                var isUsed = await _context.SecuritySystems
                    .AnyAsync(ss => ss.PanelTypeId == panelTypeId && ss.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete panel type because it is being used by security systems");
                }

                panelType.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting panel type: {panelTypeId}");
                throw;
            }
        }

        #endregion

        #region Monitoring Types

        public async Task<IEnumerable<MonitoringType>> GetMonitoringTypesAsync()
        {
            try
            {
                return await _context.MonitoringTypes
                    .Where(mt => mt.IsActive)
                    .OrderBy(mt => mt.TypeName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving monitoring types");
                throw;
            }
        }

        public async Task<MonitoringType> AddMonitoringTypeAsync(MonitoringType monitoringType)
        {
            try
            {
                monitoringType.IsActive = true;
                _context.MonitoringTypes.Add(monitoringType);
                await _context.SaveChangesAsync();
                return monitoringType;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding monitoring type: {monitoringType.TypeName}");
                throw;
            }
        }

        public async Task<MonitoringType> UpdateMonitoringTypeAsync(MonitoringType monitoringType)
        {
            try
            {
                var existing = await _context.MonitoringTypes
                    .FirstOrDefaultAsync(mt => mt.MonitoringTypeId == monitoringType.MonitoringTypeId);

                if (existing == null)
                    throw new InvalidOperationException($"Monitoring type with ID {monitoringType.MonitoringTypeId} not found");

                existing.TypeName = monitoringType.TypeName;
                existing.Description = monitoringType.Description;
                existing.IsActive = monitoringType.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating monitoring type: {monitoringType.MonitoringTypeId}");
                throw;
            }
        }

        public async Task<bool> DeleteMonitoringTypeAsync(int monitoringTypeId)
        {
            try
            {
                var monitoringType = await _context.MonitoringTypes
                    .FirstOrDefaultAsync(mt => mt.MonitoringTypeId == monitoringTypeId);

                if (monitoringType == null) return false;

                // Check if it's being used
                var isUsed = await _context.SecuritySystems
                    .AnyAsync(ss => ss.MonitoringTypeId == monitoringTypeId && ss.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete monitoring type because it is being used by security systems");
                }

                monitoringType.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting monitoring type: {monitoringTypeId}");
                throw;
            }
        }

        #endregion

        #region Device Types

        public async Task<IEnumerable<DeviceType>> GetDeviceTypesAsync()
        {
            try
            {
                return await _context.DeviceTypes
                    .Where(dt => dt.IsActive)
                    .OrderBy(dt => dt.TypeName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving device types");
                throw;
            }
        }

        public async Task<DeviceType> AddDeviceTypeAsync(DeviceType deviceType)
        {
            try
            {
                deviceType.IsActive = true;
                _context.DeviceTypes.Add(deviceType);
                await _context.SaveChangesAsync();
                return deviceType;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding device type: {deviceType.TypeName}");
                throw;
            }
        }

        public async Task<DeviceType> UpdateDeviceTypeAsync(DeviceType deviceType)
        {
            try
            {
                var existing = await _context.DeviceTypes
                    .FirstOrDefaultAsync(dt => dt.DeviceTypeId == deviceType.DeviceTypeId);

                if (existing == null)
                    throw new InvalidOperationException($"Device type with ID {deviceType.DeviceTypeId} not found");

                existing.TypeName = deviceType.TypeName;
                existing.Description = deviceType.Description;
                existing.IsActive = deviceType.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating device type: {deviceType.DeviceTypeId}");
                throw;
            }
        }

        public async Task<bool> DeleteDeviceTypeAsync(int deviceTypeId)
        {
            try
            {
                var deviceType = await _context.DeviceTypes
                    .FirstOrDefaultAsync(dt => dt.DeviceTypeId == deviceTypeId);

                if (deviceType == null) return false;

                // Check if it's being used
                var isUsed = await _context.Zones
                    .AnyAsync(z => z.DeviceTypeId == deviceTypeId && z.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete device type because it is being used by zones");
                }

                deviceType.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting device type: {deviceTypeId}");
                throw;
            }
        }

        #endregion

        #region Communicator Types

        public async Task<IEnumerable<CommunicatorType>> GetCommunicatorTypesAsync()
        {
            try
            {
                return await _context.CommunicatorTypes
                    .Where(ct => ct.IsActive)
                    .OrderBy(ct => ct.TypeName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving communicator types");
                throw;
            }
        }

        public async Task<CommunicatorType> AddCommunicatorTypeAsync(CommunicatorType communicatorType)
        {
            try
            {
                communicatorType.IsActive = true;
                _context.CommunicatorTypes.Add(communicatorType);
                await _context.SaveChangesAsync();
                return communicatorType;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding communicator type: {communicatorType.TypeName}");
                throw;
            }
        }

        public async Task<CommunicatorType> UpdateCommunicatorTypeAsync(CommunicatorType communicatorType)
        {
            try
            {
                var existing = await _context.CommunicatorTypes
                    .FirstOrDefaultAsync(ct => ct.CommunicatorTypeId == communicatorType.CommunicatorTypeId);

                if (existing == null)
                    throw new InvalidOperationException($"Communicator type with ID {communicatorType.CommunicatorTypeId} not found");

                existing.TypeName = communicatorType.TypeName;
                existing.Description = communicatorType.Description;
                existing.IsActive = communicatorType.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating communicator type: {communicatorType.CommunicatorTypeId}");
                throw;
            }
        }

        public async Task<bool> DeleteCommunicatorTypeAsync(int communicatorTypeId)
        {
            try
            {
                var communicatorType = await _context.CommunicatorTypes
                    .FirstOrDefaultAsync(ct => ct.CommunicatorTypeId == communicatorTypeId);

                if (communicatorType == null) return false;

                // Check if it's being used
                var isUsed = await _context.Communicators
                    .AnyAsync(c => c.CommunicatorTypeId == communicatorTypeId && c.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete communicator type because it is being used by communicators");
                }

                communicatorType.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting communicator type: {communicatorTypeId}");
                throw;
            }
        }

        #endregion

        #region Work Order Types

        public async Task<IEnumerable<WorkOrderType>> GetWorkOrderTypesAsync()
        {
            try
            {
                return await _context.WorkOrderTypes
                    .Where(wot => wot.IsActive)
                    .OrderBy(wot => wot.TypeName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving work order types");
                throw;
            }
        }

        public async Task<WorkOrderType> AddWorkOrderTypeAsync(WorkOrderType workOrderType)
        {
            try
            {
                workOrderType.IsActive = true;
                _context.WorkOrderTypes.Add(workOrderType);
                await _context.SaveChangesAsync();
                return workOrderType;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding work order type: {workOrderType.TypeName}");
                throw;
            }
        }

        public async Task<WorkOrderType> UpdateWorkOrderTypeAsync(WorkOrderType workOrderType)
        {
            try
            {
                var existing = await _context.WorkOrderTypes
                    .FirstOrDefaultAsync(wot => wot.WorkOrderTypeId == workOrderType.WorkOrderTypeId);

                if (existing == null)
                    throw new InvalidOperationException($"Work order type with ID {workOrderType.WorkOrderTypeId} not found");

                existing.TypeName = workOrderType.TypeName;
                existing.Description = workOrderType.Description;
                existing.IsActive = workOrderType.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating work order type: {workOrderType.WorkOrderTypeId}");
                throw;
            }
        }

        public async Task<bool> DeleteWorkOrderTypeAsync(int workOrderTypeId)
        {
            try
            {
                var workOrderType = await _context.WorkOrderTypes
                    .FirstOrDefaultAsync(wot => wot.WorkOrderTypeId == workOrderTypeId);

                if (workOrderType == null) return false;

                // Check if it's being used
                var isUsed = await _context.WorkOrders
                    .AnyAsync(wo => wo.WorkOrderTypeId == workOrderTypeId && wo.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete work order type because it is being used by work orders");
                }

                workOrderType.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting work order type: {workOrderTypeId}");
                throw;
            }
        }

        #endregion

        #region Work Order Categories

        public async Task<IEnumerable<WorkOrderCategory>> GetWorkOrderCategoriesAsync()
        {
            try
            {
                return await _context.WorkOrderCategories
                    .Where(woc => woc.IsActive)
                    .OrderBy(woc => woc.CategoryName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving work order categories");
                throw;
            }
        }

        public async Task<WorkOrderCategory> AddWorkOrderCategoryAsync(WorkOrderCategory category)
        {
            try
            {
                category.IsActive = true;
                _context.WorkOrderCategories.Add(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding work order category: {category.CategoryName}");
                throw;
            }
        }

        public async Task<WorkOrderCategory> UpdateWorkOrderCategoryAsync(WorkOrderCategory category)
        {
            try
            {
                var existing = await _context.WorkOrderCategories
                    .FirstOrDefaultAsync(woc => woc.CategoryId == category.CategoryId);

                if (existing == null)
                    throw new InvalidOperationException($"Work order category with ID {category.CategoryId} not found");

                existing.CategoryName = category.CategoryName;
                existing.Description = category.Description;
                existing.IsActive = category.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating work order category: {category.CategoryId}");
                throw;
            }
        }

        public async Task<bool> DeleteWorkOrderCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.WorkOrderCategories
                    .FirstOrDefaultAsync(woc => woc.CategoryId == categoryId);

                if (category == null) return false;

                // Check if it's being used
                var isUsed = await _context.WorkOrders
                    .AnyAsync(wo => wo.CategoryId == categoryId && wo.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete work order category because it is being used by work orders");
                }

                category.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting work order category: {categoryId}");
                throw;
            }
        }

        #endregion

        #region Work Order Statuses

        public async Task<IEnumerable<WorkOrderStatus>> GetWorkOrderStatusesAsync()
        {
            try
            {
                return await _context.WorkOrderStatuses
                    .Where(wos => wos.IsActive)
                    .OrderBy(wos => wos.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving work order statuses");
                throw;
            }
        }

        public async Task<WorkOrderStatus> AddWorkOrderStatusAsync(WorkOrderStatus status)
        {
            try
            {
                status.IsActive = true;
                _context.WorkOrderStatuses.Add(status);
                await _context.SaveChangesAsync();
                return status;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding work order status: {status.StatusName}");
                throw;
            }
        }

        public async Task<WorkOrderStatus> UpdateWorkOrderStatusAsync(WorkOrderStatus status)
        {
            try
            {
                var existing = await _context.WorkOrderStatuses
                    .FirstOrDefaultAsync(wos => wos.StatusId == status.StatusId);

                if (existing == null)
                    throw new InvalidOperationException($"Work order status with ID {status.StatusId} not found");

                existing.StatusName = status.StatusName;
                existing.Description = status.Description;
                existing.ColorCode = status.ColorCode;
                existing.SortOrder = status.SortOrder;
                existing.IsActive = status.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating work order status: {status.StatusId}");
                throw;
            }
        }

        public async Task<bool> DeleteWorkOrderStatusAsync(int statusId)
        {
            try
            {
                var status = await _context.WorkOrderStatuses
                    .FirstOrDefaultAsync(wos => wos.StatusId == statusId);

                if (status == null) return false;

                // Check if it's being used
                var isUsed = await _context.WorkOrders
                    .AnyAsync(wo => wo.StatusId == statusId && wo.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete work order status because it is being used by work orders");
                }

                status.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting work order status: {statusId}");
                throw;
            }
        }

        #endregion

        #region Technicians

        public async Task<IEnumerable<Technician>> GetTechniciansAsync()
        {
            try
            {
                return await _context.Technicians
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.LastName)
                    .ThenBy(t => t.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving technicians");
                throw;
            }
        }

        public async Task<Technician?> GetTechnicianByIdAsync(int technicianId)
        {
            try
            {
                return await _context.Technicians
                    .FirstOrDefaultAsync(t => t.TechnicianId == technicianId && t.IsActive);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving technician: {technicianId}");
                throw;
            }
        }

        public async Task<Technician> AddTechnicianAsync(Technician technician)
        {
            try
            {
                technician.IsActive = true;
                technician.CreatedDate = DateTime.Now;
                _context.Technicians.Add(technician);
                await _context.SaveChangesAsync();
                return technician;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding technician: {technician.FirstName} {technician.LastName}");
                throw;
            }
        }

        public async Task<Technician> UpdateTechnicianAsync(Technician technician)
        {
            try
            {
                var existing = await _context.Technicians
                    .FirstOrDefaultAsync(t => t.TechnicianId == technician.TechnicianId);

                if (existing == null)
                    throw new InvalidOperationException($"Technician with ID {technician.TechnicianId} not found");

                existing.FirstName = technician.FirstName;
                existing.LastName = technician.LastName;
                existing.EmailAddress = technician.EmailAddress;
                existing.PhoneNumber = technician.PhoneNumber;
                existing.CellPhone = technician.CellPhone;
                existing.EmployeeNumber = technician.EmployeeNumber;
                existing.HireDate = technician.HireDate;
                existing.Specializations = technician.Specializations;
                existing.Certifications = technician.Certifications;
                existing.IsActive = technician.IsActive;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating technician: {technician.TechnicianId}");
                throw;
            }
        }

        public async Task<bool> DeleteTechnicianAsync(int technicianId)
        {
            try
            {
                var technician = await _context.Technicians
                    .FirstOrDefaultAsync(t => t.TechnicianId == technicianId);

                if (technician == null) return false;

                // Check if technician has active work orders
                var hasActiveWorkOrders = await _context.WorkOrders
                    .AnyAsync(wo => wo.TechnicianId == technicianId && wo.IsActive &&
                             wo.StatusId != (int)WorkOrderStatusEnum.Completed &&
                             wo.StatusId != (int)WorkOrderStatusEnum.Canceled);

                if (hasActiveWorkOrders)
                {
                    throw new InvalidOperationException("Cannot delete technician because they have active work orders");
                }

                technician.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting technician: {technicianId}");
                throw;
            }
        }

        #endregion

        #region Communicators

        public async Task<IEnumerable<Communicator>> GetCommunicatorsAsync()
        {
            try
            {
                return await _context.Communicators
                    .Include(c => c.CommunicatorType)
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.CommunicatorType.TypeName)
                    .ThenBy(c => c.Manufacturer)
                    .ThenBy(c => c.ModelNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving communicators");
                throw;
            }
        }

        public async Task<Communicator> AddCommunicatorAsync(Communicator communicator)
        {
            try
            {
                communicator.IsActive = true;
                communicator.CreatedDate = DateTime.Now;
                _context.Communicators.Add(communicator);
                await _context.SaveChangesAsync();

                // Load the communicator type for return
                await _context.Entry(communicator)
                    .Reference(c => c.CommunicatorType)
                    .LoadAsync();

                return communicator;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding communicator");
                throw;
            }
        }

        public async Task<Communicator> UpdateCommunicatorAsync(Communicator communicator)
        {
            try
            {
                var existing = await _context.Communicators
                    .FirstOrDefaultAsync(c => c.CommunicatorId == communicator.CommunicatorId);

                if (existing == null)
                    throw new InvalidOperationException($"Communicator with ID {communicator.CommunicatorId} not found");

                existing.CommunicatorTypeId = communicator.CommunicatorTypeId;
                existing.Manufacturer = communicator.Manufacturer;
                existing.ModelNumber = communicator.ModelNumber;
                existing.RadioId = communicator.RadioId;
                existing.IpAddress = communicator.IpAddress;
                existing.Gateway = communicator.Gateway;
                existing.Subnet = communicator.Subnet;
                existing.PhoneNumber1 = communicator.PhoneNumber1;
                existing.PhoneNumber2 = communicator.PhoneNumber2;
                existing.Notes = communicator.Notes;
                existing.IsActive = communicator.IsActive;

                await _context.SaveChangesAsync();

                // Load the communicator type for return
                await _context.Entry(existing)
                    .Reference(c => c.CommunicatorType)
                    .LoadAsync();

                return existing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating communicator: {communicator.CommunicatorId}");
                throw;
            }
        }

        public async Task<bool> DeleteCommunicatorAsync(int communicatorId)
        {
            try
            {
                var communicator = await _context.Communicators
                    .FirstOrDefaultAsync(c => c.CommunicatorId == communicatorId);

                if (communicator == null) return false;

                // Check if it's being used by any security systems
                var isUsed = await _context.SecuritySystems
                    .AnyAsync(ss => (ss.PrimaryCommunicatorId == communicatorId ||
                                   ss.SecondaryCommunicatorId == communicatorId) && ss.IsActive);

                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete communicator because it is being used by security systems");
                }

                communicator.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting communicator: {communicatorId}");
                throw;
            }
        }

        #endregion
    }
}