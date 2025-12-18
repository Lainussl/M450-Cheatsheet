using LibraryManagement.Interfaces;
using LibraryManagement.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LibraryManagement.Tests.EdgeCaseTests
{
    [TestClass]
    public class LateFeeTests
    {
        private Library _library = null!;

        [TestInitialize]
        public void Setup()
        {
            var mockRepo = new Mock<IBookRepository>();
            var mockEmail = new Mock<IEmailService>();
            _library = new Library(mockRepo.Object, mockEmail.Object);
        }

        // Grenzwertanalyse Tests

        [TestMethod]
        public void CalculateLateFee_ZeroDays_ReturnsZero()
        {
            // Act
            var fee = _library.CalculateLateFee(0);

            // Assert
            Assert.AreEqual(0m, fee);
        }

        [TestMethod]
        public void CalculateLateFee_OneDay_ReturnsOne()
        {
            // Act
            var fee = _library.CalculateLateFee(1);

            // Assert
            Assert.AreEqual(1m, fee);
        }

        [TestMethod]
        public void CalculateLateFee_SevenDays_ReturnsSeven()
        {
            // Act
            var fee = _library.CalculateLateFee(7);

            // Assert
            Assert.AreEqual(7m, fee);
        }

        [TestMethod]
        public void CalculateLateFee_EightDays_ReturnsSixteen()
        {
            // Act
            var fee = _library.CalculateLateFee(8);

            // Assert
            Assert.AreEqual(16m, fee); // 8 * 2
        }

        [TestMethod]
        public void CalculateLateFee_ThirtyDays_ReturnsSixty()
        {
            // Act
            var fee = _library.CalculateLateFee(30);

            // Assert
            Assert.AreEqual(60m, fee); // 30 * 2
        }

        [TestMethod]
        public void CalculateLateFee_ThirtyOneDays_ReturnsHighFee()
        {
            // Act
            var fee = _library.CalculateLateFee(31);

            // Assert
            Assert.AreEqual(155m, fee); // 31 * 5
        }

        // Äquivalenzklassen Tests

        [TestMethod]
        public void CalculateLateFee_FiveDays_InFirstRange()
        {
            // Act
            var fee = _library.CalculateLateFee(5);

            // Assert
            Assert.AreEqual(5m, fee);
        }

        [TestMethod]
        public void CalculateLateFee_FifteenDays_InSecondRange()
        {
            // Act
            var fee = _library.CalculateLateFee(15);

            // Assert
            Assert.AreEqual(30m, fee); // 15 * 2
        }

        [TestMethod]
        public void CalculateLateFee_FiftyDays_InThirdRange()
        {
            // Act
            var fee = _library.CalculateLateFee(50);

            // Assert
            Assert.AreEqual(250m, fee); // 50 * 5
        }

        // Negativtest - Ungültige Äquivalenzklasse

        [TestMethod]
        public void CalculateLateFee_NegativeDays_ThrowsArgumentException()
        {
            // Arrange
            bool exceptionThrown = false;
            string exceptionMessage = string.Empty;

            // Act
            try
            {
                _library.CalculateLateFee(-1);
            }
            catch (ArgumentException ex)
            {
                exceptionThrown = true;
                exceptionMessage = ex.Message;
            }

            // Assert
            Assert.IsTrue(exceptionThrown,
                "Expected ArgumentException was not thrown");
            Assert.IsTrue(exceptionMessage.Contains("negative"),
                "Exception message should mention 'negative'");
        }

        [TestMethod]
        public void CalculateLateFee_NegativeDays_DoesNotReturnValue()
        {
            // Arrange & Act & Assert
            try
            {
                var fee = _library.CalculateLateFee(-5);
                Assert.Fail("Expected ArgumentException but method returned: " + fee);
            }
            catch (ArgumentException)
            {
                // Test passed - exception was thrown as expected
                Assert.IsTrue(true);
            }
        }
    }
}