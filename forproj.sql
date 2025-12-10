USE airline;

-- =====================================================================
--  AIRLINE DATABASE - FULL SQL SCRIPT WITH EXPLANATIONS
--  All tables are created from scratch for the airline booking system
-- =====================================================================

-- =====================================================================
--  TABLE 1: AirlineBrand  
--  Stores airline brand names (e.g., Delta, Air Canada, Emirates)
-- =====================================================================
CREATE TABLE AirlineBrand (
    BrandID INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Code VARCHAR(10) NOT NULL
);

-- =====================================================================
--  TABLE 2: AircraftType  
--  Defines the type/model of an aircraft (capacity, manufacturer)
-- =====================================================================
CREATE TABLE AircraftType (
    AircraftTypeID INT PRIMARY KEY AUTO_INCREMENT,
    Manufacturer VARCHAR(100) NOT NULL,
    Model VARCHAR(100) NOT NULL,
    TotalSeats INT NOT NULL
);

-- =====================================================================
--  TABLE 3: Aircraft  
--  Stores individual aircraft that belong to a brand and have a type
-- =====================================================================
CREATE TABLE Aircraft (
    AircraftID INT PRIMARY KEY AUTO_INCREMENT,
    AircraftTypeID INT NOT NULL,
    BrandID INT NOT NULL,

    FOREIGN KEY (AircraftTypeID) REFERENCES AircraftType(AircraftTypeID),
    FOREIGN KEY (BrandID) REFERENCES AirlineBrand(BrandID)
);

-- =====================================================================
--  TABLE 4: Airport  
--  Stores information about airports
-- =====================================================================
CREATE TABLE Airport (
    AirportCode VARCHAR(10) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    City VARCHAR(100) NOT NULL,
    Country VARCHAR(100) NOT NULL
);

-- =====================================================================
--  TABLE 5: Seat  
--  Stores seats inside an aircraft
-- =====================================================================
CREATE TABLE Seat (
    SeatID INT PRIMARY KEY AUTO_INCREMENT,
    AircraftID INT NOT NULL,
    SeatNumber VARCHAR(10) NOT NULL,
    SeatClass VARCHAR(20) NOT NULL,   -- e.g., Economy, Business

    FOREIGN KEY (AircraftID) REFERENCES Aircraft(AircraftID)
);

-- =====================================================================
--  TABLE 6: Flight  
--  Stores flight information: aircraft, brand, airports, time, price
-- =====================================================================
CREATE TABLE Flight (
    FlightID INT PRIMARY KEY AUTO_INCREMENT,
    FlightNumber VARCHAR(20) NOT NULL,
    BrandID INT NOT NULL,
    AircraftID INT NOT NULL,
    DepartureAirportCode VARCHAR(10) NOT NULL,
    ArrivalAirportCode VARCHAR(10) NOT NULL,
    DepartureDateTime DATETIME NOT NULL,
    ArrivalDateTime DATETIME NOT NULL,
    BasePrice DECIMAL(10,2) NOT NULL,

    FOREIGN KEY (BrandID) REFERENCES AirlineBrand(BrandID),
    FOREIGN KEY (AircraftID) REFERENCES Aircraft(AircraftID),
    FOREIGN KEY (DepartureAirportCode) REFERENCES Airport(AirportCode),
    FOREIGN KEY (ArrivalAirportCode) REFERENCES Airport(AirportCode)
);

-- =====================================================================
--  TABLE 7: Customer  
--  Stores customer (person who purchases bookings)
-- =====================================================================
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY AUTO_INCREMENT,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(200),
    Phone VARCHAR(20)
);

-- =====================================================================
--  TABLE 8: Passenger  
--  The passenger who actually boards a flight
-- =====================================================================
CREATE TABLE Passenger (
    PassengerID INT PRIMARY KEY AUTO_INCREMENT,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    DateOfBirth DATE,
    PassportNumber VARCHAR(50)
);

-- =====================================================================
--  TABLE 9: Booking  
--  A booking made by a customer for a flight
-- =====================================================================
CREATE TABLE Booking (
    BookingID INT PRIMARY KEY AUTO_INCREMENT,
    CustomerID INT NOT NULL,
    FlightID INT NOT NULL,
    BookingDate DATETIME NOT NULL,
    PaymentStatus VARCHAR(30) NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,

    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (FlightID) REFERENCES Flight(FlightID)
);

-- =====================================================================
--  TABLE 10: BookingPassenger  
--  Connects bookings with the passengers traveling in them
-- =====================================================================
CREATE TABLE BookingPassenger (
    BookingPassengerID INT PRIMARY KEY AUTO_INCREMENT,
    BookingID INT NOT NULL,
    PassengerID INT NOT NULL,

    FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    FOREIGN KEY (PassengerID) REFERENCES Passenger(PassengerID)
);

-- =====================================================================
--  TABLE 11: Ticket  
--  Stores tickets issued per passenger seat for a booking
-- =====================================================================
CREATE TABLE Ticket (
    TicketID INT PRIMARY KEY AUTO_INCREMENT,
    BookingPassengerID INT NOT NULL,
    SeatID INT NOT NULL,
    TicketNumber VARCHAR(30) NOT NULL,
    PricePaid DECIMAL(10,2) NOT NULL,

    FOREIGN KEY (BookingPassengerID) REFERENCES BookingPassenger(BookingPassengerID),
    FOREIGN KEY (SeatID) REFERENCES Seat(SeatID)
);

-- =====================================================================
--  SAMPLE DATA INSERTS
-- =====================================================================

-- ---------------- AirlineBrand ----------------
INSERT INTO AirlineBrand (Name, Code)
VALUES 
('Air Canada', 'AC'),
('Delta Airlines', 'DL'),
('Emirates', 'EK');

-- ---------------- AircraftType ----------------
INSERT INTO AircraftType (Manufacturer, Model, TotalSeats)
VALUES
('Boeing', '737 MAX', 180),
('Airbus', 'A320', 170),
('Boeing', '777-300ER', 396);

-- ---------------- Aircraft ----------------
INSERT INTO Aircraft (AircraftTypeID, BrandID)
VALUES
(1, 1), -- Air Canada Boeing 737
(2, 2), -- Delta Airbus A320
(3, 3); -- Emirates Boeing 777

-- ---------------- Airport ----------------
INSERT INTO Airport (AirportCode, Name, City, Country)
VALUES
('YYZ', 'Toronto Pearson International', 'Toronto', 'Canada'),
('JFK', 'John F. Kennedy International', 'New York', 'USA'),
('DXB', 'Dubai International Airport', 'Dubai', 'UAE');

-- ---------------- Seat ----------------
INSERT INTO Seat (AircraftID, SeatNumber, SeatClass)
VALUES
(1, '1A', 'Business'),
(1, '12C', 'Economy'),
(2, '3D', 'Economy'),
(3, '7A', 'Business');

-- ---------------- Flight ----------------
INSERT INTO Flight 
(FlightNumber, BrandID, AircraftID, DepartureAirportCode, ArrivalAirportCode, DepartureDateTime, ArrivalDateTime, BasePrice)
VALUES
('AC101', 1, 1, 'YYZ', 'JFK', '2025-01-10 09:00:00', '2025-01-10 11:00:00', 350.00),
('DL202', 2, 2, 'JFK', 'YYZ', '2025-02-03 14:00:00', '2025-02-03 16:00:00', 320.00),
('EK303', 3, 3, 'YYZ', 'DXB', '2025-03-15 20:00:00', '2025-03-16 07:30:00', 1200.00);

-- ---------------- Customer ----------------
INSERT INTO Customer (FirstName, LastName, Email, Phone)
VALUES
('Alex', 'Kazeika', 'alex@example.com', '+14372995802'),
('John', 'Smith', 'john.smith@gmail.com', '+1123456789');

-- ---------------- Passenger ----------------
INSERT INTO Passenger (FirstName, LastName, DateOfBirth, PassportNumber)
VALUES
('Alex', 'Kazeika', '2004-07-21', 'AB1234567'),
('Maria', 'Lopez', '1990-02-10', 'XY9988776');

-- ---------------- Booking ----------------
INSERT INTO Booking (CustomerID, FlightID, BookingDate, PaymentStatus, TotalAmount)
VALUES
(1, 1, '2025-01-05 12:00:00', 'Paid', 350.00),
(2, 3, '2025-02-20 09:30:00', 'Paid', 1200.00);

-- ---------------- BookingPassenger ----------------
INSERT INTO BookingPassenger (BookingID, PassengerID)
VALUES
(1, 1),
(2, 2);

-- ---------------- Ticket ----------------
INSERT INTO Ticket (BookingPassengerID, SeatID, TicketNumber, PricePaid)
VALUES
(1, 1, 'TCK-001-A', 350.00),
(2, 4, 'TCK-002-B', 1200.00);
