class LibraryManager
{
    static void Main()
    {
        string[] library = new string[5]; // Array to store book titles
        while (true)
        {
            string userAction = GetUserAction(); // Get the user's action (add/remove/exit)

            if (userAction == "add")
            {
                AddBook(library);
            }
            else if (userAction == "remove")
            {
                RemoveBook(library);
            }
            else if (userAction == "exit")
            {
                Console.WriteLine("Exiting the program. Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("Invalid action. Please type 'add', 'remove', or 'exit'.");
            }

            DisplayBooks(library); // Display the current list of books
        }
    }

    /// <summary>
    /// Prompts the user for an action and validates the input.
    /// </summary>
    /// <returns>A valid action string ('add', 'remove', or 'exit').</returns>
    static string GetUserAction()
    {
        Console.WriteLine("Would you like to add or remove a book? (add/remove/exit)");
        string? action = Console.ReadLine()?.Trim().ToLower();

        if (string.IsNullOrEmpty(action))
        {
            Console.WriteLine("Input cannot be empty. Please type 'add', 'remove', or 'exit'.");
            return string.Empty;
        }

        return action;
    }

    /// <summary>
    /// Adds a book to the library if there is space and the book doesn't already exist.
    /// </summary>
    /// <param name="library">The array representing the library.</param>
    static void AddBook(string[] library)
    {
        if (IsLibraryFull(library))
        {
            Console.WriteLine("The library is full. No more books can be added.");
            return;
        }

        Console.WriteLine("Enter the title of the book to add:");
        string? newBook = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(newBook))
        {
            Console.WriteLine("Book title cannot be empty.");
            return;
        }

        if (BookExists(library, newBook))
        {
            Console.WriteLine($"The book '{newBook}' already exists in the library.");
            return;
        }

        AddBookToLibrary(library, newBook);
    }

    /// <summary>
    /// Removes a book from the library if it exists.
    /// </summary>
    /// <param name="library">The array representing the library.</param>
    static void RemoveBook(string[] library)
    {
        if (IsLibraryEmpty(library))
        {
            Console.WriteLine("The library is empty. No books to remove.");
            return;
        }

        Console.WriteLine("Enter the title of the book to remove:");
        string? bookToRemove = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(bookToRemove))
        {
            Console.WriteLine("Book title cannot be empty.");
            return;
        }

        if (RemoveBookFromLibrary(library, bookToRemove))
        {
            Console.WriteLine($"Book '{bookToRemove}' removed.");
        }
        else
        {
            Console.WriteLine($"Book '{bookToRemove}' not found.");
        }
    }

    /// <summary>
    /// Displays the list of books in the library.
    /// </summary>
    /// <param name="library">The array representing the library.</param>
    static void DisplayBooks(string[] library)
    {
        Console.WriteLine("Available books:");
        bool hasBooks = false;

        foreach (string book in library)
        {
            if (!string.IsNullOrEmpty(book))
            {
                Console.WriteLine(book);
                hasBooks = true;
            }
        }

        if (!hasBooks)
        {
            Console.WriteLine("No books available.");
        }
    }

    /// <summary>
    /// Checks if the library is full.
    /// </summary>
    /// <param name="library">The array representing the library.</param>
    /// <returns>True if the library is full, otherwise false.</returns>
    static bool IsLibraryFull(string[] library)
    {
        return Array.TrueForAll(library, book => !string.IsNullOrEmpty(book));
    }

    /// <summary>
    /// Checks if the library is empty.
    /// </summary>
    /// <param name="library">The array representing the library.</param>
    /// <returns>True if the library is empty, otherwise false.</returns>
    static bool IsLibraryEmpty(string[] library)
    {
        return Array.TrueForAll(library, book => string.IsNullOrEmpty(book));
    }

    /// <summary>
    /// Checks if a book already exists in the library.
    /// </summary>
    /// <param name="library">The array representing the library.</param>
    /// <param name="bookTitle">The title of the book to check.</param>
    /// <returns>True if the book exists, otherwise false.</returns>
    static bool BookExists(string[] library, string bookTitle)
    {
        return Array.Exists(library, book => string.Equals(book, bookTitle, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Adds a book to the first available slot in the library.
    /// </summary>
    /// <param name="library">The array representing the library.</param>
    /// <param name="bookTitle">The title of the book to add.</param>
    static void AddBookToLibrary(string[] library, string bookTitle)
    {
        for (int i = 0; i < library.Length; i++)
        {
            if (string.IsNullOrEmpty(library[i]))
            {
                library[i] = bookTitle;
                Console.WriteLine($"Book '{bookTitle}' added.");
                return;
            }
        }
    }

    /// <summary>
    /// Removes a book from the library if it exists.
    /// </summary>
    /// <param name="library">The array representing the library.</param>
    /// <param name="bookTitle">The title of the book to remove.</param>
    /// <returns>True if the book was removed, otherwise false.</returns>
    static bool RemoveBookFromLibrary(string[] library, string bookTitle)
    {
        for (int i = 0; i < library.Length; i++)
        {
            if (!string.IsNullOrEmpty(library[i]) && string.Equals(library[i], bookTitle, StringComparison.OrdinalIgnoreCase))
            {
                library[i] = null; // Clear the book slot
                return true;
            }
        }
        return false;
    }
}