using System;
using AirCondorConsoleApp.Services;

namespace AirCondorConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("1. Add Customer");
                Console.WriteLine("2. View All Customers");
                Console.WriteLine("3. View All Flights");
                Console.WriteLine("4. Create Booking");
                Console.WriteLine("5. View All Bookings");
                Console.WriteLine("6. Add Passenger");
                Console.WriteLine("7. View All Passengers");
                Console.WriteLine("8. Add Passenger To Booking");
                Console.WriteLine("9. View Passengers For Booking");
                Console.WriteLine("10. Assign Seat & Create Ticket");
                Console.WriteLine("11. View Tickets For Booking");
                Console.WriteLine("12. Report: All Tickets (Detailed)");
                Console.WriteLine("13. Report: Revenue Per Flight");
                Console.WriteLine("14. Report: Revenue Per Brand");
                Console.WriteLine("15. Report: Seat Map For Flight");
                Console.WriteLine("0. Exit");
                Console.Write("Choose option: ");





                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        CustomerService.AddCustomer();
                        Pause();
                        break;

                    case "2":
                        CustomerService.ViewAllCustomers();
                        Pause();
                        break;

                    case "3":
                        FlightService.ViewAllFlights();
                        Pause();
                        break;

                    case "4":
                        BookingService.CreateBooking();
                        Pause();
                        break;

                    case "5":
                        BookingService.ViewAllBookings();
                        Pause();
                        break;

                    case "6":
                        PassengerService.AddPassenger();
                        Pause();
                        break;

                    case "7":
                        PassengerService.ViewAllPassengers();
                        Pause();
                        break;

                    case "8":
                        BookingPassengerService.AddPassengerToBooking();
                        Pause();
                        break;

                    case "9":
                        BookingPassengerService.ViewPassengersForBooking();
                        Pause();
                        break;

                    case "10":
                        TicketService.AssignSeatAndCreateTicket();
                        Pause();
                        break;

                    case "11":
                        TicketService.ViewTicketsForBooking();
                        Pause();
                        break;

                    case "12":
                        ReportService.ShowAllTicketsDetailed();
                        Pause();
                        break;

                    case "13":
                        ReportService.ShowFlightRevenue();
                        Pause();
                        break;

                    case "14":
                        ReportService.ShowBrandRevenue();
                        Pause();
                        break;

                    case "15":
                        ReportService.ShowSeatMapForFlight();
                        Pause();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        Pause();
                        break;
                }



            }
        }

        /// <summary>
        /// Simple helper to wait for user input before returning to the menu.
        /// </summary>
        private static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
