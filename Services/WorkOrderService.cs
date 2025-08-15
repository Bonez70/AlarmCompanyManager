using Microsoft.EntityFrameworkCore;
using AlarmCompanyManager.Data;
using AlarmCompanyManager.Models;
using AlarmCompanyManager.Utilities;

namespace AlarmCompanyManager.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly AlarmCompanyContext _context;

        public WorkOrderService(AlarmCompanyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync()
        {
            try
            {
                Logger.LogInfo("Retrieving all work orders");
                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Where(w => w.IsActive)
                    .OrderByDescending(w => w.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving all work orders");
                throw;
            }
        }

        public async Task<WorkOrder?> GetWorkOrderByIdAsync(int workOrderId)
        {
            try
            {
                Logger.LogInfo($"Retrieving work order with ID: {workOrderId}");
                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Include(w => w.WorkOrderItems.Where(i => i.IsActive))
                    .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId && w.IsActive);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving work order with ID: {workOrderId}");
                throw;
            }
        }

        public async Task<WorkOrder?> GetWorkOrderByNumberAsync(string workOrderNumber)
        {
            try
            {
                Logger.LogInfo($"Retrieving work order with number: {workOrderNumber}");
                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Include(w => w.WorkOrderItems.Where(i => i.IsActive))
                    .FirstOrDefaultAsync(w => w.WorkOrderNumber == workOrderNumber && w.IsActive);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving work order with number: {workOrderNumber}");
                throw;
            }
        }

        public async Task<WorkOrder> CreateWorkOrderAsync(WorkOrder workOrder)
        {
            try
            {
                Logger.LogInfo($"Creating new work order for customer: {workOrder.CustomerId}");

                workOrder.CreatedDate = DateTime.Now;
                workOrder.IsActive = true;

                // Generate work order number if not provided
                if (string.IsNullOrWhiteSpace(workOrder.WorkOrderNumber))
                {
                    workOrder.WorkOrderNumber = await GenerateWorkOrderNumberAsync();
                }

                _context.WorkOrders.Add(workOrder);
                await _context.SaveChangesAsync();

                // Load related entities for return
                await LoadWorkOrderRelatedEntities(workOrder);

                Logger.LogInfo($"Work order created successfully with ID: {workOrder.WorkOrderId}");
                return workOrder;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error creating work order for customer: {workOrder.CustomerId}");
                throw;
            }
        }

        public async Task<WorkOrder> UpdateWorkOrderAsync(WorkOrder workOrder)
        {
            try
            {
                Logger.LogInfo($"Updating work order with ID: {workOrder.WorkOrderId}");

                var existingWorkOrder = await _context.WorkOrders
                    .FirstOrDefaultAsync(w => w.WorkOrderId == workOrder.WorkOrderId);

                if (existingWorkOrder == null)
                {
                    throw new InvalidOperationException($"Work order with ID {workOrder.WorkOrderId} not found");
                }

                // Update properties
                existingWorkOrder.Description = workOrder.Description;
                existingWorkOrder.WorkOrderTypeId = workOrder.WorkOrderTypeId;
                existingWorkOrder.CategoryId = workOrder.CategoryId;
                existingWorkOrder.StatusId = workOrder.StatusId;
                existingWorkOrder.TechnicianId = workOrder.TechnicianId;
                existingWorkOrder.ScheduledDate = workOrder.ScheduledDate;
                existingWorkOrder.ScheduledStartTime = workOrder.ScheduledStartTime;
                existingWorkOrder.EndTime = workOrder.EndTime;
                existingWorkOrder.EstimatedHours = workOrder.EstimatedHours;
                existingWorkOrder.ActualHours = workOrder.ActualHours;
                existingWorkOrder.CompletedDate = workOrder.CompletedDate;
                existingWorkOrder.Notes = workOrder.Notes;
                existingWorkOrder.EstimatedCost = workOrder.EstimatedCost;
                existingWorkOrder.ActualCost = workOrder.ActualCost;
                existingWorkOrder.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // Load related entities for return
                await LoadWorkOrderRelatedEntities(existingWorkOrder);

                Logger.LogInfo($"Work order updated successfully: {workOrder.WorkOrderId}");
                return existingWorkOrder;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating work order: {workOrder.WorkOrderId}");
                throw;
            }
        }

        public async Task<bool> DeleteWorkOrderAsync(int workOrderId)
        {
            try
            {
                Logger.LogInfo($"Deleting work order with ID: {workOrderId}");

                var workOrder = await _context.WorkOrders
                    .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId);

                if (workOrder == null)
                {
                    Logger.LogWarning($"Work order with ID {workOrderId} not found for deletion");
                    return false;
                }

                // Soft delete
                workOrder.IsActive = false;
                workOrder.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                Logger.LogInfo($"Work order deleted successfully: {workOrderId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting work order: {workOrderId}");
                throw;
            }
        }

        public async Task<IEnumerable<WorkOrder>> SearchWorkOrdersAsync(string searchTerm)
        {
            try
            {
                Logger.LogInfo($"Searching work orders with term: {searchTerm}");

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllWorkOrdersAsync();
                }

                var term = searchTerm.ToLower().Trim();

                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Where(w => w.IsActive && (
                        w.WorkOrderNumber.ToLower().Contains(term) ||
                        w.Description.ToLower().Contains(term) ||
                        w.Customer.FirstName.ToLower().Contains(term) ||
                        w.Customer.LastName.ToLower().Contains(term) ||
                        (w.Customer.CompanyName != null && w.Customer.CompanyName.ToLower().Contains(term)) ||
                        (w.Technician != null && (w.Technician.FirstName.ToLower().Contains(term) || w.Technician.LastName.ToLower().Contains(term))) ||
                        (w.Notes != null && w.Notes.ToLower().Contains(term))
                    ))
                    .OrderByDescending(w => w.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error searching work orders with term: {searchTerm}");
                throw;
            }
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrdersByCustomerAsync(int customerId)
        {
            try
            {
                Logger.LogInfo($"Retrieving work orders for customer ID: {customerId}");

                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Where(w => w.CustomerId == customerId && w.IsActive)
                    .OrderByDescending(w => w.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving work orders for customer: {customerId}");
                throw;
            }
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrdersByTechnicianAsync(int technicianId)
        {
            try
            {
                Logger.LogInfo($"Retrieving work orders for technician ID: {technicianId}");

                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Where(w => w.TechnicianId == technicianId && w.IsActive)
                    .OrderBy(w => w.ScheduledDate)
                    .ThenBy(w => w.ScheduledStartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving work orders for technician: {technicianId}");
                throw;
            }
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrdersByStatusAsync(int statusId)
        {
            try
            {
                Logger.LogInfo($"Retrieving work orders by status ID: {statusId}");

                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Where(w => w.StatusId == statusId && w.IsActive)
                    .OrderByDescending(w => w.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving work orders by status: {statusId}");
                throw;
            }
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                Logger.LogInfo($"Retrieving work orders from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Where(w => w.IsActive &&
                           w.ScheduledDate.HasValue &&
                           w.ScheduledDate.Value.Date >= startDate.Date &&
                           w.ScheduledDate.Value.Date <= endDate.Date)
                    .OrderBy(w => w.ScheduledDate)
                    .ThenBy(w => w.ScheduledStartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving work orders by date range: {startDate} - {endDate}");
                throw;
            }
        }

        public async Task<IEnumerable<WorkOrder>> GetScheduledWorkOrdersAsync(DateTime date)
        {
            try
            {
                Logger.LogInfo($"Retrieving scheduled work orders for date: {date:yyyy-MM-dd}");

                return await _context.WorkOrders
                    .Include(w => w.Customer)
                    .Include(w => w.WorkOrderType)
                    .Include(w => w.Category)
                    .Include(w => w.Status)
                    .Include(w => w.Technician)
                    .Where(w => w.IsActive &&
                           w.ScheduledDate.HasValue &&
                           w.ScheduledDate.Value.Date == date.Date)
                    .OrderBy(w => w.ScheduledStartTime)
                    .ThenBy(w => w.Technician.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving scheduled work orders for date: {date}");
                throw;
            }
        }

        public async Task<string> GenerateWorkOrderNumberAsync()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = $"WO{year}";

                var lastWorkOrder = await _context.WorkOrders
                    .Where(w => w.WorkOrderNumber.StartsWith(prefix))
                    .OrderByDescending(w => w.WorkOrderNumber)
                    .FirstOrDefaultAsync();

                if (lastWorkOrder == null)
                {
                    return $"{prefix}-0001";
                }

                var lastNumber = lastWorkOrder.WorkOrderNumber.Substring(prefix.Length + 1);
                if (int.TryParse(lastNumber, out var number))
                {
                    return $"{prefix}-{(number + 1):D4}";
                }

                return $"{prefix}-0001";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error generating work order number");
                return $"WO{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        #region Work Order Items

        public async Task<IEnumerable<WorkOrderItem>> GetWorkOrderItemsAsync(int workOrderId)
        {
            try
            {
                Logger.LogInfo($"Retrieving work order items for work order ID: {workOrderId}");

                return await _context.WorkOrderItems
                    .Where(i => i.WorkOrderId == workOrderId && i.IsActive)
                    .OrderBy(i => i.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving work order items for work order: {workOrderId}");
                throw;
            }
        }

        public async Task<WorkOrderItem> AddWorkOrderItemAsync(WorkOrderItem item)
        {
            try
            {
                Logger.LogInfo($"Adding work order item for work order ID: {item.WorkOrderId}");

                item.CreatedDate = DateTime.Now;
                item.IsActive = true;

                _context.WorkOrderItems.Add(item);
                await _context.SaveChangesAsync();

                Logger.LogInfo($"Work order item added successfully with ID: {item.WorkOrderItemId}");
                return item;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding work order item for work order: {item.WorkOrderId}");
                throw;
            }
        }

        public async Task<WorkOrderItem> UpdateWorkOrderItemAsync(WorkOrderItem item)
        {
            try
            {
                Logger.LogInfo($"Updating work order item with ID: {item.WorkOrderItemId}");

                var existingItem = await _context.WorkOrderItems
                    .FirstOrDefaultAsync(i => i.WorkOrderItemId == item.WorkOrderItemId);

                if (existingItem == null)
                {
                    throw new InvalidOperationException($"Work order item with ID {item.WorkOrderItemId} not found");
                }

                existingItem.Description = item.Description;
                existingItem.Quantity = item.Quantity;
                existingItem.UnitPrice = item.UnitPrice;
                existingItem.PartNumber = item.PartNumber;
                existingItem.Notes = item.Notes;

                await _context.SaveChangesAsync();

                Logger.LogInfo($"Work order item updated successfully: {item.WorkOrderItemId}");
                return existingItem;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating work order item: {item.WorkOrderItemId}");
                throw;
            }
        }

        public async Task<bool> DeleteWorkOrderItemAsync(int itemId)
        {
            try
            {
                Logger.LogInfo($"Deleting work order item with ID: {itemId}");

                var item = await _context.WorkOrderItems
                    .FirstOrDefaultAsync(i => i.WorkOrderItemId == itemId);

                if (item == null)
                {
                    Logger.LogWarning($"Work order item with ID {itemId} not found for deletion");
                    return false;
                }

                // Soft delete
                item.IsActive = false;
                await _context.SaveChangesAsync();

                Logger.LogInfo($"Work order item deleted successfully: {itemId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting work order item: {itemId}");
                throw;
            }
        }

        #endregion

        private async Task LoadWorkOrderRelatedEntities(WorkOrder workOrder)
        {
            await _context.Entry(workOrder)
                .Reference(w => w.Customer)
                .LoadAsync();

            await _context.Entry(workOrder)
                .Reference(w => w.WorkOrderType)
                .LoadAsync();

            await _context.Entry(workOrder)
                .Reference(w => w.Category)
                .LoadAsync();

            await _context.Entry(workOrder)
                .Reference(w => w.Status)
                .LoadAsync();

            if (workOrder.TechnicianId.HasValue)
            {
                await _context.Entry(workOrder)
                    .Reference(w => w.Technician)
                    .LoadAsync();
            }
        }
    }
}