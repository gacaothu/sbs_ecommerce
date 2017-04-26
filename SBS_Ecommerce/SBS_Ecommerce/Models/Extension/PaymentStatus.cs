using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.Extension
{
    /// <summary>
    /// Represents a payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,
        /// <summary>
        /// Authorized
        /// </summary>
        Authorized = 20,
        /// <summary>
        /// Paid
        /// </summary>
        Paid = 30,
        /// <summary>
        /// Partially Refunded
        /// </summary>
        PartiallyRefunded = 35,
        /// <summary>
        /// Refunded
        /// </summary>
        Refunded = 40,
        /// <summary>
        /// Voided
        /// </summary>
        Voided = 50,
    }
    /// <summary>
    /// Represents an order status enumeration
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,
        /// <summary>
        /// Processing
        /// </summary>
        Processing = 20,
        /// <summary>
        /// Complete
        /// </summary>
        Completed = 40,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 50
    }

    /// <summary>
    /// Represents the shipping status enumeration
    /// </summary>
    public enum ShippingStatus
    {
        /// <summary>
        /// Shipping not required
        /// </summary>
        ShippingNotRequired = 210,
        /// <summary>
        /// Not yet shipped
        /// </summary>
        NotYetShipped = 220,
        /// <summary>
        /// Partially shipped
        /// </summary>
        PartiallyShipped = 250,
        /// <summary>
        /// Shipped
        /// </summary>
        Shipped = 230,
        /// <summary>
        /// Delivered
        /// </summary>
        Delivered = 240,
    }

    /// <summary>
    /// Represents a payment status enumeration
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Paypal
        /// </summary>
        Paypal = 1,
        /// <summary>
        /// Credit Card
        /// </summary>
        CreditCard = 2,
        /// <summary>
        /// Bank Tranfer
        /// </summary>
        BankTranfer = 3,
        /// <summary>
        /// Cash on Delivery
        /// </summary>
        CashOnDelivery = 5,
        /// <summary>
    }
    /// <summary>
    /// Represents a payment status enumeration
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Order
        /// </summary>
        Order = 1,
        /// <summary>
        /// PreOrder = 2,
        /// </summary>
        PreOrder = 2,
    }

    /// <summary>
    /// Represents a addrress type enumeration
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// ShippingAddress
        /// </summary>
        ShippingAddress = 1,
        /// <summary>
        /// BillingAddress = 2,
        /// </summary>
        BillingAddress = 2,
    }

    /// <summary>
    /// Represents a addrress type enumeration
    /// </summary>
    public enum ShippingMethod
    {
        /// <summary>
        /// Shipping
        /// </summary>
        Shipping = 1,
        /// <summary>
        /// NoShipping 
        /// </summary>
        NoShipping = -1,
    }
}