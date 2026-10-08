using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bosta.Net.Models
{
    /// <summary>
    /// A delivery address (drop-off, pickup, or return).
    /// </summary>
    public class DeliveryAddress
    {
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("zoneId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ZoneId { get; set; }

        [JsonPropertyName("districtId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DistrictId { get; set; }

        [JsonPropertyName("firstLine")]
        public string FirstLine { get; set; }

        [JsonPropertyName("secondLine")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SecondLine { get; set; }

        [JsonPropertyName("buildingNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BuildingNumber { get; set; }

        [JsonPropertyName("floor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Floor { get; set; }

        [JsonPropertyName("apartment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Apartment { get; set; }
    }

    /// <summary>
    /// The customer receiving the delivery.
    /// </summary>
    public class Receiver
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LastName { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("secondPhone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SecondPhone { get; set; }

        [JsonPropertyName("email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Email { get; set; }
    }

    /// <summary>
    /// Package specifications for a delivery.
    /// </summary>
    public class PackageSpecs
    {
        [JsonPropertyName("size")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Size { get; set; }

        [JsonPropertyName("packageType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PackageType { get; set; }

        [JsonPropertyName("packageDetails")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PackageDetails PackageDetails { get; set; }
    }

    /// <summary>
    /// Details about the package contents.
    /// </summary>
    public class PackageDetails
    {
        [JsonPropertyName("itemsCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ItemsCount { get; set; }

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; }
    }

    /// <summary>
    /// Request to create a delivery.
    /// </summary>
    public class CreateDeliveryRequest
    {
        /// <summary>Order type: 10 = Send, 15 = Cash collection, 25 = Customer return pickup, 30 = Exchange.</summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }

        /// <summary>Cash amount to collect from the customer on delivery (max 30,000 EGP; negative for CRP refunds).</summary>
        [JsonPropertyName("cod")]
        public decimal Cod { get; set; }

        [JsonPropertyName("specs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PackageSpecs Specs { get; set; }

        [JsonPropertyName("returnSpecs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PackageSpecs ReturnSpecs { get; set; }

        [JsonPropertyName("notes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Notes { get; set; }

        [JsonPropertyName("returnNotes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ReturnNotes { get; set; }

        [JsonPropertyName("dropOffAddress")]
        public DeliveryAddress DropOffAddress { get; set; }

        [JsonPropertyName("pickupAddress")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeliveryAddress PickupAddress { get; set; }

        [JsonPropertyName("returnAddress")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DeliveryAddress ReturnAddress { get; set; }

        [JsonPropertyName("receiver")]
        public Receiver Receiver { get; set; }

        /// <summary>An ID that uniquely identifies the order in your system.</summary>
        [JsonPropertyName("businessReference")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BusinessReference { get; set; }

        [JsonPropertyName("allowToOpenPackage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AllowToOpenPackage { get; set; }
    }

    /// <summary>
    /// A delivery as returned by the Bosta API.
    /// </summary>
    public class Delivery
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("state")]
        public DeliveryStateInfo State { get; set; }

        [JsonPropertyName("cod")]
        public decimal? Cod { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("receiver")]
        public Receiver Receiver { get; set; }

        [JsonPropertyName("dropOffAddress")]
        public DeliveryAddress DropOffAddress { get; set; }

        [JsonPropertyName("businessReference")]
        public string BusinessReference { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("history")]
        public List<DeliveryHistoryEntry> History { get; set; }

        /// <summary>True when the delivery reached a terminal state (delivered, cancelled, failed, returned).</summary>
        [JsonIgnore]
        public bool IsTerminal =>
            State != null && (State.Code == (int)DeliveryState.Delivered ||
                              State.Code == (int)DeliveryState.Cancelled ||
                              State.Code == (int)DeliveryState.Failed ||
                              State.Code == (int)DeliveryState.Exception ||
                              State.Code == (int)DeliveryState.ReturnedToBusiness);
    }

    /// <summary>
    /// The current state of a delivery.
    /// </summary>
    public class DeliveryStateInfo
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// A single entry in a delivery's tracking history.
    /// </summary>
    public class DeliveryHistoryEntry
    {
        [JsonPropertyName("state")]
        public DeliveryStateInfo State { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("exceptionCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ExceptionCode { get; set; }

        [JsonPropertyName("exceptionReason")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ExceptionReason { get; set; }
    }

    /// <summary>
    /// Response from creating a delivery.
    /// </summary>
    public class CreateDeliveryResponse
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonPropertyName("businessReference")]
        public string BusinessReference { get; set; }
    }

    /// <summary>
    /// Request to print airway bills.
    /// </summary>
    public class PrintAwbRequest
    {
        /// <summary>Comma-separated tracking numbers, e.g. "77873113,77873114".</summary>
        [JsonPropertyName("trackingNumbers")]
        public string TrackingNumbers { get; set; }

        /// <summary>Print size: "A4" or "A6".</summary>
        [JsonPropertyName("size")]
        public string Size { get; set; } = AwbSizes.A4;

        /// <summary>Language: "ar" or "en".</summary>
        [JsonPropertyName("lang")]
        public string Language { get; set; } = "ar";
    }

    /// <summary>
    /// Response from the AWB endpoint (base64-encoded PDF).
    /// </summary>
    public class PrintAwbResponse
    {
        [JsonPropertyName("awb")]
        public string AwbBase64 { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// Search criteria for deliveries.
    /// </summary>
    public class SearchDeliveriesRequest
    {
        /// <summary>Comma-separated tracking numbers.</summary>
        [JsonPropertyName("trackingNumbers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TrackingNumbers { get; set; }

        [JsonPropertyName("businessReference")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BusinessReference { get; set; }

        [JsonPropertyName("pageId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PageId { get; set; }
    }
}
