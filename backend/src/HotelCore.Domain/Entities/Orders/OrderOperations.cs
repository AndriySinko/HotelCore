namespace HotelCore.Domain.Entities.Orders;

/// <summary>
/// Operations that can be performed on an order.
/// </summary>
public static class OrderOperations
{
    public static class Read
    {
        public const string Name = "Read";
    }

    public static class Update
    {
        public const string Name = "Update";
    }

    public static class Delete
    {
        public const string Name = "Delete";
    }

    public static class Manage
    {
        public const string Name = "Manage";
    }

    public static class ReadConfidential
    {
        public const string Name = "ReadConfidential";
    }
}
