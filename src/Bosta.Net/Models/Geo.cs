using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bosta.Net.Models
{
    /// <summary>
    /// A city served by Bosta.
    /// </summary>
    public class City
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("countryId")]
        public string CountryId { get; set; }
    }

    /// <summary>
    /// A zone within a city.
    /// </summary>
    public class Zone
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cityId")]
        public string CityId { get; set; }
    }

    /// <summary>
    /// A district within a city.
    /// </summary>
    public class District
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("zoneId")]
        public string ZoneId { get; set; }
    }

    /// <summary>
    /// A contact person for pickup requests/locations.
    /// </summary>
    public class ContactPerson
    {
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }

        [JsonPropertyName("firstName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string LastName { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Email { get; set; }

        [JsonPropertyName("isDefault")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsDefault { get; set; }
    }

    /// <summary>
    /// A business pickup location.
    /// </summary>
    public class PickupLocation
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("locationName")]
        public string LocationName { get; set; }

        [JsonPropertyName("contacts")]
        public List<ContactPerson> Contacts { get; set; }

        [JsonPropertyName("address")]
        public DeliveryAddress Address { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// Request to create a pickup request.
    /// </summary>
    public class CreatePickupRequest
    {
        /// <summary>Scheduled date, e.g. "2026-10-09".</summary>
        [JsonPropertyName("scheduledDate")]
        public string ScheduledDate { get; set; }

        /// <summary>Time slot, e.g. "10:00-13:00".</summary>
        [JsonPropertyName("scheduledTimeSlot")]
        public string ScheduledTimeSlot { get; set; }

        [JsonPropertyName("contactPerson")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ContactPerson ContactPerson { get; set; }

        [JsonPropertyName("businessLocationId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BusinessLocationId { get; set; }

        [JsonPropertyName("notes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Notes { get; set; }

        [JsonPropertyName("noOfPackages")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? NumberOfPackages { get; set; }

        [JsonPropertyName("trackingNumbers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> TrackingNumbers { get; set; }
    }

    /// <summary>
    /// A pickup request.
    /// </summary>
    public class PickupRequest
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("scheduledDate")]
        public string ScheduledDate { get; set; }

        [JsonPropertyName("scheduledTimeSlot")]
        public string ScheduledTimeSlot { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("noOfPackages")]
        public int? NumberOfPackages { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }
    }

    /// <summary>
    /// Shipment price estimate.
    /// </summary>
    public class PriceEstimate
    {
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("zone")]
        public string Zone { get; set; }
    }
}
