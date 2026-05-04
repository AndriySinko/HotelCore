namespace HotelCore.Domain.Enums;

public enum PaymentMethod
{
    Cash,           // in-person, handled by staff
    CreditCard,     // in-person card terminal
    RoomBill,       // charged to the guest's hotel account at checkout
    OnlinePayment,  // charged immediately via payment gateway
}
