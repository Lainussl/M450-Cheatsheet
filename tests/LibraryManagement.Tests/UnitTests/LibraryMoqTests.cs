using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LibraryManagement.Tests.UnitTests
{
    [TestClass]
    public class LibraryMoqTests
    {
        [TestMethod]
        public void ReturnBook_UpdatesBookToAvailable()
        {
            // Arrange
            var mockRepo = new Mock<IBookRepository>();
            var mockEmail = new Mock<IEmailService>();
            var library = new Library(mockRepo.Object, mockEmail.Object);

            var book = new Book
            {
                ISBN = "123",
                IsAvailable = false
            };

            mockRepo.Setup(r => r.GetBookByISBN("123")).Returns(book);

            // Act
            var result = library.ReturnBook("123");

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(book.IsAvailable);
            mockRepo.Verify(r => r.UpdateBook(
                It.Is<Book>(b => b.ISBN == "123" && b.IsAvailable)),
                Times.Once);
        }

        [TestMethod]
        public void SearchByAuthor_ReturnsCorrectBooks()
        {
            // Arrange
            var mockRepo = new Mock<IBookRepository>();
            var mockEmail = new Mock<IEmailService>();
            var library = new Library(mockRepo.Object, mockEmail.Object);

            var books = new List<Book>
            {
                new Book { Author = "Martin Fowler", Title = "Refactoring" },
                new Book { Author = "Robert Martin", Title = "Clean Code" },
                new Book { Author = "Martin Fowler", Title = "UML" }
            };

            mockRepo.Setup(r => r.GetAllBooks()).Returns(books);

            // Act
            var result = library.SearchByAuthor("Martin Fowler");

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(b => b.Author == "Martin Fowler"));
            mockRepo.Verify(r => r.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        public void BorrowBook_BookNotFound_NoEmailSent()
        {
            // Arrange
            var mockRepo = new Mock<IBookRepository>();
            var mockEmail = new Mock<IEmailService>();
            var library = new Library(mockRepo.Object, mockEmail.Object);

            mockRepo.Setup(r => r.GetBookByISBN(It.IsAny<string>()))
                    .Returns((Book?)null);

            // Act
            var result = library.BorrowBook("999", "user@test.com");

            // Assert
            Assert.IsFalse(result);
            mockEmail.Verify(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void BorrowBook_VerifiesRepositoryInteractions()
        {
            // Arrange
            var mockRepo = new Mock<IBookRepository>();
            var mockEmail = new Mock<IEmailService>();
            var library = new Library(mockRepo.Object, mockEmail.Object);

            var book = new Book
            {
                ISBN = "123",
                Title = "Test Book",
                IsAvailable = true
            };

            mockRepo.Setup(r => r.GetBookByISBN("123")).Returns(book);

            // Act
            library.BorrowBook("123", "user@test.com");

            // Assert
            mockRepo.Verify(r => r.GetBookByISBN("123"), Times.Once);
            mockRepo.Verify(r => r.UpdateBook(
                It.Is<Book>(b => b.ISBN == "123" && !b.IsAvailable)),
                Times.Once);
            mockEmail.Verify(e => e.SendEmail(
                "user@test.com",
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public void BorrowBook_AvailableBook_UpdatesRepository()
        {
            // Arrange
            var mockRepo = new Mock<IBookRepository>();
            var mockEmail = new Mock<IEmailService>();
            var library = new Library(mockRepo.Object, mockEmail.Object);

            var book = new Book
            {
                ISBN = "123",
                Title = "Design Patterns",
                IsAvailable = true
            };

            mockRepo.Setup(r => r.GetBookByISBN("123")).Returns(book);

            // Act
            var result = library.BorrowBook("123", "user@test.com");

            // Assert
            Assert.IsTrue(result);
            mockRepo.Verify(r => r.UpdateBook(It.IsAny<Book>()), Times.Once);
        }
    }
}