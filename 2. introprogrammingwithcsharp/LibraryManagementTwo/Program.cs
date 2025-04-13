class LibraryManager
{
    static void Main()
    {
        string book1 = "";
        string book2 = "";
        string book3 = "";
        string book4 = "";
        string book5 = "";

        while (true)
        {
            Console.WriteLine("Would you like to add or remove a book? (add/remove/exit)");
            string action = Console.ReadLine()!.ToLower(); // Convert input to lowercase

            if (action == "add")
            {
                // Check if the library is full
                if (!string.IsNullOrEmpty(book1) && !string.IsNullOrEmpty(book2) &&
                    !string.IsNullOrEmpty(book3) && !string.IsNullOrEmpty(book4) &&
                    !string.IsNullOrEmpty(book5))
                {
                    Console.WriteLine("The library is full. No more books can be added.");
                    continue;
                }

                Console.WriteLine("Enter the title of the book to add:");
                string newBook = Console.ReadLine()!;

                if (string.IsNullOrEmpty(book1))
                {
                    book1 = newBook;
                }
                else if (string.IsNullOrEmpty(book2))
                {
                    book2 = newBook;
                }
                else if (string.IsNullOrEmpty(book3))
                {
                    book3 = newBook;
                }
                else if (string.IsNullOrEmpty(book4))
                {
                    book4 = newBook;
                }
                else if (string.IsNullOrEmpty(book5))
                {
                    book5 = newBook;
                }
            }
            else if (action == "remove")
            {
                Console.WriteLine("Enter the title of the book to remove:");
                string removeBook = Console.ReadLine()!;

                if (removeBook == book1)
                {
                    book1 = "";
                }
                else if (removeBook == book2)
                {
                    book2 = "";
                }
                else if (removeBook == book3)
                {
                    book3 = "";
                }
                else if (removeBook == book4)
                {
                    book4 = "";
                }
                else if (removeBook == book5)
                {
                    book5 = "";
                }
                else
                {
                    Console.WriteLine("Book not found.");
                }
            }
            else if (action == "exit")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid action. Please type 'add', 'remove', or 'exit'.");
            }

            // Display the list of books
            Console.WriteLine("Available books:");
            if (!string.IsNullOrEmpty(book1)) Console.WriteLine(book1);
            if (!string.IsNullOrEmpty(book2)) Console.WriteLine(book2);
            if (!string.IsNullOrEmpty(book3)) Console.WriteLine(book3);
            if (!string.IsNullOrEmpty(book4)) Console.WriteLine(book4);
            if (!string.IsNullOrEmpty(book5)) Console.WriteLine(book5);
        }
    }
}