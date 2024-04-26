﻿// <auto-generated />
using System;
using System.Collections.Generic;
using FlightsAPI.Infrastructure.DataBases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace FlightsAPI.Migrations
{
    [DbContext(typeof(FlightDbContext))]
    partial class FlightDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.AircraftsDatum", b =>
                {
                    b.Property<string>("AircraftCode")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("aircraft_code")
                        .IsFixedLength()
                        .HasComment("Aircraft code, IATA");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("model")
                        .HasComment("Aircraft model");

                    b.Property<int>("Range")
                        .HasColumnType("integer")
                        .HasColumnName("range")
                        .HasComment("Maximal flying distance, km");

                    b.HasKey("AircraftCode")
                        .HasName("aircrafts_pkey");

                    b.ToTable("aircrafts_data", "bookings", t =>
                        {
                            t.HasComment("Aircrafts (internal data)");
                        });
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.AirportsDatum", b =>
                {
                    b.Property<string>("AirportCode")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("airport_code")
                        .IsFixedLength()
                        .HasComment("Airport code");

                    b.Property<string>("AirportName")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("airport_name")
                        .HasComment("Airport name");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("city")
                        .HasComment("City");

                    b.Property<NpgsqlPoint>("Coordinates")
                        .HasColumnType("point")
                        .HasColumnName("coordinates")
                        .HasComment("Airport coordinates (longitude and latitude)");

                    b.Property<string>("Timezone")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("timezone")
                        .HasComment("Airport time zone");

                    b.HasKey("AirportCode")
                        .HasName("airports_data_pkey");

                    b.ToTable("airports_data", "bookings", t =>
                        {
                            t.HasComment("Airports (internal data)");
                        });
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.BoardingPass", b =>
                {
                    b.Property<string>("TicketNo")
                        .HasMaxLength(13)
                        .HasColumnType("character(13)")
                        .HasColumnName("ticket_no")
                        .IsFixedLength()
                        .HasComment("Ticket number");

                    b.Property<int>("FlightId")
                        .HasColumnType("integer")
                        .HasColumnName("flight_id")
                        .HasComment("Flight ID");

                    b.Property<int>("BoardingNo")
                        .HasColumnType("integer")
                        .HasColumnName("boarding_no")
                        .HasComment("Boarding pass number");

                    b.Property<string>("SeatNo")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)")
                        .HasColumnName("seat_no")
                        .HasComment("Seat number");

                    b.HasKey("TicketNo", "FlightId")
                        .HasName("boarding_passes_pkey");

                    b.HasIndex(new[] { "FlightId", "BoardingNo" }, "boarding_passes_flight_id_boarding_no_key")
                        .IsUnique();

                    b.HasIndex(new[] { "FlightId", "SeatNo" }, "boarding_passes_flight_id_seat_no_key")
                        .IsUnique();

                    b.ToTable("boarding_passes", "bookings", t =>
                        {
                            t.HasComment("Boarding passes");
                        });
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Booking", b =>
                {
                    b.Property<string>("BookRef")
                        .HasMaxLength(6)
                        .HasColumnType("character(6)")
                        .HasColumnName("book_ref")
                        .IsFixedLength()
                        .HasComment("Booking number");

                    b.Property<DateTime>("BookDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("book_date")
                        .HasComment("Booking _date");

                    b.Property<decimal>("TotalAmount")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("total_amount")
                        .HasComment("Total booking cost");

                    b.HasKey("BookRef")
                        .HasName("bookings_pkey");

                    b.ToTable("bookings", "bookings", t =>
                        {
                            t.HasComment("Bookings");
                        });
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Flight", b =>
                {
                    b.Property<int>("FlightId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("flight_id")
                        .HasDefaultValueSql("nextval('flights_flight_id_seq'::regclass)")
                        .HasComment("Flight ID");

                    b.Property<DateTimeOffset?>("ActualArrival")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("actual_arrival")
                        .HasComment("Actual arrival time");

                    b.Property<DateTimeOffset?>("ActualDeparture")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("actual_departure")
                        .HasComment("Actual departure time");

                    b.Property<string>("AircraftCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("aircraft_code")
                        .IsFixedLength()
                        .HasComment("Aircraft code, IATA");

                    b.Property<string>("ArrivalAirport")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("arrival_airport")
                        .IsFixedLength()
                        .HasComment("Airport of arrival");

                    b.Property<string>("DepartureAirport")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("departure_airport")
                        .IsFixedLength()
                        .HasComment("Airport of departure");

                    b.Property<string>("FlightNo")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character(6)")
                        .HasColumnName("flight_no")
                        .IsFixedLength()
                        .HasComment("Flight number");

                    b.Property<DateTimeOffset>("ScheduledArrival")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("scheduled_arrival")
                        .HasComment("Scheduled arrival time");

                    b.Property<DateTimeOffset>("ScheduledDeparture")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("scheduled_departure")
                        .HasComment("Scheduled departure time");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("status")
                        .HasComment("Flight status");

                    b.HasKey("FlightId")
                        .HasName("flights_pkey");

                    b.HasIndex("AircraftCode");

                    b.HasIndex("ArrivalAirport");

                    b.HasIndex("DepartureAirport");

                    b.HasIndex(new[] { "FlightNo", "ScheduledDeparture" }, "flights_flight_no_scheduled_departure_key")
                        .IsUnique();

                    b.ToTable("flights", "bookings", t =>
                        {
                            t.HasComment("Flights");
                        });
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.FlightsV", b =>
                {
                    b.Property<DateTime?>("ActualArrival")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("actual_arrival")
                        .HasComment("Actual arrival time");

                    b.Property<DateTime?>("ActualArrivalLocal")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("actual_arrival_local")
                        .HasComment("Actual arrival time, local time at the point of destination");

                    b.Property<DateTime?>("ActualDeparture")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("actual_departure")
                        .HasComment("Actual departure time");

                    b.Property<DateTime?>("ActualDepartureLocal")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("actual_departure_local")
                        .HasComment("Actual departure time, local time at the point of departure");

                    b.Property<TimeSpan?>("ActualDuration")
                        .HasColumnType("interval")
                        .HasColumnName("actual_duration")
                        .HasComment("Actual flight duration");

                    b.Property<string>("AircraftCode")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("aircraft_code")
                        .IsFixedLength()
                        .HasComment("Aircraft code, IATA");

                    b.Property<string>("ArrivalAirport")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("arrival_airport")
                        .IsFixedLength()
                        .HasComment("Arrival airport code");

                    b.Property<string>("ArrivalAirportName")
                        .HasColumnType("text")
                        .HasColumnName("arrival_airport_name")
                        .HasComment("Arrival airport name");

                    b.Property<string>("ArrivalCity")
                        .HasColumnType("text")
                        .HasColumnName("arrival_city")
                        .HasComment("City of arrival");

                    b.Property<string>("DepartureAirport")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("departure_airport")
                        .IsFixedLength()
                        .HasComment("Deprature airport code");

                    b.Property<string>("DepartureAirportName")
                        .HasColumnType("text")
                        .HasColumnName("departure_airport_name")
                        .HasComment("Departure airport name");

                    b.Property<string>("DepartureCity")
                        .HasColumnType("text")
                        .HasColumnName("departure_city")
                        .HasComment("City of departure");

                    b.Property<int?>("FlightId")
                        .HasColumnType("integer")
                        .HasColumnName("flight_id")
                        .HasComment("Flight ID");

                    b.Property<string>("FlightNo")
                        .HasMaxLength(6)
                        .HasColumnType("character(6)")
                        .HasColumnName("flight_no")
                        .IsFixedLength()
                        .HasComment("Flight number");

                    b.Property<DateTime?>("ScheduledArrival")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("scheduled_arrival")
                        .HasComment("Scheduled arrival time");

                    b.Property<DateTime?>("ScheduledArrivalLocal")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("scheduled_arrival_local")
                        .HasComment("Scheduled arrival time, local time at the point of destination");

                    b.Property<DateTime?>("ScheduledDeparture")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("scheduled_departure")
                        .HasComment("Scheduled departure time");

                    b.Property<DateTime?>("ScheduledDepartureLocal")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("scheduled_departure_local")
                        .HasComment("Scheduled departure time, local time at the point of departure");

                    b.Property<TimeSpan?>("ScheduledDuration")
                        .HasColumnType("interval")
                        .HasColumnName("scheduled_duration")
                        .HasComment("Scheduled flight duration");

                    b.Property<string>("Status")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("status")
                        .HasComment("Flight status");

                    b.ToTable((string)null);

                    b.ToView("flights_v", "bookings");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Seat", b =>
                {
                    b.Property<string>("AircraftCode")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("aircraft_code")
                        .IsFixedLength()
                        .HasComment("Aircraft code, IATA");

                    b.Property<string>("SeatNo")
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)")
                        .HasColumnName("seat_no")
                        .HasComment("Seat number");

                    b.Property<string>("FareConditions")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("fare_conditions")
                        .HasComment("Travel class");

                    b.HasKey("AircraftCode", "SeatNo")
                        .HasName("seats_pkey");

                    b.ToTable("seats", "bookings", t =>
                        {
                            t.HasComment("Seats");
                        });
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Ticket", b =>
                {
                    b.Property<string>("TicketNo")
                        .HasMaxLength(13)
                        .HasColumnType("character(13)")
                        .HasColumnName("ticket_no")
                        .IsFixedLength()
                        .HasComment("Ticket number");

                    b.Property<string>("BookRef")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character(6)")
                        .HasColumnName("book_ref")
                        .IsFixedLength()
                        .HasComment("Booking number");

                    b.Property<string>("ContactData")
                        .HasColumnType("jsonb")
                        .HasColumnName("contact_data")
                        .HasComment("Passenger contact information");

                    b.Property<string>("PassengerId")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("passenger_id")
                        .HasComment("Passenger ID");

                    b.Property<string>("PassengerName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("passenger_name")
                        .HasComment("Passenger name");

                    b.HasKey("TicketNo")
                        .HasName("tickets_pkey");

                    b.HasIndex("BookRef");

                    b.ToTable("tickets", "bookings", t =>
                        {
                            t.HasComment("Tickets");
                        });
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.TicketFlight", b =>
                {
                    b.Property<string>("TicketNo")
                        .HasMaxLength(13)
                        .HasColumnType("character(13)")
                        .HasColumnName("ticket_no")
                        .IsFixedLength()
                        .HasComment("Ticket number");

                    b.Property<int>("FlightId")
                        .HasColumnType("integer")
                        .HasColumnName("flight_id")
                        .HasComment("Flight ID");

                    b.Property<decimal>("Amount")
                        .HasPrecision(10, 2)
                        .HasColumnType("numeric(10,2)")
                        .HasColumnName("amount")
                        .HasComment("Travel cost");

                    b.Property<string>("FareConditions")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("fare_conditions")
                        .HasComment("Travel class");

                    b.HasKey("TicketNo", "FlightId")
                        .HasName("ticket_flights_pkey");

                    b.HasIndex("FlightId");

                    b.ToTable("ticket_flights", "bookings", t =>
                        {
                            t.HasComment("Flight segment");
                        });
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.dbAircraft", b =>
                {
                    b.Property<string>("AircraftCode")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("aircraft_code")
                        .IsFixedLength()
                        .HasComment("Aircraft code, IATA");

                    b.Property<string>("Model")
                        .HasColumnType("text")
                        .HasColumnName("model")
                        .HasComment("Aircraft model");

                    b.Property<int?>("Range")
                        .HasColumnType("integer")
                        .HasColumnName("range")
                        .HasComment("Maximal flying distance, km");

                    b.ToTable((string)null);

                    b.ToView("aircrafts", "bookings");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.dbAirport", b =>
                {
                    b.Property<string>("AirportCode")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("airport_code")
                        .IsFixedLength()
                        .HasComment("Airport code");

                    b.Property<string>("AirportName")
                        .HasColumnType("text")
                        .HasColumnName("airport_name")
                        .HasComment("Airport name");

                    b.Property<string>("City")
                        .HasColumnType("text")
                        .HasColumnName("city")
                        .HasComment("City");

                    b.Property<NpgsqlPoint?>("Coordinates")
                        .HasColumnType("point")
                        .HasColumnName("coordinates")
                        .HasComment("Airport coordinates (longitude and latitude)");

                    b.Property<string>("Timezone")
                        .HasColumnType("text")
                        .HasColumnName("timezone")
                        .HasComment("Airport time zone");

                    b.ToTable((string)null);

                    b.ToView("airports", "bookings");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.dbRoute", b =>
                {
                    b.Property<string>("AircraftCode")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("aircraft_code")
                        .IsFixedLength()
                        .HasComment("Aircraft code, IATA");

                    b.Property<string>("ArrivalAirport")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("arrival_airport")
                        .IsFixedLength()
                        .HasComment("Code of airport of arrival");

                    b.Property<string>("ArrivalAirportName")
                        .HasColumnType("text")
                        .HasColumnName("arrival_airport_name")
                        .HasComment("Name of airport of arrival");

                    b.Property<string>("ArrivalCity")
                        .HasColumnType("text")
                        .HasColumnName("arrival_city")
                        .HasComment("City of arrival");

                    b.Property<List<int>>("DaysOfWeek")
                        .HasColumnType("integer[]")
                        .HasColumnName("days_of_week")
                        .HasComment("Days of week on which flights are scheduled");

                    b.Property<string>("DepartureAirport")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("departure_airport")
                        .IsFixedLength()
                        .HasComment("Code of airport of departure");

                    b.Property<string>("DepartureAirportName")
                        .HasColumnType("text")
                        .HasColumnName("departure_airport_name")
                        .HasComment("Name of airport of departure");

                    b.Property<string>("DepartureCity")
                        .HasColumnType("text")
                        .HasColumnName("departure_city")
                        .HasComment("City of departure");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("duration")
                        .HasComment("Scheduled duration of flight");

                    b.Property<string>("FlightNo")
                        .HasMaxLength(6)
                        .HasColumnType("character(6)")
                        .HasColumnName("flight_no")
                        .IsFixedLength()
                        .HasComment("Flight number");

                    b.ToTable((string)null);

                    b.ToView("routes", "bookings");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.BoardingPass", b =>
                {
                    b.HasOne("FlightsAPI.Models.FlightDb.TicketFlight", "TicketFlight")
                        .WithOne("BoardingPass")
                        .HasForeignKey("FlightsAPI.Models.FlightDb.BoardingPass", "TicketNo", "FlightId")
                        .IsRequired()
                        .HasConstraintName("boarding_passes_ticket_no_fkey");

                    b.Navigation("TicketFlight");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Flight", b =>
                {
                    b.HasOne("FlightsAPI.Models.FlightDb.AircraftsDatum", "AircraftCodeNavigation")
                        .WithMany("Flights")
                        .HasForeignKey("AircraftCode")
                        .IsRequired()
                        .HasConstraintName("flights_aircraft_code_fkey");

                    b.HasOne("FlightsAPI.Models.FlightDb.AirportsDatum", "ArrivalAirportNavigation")
                        .WithMany("FlightArrivalAirportNavigations")
                        .HasForeignKey("ArrivalAirport")
                        .IsRequired()
                        .HasConstraintName("flights_arrival_airport_fkey");

                    b.HasOne("FlightsAPI.Models.FlightDb.AirportsDatum", "DepartureAirportNavigation")
                        .WithMany("FlightDepartureAirportNavigations")
                        .HasForeignKey("DepartureAirport")
                        .IsRequired()
                        .HasConstraintName("flights_departure_airport_fkey");

                    b.Navigation("AircraftCodeNavigation");

                    b.Navigation("ArrivalAirportNavigation");

                    b.Navigation("DepartureAirportNavigation");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Seat", b =>
                {
                    b.HasOne("FlightsAPI.Models.FlightDb.AircraftsDatum", "AircraftCodeNavigation")
                        .WithMany("Seats")
                        .HasForeignKey("AircraftCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("seats_aircraft_code_fkey");

                    b.Navigation("AircraftCodeNavigation");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Ticket", b =>
                {
                    b.HasOne("FlightsAPI.Models.FlightDb.Booking", "BookRefNavigation")
                        .WithMany("Tickets")
                        .HasForeignKey("BookRef")
                        .IsRequired()
                        .HasConstraintName("tickets_book_ref_fkey");

                    b.Navigation("BookRefNavigation");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.TicketFlight", b =>
                {
                    b.HasOne("FlightsAPI.Models.FlightDb.Flight", "Flight")
                        .WithMany("TicketFlights")
                        .HasForeignKey("FlightId")
                        .IsRequired()
                        .HasConstraintName("ticket_flights_flight_id_fkey");

                    b.HasOne("FlightsAPI.Models.FlightDb.Ticket", "TicketNoNavigation")
                        .WithMany("TicketFlights")
                        .HasForeignKey("TicketNo")
                        .IsRequired()
                        .HasConstraintName("ticket_flights_ticket_no_fkey");

                    b.Navigation("Flight");

                    b.Navigation("TicketNoNavigation");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.AircraftsDatum", b =>
                {
                    b.Navigation("Flights");

                    b.Navigation("Seats");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.AirportsDatum", b =>
                {
                    b.Navigation("FlightArrivalAirportNavigations");

                    b.Navigation("FlightDepartureAirportNavigations");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Booking", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Flight", b =>
                {
                    b.Navigation("TicketFlights");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.Ticket", b =>
                {
                    b.Navigation("TicketFlights");
                });

            modelBuilder.Entity("FlightsAPI.Models.FlightDb.TicketFlight", b =>
                {
                    b.Navigation("BoardingPass");
                });
#pragma warning restore 612, 618
        }
    }
}
