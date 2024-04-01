using System;
using System.Collections.Generic;
using FlightsAPI.Models.FlightDb;

namespace FlightsAPI.Models.FlightDb;

/// <summary>
/// Boarding passes
/// </summary>
public partial class BoardingPass
{
    /// <summary>
    /// Ticket number
    /// </summary>
    public string TicketNo { get; set; } = null!;

    /// <summary>
    /// Flight ID
    /// </summary>
    public int FlightId { get; set; }

    /// <summary>
    /// Boarding pass number
    /// </summary>
    public int BoardingNo { get; set; }

    /// <summary>
    /// Seat number
    /// </summary>
    public string SeatNo { get; set; } = null!;

    public virtual TicketFlight TicketFlight { get; set; } = null!;
}
