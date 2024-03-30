using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend.Tests
{
    [TestFixture]
    public class SignupLoginControllerTests
    {
        private SignupLoginController _controller;
        private DbContextOptions<AppDbContext> _dbContextOptions;
        private IOptions<JwtSettings> _jwtSettings;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;

            using (var context = new AppDbContext(_dbContextOptions))
            {
                context.Users.Add(new Users { UserId = 1, Email = "test@example.com", Password = "password" });
                context.SaveChanges();
            }

            var jwtSettings = new JwtSettings
            {
                Key = "PrayPrayPrayPrayPrayPrayPrayPray", // Ensure that the key size matches 256 bits (32 bytes)
                Secret = "RelevantzRelevantzRelevantzRelevantz",
                Issuer = "Relevantz",
                Audience = "Trainee"
            };

            _jwtSettings = Options.Create(jwtSettings);

            // Mock IOptions<JwtSettings>
            var jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            jwtSettingsMock.Setup(x => x.Value).Returns(jwtSettings);

            using (var context = new AppDbContext(_dbContextOptions))
            {
                _controller = new SignupLoginController(context, jwtSettingsMock.Object);
            }

            Assert.IsNotNull(_controller, "Controller instance is null.");
            Assert.IsNotNull(_jwtSettings, "JwtSettings instance is null.");
        }


        [Test]
        public void Login_InvalidUser_ReturnsNotFound()
        {
            // Arrange
            var loginModel = new Login { Email = "nonexistent@example.com", Password = "password" };

            // Create a new context instance
            using (var context = new AppDbContext(_dbContextOptions))
            {
                // Create the controller using the new context instance
                var controller = new SignupLoginController(context, _jwtSettings);

                // Act
                var result = controller.Login(loginModel) as ObjectResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(404, result.StatusCode);
                Assert.AreEqual("Email or Password Not Found", result.Value);
            }
        }


        [Test]
        public void Signup_ValidUser_ReturnsCreatedAtAction()
        {
            // Arrange
            var newUser = new Users { UserId = 2, Email = "newuser@example.com", Password = "password" };

            // Act
            using (var context = new AppDbContext(_dbContextOptions)) // Create a new instance of AppDbContext
            {
                var controller = new SignupLoginController(context, _jwtSettings);
                var result = controller.Signup(newUser) as CreatedAtActionResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(nameof(SignupLoginController.Signup), result.ActionName);
                Assert.AreEqual(201, result.StatusCode);
                Assert.AreEqual(newUser, result.Value);
            }
        }

        [Test]
        public void GenerateOTP_ValidEmail_ReturnsOk()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var result = _controller.GenerateOTP(email) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);

            // Convert the result value to string
            var sOTP = result.Value.ToString();

            Assert.IsNotNull(sOTP);
            Assert.IsInstanceOf<string>(sOTP);
        }


        [TearDown]
        public void TearDown()
        {
            // Dispose of the DbContext instance after all tests are complete
            using (var context = new AppDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}
