using System.Linq;
using Dima.Api.Data;
using Dima.Api.Handlers;
using Dima.Core.Enums;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Tests.Handlers
{
    public class TransactionHandlerTests
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _appDbContext;
        private readonly TransactionHandler _handler;
        private readonly string _userId = Guid.NewGuid().ToString();
        private readonly Category _category = null!;

        public TransactionHandlerTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _appDbContext = new AppDbContext(_options);
            _handler = new TransactionHandler(_appDbContext);

            _category = new Category
            {
                UserId = _userId,
                Title = "Title original",
                Description = "Description original"
            };
        }

        private async Task CreateCategory()
        {
            await _appDbContext.Categories.AddAsync(_category);
            await _appDbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnSuccessResponse()
        {
            // Arrange
            await CreateCategory();

            var createTransactionRequest = new CreateTransactionRequest
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "Transaction Test",
                Amount = 1.0m,
                Type = ETransactionType.Withdraw,
                PaidOrReceivedAt = DateTime.Now,
                CategoryId = _category.Id
            };

            // Act
            var result = await _handler.CreateAsync(createTransactionRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(createTransactionRequest.UserId, result.Data.UserId);
            Assert.Equal(createTransactionRequest.Title, result.Data.Title);
            Assert.Equal(createTransactionRequest.Amount, result.Data.Amount);
            Assert.Equal(createTransactionRequest.Type, result.Data.Type);
            Assert.Equal(createTransactionRequest.PaidOrReceivedAt, result.Data.PaidOrReceivedAt);
            Assert.Equal(createTransactionRequest.CategoryId, result.Data.CategoryId);
        }

        [Fact]
        public async Task UpdateAsync_TransactionNotExists_ReturnsNotFoundResponse()
        {
            // Arrange
            await CreateCategory();
            var updateTransactionRequest = new UpdateTransactionRequest
            {
                Id = 10000,
                UserId = Guid.NewGuid().ToString(),
                Title = "Transaction Test",
                Amount = 1.0m,
                Type = ETransactionType.Withdraw,
                PaidOrReceivedAt = DateTime.Now,
                CategoryId = _category.Id
            };

            // Act
            var result = await _handler.UpdateAsync(updateTransactionRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_TranscationExists_ReturnsSuccessResponse()
        {
            // Arrange
            await CreateCategory();
            var transaction = new Transaction
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "Transaction original",
                Amount = 1.0m,
                Type = ETransactionType.Withdraw,
                PaidOrReceivedAt = DateTime.Now.Date,
                CategoryId = _category.Id
            };

            await _appDbContext.Transactions.AddAsync(transaction);
            await _appDbContext.SaveChangesAsync();

            var updateTransactionRequest = new UpdateTransactionRequest
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                Title = "Transaction new",
                Amount = 10.0m,
                Type = ETransactionType.Deposit,
                PaidOrReceivedAt = DateTime.Now.Date,
                CategoryId = _category.Id
            };

            // Act
            var result = await _handler.UpdateAsync(updateTransactionRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(updateTransactionRequest.Title, result.Data.Title);
            Assert.Equal(updateTransactionRequest.Amount, result.Data.Amount);
            Assert.Equal(updateTransactionRequest.Type, result.Data.Type);
            Assert.Equal(updateTransactionRequest.PaidOrReceivedAt, result.Data.PaidOrReceivedAt);
        }

        [Fact]
        public async Task DeleteAsync_TransactionNotExists_ReturnsNotFoundResponse()
        {
            // Arrange
            var deleteTransactionRequest = new DeleteTransactionRequest
            {
                Id = 10000,
                UserId = Guid.NewGuid().ToString()
            };

            // Act
            var result = await _handler.DeleteAsync(deleteTransactionRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_TransactionExists_ReturnsSuccessResponse()
        {
            // Arrange
            await CreateCategory();
            var transaction = new Transaction
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "Transaction original",
                Amount = 1.0m,
                Type = ETransactionType.Withdraw,
                PaidOrReceivedAt = DateTime.Now.Date,
                CategoryId = _category.Id
            };

            await _appDbContext.Transactions.AddAsync(transaction);
            await _appDbContext.SaveChangesAsync();

            var deleteTransactionRequest = new DeleteTransactionRequest
            {
                Id = transaction.Id,
                UserId = transaction.UserId
            };

            // Act
            var result = await _handler.DeleteAsync(deleteTransactionRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_TransactionExists_ReturnsSuccessResponse()
        {
            // Arrange
            await CreateCategory();
            var transaction = new Transaction
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "Transaction original",
                Amount = 1.0m,
                Type = ETransactionType.Withdraw,
                PaidOrReceivedAt = DateTime.Now.Date,
                CategoryId = _category.Id
            };

            await _appDbContext.Transactions.AddAsync(transaction);
            await _appDbContext.SaveChangesAsync();

            var getTransactionByIdRequest = new GetTransactionByIdRequest
            {
                Id = transaction.Id,
                UserId = transaction.UserId
            };

            // Act
            var result = await _handler.GetByIdAsync(getTransactionByIdRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(transaction.UserId, result.Data.UserId);
            Assert.Equal(transaction.Title, result.Data.Title);
            Assert.Equal(transaction.Amount, result.Data.Amount);
            Assert.Equal(transaction.Type, result.Data.Type);
            Assert.Equal(transaction.PaidOrReceivedAt, result.Data.PaidOrReceivedAt);
            Assert.Equal(transaction.CategoryId, result.Data.CategoryId);
        }

        [Fact]
        public async Task GetPerioddAsync_TransactionExists_ReturnsSuccessResponse()
        {
            // Arrange
            await CreateCategory();

            var userId = Guid.NewGuid().ToString();
            var transactions = new List<Transaction>
            {
                new() {
                    UserId = userId,
                    Title = "Transaction filtered",
                    Amount = 1.0m,
                    Type = ETransactionType.Withdraw,
                    PaidOrReceivedAt = DateTime.Now.AddDays(-1).Date,
                    CategoryId = _category.Id
                },
                new() {
                    UserId = userId,
                    Title = "Transaction not filtered",
                    Amount = 5.0m,
                    Type = ETransactionType.Withdraw,
                    PaidOrReceivedAt = DateTime.Now.AddDays(-3).Date,
                    CategoryId = _category.Id
                }
            };

            await _appDbContext.Transactions.AddRangeAsync(transactions);
            await _appDbContext.SaveChangesAsync();

            var getTransactionByPeriodRequest = new GetTransactionsByPeriodRequest
            {
                UserId = userId,
                StartDate = DateTime.Now.AddDays(-2).Date,
                EndDate = DateTime.Now.Date
            };

            // Act
            var result = await _handler.GetByPeriodAsync(getTransactionByPeriodRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(result.Data.Single().UserId, transactions.First().UserId);
            Assert.Equal(result.Data.Single().Title, transactions.First().Title);
            Assert.Equal(result.Data.Single().Amount, transactions.First().Amount);
            Assert.Equal(result.Data.Single().Type, transactions.First().Type);
            Assert.Equal(result.Data.Single().PaidOrReceivedAt, transactions.First().PaidOrReceivedAt);
            Assert.Equal(result.Data.Single().CategoryId, transactions.First().CategoryId);
        }
    }
}
