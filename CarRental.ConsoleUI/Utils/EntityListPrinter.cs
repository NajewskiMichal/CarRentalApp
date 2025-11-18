using System;
using System.Collections.Generic;
using CarRental.Application.DTOs;

namespace CarRental.ConsoleUI.Utils
{
    /// <summary>
    /// Helper methods for printing common entity lists to the console.
    /// </summary>
    public static class EntityListPrinter
    {
        public static void PrintCars(IReadOnlyList<CarDto> cars)
        {
            foreach (var car in cars)
            {
                Console.WriteLine(
                    $"ID={car.Id} | {car.Brand} {car.Model} ({car.Year}) | VIN={car.Vin}");
            }
        }

        public static void PrintCustomers(IReadOnlyList<CustomerDto> customers)
        {
            foreach (var customer in customers)
            {
                Console.WriteLine(
                    $"ID={customer.Id} | {customer.Name} | {customer.Email}");
            }
        }

        public static void PrintRentals(IReadOnlyList<RentalDto> rentals)
        {
            foreach (var rental in rentals)
            {
                var status = rental.IsActive ? "ACTIVE" : "CLOSED";
                Console.WriteLine(
                    $"ID={rental.Id} | CustomerId={rental.CustomerId} | CarId={rental.CarId} | Rent={rental.RentDate} | Return={rental.ReturnDate} | {status}");
            }
        }
    }
}
