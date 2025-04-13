class LibraryManager
{
    static void Main()
    {
        Dictionary<string, bool> library = new Dictionary<string, bool>(); // Dictionary to store book titles and their checked-out status
        List<string> borrowedBooks = new List<string>(); // List to track borrowed books

        while (true)
        {
            string userAction = GetUserAction(); // Get the user's action (add/remove/search/borrow/checkin/exit)

            if (userAction == "add")
            {
                AddBook(library);
            }
            else if (userAction == "remove")
            {
                RemoveBook(library);
            }
            else if (userAction == "search")
            {
                SearchBook(library);
            }
            else if (userAction == "borrow")
            {
                BorrowBook(library, borrowedBooks);
            }
            else if (userAction == "checkin")
            {
                CheckInBook(library, borrowedBooks);
            }
            else if (userAction == "exit")
            {
                Console.WriteLine("Exiting the program. Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("Invalid action. Please type 'add', 'remove', 'search', 'borrow', 'checkin', or 'exit'.");
            }

            DisplayBooks(library); // Display the current list of books
        }
    }

    /// <summary>
    /// Prompts the user for an action and validates the input.
    /// </summary>
    /// <returns>A valid action string ('add', 'remove', 'search', or 'exit').</returns>
    static string GetUserAction()
    {
        Console.WriteLine("Would you like to add, remove, search, borrow, checkin for a book, or exit? (add/remove/search/checkin/exit)");
        string? action = Console.ReadLine()?.Trim().ToLower();

        if (string.IsNullOrEmpty(action))
        {
            Console.WriteLine("Input cannot be empty. Please type 'add', 'remove', 'search', or 'exit'.");
            return string.Empty;
        }

        return action;
    }


    /// <summary>
    /// Allows the user to check in a book that has been borrowed.
    /// </summary>
    /// <param name="library">The dictionary representing the library.</param>
    /// <param name="borrowedBooks">The list of books the user has borrowed.</param>
    static void CheckInBook(Dictionary<string, bool> library, List<string> borrowedBooks)
    {
        Console.WriteLine("Enter the title of the book to check in:");
        string? bookToCheckIn = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(bookToCheckIn))
        {
            Console.WriteLine("Book title cannot be empty.");
            return;
        }

        if (borrowedBooks.Contains(bookToCheckIn))
        {
            borrowedBooks.Remove(bookToCheckIn);
            library[bookToCheckIn] = false; // Mark the book as available
            Console.WriteLine($"The book '{bookToCheckIn}' has been checked in.");
        }
        else
        {
            Console.WriteLine($"The book '{bookToCheckIn}' is not currently checked out.");
        }
    }

    /// <summary>
    /// Allows the user to borrow a book if it exists and is not already checked out.
    /// </summary>
    /// <param name="library">The dictionary representing the library.</param>
    /// <param name="borrowedBooks">The list of books the user has borrowed.</param>
    static void BorrowBook(Dictionary<string, bool> library, List<string> borrowedBooks)
    {
        if (borrowedBooks.Count >= 3)
        {
            Console.WriteLine("You have already borrowed the maximum number of books (3). Return a book to borrow more.");
            return;
        }

        Console.WriteLine("Enter the title of the book to borrow:");
        string? bookToBorrow = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(bookToBorrow))
        {
            Console.WriteLine("Book title cannot be empty.");
            return;
        }

        if (library.ContainsKey(bookToBorrow) && !library[bookToBorrow])
        {
            borrowedBooks.Add(bookToBorrow);
            library[bookToBorrow] = true; // Mark the book as checked out
            Console.WriteLine($"You have borrowed '{bookToBorrow}'. You can borrow {3 - borrowedBooks.Count} more book(s).");
        }
        else if (library.ContainsKey(bookToBorrow) && library[bookToBorrow])
        {
            Console.WriteLine($"The book '{bookToBorrow}' is already checked out.");
        }
        else
        {
            Console.WriteLine($"The book '{bookToBorrow}' is not available in the library.");
        }
    }

    /// <summary>
    /// Adds a book to the library if it doesn't already exist.
    /// </summary>
    /// <param name="library">The dictionary representing the library.</param>
    static void AddBook(Dictionary<string, bool> library)
    {
        Console.WriteLine("Enter the title of the book to add:");
        string? newBook = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(newBook))
        {
            Console.WriteLine("Book title cannot be empty.");
            return;
        }

        if (library.ContainsKey(newBook))
        {
            Console.WriteLine($"The book '{newBook}' already exists in the library.");
            return;
        }

        library[newBook] = false; // Add the book as available
        Console.WriteLine($"Book '{newBook}' added to the library.");
    }

    /// <summary>
    /// Removes a book from the library if it exists.
    /// </summary>
    /// <param name="library">The dictionary representing the library.</param>
    static void RemoveBook(Dictionary<string, bool> library)
    {
        Console.WriteLine("Enter the title of the book to remove:");
        string? bookToRemove = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(bookToRemove))
        {
            Console.WriteLine("Book title cannot be empty.");
            return;
        }

        if (library.Remove(bookToRemove))
        {
            Console.WriteLine($"Book '{bookToRemove}' removed from the library.");
        }
        else
        {
            Console.WriteLine($"Book '{bookToRemove}' not found in the library.");
        }
    }

    /// <summary>
    /// Searches for a book in the library by title.
    /// </summary>
    /// <param name="library">The dictionary representing the library.</param>
    static void SearchBook(Dictionary<string, bool> library)
    {
        Console.WriteLine("Enter the title of the book to search:");
        string? bookToSearch = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(bookToSearch))
        {
            Console.WriteLine("Book title cannot be empty.");
            return;
        }

        if (library.ContainsKey(bookToSearch))
        {
            string status = library[bookToSearch] ? "checked out" : "available";
            Console.WriteLine($"The book '{bookToSearch}' is {status} in the library.");
        }
        else
        {
            Console.WriteLine($"The book '{bookToSearch}' is not in the library.");
        }
    }

    /// <summary>
    /// Displays the list of books in the library with their status.
    /// </summary>
    /// <param name="library">The dictionary representing the library.</param>
    static void DisplayBooks(Dictionary<string, bool> library)
    {
        Console.WriteLine("Available books:");
        if (library.Count == 0)
        {
            Console.WriteLine("No books available.");
            return;
        }

        foreach (var book in library)
        {
            string status = book.Value ? "Checked Out" : "Available";
            Console.WriteLine($"- {book.Key} ({status})");
        }
    }
}

