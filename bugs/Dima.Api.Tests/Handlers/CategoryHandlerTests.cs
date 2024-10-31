using Dima.Api.Data;
using Dima.Api.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Tests.Handlers
{
    public class CategoryHandlerTests
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _appDbContext;
        private readonly CategoryHandler _handler;

        public CategoryHandlerTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _appDbContext = new AppDbContext(_options);
            _handler = new CategoryHandler(_appDbContext);
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnSuccessResponse()
        {
            // Arrange
            var createCategoryRequest = new CreateCategoryRequest
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "Category Test",
                Description = "Description Test"
            };

            // Act
            var result = await _handler.CreateAsync(createCategoryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(createCategoryRequest.UserId, result.Data.UserId);
            Assert.Equal(createCategoryRequest.Title, result.Data.Title);
            Assert.Equal(createCategoryRequest.Description, result.Data.Description);
        }

        [Fact]
        public async Task UpdateAsync_CategoryNotExists_ReturnsNotFoundResponse()
        {
            // Arrange
            var updateCategoryRequest = new UpdateCategoryRequest
            {
                Id = 10000,
                UserId = Guid.NewGuid().ToString(),
                Title = "Category Test",
                Description = "Description Test"
            };

            // Act
            var result = await _handler.UpdateAsync(updateCategoryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_CategoryExists_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var category = new Category
            {
                UserId = userId,
                Title = "Title original",
                Description = "Description original"
            };

            await _appDbContext.Categories.AddAsync(category);
            await _appDbContext.SaveChangesAsync();

            var updateCategoryRequest = new UpdateCategoryRequest
            {
                Id = category.Id,
                UserId = userId,
                Title = "New Title",
                Description = "New Description"
            };

            // Act
            var result = await _handler.UpdateAsync(updateCategoryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(updateCategoryRequest.Title, result.Data.Title);
            Assert.Equal(updateCategoryRequest.Description, result.Data.Description);
        }

        [Fact]
        public async Task DeleteAsync_CategoryNotExists_ReturnsNotFoundResponse()
        {
            // Arrange
            var deleteCategoryRequest = new DeleteCategoryRequest
            {
                Id = 10000,
                UserId = Guid.NewGuid().ToString()
            };

            // Act
            var result = await _handler.DeleteAsync(deleteCategoryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_CategoryExists_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var category = new Category
            {
                UserId = userId,
                Title = "Title original",
                Description = "Description original"
            };

            await _appDbContext.Categories.AddAsync(category);
            await _appDbContext.SaveChangesAsync();

            var deleteCategoryRequest = new DeleteCategoryRequest
            {
                Id = category.Id,
                UserId = userId
            };

            // Act
            var result = await _handler.DeleteAsync(deleteCategoryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_CategoryExists_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var category = new Category
            {
                UserId = userId,
                Title = "Title original",
                Description = "Description original"
            };

            await _appDbContext.Categories.AddAsync(category);
            await _appDbContext.SaveChangesAsync();

            var getCategoryByIdRequest = new GetCategoryByIdRequest
            {
                Id = category.Id,
                UserId = userId
            };

            // Act
            var result = await _handler.GetByIdAsync(getCategoryByIdRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(category.Title, result.Data.Title);
            Assert.Equal(category.Description, result.Data.Description);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var categories = new List<Category>
            {
                new(){ UserId = userId, Title = "Category number one" },
                new(){ UserId = userId, Title = "Category number two" },
                new(){ UserId = userId, Title = "Category number three" }
            };

            await _appDbContext.Categories.AddRangeAsync(categories);
            await _appDbContext.SaveChangesAsync();

            var getAllCategoriesRequest = new GetAllCategoriesRequest
            {
                UserId = userId
            };

            // Act
            var result = await _handler.GetAllAsync(getAllCategoriesRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Data.Count);
            Assert.Collection(result.Data,
                category1 =>
                {
                    Assert.Equal("Category number one", category1.Title);
                },
                category2 =>
                {
                    Assert.Equal("Category number three", category2.Title);
                },
                category3 =>
                {
                    Assert.Equal("Category number two", category3.Title);
                });
        }
    }
}
