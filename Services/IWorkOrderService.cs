using AlarmCompanyManager.Models;

namespace AlarmCompanyManager.Services
{
    public interface IWorkOrderService
    {
        Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync();
        Task<WorkOrder?> GetWorkOrderByIdAsync(int workOrderId);
        Task<WorkOrder?> GetWorkOrderByNumberAsync(string workOrderNumber);
        Task<WorkOrder> CreateWorkOrderAsync(WorkOrder workOrder);
        Task<WorkOrder> UpdateWorkOrderAsync(WorkOrder workOrder);
        Task<bool> DeleteWorkOrderAsync(int workOrderId);
        Task<IEnumerable<WorkOrder>> SearchWorkOrdersAsync(string searchTerm);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByCustomerAsync(int customerId);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByTechnicianAsync(int technicianId);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByStatusAsync(int statusId);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<WorkOrder>> GetScheduledWorkOrdersAsync(DateTime date);
        Task<string> GenerateWorkOrderNumberAsync();

        // Work Order Items
        Task<IEnumerable<WorkOrderItem>> GetWorkOrderItemsAsync(int workOrderId);
        Task<WorkOrderItem> AddWorkOrderItemAsync(WorkOrderItem item);
        Task<WorkOrderItem> UpdateWorkOrderItemAsync(WorkOrderItem item);
        Task<bool> DeleteWorkOrderItemAsync(int itemId);
    }
}