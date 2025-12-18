using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Tests.TestDoubles
{
    public class FakeBookRepository : IBookRepository
    {
        private readonly List<Book> _books = new();
        public List<string> UpdatedISBNs { get; } = new();

        public void AddBook(Book book)
        {
            _books.Add(book);
        }

        public Book? GetBookByISBN(string isbn)
        {
            return _books.FirstOrDefault(b => b.ISBN == isbn);
        }

        public void UpdateBook(Book book)
        {
            UpdatedISBNs.Add(book.ISBN);
            var existingBook = _books.FirstOrDefault(b => b.ISBN == book.ISBN);

            if (existingBook != null)
            {
                existingBook.IsAvailable = book.IsAvailable;
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
            }
        }

        public List<Book> GetAllBooks()
        {
            return _books;
        }
    }
}