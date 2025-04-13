using System;
using System.Collections.Generic;

namespace LibraryManagement
{
    class Program
    {
        // List to store the books in the library
        static List<string> libraryBooks = new List<string>();

        // Maximum number of books allowed in the library
        static int maxBooksAllowed = 5;

        static void Main(string[] args)
        {
            // Main program loop
            while (true)
            {
                // Display menu options
                Console.WriteLine("\nLibrary Management System");
                Console.WriteLine("Options:");
                Console.WriteLine("add - Add a Book");
                Console.WriteLine("remove - Remove a Book");
                Console.WriteLine("display - Display Books");
                Console.WriteLine("exit - Exit the Program");
                Console.Write("Choose an option (lowercase only): ");

                // Read and process user input
                string choice = Console.ReadLine()?.Trim().ToLower();

                // Validate input
                if (string.IsNullOrEmpty(choice))
                {
                    Console.WriteLine("Invalid input. Please enter a valid option.");
                    continue;
                }

                // Handle user choice
                switch (choice)
                {
                    case "add":
                        AddBook();
                        break;
                    case "remove":
                        RemoveBook();
                        break;
                    case "display":
                        DisplayBooks();
                        break;
                    case "exit":
                        Console.WriteLine("Exiting the program...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please enter a valid option in lowercase (e.g., 'add', 'remove', 'display', 'exit').");
                        break;
                }
            }
        }

        // Method to add a book to the library
        static void AddBook()
        {
            // Check if the library is full
            if (IsLibraryFull())
            {
                Console.WriteLine("The library is full. You cannot add more books.");
                return;
            }

            // Get the book title from the user
            string bookTitle = GetBookTitle("Enter the title of the book to add: ");
            if (bookTitle == null) return;

            // Check if the book already exists in the library
            if (libraryBooks.Contains(bookTitle, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine("This book is already in the library. Please enter a different title.");
                return;
            }

            // Add the book to the library
            libraryBooks.Add(bookTitle);
            Console.WriteLine($"Book '{bookTitle}' added to the library.");
        }

        // Method to remove a book from the library
        static void RemoveBook()
        {
            // Check if the library is empty
            if (IsLibraryEmpty())
            {
                Console.WriteLine("The library is empty. There are no books to remove.");
                return;
            }

            // Get the book title from the user
            string bookTitle = GetBookTitle("Enter the title of the book to remove: ");
            if (bookTitle == null) return;

            // Find and remove the book (case-insensitive)
            var foundBook = libraryBooks.Find(b => string.Equals(b, bookTitle, StringComparison.OrdinalIgnoreCase));
            if (foundBook != null)
            {
                libraryBooks.Remove(foundBook);
                Console.WriteLine($"Book '{bookTitle}' removed from the library.");
            }
            else
            {
                Console.WriteLine($"Book '{bookTitle}' not found in the collection.");
            }
        }

        // Method to display all books in the library
        static void DisplayBooks()
        {
            // Check if the library is empty
            if (IsLibraryEmpty())
            {
                Console.WriteLine("No books available in the library.");
                return;
            }

            // Display the list of books
            Console.WriteLine("Books available in the library:");
            libraryBooks.ForEach(book => Console.WriteLine($"- {book}"));
        }

        // Helper method to check if the library is full
        static bool IsLibraryFull()
        {
            // Return true if the number of books has reached the maximum limit
            return libraryBooks.Count >= maxBooksAllowed;
        }

        // Helper method to check if the library is empty
        static bool IsLibraryEmpty()
        {
            // Return true if there are no books in the library
            return libraryBooks.Count == 0;
        }

        // Helper method to get a valid book title from the user
        static string GetBookTitle(string prompt)
        {
            // Prompt the user for a book title
            Console.Write(prompt);
            string bookTitle = Console.ReadLine();

            // Validate the input
            if (string.IsNullOrEmpty(bookTitle))
            {
                Console.WriteLine("Invalid book title. Please enter a valid title.");
                return null;
            }

            return bookTitle;
        }
    }
}