using System;

namespace LibraryManagementOne
{
    class Program
    {
        static string book1 = null;
        static string book2 = null;
        static string book3 = null;
        static string book4 = null;
        static string book5 = null;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nLibrary Management System");
                Console.WriteLine("1. Add a Book");
                Console.WriteLine("2. Remove a Book");
                Console.WriteLine("3. Display Books");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option (1-4): ");
                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        AddBook();
                        break;

                    case "2":
                        RemoveBook();
                        break;

                    case "3":
                        DisplayBooks();
                        break;

                    case "4":
                        ExitProgram();
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please select a valid option (1-4).");
                        break;
                }
            }
        }

        static void AddBook()
        {
            if (book1 != null && book2 != null && book3 != null && book4 != null && book5 != null)
            {
                Console.WriteLine("The library is full. No more books can be added.");
                return;
            }

            Console.Write("Enter the title of the book to add: ");
            string newBook = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(newBook))
            {
                Console.WriteLine("Invalid input. Book title cannot be empty.");
                return;
            }

            if (book1 == null) book1 = newBook;
            else if (book2 == null) book2 = newBook;
            else if (book3 == null) book3 = newBook;
            else if (book4 == null) book4 = newBook;
            else if (book5 == null) book5 = newBook;

            Console.WriteLine($"Book '{newBook}' has been added.");
        }

        static void RemoveBook()
        {
            if (book1 == null && book2 == null && book3 == null && book4 == null && book5 == null)
            {
                Console.WriteLine("The library is empty. No books to remove.");
                return;
            }

            Console.Write("Enter the title of the book to remove: ");
            string removeBook = Console.ReadLine();

            if (book1 == removeBook) book1 = null;
            else if (book2 == removeBook) book2 = null;
            else if (book3 == removeBook) book3 = null;
            else if (book4 == removeBook) book4 = null;
            else if (book5 == removeBook) book5 = null;
            else
            {
                Console.WriteLine($"Book '{removeBook}' not found in the library.");
                return;
            }

            Console.WriteLine($"Book '{removeBook}' has been removed.");
        }

        static void DisplayBooks()
        {
            if (book1 == null && book2 == null && book3 == null && book4 == null && book5 == null)
            {
                Console.WriteLine("No books in the library.");
            }

            Console.WriteLine("\nBooks in the Library:");
            if (book1 != null) Console.WriteLine($"- {book1}");
            if (book2 != null) Console.WriteLine($"- {book2}");
            if (book3 != null) Console.WriteLine($"- {book3}");
            if (book4 != null) Console.WriteLine($"- {book4}");
            if (book5 != null) Console.WriteLine($"- {book5}");
        }

        static void ExitProgram()
        {
            Console.WriteLine("Exiting the program. Goodbye!");
        }
    }
}