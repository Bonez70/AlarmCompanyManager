namespace AlarmCompanyManager.Models
{
    public enum CustomerTypeEnum
    {
        Residential = 1,
        Commercial = 2,
        Government = 3,
        Education = 4
    }

    public enum ContactTypeEnum
    {
        Owner = 1,
        Primary = 2,
        Secondary = 3
    }

    public enum MonitoringTypeEnum
    {
        SecurityMonitored = 1,
        FireULMonitored = 2,
        UnMonitored = 3
    }

    public enum WorkOrderTypeEnum
    {
        ServiceCall = 1,
        Installation = 2,
        Inspection = 3
    }

    public enum WorkOrderStatusEnum
    {
        Unscheduled = 1,
        Scheduled = 2,
        InProgress = 3,
        Pending = 4,
        Canceled = 5,
        Completed = 6
    }

    public enum WorkOrderCategoryEnum
    {
        SecuritySystem = 1,
        FireSystem = 2,
        CCTV = 3,
        AccessControl = 4
    }

    public enum CommunicatorTypeEnum
    {
        GSMCell = 1,
        AES = 2,
        IP = 3,
        POTS = 4,
        Other = 5
    }

    public enum DeviceTypeEnum
    {
        DoorWindow = 1,
        Motion = 2,
        Glass = 3,
        Smoke = 4,
        Heat = 5,
        CO = 6,
        Panic = 7,
        Medical = 8,
        Other = 9
    }
}