GROUP14
Leader: Catangay
Asst. Leader: Sulibit
Members: Cedula, Medina, Parrocha
using System;
using System.Collections.Generic;
using FinalProject.Models;

namespace FinalProject
{
    class Program
    {
        static List<User> users = new List<User>();
        static List<Product> products = new List<Product>();
        static List<Order> orders = new List<Order>();

        static void Main(string[] args)
        {
            AddNewUser("admin", "admin123", "Admin");
            AddNewUser("jayson_warehouse", "1234", "Warehouse");
            AddNewUser("jayson_user", "1234", "User");

            while (true)
            {
                User loggedIn = Login();

                if (loggedIn == null)
                {
                    continue; 
                }

                if (loggedIn.Role == "Admin")
                {
                    AdminMenu(loggedIn);
                }
                else if (loggedIn.Role == "Warehouse")
                {
                    WarehouseMenu(loggedIn);
                }
                else
                {
                    UserMenu(loggedIn);
                }
            }
        }

        static void AddNewUser(string username, string password, string role)
        {
            User u = new User();
            u.Username = username;
            u.Password = password;
            u.Role = role;
            u.IsActive = true;

            users.Add(u);
        }

        static int ReadNumber()
        {
            int number;
            string input = Console.ReadLine();

            if (int.TryParse(input, out number))
            {
                return number;
            }

            return -1;
        }

        static bool IsBlank(string text)
        {
            if (text == null || text.Trim() == "")
            {
                return true;
            }

            return false;
        }

        static bool HasSpace(string text)
        {
            if (text.Contains(" "))
            {
                return true;
            }

            return false;
        }

        static bool IsValidEntry(string text)
        {
            if (IsBlank(text) || HasSpace(text))
            {
                return false;
            }

            return true;
        }

        static bool ProductExists(string name)
        {
            foreach (Product p in products)
            {
                if (p.Name.ToLower() == name.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        static List<Product> GetAvailableProducts()
        {
            List<Product> available = new List<Product>();

            foreach (Product p in products)
            {
                if (p.Quantity > 0)
                {
                    available.Add(p);
                }
            }

            return available;
        }

        static List<User> GetActiveUsersExcept(User exceptUser)
        {
            List<User> activeUsers = new List<User>();

            foreach (User u in users)
            {
                if (u.IsActive && u != exceptUser)
                {
                    activeUsers.Add(u);
                }
            }

            return activeUsers;
        }

        static User Login()
        {
            Console.WriteLine();
            Console.WriteLine("Please select a user role to login:");
            Console.WriteLine("1 Admin");
            Console.WriteLine("2 Warehouse");
            Console.WriteLine("3 User");

            int choice = ReadNumber();
            string role = "";

            if (choice == 1)
            {
                role = "Admin";
            }
            else if (choice == 2)
            {
                role = "Warehouse";
            }
            else if (choice == 3)
            {
                role = "User";
            }
            else
            {
                Console.WriteLine("Invalid role.");
                return null;
            }

            Console.Write("Please enter your username: ");
            string username = Console.ReadLine();

            Console.Write("Please enter your password: ");
            string password = Console.ReadLine();

            if (IsValidEntry(username) == false || IsValidEntry(password) == false)
            {
                Console.WriteLine("Username and password must not be blank or have spaces.");
                return null;
            }

            foreach (User u in users)
            {
                if (u.Username.ToLower() == username.ToLower() && u.Password == password && u.Role == role)
                {
                    if (u.IsActive == false)
                    {
                        Console.WriteLine("This account is inactive.");
                        return null;
                    }

                    Console.WriteLine("Login successful. Welcome, " + u.Username + "!");
                    return u;
                }
            }

            Console.WriteLine("Wrong username or password.");
            return null;
        }

        static void AdminMenu(User admin)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Update Password");
                Console.WriteLine("2. Inactivate User");
                Console.WriteLine("3. Add User");
                Console.WriteLine("4. Display All Users");
                Console.WriteLine("5. Display Inactive Users");
                Console.WriteLine("6. Display Active Users");
                Console.WriteLine("7. Display Products");
                Console.WriteLine("8. Display Orders");
                Console.WriteLine("9. Logout");

                int choice = ReadNumber();

                if (choice == 1)
                {
                    UpdatePassword(admin);
                }
                else if (choice == 2)
                {
                    InactivateUser(admin);
                }
                else if (choice == 3)
                {
                    AddUser();
                }
                else if (choice == 4)
                {
                    DisplayUsers("all");
                }
                else if (choice == 5)
                {
                    DisplayUsers("inactive");
                }
                else if (choice == 6)
                {
                    DisplayUsers("active");
                }
                else if (choice == 7)
                {
                    DisplayProducts();
                }
                else if (choice == 8)
                {
                    DisplayAllOrders();
                }
                else if (choice == 9)
                {
                    Console.WriteLine("Logged out.");
                    running = false;
                }
                else
                {
                    Console.WriteLine("Invalid option.");
                }
            }
        }

        static void UpdatePassword(User admin)
        {
            List<User> activeUsers = GetActiveUsersExcept(admin);

            Console.WriteLine();
            Console.WriteLine("1. My account");

            for (int i = 0; i < activeUsers.Count; i++)
            {
                Console.WriteLine((i + 2) + ". " + activeUsers[i].Username);
            }

            int choice = ReadNumber();
            User selected = null;

            if (choice == 1)
            {
                selected = admin;
            }
            else if (choice >= 2 && choice <= activeUsers.Count + 1)
            {
                selected = activeUsers[choice - 2];
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            Console.Write("Please enter the new password for user " + selected.Username + ": ");
            string newPassword = Console.ReadLine();

            if (IsValidEntry(newPassword) == false)
            {
                Console.WriteLine("Password must not be blank or have spaces. Password not changed.");
                return;
            }

            Console.Write("Please confirm the password: ");
            string confirm = Console.ReadLine();

            if (newPassword == confirm)
            {
                selected.Password = newPassword;
                Console.WriteLine("Password updated successfully.");
            }
            else
            {
                Console.WriteLine("Passwords do not match. Password not changed.");
            }
        }

        static void InactivateUser(User admin)
        {
            List<User> activeUsers = GetActiveUsersExcept(admin);

            if (activeUsers.Count == 0)
            {
                Console.WriteLine("No active users to inactivate.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("List of Active Users");

            for (int i = 0; i < activeUsers.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + activeUsers[i].Username);
            }

            Console.Write("Please enter the number of the user to inactivate: ");
            int choice = ReadNumber();

            if (choice >= 1 && choice <= activeUsers.Count)
            {
                User selected = activeUsers[choice - 1];
                selected.IsActive = false;

                Console.WriteLine(selected.Username + " has been inactivated.");
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }

        static void AddUser()
        {
            Console.Write("Please enter the new username: ");
            string username = Console.ReadLine();

            if (IsValidEntry(username) == false)
            {
                Console.WriteLine("Username must not be blank or have spaces.");
                return;
            }

            foreach (User u in users)
            {
                if (u.Username.ToLower() == username.ToLower())
                {
                    Console.WriteLine("That username already exists.");
                    return;
                }
            }

            Console.Write("Please enter the password: ");
            string password = Console.ReadLine();

            if (IsValidEntry(password) == false)
            {
                Console.WriteLine("Password must not be blank or have spaces.");
                return;
            }

            Console.WriteLine("Please select the role:");
            Console.WriteLine("1 Admin");
            Console.WriteLine("2 Warehouse");
            Console.WriteLine("3 User");

            int choice = ReadNumber();
            string role = "";

            if (choice == 1)
            {
                role = "Admin";
            }
            else if (choice == 2)
            {
                role = "Warehouse";
            }
            else if (choice == 3)
            {
                role = "User";
            }
            else
            {
                Console.WriteLine("Invalid role.");
                return;
            }

            AddNewUser(username, password, role);
            Console.WriteLine("User has been successfully added.");
        }

        static void DisplayUsers(string type)
        {
            Console.WriteLine();

            if (type == "all")
            {
                Console.WriteLine("List of All Users");
            }
            else if (type == "active")
            {
                Console.WriteLine("List of Active Users");
            }
            else
            {
                Console.WriteLine("List of Inactive Users");
            }

            Console.WriteLine("-----------------------");

            int count = 0;

            foreach (User u in users)
            {
                bool show = false;

                if (type == "all")
                {
                    show = true;
                }
                else if (type == "active" && u.IsActive)
                {
                    show = true;
                }
                else if (type == "inactive" && u.IsActive == false)
                {
                    show = true;
                }

                if (show)
                {
                    count++;

                    Console.WriteLine();
                    Console.WriteLine("Username: " + u.Username);
                    Console.WriteLine("Role: " + u.Role);

                    if (type == "all")
                    {
                        string status = "Inactive";

                        if (u.IsActive)
                        {
                            status = "Active";
                        }

                        Console.WriteLine("Status: " + status);
                    }
                }
            }

            if (count == 0)
            {
                Console.WriteLine("No users found.");
            }
        }

        static void WarehouseMenu(User warehouse)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Add Products");
                Console.WriteLine("2. Display Products");
                Console.WriteLine("3. Logout");

                int choice = ReadNumber();

                if (choice == 1)
                {
                    AddProducts(warehouse);
                }
                else if (choice == 2)
                {
                    DisplayProducts();
                }
                else if (choice == 3)
                {
                    Console.WriteLine("Logged out.");
                    running = false;
                }
                else
                {
                    Console.WriteLine("Invalid option.");
                }
            }
        }

        static void AddProducts(User warehouse)
        {
            Console.Write("Please enter the number of products you want to add: ");
            int howMany = ReadNumber();

            if (howMany < 1)
            {
                Console.WriteLine("Invalid number.");
                return;
            }

            for (int i = 1; i <= howMany; i++)
            {
                Product p = new Product();
                string name = "";
                bool nameIsOk = false;

                while (nameIsOk == false)
                {
                    Console.Write(i + ". Please enter the name of the product : ");
                    name = Console.ReadLine();

                    if (IsBlank(name))
                    {
                        Console.WriteLine("Product name must not be blank.");
                    }
                    else if (ProductExists(name.Trim()))
                    {
                        Console.WriteLine("This product already exists.");
                    }
                    else
                    {
                        nameIsOk = true;
                    }
                }

                int quantity = -1;

                while (quantity < 0)
                {
                    Console.Write(i + ". Please enter the quantity of the product : ");
                    quantity = ReadNumber();

                    if (quantity < 0)
                    {
                        Console.WriteLine("Quantity must be a number and must not be negative.");
                    }
                }

                p.Name = name.Trim();
                p.Quantity = quantity;
                p.IsActive = true;
                p.CreatedDate = DateTime.Now;
                p.CreatedBy = warehouse.Username;

                products.Add(p);
            }

            Console.WriteLine("All products have been successfully created.");
        }

        static void DisplayProducts()
        {
            Console.WriteLine();
            Console.WriteLine("List of Products");
            Console.WriteLine("-----------------------");

            if (products.Count == 0)
            {
                Console.WriteLine("No products yet.");
                return;
            }

            foreach (Product p in products)
            {
                string active = "No";

                if (p.IsActive)
                {
                    active = "Yes";
                }

                Console.WriteLine();
                Console.WriteLine("Product Name: " + p.Name);
                Console.WriteLine("Quantity: " + p.Quantity);
                Console.WriteLine("Active : " + active);
                Console.WriteLine("Created Date: " + p.CreatedDate);
                Console.WriteLine("Created By: " + p.CreatedBy);
            }
        }

        static void UserMenu(User user)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Order");
                Console.WriteLine("2. Display Order");
                Console.WriteLine("3. Logout");

                int choice = ReadNumber();

                if (choice == 1)
                {
                    MakeOrder(user);
                }
                else if (choice == 2)
                {
                    DisplayMyOrders(user);
                }
                else if (choice == 3)
                {
                    Console.WriteLine("Logged out.");
                    running = false;
                }
                else
                {
                    Console.WriteLine("Invalid option.");
                }
            }
        }

        static void MakeOrder(User user)
        {
            List<Product> available = GetAvailableProducts();

            if (available.Count == 0)
            {
                Console.WriteLine("There are no products available to order.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("List of Available Products");
            Console.WriteLine("-----------------------");

            for (int i = 0; i < available.Count; i++)
            {
                Console.WriteLine((i + 1) + ".Product Name: " + available[i].Name);
                Console.WriteLine("Quantity: " + available[i].Quantity);
                Console.WriteLine();
            }

            Console.Write("Please enter the number of the product you want to order: ");
            int number = ReadNumber();

            if (number < 1 || number > available.Count)
            {
                Console.WriteLine("Invalid product number.");
                return;
            }

            Product chosen = available[number - 1];
            Console.Write("Please enter the quantity of the product you want to order: ");
            int quantity = ReadNumber();

            if (quantity < 1)
            {
                Console.WriteLine("Invalid quantity.");
                return;
            }

            if (quantity > chosen.Quantity)
            {
                Console.WriteLine("Not enough stock. Only " + chosen.Quantity + " left.");
                return;
            }

            Order o = new Order();
            o.ProductName = chosen.Name;
            o.Quantity = quantity;
            o.OrderedBy = user.Username;
            orders.Add(o);

            chosen.Quantity = chosen.Quantity - quantity;

            Console.WriteLine("Order has been successfully created.");
            Console.WriteLine();
            Console.WriteLine("Order Details:");
            Console.WriteLine("Product Name: " + o.ProductName);
            Console.WriteLine("Quantity: " + o.Quantity);
        }

        static void DisplayMyOrders(User user)
        {
            Console.WriteLine();
            Console.WriteLine("My Orders");
            Console.WriteLine("-----------------------");

            int count = 0;

            foreach (Order o in orders)
            {
                if (o.OrderedBy == user.Username)
                {
                    count++;

                    Console.WriteLine();
                    Console.WriteLine("Product Name: " + o.ProductName);
                    Console.WriteLine("Quantity: " + o.Quantity);
                }
            }

            if (count == 0)
            {
                Console.WriteLine("You have no orders yet.");
            }
        }

        static void DisplayAllOrders()
        {
            Console.WriteLine();
            Console.WriteLine("List of Orders");
            Console.WriteLine("-----------------------");

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders yet.");
                return;
            }

            foreach (Order o in orders)
            {
                Console.WriteLine();
                Console.WriteLine("Product Name: " + o.ProductName);
                Console.WriteLine("Quantity: " + o.Quantity);
                Console.WriteLine("Ordered By: " + o.OrderedBy);
            }
        }
    }
}
