using Moq;
using StockFlow.Application.Inventory;
using StockFlow.Application.Inventory.Exceptions;

namespace StockFlow.Application.Tests.Inventory;

public class InventoryServiceTests
{
    [Fact]
    public async Task StockOutAsync_WhenInventoryIsAvailable_ShouldCommit()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var inventoryRepository = new Mock<IInventoryRepository>();
        var movementRepository = new Mock<IInventoryMovementRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        inventoryRepository
            .Setup(x => x.StockOutAsync(
                productId,
                It.IsAny<StockOutRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var service = new InventoryService(
            inventoryRepository.Object,
            movementRepository.Object,
            unitOfWork.Object);

        var request = new StockOutRequest(5);

        // Act
        await service.StockOutAsync(
            productId,
            request,
            CancellationToken.None);

        await service.GetAllAsync(CancellationToken.None);

        // Assert
        unitOfWork.Verify(
            x => x.BeginTransactionAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);

        movementRepository.Verify(
            x => x.AddAsync(
                It.Is<InventoryMovementDraft>(m =>
                    m.ProductId == productId &&
                    m.Type == InventoryMovementType.StockOut &&
                    m.Quantity == 5),
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            x => x.CommitAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task StockOutAsync_WhenInventoryIsInsufficient_ShouldRollback()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var inventoryRepository = new Mock<IInventoryRepository>();
        var movementRepository = new Mock<IInventoryMovementRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        inventoryRepository
            .Setup(x => x.StockOutAsync(
                productId,
                It.IsAny<StockOutRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var service = new InventoryService(
            inventoryRepository.Object,
            movementRepository.Object,
            unitOfWork.Object);

        var request = new StockOutRequest(5);

        // Act
        await Assert.ThrowsAsync<InsufficientInventoryException>(
            () => service.StockOutAsync(
                productId,
                request,
                CancellationToken.None));

        // Assert
        unitOfWork.Verify(
            x => x.RollbackAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);

        unitOfWork.Verify(
            x => x.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        movementRepository.Verify(
            x => x.AddAsync(
                It.IsAny<InventoryMovementDraft>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task StockOutAsync_WhenAddingMovementFails_ShouldRollback()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var inventoryRepository = new Mock<IInventoryRepository>();
        var movementRepository = new Mock<IInventoryMovementRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        inventoryRepository
            .Setup(x => x.StockOutAsync(
                productId,
                It.IsAny<StockOutRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        movementRepository
            .Setup(x => x.AddAsync(
                It.IsAny<InventoryMovementDraft>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var service = new InventoryService(
            inventoryRepository.Object,
            movementRepository.Object,
            unitOfWork.Object);

        var request = new StockOutRequest(5);

        // Act
        await Assert.ThrowsAsync<Exception>(
            () => service.StockOutAsync(
                productId,
                request,
                CancellationToken.None));

        // Assert
        unitOfWork.Verify(
            x => x.RollbackAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);

        unitOfWork.Verify(
            x => x.CommitAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}