using NUnit.Framework;
using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Tests
{
    [TestFixture]
    public class PersonalDetailsControllerTests
    {
        private PersonalDetailsController _controller;
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(options);
            _controller = new PersonalDetailsController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }



        [Test]
        public async Task GetPersonalDetailsById_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetPersonalDetails(100);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PostPersonalDetails_ValidData_CreatesItem()
        {
            // Arrange
            var userData = new Users { UserId = 1 };
            _context.Users.Add(userData);
            _context.SaveChanges();

            // Act
            var result = await _controller.PostPersonalDetails(1, "John Doe", 1234567890, "123456789012", "123 Main St", 1);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual("GetPersonalDetails", createdAtActionResult.ActionName);
            var item = createdAtActionResult.Value as PersonalDetails;
            Assert.IsNotNull(item);
            Assert.AreEqual("John Doe", item.Name);
        }

        [Test]
        public async Task DeletePersonalDetails_ExistingId_RemovesItem()
        {
            // Arrange
            var testData = new PersonalDetails { PersonId = 1, Name = "John Doe" };
            _context.PersonalDetails.Add(testData);
            _context.SaveChanges();

            // Act
            var result = await _controller.DeletePersonalDetails(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            var deletedItem = await _context.PersonalDetails.FindAsync(1);
            Assert.IsNull(deletedItem);
        }

        [Test]
        public async Task DeletePersonalDetails_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeletePersonalDetails(100);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Add more test cases to cover edge cases, validation scenarios, and error handling
    }
}