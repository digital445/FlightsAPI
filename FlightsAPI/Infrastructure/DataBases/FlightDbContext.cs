using FlightsAPI.Models.FlightDb;
using Microsoft.EntityFrameworkCore;

namespace FlightsAPI.Infrastructure.DataBases;

public partial class FlightDbContext : DbContext
{
    public FlightDbContext()
    {
	}

	public FlightDbContext(DbContextOptions<FlightDbContext> contextOptions)
        : base(contextOptions)
    {
    }

    public virtual DbSet<dbAircraft> Aircrafts { get; set; }

    public virtual DbSet<AircraftsDatum> AircraftsData { get; set; }

    public virtual DbSet<dbAirport> Airports { get; set; }

    public virtual DbSet<AirportsDatum> AirportsData { get; set; }

    public virtual DbSet<BoardingPass> BoardingPasses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightsV> FlightsVs { get; set; }

    public virtual DbSet<dbRoute> Routes { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketFlight> TicketFlights { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<dbAircraft>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("aircrafts", "bookings");

            entity.Property(e => e.AircraftCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Aircraft code, IATA")
                .HasColumnName("aircraft_code");
            entity.Property(e => e.Model)
                .HasComment("Aircraft model")
                .HasColumnName("model");
            entity.Property(e => e.Range)
                .HasComment("Maximal flying distance, km")
                .HasColumnName("range");
        });

        modelBuilder.Entity<AircraftsDatum>(entity =>
        {
            entity.HasKey(e => e.AircraftCode).HasName("aircrafts_pkey");

            entity.ToTable("aircrafts_data", "bookings", tb => tb.HasComment("Aircrafts (internal data)"));

            entity.Property(e => e.AircraftCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Aircraft code, IATA")
                .HasColumnName("aircraft_code");
            entity.Property(e => e.Model)
                .HasComment("Aircraft model")
                .HasColumnType("jsonb")
                .HasColumnName("model");
            entity.Property(e => e.Range)
                .HasComment("Maximal flying distance, km")
                .HasColumnName("range");
        });

        modelBuilder.Entity<dbAirport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("airports", "bookings");

            entity.Property(e => e.AirportCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport code")
                .HasColumnName("airport_code");
            entity.Property(e => e.AirportName)
                .HasComment("Airport name")
                .HasColumnName("airport_name");
            entity.Property(e => e.City)
                .HasComment("City")
                .HasColumnName("city");
            entity.Property(e => e.Coordinates)
                .HasComment("Airport coordinates (longitude and latitude)")
                .HasColumnName("coordinates");
            entity.Property(e => e.Timezone)
                .HasComment("Airport time zone")
                .HasColumnName("timezone");
        });

        modelBuilder.Entity<AirportsDatum>(entity =>
        {
            entity.HasKey(e => e.AirportCode).HasName("airports_data_pkey");

            entity.ToTable("airports_data", "bookings", tb => tb.HasComment("Airports (internal data)"));

            entity.Property(e => e.AirportCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport code")
                .HasColumnName("airport_code");
            entity.Property(e => e.AirportName)
                .HasComment("Airport name")
                .HasColumnType("jsonb")
                .HasColumnName("airport_name");
            entity.Property(e => e.City)
                .HasComment("City")
                .HasColumnType("jsonb")
                .HasColumnName("city");
            entity.Property(e => e.Coordinates)
                .HasComment("Airport coordinates (longitude and latitude)")
                .HasColumnName("coordinates");
            entity.Property(e => e.Timezone)
                .HasComment("Airport time zone")
                .HasColumnName("timezone");
        });

        modelBuilder.Entity<BoardingPass>(entity =>
        {
            entity.HasKey(e => new { e.TicketNo, e.FlightId }).HasName("boarding_passes_pkey");

            entity.ToTable("boarding_passes", "bookings", tb => tb.HasComment("Boarding passes"));

            entity.HasIndex(e => new { e.FlightId, e.BoardingNo }, "boarding_passes_flight_id_boarding_no_key").IsUnique();

            entity.HasIndex(e => new { e.FlightId, e.SeatNo }, "boarding_passes_flight_id_seat_no_key").IsUnique();

            entity.Property(e => e.TicketNo)
                .HasMaxLength(13)
                .IsFixedLength()
                .HasComment("Ticket number")
                .HasColumnName("ticket_no");
            entity.Property(e => e.FlightId)
                .HasComment("Flight ID")
                .HasColumnName("flight_id");
            entity.Property(e => e.BoardingNo)
                .HasComment("Boarding pass number")
                .HasColumnName("boarding_no");
            entity.Property(e => e.SeatNo)
                .HasMaxLength(4)
                .HasComment("Seat number")
                .HasColumnName("seat_no");

            entity.HasOne(d => d.TicketFlight).WithOne(p => p.BoardingPass)
                .HasForeignKey<BoardingPass>(d => new { d.TicketNo, d.FlightId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("boarding_passes_ticket_no_fkey");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookRef).HasName("bookings_pkey");

            entity.ToTable("bookings", "bookings", tb => tb.HasComment("Bookings"));

            entity.Property(e => e.BookRef)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasComment("Booking number")
                .HasColumnName("book_ref");
            entity.Property(e => e.BookDate)
                .HasComment("Booking _date")
                .HasColumnName("book_date");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasComment("Total booking cost")
                .HasColumnName("total_amount");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("flights_pkey");

            entity.ToTable("flights", "bookings", tb => tb.HasComment("Flights"));

            entity.HasIndex(e => new { e.FlightNo, e.ScheduledDeparture }, "flights_flight_no_scheduled_departure_key").IsUnique();

            entity.Property(e => e.FlightId)
                .HasDefaultValueSql("nextval('flights_flight_id_seq'::regclass)")
                .HasComment("Flight ID")
                .HasColumnName("flight_id");
            entity.Property(e => e.ActualArrival)
                .HasComment("Actual arrival time")
                .HasColumnName("actual_arrival");
            entity.Property(e => e.ActualDeparture)
                .HasComment("Actual departure time")
                .HasColumnName("actual_departure");
            entity.Property(e => e.AircraftCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Aircraft code, IATA")
                .HasColumnName("aircraft_code");
            entity.Property(e => e.ArrivalAirport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport of arrival")
                .HasColumnName("arrival_airport");
            entity.Property(e => e.DepartureAirport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Airport of departure")
                .HasColumnName("departure_airport");
            entity.Property(e => e.FlightNo)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasComment("Flight number")
                .HasColumnName("flight_no");
            entity.Property(e => e.ScheduledArrival)
                .HasComment("Scheduled arrival time")
                .HasColumnName("scheduled_arrival");
            entity.Property(e => e.ScheduledDeparture)
                .HasComment("Scheduled departure time")
                .HasColumnName("scheduled_departure");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasComment("Flight status")
                .HasColumnName("status");

            entity.HasOne(d => d.AircraftCodeNavigation).WithMany(p => p.Flights)
                .HasForeignKey(d => d.AircraftCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("flights_aircraft_code_fkey");

            entity.HasOne(d => d.ArrivalAirportNavigation).WithMany(p => p.FlightArrivalAirportNavigations)
                .HasForeignKey(d => d.ArrivalAirport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("flights_arrival_airport_fkey");

            entity.HasOne(d => d.DepartureAirportNavigation).WithMany(p => p.FlightDepartureAirportNavigations)
                .HasForeignKey(d => d.DepartureAirport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("flights_departure_airport_fkey");
        });

        modelBuilder.Entity<FlightsV>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("flights_v", "bookings");

            entity.Property(e => e.ActualArrival)
                .HasComment("Actual arrival time")
                .HasColumnName("actual_arrival");
            entity.Property(e => e.ActualArrivalLocal)
                .HasComment("Actual arrival time, local time at the point of destination")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actual_arrival_local");
            entity.Property(e => e.ActualDeparture)
                .HasComment("Actual departure time")
                .HasColumnName("actual_departure");
            entity.Property(e => e.ActualDepartureLocal)
                .HasComment("Actual departure time, local time at the point of departure")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actual_departure_local");
            entity.Property(e => e.ActualDuration)
                .HasComment("Actual flight duration")
                .HasColumnName("actual_duration");
            entity.Property(e => e.AircraftCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Aircraft code, IATA")
                .HasColumnName("aircraft_code");
            entity.Property(e => e.ArrivalAirport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Arrival airport code")
                .HasColumnName("arrival_airport");
            entity.Property(e => e.ArrivalAirportName)
                .HasComment("Arrival airport name")
                .HasColumnName("arrival_airport_name");
            entity.Property(e => e.ArrivalCity)
                .HasComment("City of arrival")
                .HasColumnName("arrival_city");
            entity.Property(e => e.DepartureAirport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Deprature airport code")
                .HasColumnName("departure_airport");
            entity.Property(e => e.DepartureAirportName)
                .HasComment("Departure airport name")
                .HasColumnName("departure_airport_name");
            entity.Property(e => e.DepartureCity)
                .HasComment("City of departure")
                .HasColumnName("departure_city");
            entity.Property(e => e.FlightId)
                .HasComment("Flight ID")
                .HasColumnName("flight_id");
            entity.Property(e => e.FlightNo)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasComment("Flight number")
                .HasColumnName("flight_no");
            entity.Property(e => e.ScheduledArrival)
                .HasComment("Scheduled arrival time")
                .HasColumnName("scheduled_arrival");
            entity.Property(e => e.ScheduledArrivalLocal)
                .HasComment("Scheduled arrival time, local time at the point of destination")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scheduled_arrival_local");
            entity.Property(e => e.ScheduledDeparture)
                .HasComment("Scheduled departure time")
                .HasColumnName("scheduled_departure");
            entity.Property(e => e.ScheduledDepartureLocal)
                .HasComment("Scheduled departure time, local time at the point of departure")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("scheduled_departure_local");
            entity.Property(e => e.ScheduledDuration)
                .HasComment("Scheduled flight duration")
                .HasColumnName("scheduled_duration");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasComment("Flight status")
                .HasColumnName("status");
        });

        modelBuilder.Entity<dbRoute>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("routes", "bookings");

            entity.Property(e => e.AircraftCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Aircraft code, IATA")
                .HasColumnName("aircraft_code");
            entity.Property(e => e.ArrivalAirport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Code of airport of arrival")
                .HasColumnName("arrival_airport");
            entity.Property(e => e.ArrivalAirportName)
                .HasComment("Name of airport of arrival")
                .HasColumnName("arrival_airport_name");
            entity.Property(e => e.ArrivalCity)
                .HasComment("City of arrival")
                .HasColumnName("arrival_city");
            entity.Property(e => e.DaysOfWeek)
                .HasComment("Days of week on which flights are scheduled")
                .HasColumnName("days_of_week");
            entity.Property(e => e.DepartureAirport)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Code of airport of departure")
                .HasColumnName("departure_airport");
            entity.Property(e => e.DepartureAirportName)
                .HasComment("Name of airport of departure")
                .HasColumnName("departure_airport_name");
            entity.Property(e => e.DepartureCity)
                .HasComment("City of departure")
                .HasColumnName("departure_city");
            entity.Property(e => e.Duration)
                .HasComment("Scheduled duration of flight")
                .HasColumnName("duration");
            entity.Property(e => e.FlightNo)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasComment("Flight number")
                .HasColumnName("flight_no");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => new { e.AircraftCode, e.SeatNo }).HasName("seats_pkey");

            entity.ToTable("seats", "bookings", tb => tb.HasComment("Seats"));

            entity.Property(e => e.AircraftCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasComment("Aircraft code, IATA")
                .HasColumnName("aircraft_code");
            entity.Property(e => e.SeatNo)
                .HasMaxLength(4)
                .HasComment("Seat number")
                .HasColumnName("seat_no");
            entity.Property(e => e.FareConditions)
                .HasMaxLength(10)
                .HasComment("Travel class")
                .HasColumnName("fare_conditions");

            entity.HasOne(d => d.AircraftCodeNavigation).WithMany(p => p.Seats)
                .HasForeignKey(d => d.AircraftCode)
                .HasConstraintName("seats_aircraft_code_fkey");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketNo).HasName("tickets_pkey");

            entity.ToTable("tickets", "bookings", tb => tb.HasComment("Tickets"));

            entity.Property(e => e.TicketNo)
                .HasMaxLength(13)
                .IsFixedLength()
                .HasComment("Ticket number")
                .HasColumnName("ticket_no");
            entity.Property(e => e.BookRef)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasComment("Booking number")
                .HasColumnName("book_ref");
            entity.Property(e => e.ContactData)
                .HasComment("Passenger contact information")
                .HasColumnType("jsonb")
                .HasColumnName("contact_data");
            entity.Property(e => e.PassengerId)
                .HasMaxLength(20)
                .HasComment("Passenger ID")
                .HasColumnName("passenger_id");
            entity.Property(e => e.PassengerName)
                .HasComment("Passenger name")
                .HasColumnName("passenger_name");

            entity.HasOne(d => d.BookRefNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BookRef)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_book_ref_fkey");
        });

        modelBuilder.Entity<TicketFlight>(entity =>
        {
            entity.HasKey(e => new { e.TicketNo, e.FlightId }).HasName("ticket_flights_pkey");

            entity.ToTable("ticket_flights", "bookings", tb => tb.HasComment("Flight segment"));

            entity.Property(e => e.TicketNo)
                .HasMaxLength(13)
                .IsFixedLength()
                .HasComment("Ticket number")
                .HasColumnName("ticket_no");
            entity.Property(e => e.FlightId)
                .HasComment("Flight ID")
                .HasColumnName("flight_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasComment("Travel cost")
                .HasColumnName("amount");
            entity.Property(e => e.FareConditions)
                .HasMaxLength(10)
                .HasComment("Travel class")
                .HasColumnName("fare_conditions");

            entity.HasOne(d => d.Flight).WithMany(p => p.TicketFlights)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticket_flights_flight_id_fkey");

            entity.HasOne(d => d.TicketNoNavigation).WithMany(p => p.TicketFlights)
                .HasForeignKey(d => d.TicketNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticket_flights_ticket_no_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
