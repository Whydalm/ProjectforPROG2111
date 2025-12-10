# Air Condor Airline Reservation System

Final project for **PROG2111 – Relational Databases**  
Conestoga College  
**Student:** Aliaksandr Kazeika (9061886)

---

## Project Overview

This repository contains the complete implementation of the **Air Condor Airline Reservation System** database project.

The goal of the project is to design and implement a fully normalized relational database for an airline reservation system and to demonstrate its usage from a C# console application using ADO.NET (MySQL).

The system supports:

- Airline brands and aircraft fleet
- Airports and flights
- Customers and passengers (customer ≠ passenger)
- Bookings
- Seat assignments
- Ticket generation
- Reporting and analytics queries

Database Design

The database is fully normalized to Third Normal Form (3NF) and includes the following main tables:

AirlineBrand

AircraftType

Aircraft

Seat

Airport

Flight

Customer

Passenger

Booking

BookingPassenger

Ticket

Key points:

Customer is the person who pays for the booking.

Passenger is the person who actually travels.

The BookingPassenger table resolves the many-to-many relationship between bookings and passengers.

Ticket links a passenger (in a booking) to a specific seat.

The ERD diagram is provided in Documentation/ERD.pdf.

How to Set Up the Database

Install MySQL Server and MySQL Workbench (version 8+).

Create a database:

CREATE DATABASE airline;
USE airline;


Open SQL/airline_schema.sql in MySQL Workbench.

Execute the whole script.

All tables will be created and sample data will be inserted.

You can verify the schema using:

SHOW TABLES;
SELECT * FROM Flight;
SELECT * FROM Booking;

C# Console Application

The C# console application demonstrates full CRUD interaction with the airline database using ADO.NET (MySql.Data).

Main Features

Customer Management

Add new customers

View all customers

Passenger Management

Add new passengers

View all passengers

Flight Browsing

List all flights (brand, aircraft, airports, price, schedule)

Booking Management

Create a booking for a customer and a specific flight

View bookings

Passenger → Booking Assignment

Add passengers to an existing booking (BookingPassenger)

Seat Assignment & Ticket Creation

For each passenger in a booking:

select a seat

generate a ticket number

insert into Ticket

Reports (SQL + C#)

Detailed ticket report (customer, passenger, flight, airports, seat, price)

Revenue per flight

Revenue per airline brand

Seat map for a flight (which seats are free / taken)

How to Run the C# Application

Open the solution in Visual Studio.

Make sure the MySql.Data NuGet package is installed.

Update the connection string in the database helper class if needed, for example:

public static MySqlConnection GetConnection()
{
    return new MySqlConnection(
        "server=localhost;user=root;password=;database=airline");
}


Build the solution.

Run the console application.

Use the menu to:

manage customers and passengers

create bookings

assign passengers and seats

view tickets and reports

Technologies Used

MySQL 8.x

MySQL Workbench

C# (.NET)

ADO.NET (MySql.Data)

ERD modelling tools (draw.io / MySQL Workbench)

Microsoft Word / PDF for documentation
