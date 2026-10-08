namespace Bosta.Net.Models
{
    /// <summary>
    /// Delivery (order) types supported by Bosta.
    /// </summary>
    public enum DeliveryType
    {
        /// <summary>Standard package delivery (code 10). COD must be positive.</summary>
        Send = 10,
        /// <summary>Cash collection (code 15).</summary>
        CashCollection = 15,
        /// <summary>Customer return pickup (code 25). COD must be negative when a refund is issued.</summary>
        CustomerReturnPickup = 25,
        /// <summary>Exchange (code 30).</summary>
        Exchange = 30
    }

    /// <summary>
    /// Known delivery states. Bosta reports states as numeric codes;
    /// the most common terminal states are listed here.
    /// </summary>
    public enum DeliveryState
    {
        /// <summary>Unknown / unmapped state code.</summary>
        Unknown = 0,
        /// <summary>Pickup requested.</summary>
        PickupRequested = 21,
        /// <summary>Picked up.</summary>
        PickedUp = 22,
        /// <summary>Out for delivery / delivering.</summary>
        Delivering = 43,
        /// <summary>Delivered.</summary>
        Delivered = 45,
        /// <summary>Delivery failed / exception.</summary>
        Exception = 46,
        /// <summary>Delivery failed.</summary>
        Failed = 47,
        /// <summary>Cancelled / terminated.</summary>
        Cancelled = 48,
        /// <summary>Returned to business.</summary>
        ReturnedToBusiness = 49
    }

    /// <summary>
    /// Package sizes accepted by Bosta.
    /// </summary>
    public static class PackageSizes
    {
        public const string Small = "SMALL";
        public const string Medium = "MEDIUM";
        public const string Large = "LARGE";
        public const string LightBulky = "Light Bulky";
        public const string HeavyBulky = "Heavy Bulky";
    }

    /// <summary>
    /// Package types accepted by Bosta.
    /// </summary>
    public static class PackageTypes
    {
        public const string Parcel = "Parcel";
        public const string Document = "Document";
        public const string LightBulky = "Light Bulky";
        public const string HeavyBulky = "Heavy Bulky";
    }

    /// <summary>
    /// AWB print sizes.
    /// </summary>
    public static class AwbSizes
    {
        public const string A4 = "A4";
        public const string A6 = "A6";
    }
}
