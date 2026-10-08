<div align="center">

# Bosta.Net

**The .NET SDK for the Bosta shipping API**

[![NuGet](https://img.shields.io/nuget/v/Bosta.Net.svg?style=flat-square)](https://www.nuget.org/packages/Bosta.Net/)
[![Build](https://img.shields.io/github/actions/workflow/status/SENESO/Bosta.Net/ci.yml?style=flat-square)](https://github.com/SENESO/Bosta.Net/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-gold?style=flat-square)](LICENSE)

</div>

Create and track shipments, schedule pickups, print airway bills, and handle webhooks — all from clean, strongly-typed C#.

Bosta is Egypt's tech-first shipping company. Their official SDKs cover PHP, Node.js, Python, Java and Ruby — this is the missing .NET piece.

## Installation

```bash
dotnet add package Bosta.Net
```

## Quick start

```csharp
using Bosta.Net;
using Bosta.Net.Models;

var client = new BostaClient("your-api-key");

// Create a delivery (COD)
var delivery = await client.Deliveries.CreateAsync(new CreateDeliveryRequest
{
    Type = (int)DeliveryType.Send,          // 10 = Send, 15 = Cash collection, 25 = Return, 30 = Exchange
    Cod = 250,                             // cash to collect on delivery (EGP)
    Receiver = new Receiver
    {
        FirstName = "Ahmed",
        Phone = "01001234567"
    },
    DropOffAddress = new DeliveryAddress
    {
        City = "Cairo",
        FirstLine = "12 Tahrir St, Downtown"
    },
    BusinessReference = "ORDER-1001",      // your own order id
    Notes = "Call before arrival"
});

Console.WriteLine($"Tracking: {delivery.TrackingNumber}");
```

## Tracking

```csharp
var shipment = await client.Deliveries.TrackAsync("12345678");

Console.WriteLine($"State: {shipment.State.Value}");   // e.g. "Delivered"
Console.WriteLine($"Terminal: {shipment.IsTerminal}");

foreach (var e in shipment.History)
    Console.WriteLine($"{e.Timestamp}: {e.State.Value}");
```

## Cancel a delivery

```csharp
await client.Deliveries.TerminateAsync("12345678");
```

## Cities, zones & districts

```csharp
var cities = await client.Cities.ListAsync();
var zones = await client.Cities.GetZonesAsync(cities[0].Id);
var districts = await client.Cities.GetDistrictsAsync(cities[0].Id);
```

## Pickup requests

```csharp
var pickup = await client.Pickups.CreateAsync(new CreatePickupRequest
{
    ScheduledDate = "2026-10-09",
    ScheduledTimeSlot = "10:00-13:00",
    NumberOfPackages = 3,
    Notes = "Call on arrival"
});

var locations = await client.PickupLocations.ListAsync();
```

## Print airway bills

```csharp
var awb = await client.Deliveries.PrintAwbAsync(new PrintAwbRequest
{
    TrackingNumbers = "11111111,22222222",
    Size = AwbSizes.A4,      // or AwbSizes.A6
    Language = "ar"          // or "en"
});

File.WriteAllBytes("awb.pdf", Convert.FromBase64String(awb.AwbBase64));
```

## Price estimate

```csharp
var estimate = await client.Pricing.CalculateShipmentPriceAsync(cityId, size: PackageSizes.Medium);
```

## Webhooks

Register your webhook URL in the Bosta dashboard. Bosta POSTs a JSON payload on every state change, including the secret you configured. Validate it with a constant-time comparison (fail-closed):

```csharp
using Bosta.Net.Webhooks;

[HttpPost("bosta-webhook")]
public IActionResult BostaWebhook([FromQuery] string secret, [FromBody] BostaWebhookEvent evt)
{
    if (!BostaWebhookValidator.IsValid(secret, _config["Bosta:WebhookSecret"]))
        return Unauthorized();

    if (evt.State == (int)DeliveryState.Delivered)
        _orders.MarkDelivered(evt.BusinessReference);

    return Ok();
}
```

> Note: Bosta webhooks carry no HMAC signature — authentication is the dashboard-registered secret compared in constant time.

## ASP.NET Core DI

```csharp
builder.Services.AddBosta("your-api-key");
// or
builder.Services.AddBosta(o => o.ApiKey = configuration["Bosta:ApiKey"]);

// then inject BostaClient anywhere
```

## Error handling

All API failures throw `BostaApiException` with the API's message, plus `ErrorCode` and `StatusCode` when available:

```csharp
try
{
    await client.Deliveries.CreateAsync(request);
}
catch (BostaApiException ex)
{
    Console.WriteLine($"{ex.StatusCode}: {ex.Message}");
}
```

## API coverage

| Area | Methods |
|---|---|
| Deliveries | Create, bulk create, get/track, search, terminate, print AWB |
| Pickups | List, get, create, update, delete, available dates |
| Geo | Cities, zones, districts |
| Pickup locations | List, get |
| Pricing | Shipment price estimate |
| Webhooks | Event model + secret validation |

## License

MIT — see [LICENSE](LICENSE).
