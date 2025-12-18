using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Interfaces
{
    public interface IBookRepository
    {
        Book? GetBookByISBN(string isbn);
        void UpdateBook(Book book);
        List<Book> GetAllBooks();
    }
}
