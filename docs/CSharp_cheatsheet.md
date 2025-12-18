# **M450 Prüfung - Praktisches Cheatsheet**

## **Inhaltsverzeichnis**
1. [AAA-Pattern](#aaa-pattern)
2. [MSTest2 Grundlagen](#mstest2-grundlagen)
3. [Assert Methoden](#assert-methoden)
4. [Test Doubles - Eigene Implementierung](#test-doubles---eigene-implementierung)
5. [MOQ Framework](#moq-framework)
6. [Äquivalenzklassen](#äquivalenzklassen)
7. [Grenzwertanalyse](#grenzwertanalyse)
8. [Zustandsbasierte Tests](#zustandsbasierte-tests)
9. [Exception Testing](#exception-testing)
10. [Best Practices](#best-practices)

---

## **AAA-Pattern**

**Arrange - Act - Assert**: Strukturierung jedes Tests in 3 Phasen

```csharp
[TestMethod]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Setup: Objekte erstellen, Daten vorbereiten
    var calculator = new Calculator();
    int a = 5;
    int b = 3;
    int expected = 8;
    
    // Act - Ausführung: Die zu testende Methode aufrufen
    int result = calculator.Add(a, b);
    
    // Assert - Überprüfung: Ergebnis validieren
    Assert.AreEqual(expected, result);
}
```

### **Naming Convention**

```csharp
[TestMethod]
public void MethodName_StateUnderTest_ExpectedBehavior()
{
    // Beispiele:
    // Add_TwoPositiveNumbers_ReturnsSum
    // Login_InvalidPassword_ReturnsFalse
    // BorrowBook_AvailableBook_ReturnsTrue
}
```

---

## **MSTest2 Grundlagen**

### **Test Class Setup**

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyProject.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        private Calculator _calculator = null!;
        
        // Wird vor JEDEM Test ausgeführt
        [TestInitialize]
        public void Setup()
        {
            _calculator = new Calculator();
        }
        
        // Wird nach JEDEM Test ausgeführt
        [TestCleanup]
        public void Cleanup()
        {
            _calculator = null;
        }
        
        // Wird 1x vor allen Tests ausgeführt
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            // Einmalige Initialisierung
        }
        
        // Wird 1x nach allen Tests ausgeführt
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Aufräumarbeiten
        }
        
        [TestMethod]
        public void MyTest()
        {
            // Test Code
        }
    }
}
```

---

## **Assert Methoden**

### **Gleichheit**

```csharp
[TestMethod]
public void EqualityAssertions()
{
    // Gleichheit prüfen
    Assert.AreEqual(5, result);
    Assert.AreEqual(expected, actual);
    Assert.AreEqual(3.14, result, 0.01); // Mit Toleranz für Doubles
    
    // Ungleichheit prüfen
    Assert.AreNotEqual(0, result);
    Assert.AreNotEqual(expected, actual);
}
```

### **Boolean**

```csharp
[TestMethod]
public void BooleanAssertions()
{
    // Boolean Werte
    Assert.IsTrue(condition);
    Assert.IsTrue(user.IsActive);
    
    Assert.IsFalse(condition);
    Assert.IsFalse(user.IsDeleted);
}
```

### **Null Checks**

```csharp
[TestMethod]
public void NullAssertions()
{
    // Null prüfen
    Assert.IsNull(result);
    Assert.IsNull(user);
    
    // Nicht Null prüfen
    Assert.IsNotNull(result);
    Assert.IsNotNull(user);
}
```

### **Type Checks**

```csharp
[TestMethod]
public void TypeAssertions()
{
    object obj = new Customer();
    
    // Typ prüfen
    Assert.IsInstanceOfType(obj, typeof(Customer));
    Assert.IsInstanceOfType(result, typeof(IEnumerable<string>));
    
    // Typ verneinen
    Assert.IsNotInstanceOfType(obj, typeof(Admin));
}
```

### **String Assertions**

```csharp
[TestMethod]
public void StringAssertions()
{
    string text = "Hello World";
    
    // Enthält
    StringAssert.Contains(text, "World");
    
    // Beginnt mit
    StringAssert.StartsWith(text, "Hello");
    
    // Endet mit
    StringAssert.EndsWith(text, "World");
    
    // Regex Match
    StringAssert.Matches(text, new Regex(@"\w+ \w+"));
}
```

### **Collection Assertions**

```csharp
[TestMethod]
public void CollectionAssertions()
{
    var list = new List<int> { 1, 2, 3, 4, 5 };
    
    // Count prüfen
    Assert.AreEqual(5, list.Count);
    
    // Leer prüfen
    Assert.AreEqual(0, emptyList.Count);
    
    // Nicht leer
    Assert.IsTrue(list.Count > 0);
    
    // Enthält Element
    CollectionAssert.Contains(list, 3);
    
    // Alle Elemente sind einzigartig
    CollectionAssert.AllItemsAreUnique(list);
    
    // Gleiche Elemente, gleiche Reihenfolge
    var expected = new List<int> { 1, 2, 3 };
    var actual = new List<int> { 1, 2, 3 };
    CollectionAssert.AreEqual(expected, actual);
    
    // Gleiche Elemente, Reihenfolge egal
    var list1 = new List<int> { 1, 2, 3 };
    var list2 = new List<int> { 3, 1, 2 };
    CollectionAssert.AreEquivalent(list1, list2);
    
    // Ist Subset
    var subset = new List<int> { 1, 2 };
    CollectionAssert.IsSubsetOf(subset, list);
}
```

### **Numerische Assertions**

```csharp
[TestMethod]
public void NumericAssertions()
{
    int value = 5;
    
    // Größer/Kleiner (manuell)
    Assert.IsTrue(value > 3);
    Assert.IsTrue(value < 10);
    Assert.IsTrue(value >= 5);
    Assert.IsTrue(value <= 5);
    
    // In Range
    Assert.IsTrue(value >= 1 && value <= 10);
}
```

---

## **Test Doubles - Eigene Implementierung**

### **Fake Repository**

```csharp
// Interface
public interface IUserRepository
{
    User? GetById(int id);
    void Add(User user);
    void Update(User user);
    void Delete(int id);
    List<User> GetAll();
}

// Fake Implementierung
public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;
    
    // Tracking für Tests
    public List<User> AddedUsers { get; } = new();
    public List<User> UpdatedUsers { get; } = new();
    public List<int> DeletedIds { get; } = new();
    
    public User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }
    
    public void Add(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
        AddedUsers.Add(user);
    }
    
    public void Update(User user)
    {
        var existing = GetById(user.Id);
        if (existing != null)
        {
            _users.Remove(existing);
            _users.Add(user);
            UpdatedUsers.Add(user);
        }
    }
    
    public void Delete(int id)
    {
        var user = GetById(id);
        if (user != null)
        {
            _users.Remove(user);
            DeletedIds.Add(id);
        }
    }
    
    public List<User> GetAll()
    {
        return _users.ToList();
    }
    
    // Helper für Tests
    public void AddTestData(params User[] users)
    {
        foreach (var user in users)
        {
            _users.Add(user);
        }
    }
}
```

### **Fake Service**

```csharp
// Interface
public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
    void SendEmailWithAttachment(string to, string subject, 
                                  string body, byte[] attachment);
}

// Fake Implementierung
public class FakeEmailService : IEmailService
{
    public List<EmailRecord> SentEmails { get; } = new();
    public bool ThrowException { get; set; } = false;
    public int CallCount { get; private set; } = 0;
    
    public void SendEmail(string to, string subject, string body)
    {
        CallCount++;
        
        if (ThrowException)
            throw new InvalidOperationException("Email service unavailable");
        
        SentEmails.Add(new EmailRecord(to, subject, body, null));
    }
    
    public void SendEmailWithAttachment(string to, string subject, 
                                       string body, byte[] attachment)
    {
        CallCount++;
        SentEmails.Add(new EmailRecord(to, subject, body, attachment));
    }
    
    // Helper Methoden
    public bool WasEmailSentTo(string email)
    {
        return SentEmails.Any(e => e.To == email);
    }
    
    public EmailRecord? GetLastEmail()
    {
        return SentEmails.LastOrDefault();
    }
}

public record EmailRecord(string To, string Subject, string Body, 
                         byte[]? Attachment);
```

### **Test mit eigenem Fake**

```csharp
[TestClass]
public class UserServiceTests
{
    [TestMethod]
    public void RegisterUser_ValidData_AddsUserAndSendsEmail()
    {
        // Arrange
        var fakeRepo = new FakeUserRepository();
        var fakeEmail = new FakeEmailService();
        var service = new UserService(fakeRepo, fakeEmail);
        
        var newUser = new User 
        { 
            Name = "John Doe", 
            Email = "john@test.com" 
        };
        
        // Act
        service.RegisterUser(newUser);
        
        // Assert
        Assert.AreEqual(1, fakeRepo.AddedUsers.Count);
        Assert.AreEqual("John Doe", fakeRepo.AddedUsers[0].Name);
        
        Assert.AreEqual(1, fakeEmail.SentEmails.Count);
        Assert.AreEqual("john@test.com", fakeEmail.SentEmails[0].To);
        Assert.IsTrue(fakeEmail.SentEmails[0].Subject.Contains("Welcome"));
    }
}
```

---

## **MOQ Framework**

### **Basis Setup**

```csharp
using Moq;

[TestClass]
public class UserServiceMoqTests
{
    [TestMethod]
    public void BasicMockExample()
    {
        // Mock erstellen
        var mockRepository = new Mock<IUserRepository>();
        
        // Mock in Service injizieren
        var service = new UserService(mockRepository.Object);
        
        // Service verwenden
        service.DoSomething();
    }
}
```

### **Setup - Return Values**

```csharp
[TestMethod]
public void SetupReturnValues()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    
    // Spezifischer Parameter
    mockRepo.Setup(r => r.GetById(1))
            .Returns(new User { Id = 1, Name = "John" });
    
    // Beliebiger Parameter (It.IsAny)
    mockRepo.Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(new User { Id = 999, Name = "Default" });
    
    // Conditional Setup
    mockRepo.Setup(r => r.GetById(It.Is<int>(id => id > 0)))
            .Returns(new User { Id = 1, Name = "Valid" });
    
    mockRepo.Setup(r => r.GetById(It.Is<int>(id => id <= 0)))
            .Returns((User?)null);
    
    // Callback für komplexe Logik
    mockRepo.Setup(r => r.GetById(It.IsAny<int>()))
            .Returns<int>(id => new User { Id = id, Name = $"User{id}" });
}
```

### **Setup - Collections**

```csharp
[TestMethod]
public void SetupCollections()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    
    var users = new List<User>
    {
        new User { Id = 1, Name = "John" },
        new User { Id = 2, Name = "Jane" },
        new User { Id = 3, Name = "Bob" }
    };
    
    mockRepo.Setup(r => r.GetAll())
            .Returns(users);
    
    // Act
    var service = new UserService(mockRepo.Object);
    var result = service.GetAllActiveUsers();
    
    // Assert
    Assert.IsTrue(result.Count > 0);
}
```

### **Setup - Exceptions**

```csharp
[TestMethod]
public void SetupExceptions()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    
    // Exception werfen
    mockRepo.Setup(r => r.GetById(It.IsAny<int>()))
            .Throws<InvalidOperationException>();
    
    mockRepo.Setup(r => r.GetById(999))
            .Throws(new ArgumentException("User not found"));
    
    // Act & Assert
    var service = new UserService(mockRepo.Object);
    
    try
    {
        service.GetUser(999);
        Assert.Fail("Expected exception was not thrown");
    }
    catch (ArgumentException ex)
    {
        Assert.IsTrue(ex.Message.Contains("not found"));
    }
}
```

### **Verify - Methodenaufrufe prüfen**

```csharp
[TestMethod]
public void VerifyMethodCalls()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    var mockEmail = new Mock<IEmailService>();
    var service = new UserService(mockRepo.Object, mockEmail.Object);
    
    var user = new User { Id = 1, Name = "John" };
    mockRepo.Setup(r => r.GetById(1)).Returns(user);
    
    // Act
    service.DeleteUser(1);
    
    // Assert - Verify
    
    // Methode wurde genau 1x aufgerufen
    mockRepo.Verify(r => r.Delete(1), Times.Once);
    
    // Methode wurde nie aufgerufen
    mockEmail.Verify(e => e.SendEmail(
        It.IsAny<string>(), 
        It.IsAny<string>(), 
        It.IsAny<string>()), 
        Times.Never);
    
    // Methode wurde mindestens 1x aufgerufen
    mockRepo.Verify(r => r.GetById(It.IsAny<int>()), Times.AtLeastOnce);
    
    // Methode wurde genau 2x aufgerufen
    mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Exactly(2));
    
    // Methode wurde mit spezifischen Parametern aufgerufen
    mockRepo.Verify(r => r.Delete(It.Is<int>(id => id == 1)), Times.Once);
}
```

### **Verify mit It.Is - Komplexe Parameter**

```csharp
[TestMethod]
public void VerifyWithComplexParameters()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    var service = new UserService(mockRepo.Object);
    
    // Act
    service.UpdateUserEmail(1, "new@email.com");
    
    // Assert - Verify mit Bedingungen
    mockRepo.Verify(r => r.Update(It.Is<User>(u => 
        u.Id == 1 && 
        u.Email == "new@email.com")), 
        Times.Once);
}
```

### **Times Options**

```csharp
[TestMethod]
public void TimesOptions()
{
    var mock = new Mock<IService>();
    
    // Genau X mal
    mock.Verify(m => m.Method(), Times.Once);
    mock.Verify(m => m.Method(), Times.Exactly(3));
    
    // Nie
    mock.Verify(m => m.Method(), Times.Never);
    
    // Mindestens / Höchstens
    mock.Verify(m => m.Method(), Times.AtLeastOnce);
    mock.Verify(m => m.Method(), Times.AtLeast(2));
    mock.Verify(m => m.Method(), Times.AtMost(5));
    
    // Zwischen
    mock.Verify(m => m.Method(), Times.Between(1, 3, Range.Inclusive));
}
```

### **It.IsAny vs It.Is**

```csharp
[TestMethod]
public void ItIsAnyVsItIs()
{
    var mock = new Mock<IUserRepository>();
    
    // It.IsAny - Beliebiger Wert dieses Typs
    mock.Setup(r => r.GetById(It.IsAny<int>()))
        .Returns(new User());
    
    // It.Is - Mit Bedingung
    mock.Setup(r => r.GetById(It.Is<int>(id => id > 0)))
        .Returns(new User { Id = 1 });
    
    mock.Setup(r => r.GetById(It.Is<int>(id => id <= 0)))
        .Returns((User?)null);
    
    // Verify mit It.Is
    mock.Verify(r => r.Update(It.Is<User>(u => u.IsActive)), Times.Once);
}
```

### **Komplettes MOQ Beispiel**

```csharp
[TestClass]
public class OrderServiceTests
{
    [TestMethod]
    public void CreateOrder_ValidData_SavesOrderAndSendsConfirmation()
    {
        // Arrange
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockPaymentService = new Mock<IPaymentService>();
        
        var order = new Order 
        { 
            CustomerId = 1, 
            Total = 100.00m 
        };
        
        // Setup Payment Service
        mockPaymentService
            .Setup(p => p.ProcessPayment(It.IsAny<decimal>()))
            .Returns(true);
        
        // Setup Email Service (nichts zu returnen, nur tracking)
        mockEmailService
            .Setup(e => e.SendEmail(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()));
        
        var service = new OrderService(
            mockOrderRepo.Object, 
            mockEmailService.Object,
            mockPaymentService.Object
        );
        
        // Act
        var result = service.CreateOrder(order, "customer@test.com");
        
        // Assert
        Assert.IsTrue(result);
        
        // Verify: Payment wurde verarbeitet
        mockPaymentService.Verify(
            p => p.ProcessPayment(100.00m), 
            Times.Once
        );
        
        // Verify: Order wurde gespeichert
        mockOrderRepo.Verify(
            r => r.Save(It.Is<Order>(o => 
                o.CustomerId == 1 && 
                o.Total == 100.00m)), 
            Times.Once
        );
        
        // Verify: Email wurde gesendet
        mockEmailService.Verify(
            e => e.SendEmail(
                "customer@test.com",
                It.Is<string>(s => s.Contains("Order Confirmation")),
                It.IsAny<string>()),
            Times.Once
        );
    }
}
```

---

## **Äquivalenzklassen**

### **Konzept**

Eingabewerte in Klassen gruppieren, die gleich behandelt werden.

```csharp
// Beispiel: Altersprüfung (18-65 Jahre)

// Äquivalenzklassen:
// 1. Ungültig: < 18 (z.B. 0, 10, 17)
// 2. Gültig: 18-65 (z.B. 18, 30, 65)
// 3. Ungültig: > 65 (z.B. 66, 100)
```

### **Implementierung**

```csharp
public class AgeValidator
{
    public bool IsValidAge(int age)
    {
        return age >= 18 && age <= 65;
    }
}

[TestClass]
public class AgeValidatorTests
{
    private AgeValidator _validator = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _validator = new AgeValidator();
    }
    
    // Äquivalenzklasse 1: Ungültig - Zu jung
    [TestMethod]
    public void IsValidAge_BelowMinimum_ReturnsFalse()
    {
        // Repräsentativer Wert aus Klasse 1
        var result = _validator.IsValidAge(10);
        Assert.IsFalse(result);
    }
    
    // Äquivalenzklasse 2: Gültig
    [TestMethod]
    public void IsValidAge_WithinRange_ReturnsTrue()
    {
        // Repräsentativer Wert aus Klasse 2
        var result = _validator.IsValidAge(30);
        Assert.IsTrue(result);
    }
    
    // Äquivalenzklasse 3: Ungültig - Zu alt
    [TestMethod]
    public void IsValidAge_AboveMaximum_ReturnsFalse()
    {
        // Repräsentativer Wert aus Klasse 3
        var result = _validator.IsValidAge(70);
        Assert.IsFalse(result);
    }
}
```

### **Mehrere Dimensionen**

```csharp
// Beispiel: Rabatt berechnen
// - Kunde Typ: Standard, Premium, VIP
// - Bestellwert: < 100, 100-500, > 500

[TestClass]
public class DiscountCalculatorTests
{
    // Standard Kunde, kleiner Wert
    [TestMethod]
    public void CalculateDiscount_StandardCustomer_SmallOrder_NoDiscount()
    {
        var calculator = new DiscountCalculator();
        var discount = calculator.Calculate(CustomerType.Standard, 50);
        Assert.AreEqual(0m, discount);
    }
    
    // Premium Kunde, mittlerer Wert
    [TestMethod]
    public void CalculateDiscount_PremiumCustomer_MediumOrder_Gets10Percent()
    {
        var calculator = new DiscountCalculator();
        var discount = calculator.Calculate(CustomerType.Premium, 200);
        Assert.AreEqual(20m, discount); // 10% von 200
    }
    
    // VIP Kunde, großer Wert
    [TestMethod]
    public void CalculateDiscount_VIPCustomer_LargeOrder_Gets20Percent()
    {
        var calculator = new DiscountCalculator();
        var discount = calculator.Calculate(CustomerType.VIP, 1000);
        Assert.AreEqual(200m, discount); // 20% von 1000
    }
}
```

---

## **Grenzwertanalyse**

### **Konzept**

Teste die Grenzen zwischen Äquivalenzklassen plus direkte Nachbarn.

```csharp
// Beispiel: Alter 18-65
// Grenzwerte:
// - 17 (ungültig, direkt vor Untergrenze)
// - 18 (gültig, Untergrenze)
// - 19 (gültig, direkt nach Untergrenze)
// - 64 (gültig, direkt vor Obergrenze)
// - 65 (gültig, Obergrenze)
// - 66 (ungültig, direkt nach Obergrenze)
```

### **Implementierung**

```csharp
[TestClass]
public class AgeValidatorBoundaryTests
{
    private AgeValidator _validator = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _validator = new AgeValidator();
    }
    
    // Untergrenze Tests
    [TestMethod]
    public void IsValidAge_OneLessThanMinimum_ReturnsFalse()
    {
        var result = _validator.IsValidAge(17);
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void IsValidAge_AtMinimum_ReturnsTrue()
    {
        var result = _validator.IsValidAge(18);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsValidAge_OneMoreThanMinimum_ReturnsTrue()
    {
        var result = _validator.IsValidAge(19);
        Assert.IsTrue(result);
    }
    
    // Obergrenze Tests
    [TestMethod]
    public void IsValidAge_OneLessThanMaximum_ReturnsTrue()
    {
        var result = _validator.IsValidAge(64);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsValidAge_AtMaximum_ReturnsTrue()
    {
        var result = _validator.IsValidAge(65);
        Assert.IsTrue(result);
    }
    
    [TestMethod]
    public void IsValidAge_OneMoreThanMaximum_ReturnsFalse()
    {
        var result = _validator.IsValidAge(66);
        Assert.IsFalse(result);
    }
}
```

### **Komplexes Beispiel mit Gebühren**

```csharp
public class LateFeeCalculator
{
    public decimal Calculate(int daysLate)
    {
        if (daysLate < 0)
            throw new ArgumentException("Days cannot be negative");
        
        if (daysLate == 0) return 0m;
        if (daysLate <= 7) return daysLate * 1m;
        if (daysLate <= 30) return daysLate * 2m;
        return daysLate * 5m;
    }
}

[TestClass]
public class LateFeeCalculatorTests
{
    private LateFeeCalculator _calculator = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _calculator = new LateFeeCalculator();
    }
    
    // Grenze 1: 0 Tage
    [TestMethod]
    public void Calculate_ZeroDays_ReturnsZero()
    {
        Assert.AreEqual(0m, _calculator.Calculate(0));
    }
    
    [TestMethod]
    public void Calculate_OneDay_ReturnsOneFranc()
    {
        Assert.AreEqual(1m, _calculator.Calculate(1));
    }
    
    // Grenze 2: 7 Tage (Übergang 1 CHF → 2 CHF)
    [TestMethod]
    public void Calculate_SevenDays_ReturnsSevenFrancs()
    {
        Assert.AreEqual(7m, _calculator.Calculate(7));
    }
    
    [TestMethod]
    public void Calculate_EightDays_ReturnsSixteenFrancs()
    {
        Assert.AreEqual(16m, _calculator.Calculate(8)); // 8 * 2
    }
    
    // Grenze 3: 30 Tage (Übergang 2 CHF → 5 CHF)
    [TestMethod]
    public void Calculate_ThirtyDays_ReturnsSixtyFrancs()
    {
        Assert.AreEqual(60m, _calculator.Calculate(30)); // 30 * 2
    }
    
    [TestMethod]
    public void Calculate_ThirtyOneDays_Returns155Francs()
    {
        Assert.AreEqual(155m, _calculator.Calculate(31)); // 31 * 5
    }
    
    // Negativer Grenzwert
    [TestMethod]
    public void Calculate_NegativeDays_ThrowsException()
    {
        try
        {
            _calculator.Calculate(-1);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }
}
```

---

## **Zustandsbasierte Tests**

### **State Machine Beispiel**

```csharp
public enum OrderState
{
    New,
    Confirmed,
    Paid,
    Shipped,
    Delivered,
    Cancelled
}

public class Order
{
    public int Id { get; set; }
    public OrderState State { get; private set; } = OrderState.New;
    
    public void Confirm()
    {
        if (State != OrderState.New)
            throw new InvalidOperationException(
                "Can only confirm new orders");
        
        State = OrderState.Confirmed;
    }
    
    public void Pay()
    {
        if (State != OrderState.Confirmed)
            throw new InvalidOperationException(
                "Can only pay confirmed orders");
        
        State = OrderState.Paid;
    }
    
    public void Ship()
    {
        if (State != OrderState.Paid)
            throw new InvalidOperationException(
                "Can only ship paid orders");
        
        State = OrderState.Shipped;
    }
    
    public void Deliver()
    {
        if (State != OrderState.Shipped)
            throw new InvalidOperationException(
                "Can only deliver shipped orders");
        
        State = OrderState.Delivered;
    }
    
    public void Cancel()
    {
        if (State == OrderState.Delivered)
            throw new InvalidOperationException(
                "Cannot cancel delivered orders");
        
        State = OrderState.Cancelled;
    }
}
```

### **Zustandsübergänge testen**

```csharp
[TestClass]
public class OrderStateTests
{
    // Gültige Übergänge
    
    [TestMethod]
    public void Order_NewToConfirmed_StateChangesCorrectly()
    {
        // Arrange
        var order = new Order();
        Assert.AreEqual(OrderState.New, order.State);
        
        // Act
        order.Confirm();
        
        // Assert
        Assert.AreEqual(OrderState.Confirmed, order.State);
    }
    
    [TestMethod]
    public void Order_ConfirmedToPaid_StateChangesCorrectly()
    {
        // Arrange
        var order = new Order();
        order.Confirm();
        
        // Act
        order.Pay();
        
        // Assert
        Assert.AreEqual(OrderState.Paid, order.State);
    }
    
    [TestMethod]
    public void Order_PaidToShipped_StateChangesCorrectly()
    {
        // Arrange
        var order = new Order();
        order.Confirm();
        order.Pay();
        
        // Act
        order.Ship();
        
        // Assert
        Assert.AreEqual(OrderState.Shipped, order.State);
    }
    
    [TestMethod]
    public void Order_ShippedToDelivered_StateChangesCorrectly()
    {
        // Arrange
        var order = new Order();
        order.Confirm();
        order.Pay();
        order.Ship();
        
        // Act
        order.Deliver();
        
        // Assert
        Assert.AreEqual(OrderState.Delivered, order.State);
    }
    
    // Ungültige Übergänge
    
    [TestMethod]
    public void Order_NewToShipped_ThrowsException()
    {
        // Arrange
        var order = new Order();
        
        // Act & Assert
        try
        {
            order.Ship();
            Assert.Fail("Expected InvalidOperationException");
        }
        catch (InvalidOperationException ex)
        {
            Assert.IsTrue(ex.Message.Contains("paid"));
        }
    }
    
    [TestMethod]
    public void Order_CancelDelivered_ThrowsException()
    {
        // Arrange
        var order = new Order();
        order.Confirm();
        order.Pay();
        order.Ship();
        order.Deliver();
        
        // Act & Assert
        try
        {
            order.Cancel();
            Assert.Fail("Expected InvalidOperationException");
        }
        catch (InvalidOperationException ex)
        {
            Assert.IsTrue(ex.Message.Contains("delivered"));
        }
    }
    
    // Kompletter Workflow
    
    [TestMethod]
    public void Order_CompleteWorkflow_AllStatesCorrect()
    {
        // Arrange
        var order = new Order();
        
        // Act & Assert - Schritt für Schritt
        Assert.AreEqual(OrderState.New, order.State);
        
        order.Confirm();
        Assert.AreEqual(OrderState.Confirmed, order.State);
        
        order.Pay();
        Assert.AreEqual(OrderState.Paid, order.State);
        
        order.Ship();
        Assert.AreEqual(OrderState.Shipped, order.State);
        
        order.Deliver();
        Assert.AreEqual(OrderState.Delivered, order.State);
    }
}
```

### **Zustandsbaum visualisieren**

```
State Transition Diagram:

    New ──────> Confirmed ──────> Paid ──────> Shipped ──────> Delivered
     │              │              │              │
     │              │              │              │
     └──> Cancel <──┴──> Cancel <──┴──> Cancel <──┘

Allowed Transitions:
- New → Confirmed
- Confirmed → Paid
- Paid → Shipped
- Shipped → Delivered
- New/Confirmed/Paid/Shipped → Cancelled

Forbidden Transitions:
- New → Paid/Shipped/Delivered
- Delivered → Cancelled
- etc.
```

---

## **Exception Testing**

### **Methode 1: Try-Catch (Empfohlen)**

```csharp
[TestMethod]
public void Method_InvalidInput_ThrowsException()
{
    // Arrange
    var calculator = new Calculator();
    bool exceptionThrown = false;
    
    // Act
    try
    {
        calculator.Divide(10, 0);
        Assert.Fail("Expected DivideByZeroException was not thrown");
    }
    catch (DivideByZeroException ex)
    {
        exceptionThrown = true;
        
        // Optional: Exception Details prüfen
        Assert.IsTrue(ex.Message.Contains("zero"));
    }
    
    // Assert
    Assert.IsTrue(exceptionThrown);
}
```

### **Methode 2: Mit Exception Variable**

```csharp
[TestMethod]
public void Method_InvalidInput_ThrowsCorrectException()
{
    // Arrange
    var validator = new EmailValidator();
    Exception? caughtException = null;
    
    // Act
    try
    {
        validator.Validate("invalid-email");
    }
    catch (Exception ex)
    {
        caughtException = ex;
    }
    
    // Assert
    Assert.IsNotNull(caughtException, "No exception was thrown");
    Assert.IsInstanceOfType(caughtException, typeof(ArgumentException));
    
    var argEx = (ArgumentException)caughtException;
    Assert.AreEqual("email", argEx.ParamName);
    Assert.IsTrue(argEx.Message.Contains("invalid format"));
}
```

### **Exception mit MOQ**

```csharp
[TestMethod]
public void Service_RepositoryThrowsException_HandlesGracefully()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetById(It.IsAny<int>()))
            .Throws<DatabaseException>();
    
    var service = new UserService(mockRepo.Object);
    
    // Act
    var result = service.TryGetUser(1);
    
    // Assert
    Assert.IsNull(result, "Service should return null on exception");
}
```

### **Multiple Exception Types**

```csharp
[TestMethod]
public void Validator_DifferentInvalidInputs_ThrowsCorrectExceptions()
{
    var validator = new UserValidator();
    
    // Null Input
    try
    {
        validator.Validate(null);
        Assert.Fail("Expected ArgumentNullException");
    }
    catch (ArgumentNullException ex)
    {
        Assert.AreEqual("user", ex.ParamName);
    }
    
    // Empty Name
    try
    {
        validator.Validate(new User { Name = "" });
        Assert.Fail("Expected ArgumentException");
    }
    catch (ArgumentException ex)
    {
        Assert.IsTrue(ex.Message.Contains("Name"));
    }
    
    // Invalid Email
    try
    {
        validator.Validate(new User { Name = "John", Email = "invalid" });
        Assert.Fail("Expected FormatException");
    }
    catch (FormatException ex)
    {
        Assert.IsTrue(ex.Message.Contains("Email"));
    }
}
```

---

## **Best Practices**

### **Test Naming**

```csharp
// ✅ GUTE Namen (selbsterklärend)
[TestMethod]
public void Add_TwoPositiveNumbers_ReturnsSum()

[TestMethod]
public void Login_InvalidPassword_ReturnsFalse()

[TestMethod]
public void GetUser_NonExistentId_ReturnsNull()

// ❌ SCHLECHTE Namen
[TestMethod]
public void Test1()

[TestMethod]
public void AddTest()

[TestMethod]
public void TestUserLogin()
```

### **Ein Assert pro Test (wenn möglich)**

```csharp
// ✅ GUT - Fokussiert
[TestMethod]
public void Add_TwoPositiveNumbers_ReturnsCorrectSum()
{
    var result = calculator.Add(5, 3);
    Assert.AreEqual(8, result);
}

[TestMethod]
public void Add_TwoPositiveNumbers_DoesNotReturnZero()
{
    var result = calculator.Add(5, 3);
    Assert.AreNotEqual(0, result);
}

// ⚠️ AKZEPTABEL - Logisch zusammenhängend
[TestMethod]
public void CreateUser_ValidData_UserHasCorrectProperties()
{
    var user = service.CreateUser("John", "john@test.com");
    
    Assert.IsNotNull(user);
    Assert.AreEqual("John", user.Name);
    Assert.AreEqual("john@test.com", user.Email);
    Assert.IsTrue(user.Id > 0);
}
```

### **Test Isolation**

```csharp
// ✅ GUT - Jeder Test ist unabhängig
[TestClass]
public class IsolatedTests
{
    [TestInitialize]
    public void Setup()
    {
        // Frische Instanz für jeden Test
        _calculator = new Calculator();
    }
    
    [TestMethod]
    public void Test1()
    {
        _calculator.Add(5, 3);
        // Beeinflusst NICHT Test2
    }
    
    [TestMethod]
    public void Test2()
    {
        // Startet mit frischer Instanz
        _calculator.Add(2, 2);
    }
}

// ❌ SCHLECHT - Tests beeinflussen sich gegenseitig
[TestClass]
public class DependentTests
{
    private static Calculator _sharedCalculator = new Calculator();
    
    [TestMethod]
    public void Test1()
    {
        _sharedCalculator.Add(5, 3);
        // Ändert Zustand für Test2!
    }
    
    [TestMethod]
    public void Test2()
    {
        // Hängt von Test1 ab
        var result = _sharedCalculator.GetLastResult();
    }
}
```

### **Arrange-Act-Assert klar trennen**

```csharp
// ✅ GUT - Klar getrennt
[TestMethod]
public void Test_ClearStructure()
{
    // Arrange
    var repository = new FakeRepository();
    var service = new UserService(repository);
    var user = new User { Name = "John" };
    
    // Act
    var result = service.Register(user);
    
    // Assert
    Assert.IsTrue(result);
}

// ❌ SCHLECHT - Vermischt
[TestMethod]
public void Test_MixedStructure()
{
    var service = new UserService(new FakeRepository());
    var result = service.Register(new User { Name = "John" });
    Assert.IsTrue(result);
    var allUsers = service.GetAll();
    Assert.AreEqual(1, allUsers.Count);
}
```

### **Aussagekräftige Assert Messages**

```csharp
// ✅ GUT - Mit Message
[TestMethod]
public void Test_WithMessage()
{
    var result = calculator.Add(5, 3);
    Assert.AreEqual(8, result, 
        $"Expected 5 + 3 = 8, but got {result}");
}

// Bei komplexen Bedingungen
[TestMethod]
public void Test_ComplexAssertion()
{
    Assert.IsTrue(user.IsActive && user.IsVerified, 
        $"User should be active and verified. " +
        $"IsActive: {user.IsActive}, IsVerified: {user.IsVerified}");
}
```

### **Test Data Builders**

```csharp
// Helper Klasse für Testdaten
public class UserBuilder
{
    private string _name = "Default User";
    private string _email = "default@test.com";
    private bool _isActive = true;
    
    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }
    
    public UserBuilder Inactive()
    {
        _isActive = false;
        return this;
    }
    
    public User Build()
    {
        return new User
        {
            Name = _name,
            Email = _email,
            IsActive = _isActive
        };
    }
}

// Verwendung
[TestMethod]
public void Test_WithBuilder()
{
    // Arrange
    var user = new UserBuilder()
        .WithName("John Doe")
        .WithEmail("john@test.com")
        .Build();
    
    // Act & Assert
    Assert.AreEqual("John Doe", user.Name);
}
```

### **Positive und Negative Tests**

```csharp
[TestClass]
public class BalancedTests
{
    // ✅ Positive Tests (Happy Path)
    [TestMethod]
    public void Login_ValidCredentials_ReturnsTrue()
    {
        var result = auth.Login("user", "correct-password");
        Assert.IsTrue(result);
    }
    
    // ✅ Negative Tests (Error Cases)
    [TestMethod]
    public void Login_InvalidPassword_ReturnsFalse()
    {
        var result = auth.Login("user", "wrong-password");
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void Login_NonExistentUser_ReturnsFalse()
    {
        var result = auth.Login("unknown", "password");
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void Login_EmptyPassword_ThrowsException()
    {
        try
        {
            auth.Login("user", "");
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            // Expected
        }
    }
}
```

### **Test Organisation**

```csharp
[TestClass]
public class UserServiceTests
{
    // Gruppiere Tests nach Feature/Methode
    
    #region Register Tests
    
    [TestMethod]
    public void Register_ValidUser_ReturnsTrue()
    {
        // ...
    }
    
    [TestMethod]
    public void Register_DuplicateEmail_ReturnsFalse()
    {
        // ...
    }
    
    #endregion
    
    #region Login Tests
    
    [TestMethod]
    public void Login_ValidCredentials_ReturnsToken()
    {
        // ...
    }
    
    [TestMethod]
    public void Login_InvalidCredentials_ReturnsNull()
    {
        // ...
    }
    
    #endregion
}
```

---

## **Schnellreferenz - Häufige Patterns**

### **Repository Test Pattern**

```csharp
[TestMethod]
public void Service_UsesRepository_CorrectInteraction()
{
    // Arrange
    var mockRepo = new Mock<IRepository>();
    mockRepo.Setup(r => r.GetById(1))
            .Returns(new Entity { Id = 1 });
    
    var service = new Service(mockRepo.Object);
    
    // Act
    service.DoSomething(1);
    
    // Assert
    mockRepo.Verify(r => r.GetById(1), Times.Once);
    mockRepo.Verify(r => r.Update(It.IsAny<Entity>()), Times.Once);
}
```

### **Email Service Test Pattern**

```csharp
[TestMethod]
public void Service_SendsEmail_WithCorrectContent()
{
    // Arrange
    var fakeEmail = new FakeEmailService();
    var service = new Service(fakeEmail);
    
    // Act
    service.ProcessOrder(orderId: 123);
    
    // Assert
    Assert.AreEqual(1, fakeEmail.SentEmails.Count);
    Assert.AreEqual("customer@test.com", fakeEmail.SentEmails[0].To);
    Assert.IsTrue(fakeEmail.SentEmails[0].Subject.Contains("Order"));
}
```

### **State Validation Pattern**

```csharp
[TestMethod]
public void Service_ChangesState_Correctly()
{
    // Arrange
    var entity = new Entity { State = EntityState.New };
    var service = new Service();
    
    // Act
    service.Process(entity);
    
    // Assert
    Assert.AreEqual(EntityState.Processed, entity.State);
}
```

### **Collection Validation Pattern**

```csharp
[TestMethod]
public void Service_ReturnsFilteredList()
{
    // Arrange
    var mockRepo = new Mock<IRepository>();
    mockRepo.Setup(r => r.GetAll()).Returns(new List<Item>
    {
        new Item { IsActive = true, Name = "A" },
        new Item { IsActive = false, Name = "B" },
        new Item { IsActive = true, Name = "C" }
    });
    
    var service = new Service(mockRepo.Object);
    
    // Act
    var result = service.GetActiveItems();
    
    // Assert
    Assert.AreEqual(2, result.Count);
    Assert.IsTrue(result.All(i => i.IsActive));
}
```
