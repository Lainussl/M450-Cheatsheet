using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryManagement.Tests.UnitTests
{
    [TestClass]
    public class LibraryTests
    {
        [TestMethod]
        public void BorrowBook_AvailableBook_ReturnsTrue()
        {
            // Arrange
            var repo = new FakeBookRepository();
            var emailService = new FakeEmailService();
            var library = new Library(repo, emailService);

            var book = new Book
            {
                ISBN = "123",
                Title = "Clean Code",
                Author = "Robert Martin",
                IsAvailable = true
            };
            repo.AddBook(book);

            // Act
            var result = library.BorrowBook("123", "user@test.com");

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(book.IsAvailable);
        }

        [TestMethod]
        public void BorrowBook_UnavailableBook_ReturnsFalse()
        {
            // Arrange
            var repo = new FakeBookRepository();
            var emailService = new FakeEmailService();
            var library = new Library(repo, emailService);

            var book = new Book
            {
                ISBN = "123",
                Title = "Clean Code",
                IsAvailable = false
            };
            repo.AddBook(book);

            // Act
            var result = library.BorrowBook("123", "user@test.com");

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, emailService.EmailsSentCount);
        }

        [TestMethod]
        public void BorrowBook_SendsEmailToUser()
        {
            // Arrange
            var repo = new FakeBookRepository();
            var emailService = new FakeEmailService();
            var library = new Library(repo, emailService);

            var book = new Book
            {
                ISBN = "123",
                Title = "Clean Code",
                IsAvailable = true
            };
            repo.AddBook(book);

            // Act
            library.BorrowBook("123", "user@test.com");

            // Assert
            Assert.AreEqual(1, emailService.SentEmails.Count);
            Assert.AreEqual("user@test.com", emailService.SentEmails[0].To);
            Assert.AreEqual("Book Borrowed Successfully",
                          emailService.SentEmails[0].Subject);
        }

        [TestMethod]
        public void BorrowBook_NonExistentBook_ReturnsFalse()
        {
            // Arrange
            var repo = new FakeBookRepository();
            var emailService = new FakeEmailService();
            var library = new Library(repo, emailService);

            // Act
            var result = library.BorrowBook("999", "user@test.com");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReturnBook_SetsBookToAvailable()
        {
            // Arrange
            var repo = new FakeBookRepository();
            var emailService = new FakeEmailService();
            var library = new Library(repo, emailService);

            var book = new Book
            {
                ISBN = "123",
                IsAvailable = false
            };
            repo.AddBook(book);

            // Act
            var result = library.ReturnBook("123");

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(book.IsAvailable);
        }

        [TestMethod]
        public void SearchByAuthor_FindsMultipleBooks()
        {
            // Arrange
            var repo = new FakeBookRepository();
            var emailService = new FakeEmailService();
            var library = new Library(repo, emailService);

            repo.AddBook(new Book { Author = "Martin Fowler", Title = "Refactoring" });
            repo.AddBook(new Book { Author = "Robert Martin", Title = "Clean Code" });
            repo.AddBook(new Book { Author = "Martin Fowler", Title = "UML" });

            // Act
            var result = library.SearchByAuthor("Martin Fowler");

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(b => b.Author == "Martin Fowler"));
        }

        [TestMethod]
        public void SearchByAuthor_CaseInsensitive_FindsBooks()
        {
            // Arrange
            var repo = new FakeBookRepository();
            var emailService = new FakeEmailService();
            var library = new Library(repo, emailService);

            repo.AddBook(new Book { Author = "Martin Fowler", Title = "Refactoring" });

            // Act
            var result = library.SearchByAuthor("martin fowler");

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void SearchByAuthor_NoMatch_ReturnsEmptyList()
        {
            // Arrange
            var repo = new FakeBookRepository();
            var emailService = new FakeEmailService();
            var library = new Library(repo, emailService);

            repo.AddBook(new Book { Author = "Martin Fowler", Title = "Refactoring" });

            // Act
            var result = library.SearchByAuthor("Unknown Author");

            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}