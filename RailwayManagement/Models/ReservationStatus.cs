namespace RailwayManagement.Models
{
    /// <summary>
    /// Enumeration for reservation status tracking
    /// </summary>
    public enum ReservationStatus
    {
        PendingPayment = 0,
        Confirmed = 1,
        Cancelled = 2
    }
}