using AlarmCompanyManager.Models;

namespace AlarmCompanyManager.Services
{
    public interface ISettingsService
    {
        // Customer Types
        Task<IEnumerable<CustomerType>> GetCustomerTypesAsync();
        Task<CustomerType> AddCustomerTypeAsync(CustomerType customerType);
        Task<CustomerType> UpdateCustomerTypeAsync(CustomerType customerType);
        Task<bool> DeleteCustomerTypeAsync(int customerTypeId);

        // Contact Types
        Task<IEnumerable<ContactType>> GetContactTypesAsync();
        Task<ContactType> AddContactTypeAsync(ContactType contactType);
        Task<ContactType> UpdateContactTypeAsync(ContactType contactType);
        Task<bool> DeleteContactTypeAsync(int contactTypeId);

        // Panel Types
        Task<IEnumerable<PanelType>> GetPanelTypesAsync();
        Task<PanelType> AddPanelTypeAsync(PanelType panelType);
        Task<PanelType> UpdatePanelTypeAsync(PanelType panelType);
        Task<bool> DeletePanelTypeAsync(int panelTypeId);

        // Monitoring Types
        Task<IEnumerable<MonitoringType>> GetMonitoringTypesAsync();
        Task<MonitoringType> AddMonitoringTypeAsync(MonitoringType monitoringType);
        Task<MonitoringType> UpdateMonitoringTypeAsync(MonitoringType monitoringType);
        Task<bool> DeleteMonitoringTypeAsync(int monitoringTypeId);

        // Device Types
        Task<IEnumerable<DeviceType>> GetDeviceTypesAsync();
        Task<DeviceType> AddDeviceTypeAsync(DeviceType deviceType);
        Task<DeviceType> UpdateDeviceTypeAsync(DeviceType deviceType);
        Task<bool> DeleteDeviceTypeAsync(int deviceTypeId);

        // Communicator Types
        Task<IEnumerable<CommunicatorType>> GetCommunicatorTypesAsync();
        Task<CommunicatorType> AddCommunicatorTypeAsync(CommunicatorType communicatorType);
        Task<CommunicatorType> UpdateCommunicatorTypeAsync(CommunicatorType communicatorType);
        Task<bool> DeleteCommunicatorTypeAsync(int communicatorTypeId);

        // Work Order Types
        Task<IEnumerable<WorkOrderType>> GetWorkOrderTypesAsync();
        Task<WorkOrderType> AddWorkOrderTypeAsync(WorkOrderType workOrderType);
        Task<WorkOrderType> UpdateWorkOrderTypeAsync(WorkOrderType workOrderType);
        Task<bool> DeleteWorkOrderTypeAsync(int workOrderTypeId);

        // Work Order Categories
        Task<IEnumerable<WorkOrderCategory>> GetWorkOrderCategoriesAsync();
        Task<WorkOrderCategory> AddWorkOrderCategoryAsync(WorkOrderCategory category);
        Task<WorkOrderCategory> UpdateWorkOrderCategoryAsync(WorkOrderCategory category);
        Task<bool> DeleteWorkOrderCategoryAsync(int categoryId);

        // Work Order Statuses
        Task<IEnumerable<WorkOrderStatus>> GetWorkOrderStatusesAsync();
        Task<WorkOrderStatus> AddWorkOrderStatusAsync(WorkOrderStatus status);
        Task<WorkOrderStatus> UpdateWorkOrderStatusAsync(WorkOrderStatus status);
        Task<bool> DeleteWorkOrderStatusAsync(int statusId);

        // Technicians
        Task<IEnumerable<Technician>> GetTechniciansAsync();
        Task<Technician> AddTechnicianAsync(Technician technician);
        Task<Technician> UpdateTechnicianAsync(Technician technician);
        Task<bool> DeleteTechnicianAsync(int technicianId);
        Task<Technician?> GetTechnicianByIdAsync(int technicianId);

        // Communicators
        Task<IEnumerable<Communicator>> GetCommunicatorsAsync();
        Task<Communicator> AddCommunicatorAsync(Communicator communicator);
        Task<Communicator> UpdateCommunicatorAsync(Communicator communicator);
        Task<bool> DeleteCommunicatorAsync(int communicatorId);
    }
}