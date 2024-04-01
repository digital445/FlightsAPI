using System;
using System.Collections.Generic;

namespace FlightsAPI.Models.FlightDb;

public partial class FlightsV
{
    /// <summary>
    /// Flight ID
    /// </summary>
    public int? FlightId { get; set; }

    /// <summary>
    /// Flight number
    /// </summary>
    public string? FlightNo { get; set; }

    /// <summary>
    /// Scheduled departure time
    /// </summary>
    public DateTime? ScheduledDeparture { get; set; }

    /// <summary>
    /// Scheduled departure time, local time at the point of departure
    /// </summary>
    public DateTime? ScheduledDepartureLocal { get; set; }

    /// <summary>
    /// Scheduled arrival time
    /// </summary>
    public DateTime? ScheduledArrival { get; set; }

    /// <summary>
    /// Scheduled arrival time, local time at the point of destination
    /// </summary>
    public DateTime? ScheduledArrivalLocal { get; set; }

    /// <summary>
    /// Scheduled flight duration
    /// </summary>
    public TimeSpan? ScheduledDuration { get; set; }

    /// <summary>
    /// Deprature airport code
    /// </summary>
    public string? DepartureAirport { get; set; }

    /// <summary>
    /// Departure airport name
    /// </summary>
    public string? DepartureAirportName { get; set; }

    /// <summary>
    /// City of departure
    /// </summary>
    public string? DepartureCity { get; set; }

    /// <summary>
    /// Arrival airport code
    /// </summary>
    public string? ArrivalAirport { get; set; }

    /// <summary>
    /// Arrival airport name
    /// </summary>
    public string? ArrivalAirportName { get; set; }

    /// <summary>
    /// City of arrival
    /// </summary>
    public string? ArrivalCity { get; set; }

    /// <summary>
    /// Flight status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Aircraft code, IATA
    /// </summary>
    public string? AircraftCode { get; set; }

    /// <summary>
    /// Actual departure time
    /// </summary>
    public DateTime? ActualDeparture { get; set; }

    /// <summary>
    /// Actual departure time, local time at the point of departure
    /// </summary>
    public DateTime? ActualDepartureLocal { get; set; }

    /// <summary>
    /// Actual arrival time
    /// </summary>
    public DateTime? ActualArrival { get; set; }

    /// <summary>
    /// Actual arrival time, local time at the point of destination
    /// </summary>
    public DateTime? ActualArrivalLocal { get; set; }

    /// <summary>
    /// Actual flight duration
    /// </summary>
    public TimeSpan? ActualDuration { get; set; }
}
