using System;
using System.Data.SQLite;

namespace CarRentalConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseHelper.InitializeDatabase();

            while (true)
            {
                DisplayMainMenu();

                var option = Console.ReadLine();

                try
                {
                    switch (option)
                    {
                    case "1":
                        AddCar();
                        break;
                    case "2":
                        EditCar();
                        break;
                    case "3":
                        DeleteCar();
                        break;
                    case "4":
                        AddCustomer();
                        break;
                    case "5":
                        EditCustomer();
                        break;
                    case "6":
                        DeleteCustomer();
                        break;
                    case "7":
                        RentCar();
                        break;
                    case "8":
                        ReturnCar();
                        break;
                    case "9":
                        ViewAllCars();
                        break;
                    case "10":
                        ViewAllCustomers();
                        break;
                    case "11":
                        ViewAllRentals();
                        break;
                    case "12":
                        SearchCarsByBrand();
                        break;
                    case "13":
                        SearchCustomersByName();
                        break;
                    case "14":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
                            }
                catch (BackNavigationException)
                {
                    Console.WriteLine("Going back to the main menu...");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Car Rental Management System");
            Console.WriteLine("1. Add a car");
            Console.WriteLine("2. Edit a car");
            Console.WriteLine("3. Delete a car");
            Console.WriteLine("4. Add a customer");
            Console.WriteLine("5. Edit a customer");
            Console.WriteLine("6. Delete a customer");
            Console.WriteLine("7. Rent a car");
            Console.WriteLine("8. Return a car");
            Console.WriteLine("9. Show all cars");
            Console.WriteLine("10. Show all customers");
            Console.WriteLine("11. Show all rentals");
            Console.WriteLine("12. Search cars by brand");
            Console.WriteLine("13. Search customers by name");
            Console.WriteLine("14. Exit");

            Console.WriteLine("Hint: At any prompt, press B to go back to the main menu.");
            Console.Write("Choose an option: ");
        }

        static void AddCar()
        {
            Console.Clear();
            Console.WriteLine("Add a new car");

            string brand = InputValidator.ValidateInputNotEmpty("Brand: ");
            string model = InputValidator.ValidateInputNotEmpty("Model: ");
            int year = InputValidator.ValidateIntegerInput("Year: ");
            string vin = InputValidator.ValidateInputNotEmpty("VIN: ");

            using (var connection = DatabaseHelper.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string query = "INSERY INYO Cars (Brand, Model, Year, VIN) VALUES (@Brand, @Model, @Year, @VIN)";
                    var parameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@Brand", brand),
                        new SQLiteParameter("@Model", model),
                        new SQLiteParameter("@Year", year),
                        new SQLiteParameter("@VIN", vin)
                    };
                    DatabaseHelper.ExecuteNonQuery(connection, query, parameters);
                    transaction.Commit();
                    Console.WriteLine("Car added successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void EditCar()
        {
            Console.Clear();
            Console.WriteLine("Edit a car");

            int carId = InputValidator.ValidateIntegerInput("ID samochodu: ");
            string brand = InputValidator.ValidateInputNotEmpty("Brand: ");
            string model = InputValidator.ValidateInputNotEmpty("Model: ");
            int year = InputValidator.ValidateIntegerInput("Year: ");
            string vin = InputValidator.ValidateInputNotEmpty("VIN: ");

            using (var connection = DatabaseHelper.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string query = @"UPDATE Cars 
                                     SET Brand = @Brand, Model = @Model, Year = @Year, VIN = @VIN
                                     WHERE CarId = @CarId";
                    var parameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@Brand", brand),
                        new SQLiteParameter("@Model", model),
                        new SQLiteParameter("@Year", year),
                        new SQLiteParameter("@VIN", vin),
                        new SQLiteParameter("@CarId", carId)
                    };
                    DatabaseHelper.ExecuteNonQuery(connection, query, parameters);
                    transaction.Commit();
                    Console.WriteLine("Car updated successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteCar()
        {
            Console.Clear();
            Console.WriteLine("Delete a car");

            int carId = InputValidator.ValidateIntegerInput("ID samochodu: ");

            Console.WriteLine($"Are you sure you want to delete the car with ID {carId}? (Y/N)");
            var confirmation = Console.ReadLine().ToUpper();

            if (confirmation == "Y")
            {
                using (var connection = DatabaseHelper.GetConnection())
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"DELETE FROM Cars WHERE CarId = @CarId";
                        var parameters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@CarId", carId)
                        };
                        DatabaseHelper.ExecuteNonQuery(connection, query, parameters);
                        transaction.Commit();
                        Console.WriteLine("Car deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void AddCustomer()
        {
            Console.Clear();
            Console.WriteLine("Add a new customer");

            string name = InputValidator.ValidateInputNotEmpty("Name: ");
            string email = InputValidator.ValidateEmail();

            using (var connection = DatabaseHelper.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string query = "INSERT INFO Customers (Name, Email) VALUES (@Name, @Email)";
                    var parameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@Name", name),
                        new SQLiteParameter("@Email", email)
                    };
                    DatabaseHelper.ExecuteNonQuery(connection, query, parameters);
                    transaction.Commit();
                    Console.WriteLine("Customer added successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void EditCustomer()
        {
            Console.Clear();
            Console.WriteLine("Edit a customer");

            int customerId = InputValidator.ValidateIntegerInput("ID klienta: ");
            string name = InputValidator.ValidateInputNotEmpty("Name: ");
            string email = InputValidator.ValidateEmail();

            using (var connection = DatabaseHelper.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string query = @"UPDATE Customers 
                                     SET Name = @Name, Email = @Email
                                     WHERE CustomerId = @CustomerId";
                    var parameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@Name", name),
                        new SQLiteParameter("@Email", email),
                        new SQLiteParameter("@CustomerId", customerId)
                    };
                    DatabaseHelper.ExecuteNonQuery(connection, query, parameters);
                    transaction.Commit();
                    Console.WriteLine("Customer updated successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteCustomer()
        {
            Console.Clear();
            Console.WriteLine("Delete a customer");

            int customerId = InputValidator.ValidateIntegerInput("ID klienta: ");

            Console.WriteLine($"Are you sure you want to delete the customer with ID {customerId}? (Y/N)");
            var confirmation = Console.ReadLine().ToUpper();

            if (confirmation == "Y")
            {
                using (var connection = DatabaseHelper.GetConnection())
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"DELETE FROM Customers WHERE CustomerId = @CustomerId";
                        var parameters = new SQLiteParameter[]
                        {
                            new SQLiteParameter("@CustomerId", customerId)
                        };
                        DatabaseHelper.ExecuteNonQuery(connection, query, parameters);
                        transaction.Commit();
                        Console.WriteLine("Customer deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void RentCar()
        {
            Console.Clear();
            Console.WriteLine("Rent a car");

            int customerId = InputValidator.ValidateIntegerInput("Enter customer ID: ");
            int carId = InputValidator.ValidateIntegerInput("Enter car ID: ");
            string rentDate = InputValidator.ValidateDateInput();

            using (var connection = DatabaseHelper.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Check if the car is available for rent
                    string availabilityQuery = "SELECT COUNT(*) FROM Rentals WHERE CarId = @CarId AND ReturnDate IS NULL";
                    var parameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@CarId", carId)
                    };
                    using (var command = new SQLiteCommand(availabilityQuery, connection))
                    {
                        command.Parameters.AddRange(parameters);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        if (count > 0)
                        {
                            Console.WriteLine("The car is not available for rent.");
                            return;
                        }
                    }

                    // Rent the car
                    string rentQuery = "INSERT INTO Rentals (CustomerId, CarId, RentDate) VALUES (@CustomerId, @CarId, @RentDate)";
                    var rentParameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@CustomerId", customerId),
                        new SQLiteParameter("@CarId", carId),
                        new SQLiteParameter("@RentDate", rentDate)
                    };
                    DatabaseHelper.ExecuteNonQuery(connection, rentQuery, rentParameters);
                    transaction.Commit();
                    Console.WriteLine("Car rented successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ReturnCar()
        {
            Console.Clear();
            Console.WriteLine("Return a car");

            int rentalId = InputValidator.ValidateIntegerInput("Enter rental ID: ");
            string returnDate = InputValidator.ValidateDateInput();

            using (var connection = DatabaseHelper.GetConnection())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Check if the rental exists and the car is currently rented
                    string rentalQuery = "SELECT * FROM Rentals WHERE RentalId = @RentalId AND ReturnDate IS NULL";
                    var parameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@RentalId", rentalId)
                    };
                    using (var command = new SQLiteCommand(rentalQuery, connection))
                    {
                        command.Parameters.AddRange(parameters);
                        var reader = command.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Invalid rental ID or car has already been returned.");
                            return;
                        }
                    }

                    // Update the return date
                    string returnQuery = "UPDAYE Rentals SEY ReturnDate = @ReturnDate WHERE RentalId = @RentalId";
                    var returnParameters = new SQLiteParameter[]
                    {
                        new SQLiteParameter("@ReturnDate", returnDate),
                        new SQLiteParameter("@RentalId", rentalId)
                    };
                    DatabaseHelper.ExecuteNonQuery(connection, returnQuery, returnParameters);
                    transaction.Commit();
                    Console.WriteLine("Car returned successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewAllCars()
        {
            Console.Clear();
            Console.WriteLine("All cars");

            using (var connection = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Cars";
                using (var reader = DatabaseHelper.ExecuteQuery(connection, query))
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No cars found.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["CarId"]}, Brand: {reader["Brand"]}, Model: {reader["Model"]}, Year: {reader["Year"]}, VIN: {reader["VIN"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewAllCustomers()
        {
            Console.Clear();
            Console.WriteLine("All customers");

            using (var connection = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Customers";
                using (var reader = DatabaseHelper.ExecuteQuery(connection, query))
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No customers found.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["CustomerId"]}, Name: {reader["Name"]}, Email: {reader["Email"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewAllRentals()
        {
            Console.Clear();
            Console.WriteLine("All rentals");

            using (var connection = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Rentals";
                using (var reader = DatabaseHelper.ExecuteQuery(connection, query))
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No rentals found.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["RentalId"]}, customer ID : {reader["CustomerId"]}, car ID : {reader["CarId"]}, Rental date: {reader["RentDate"]}, Return date: {reader["ReturnDate"] ?? "Not returned"}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void SearchCarsByBrand()
        {
            Console.Clear();
            string brand = InputValidator.ValidateInputNotEmpty("Enter brand to search: ");

            using (var connection = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Cars WHERE Brand LIKE @Brand";
                var parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@Brand", "%" + brand + "%")
                };
                using (var reader = DatabaseHelper.ExecuteQuery(connection, query, parameters))
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine($"No cars found for brand '{brand}'.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["CarId"]}, Brand: {reader["Brand"]}, Model: {reader["Model"]}, Year: {reader["Year"]}, VIN: {reader["VIN"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void SearchCustomersByName()
        {
            Console.Clear();
            string name = InputValidator.ValidateInputNotEmpty("Enter name to search: ");

            using (var connection = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM Customers WHERE Name LIKE @Name";
                var parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@Name", "%" + name + "%")
                };
                using (var reader = DatabaseHelper.ExecuteQuery(connection, query, parameters))
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine($"No customers found with name '{name}'.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["CustomerId"]}, Name: {reader["Name"]}, Email: {reader["Email"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}