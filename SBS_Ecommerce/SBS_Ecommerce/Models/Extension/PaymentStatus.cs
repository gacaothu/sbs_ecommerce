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
        Pending = 0,
        /// <summary>
        /// Processed
        /// </summary>
        Processed = 1,
        /// <summary>
        /// Delivered
        /// </summary>
        Delivered = 2,
        /// <summary>
        /// Canceled
        /// </summary>
        Canceled = 3,
        /// <summary>
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
}