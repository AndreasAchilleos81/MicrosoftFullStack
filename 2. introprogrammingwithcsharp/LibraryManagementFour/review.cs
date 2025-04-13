
static void BorrowBook(List<string> books, List<string> borrowedBooks, int maxBorrowedBooks)
{
    if (borrowedBooks.Count >= maxBorrowedBooks) 
    {
         Console.WriteLine($"You have already borrowed the maximum number of books ({maxBorrowedBooks}). Please return a book before borrowing another."); return; }
    
    if (books.Count == 0) 
    { Console.WriteLine("The library is empty. No books available to borrow."); return; }
    Console.WriteLine("Enter the title of the book to borrow:"); string borrowBook = Console.ReadLine()?.Trim(); if (string.IsNullOrEmpty(borrowBook)) { Console.WriteLine("Invalid book title. Please try again."); return; }
    string normalizedBook = borrowBook.ToLower(); if (books.Remove(normalizedBook)) { borrowedBooks.Add(normalizedBook); Console.WriteLine($"You have borrowed '{borrowBook}'."); } else { Console.WriteLine($"The book '{borrowBook}' is not available in the library."); }
}



static void ReturnBook(List<string> borrowedBooks) { if (borrowedBooks.Count == 0) { Console.WriteLine("You have not borrowed any books."); return; } Console.WriteLine("Enter the title of the book to return:"); string returnBook = Console.ReadLine()?.Trim(); if (string.IsNullOrEmpty(returnBook)) { Console.WriteLine("Invalid book title. Please try again."); return; } string normalizedBook = returnBook.ToLower(); if (borrowedBooks.Remove(normalizedBook)) { Console.WriteLine($"You have returned '{returnBook}'."); } else { Console.WriteLine($"The book '{returnBook}' is not in your borrowed list."); } }