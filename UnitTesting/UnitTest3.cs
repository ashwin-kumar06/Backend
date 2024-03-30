using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Moq;
using static Mysqlx.Expect.Open.Types;

[TestFixture]
public class ProductsControllerTests
{
    private ProductsController _controller;
    private AppDbContext _context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "test_db")
            .Options;
        _context = new AppDbContext(options);

        // Seed some products into the in-memory database for testing
        _context.Products.AddRange(new List<Products>
    {
        new Products { ProductId = 1, Title = "Product 1", Description = "Description 1" },
        new Products { ProductId = 2, Title = "Product 2", Description = "Description 2" }
    });
        _context.SaveChanges();

        var hostingEnvironmentMock = new Mock<IWebHostEnvironment>();
        var configurationMock = new Mock<IConfiguration>();

        _controller = new ProductsController(_context, hostingEnvironmentMock.Object, configurationMock.Object);
    }


    [Test]
    public async Task GetProducts_ReturnsNoContentWhenNoProducts()
    {
        // Arrange: Clear the database to simulate no products
        _context.Products.RemoveRange(_context.Products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetProducts();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<NoContentResult>(result.Result);
    }



    [Test]
    public async Task DeleteProducts_ReturnsNoContentWhenProductDeleted()
    {
        // Arrange
        var product = new Products { ProductId = 1, Title = "Product 1", Description = "Description 1" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteProducts(product.ProductId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<NoContentResult>(result);
    }

    [Test]
    public async Task DeleteProducts_ReturnsNotFoundWhenProductNotFound()
    {
        // Arrange
        var productId = 999;

        // Act
        var result = await _controller.DeleteProducts(productId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
    }
}
