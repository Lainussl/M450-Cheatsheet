using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Services
{
    public class Library
    {
        private readonly IBookRepository _repository;
        private readonly IEmailService _emailService;

        public Library(IBookRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public bool BorrowBook(string isbn, string userEmail)
        {
            var book = _repository.GetBookByISBN(isbn);

            if (book == null || !book.IsAvailable)
                return false;

            book.IsAvailable = false;
            _repository.UpdateBook(book);

            _emailService.SendEmail(
                userEmail,
                "Book Borrowed Successfully",
                $"You borrowed: {book.Title}"
            );

            return true;
        }

        public bool ReturnBook(string isbn)
        {
            var book = _repository.GetBookByISBN(isbn);

            if (book == null)
                return false;

            book.IsAvailable = true;
            _repository.UpdateBook(book);

            return true;
        }

        public List<Book> SearchByAuthor(string author)
        {
            var allBooks = _repository.GetAllBooks();

            return allBooks
                .Where(b => b.Author.Equals(
                    author,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public decimal CalculateLateFee(int daysLate)
        {
            if (daysLate < 0)
                throw new ArgumentException(
                    "Days late cannot be negative",
                    nameof(daysLate));

            if (daysLate == 0)
                return 0m;

            if (daysLate <= 7)
                return daysLate * 1m;

            if (daysLate <= 30)
                return daysLate * 2m;

            return daysLate * 5m;
        }
    }
}