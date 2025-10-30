using AppBase.Config.Data;
using AppBase.Controller;
using AppBase.Model.Dto;
using AppBase.Model.Entity;
using AppBase.Model.Repositories;
using AppBase.Services;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace AppBaseTest.Controller;

public class SquareControllerTest
{
    private readonly ApiDbContext _dbContext;
    private readonly SquareController _squareController;

    public SquareControllerTest()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApiDbContext(options);

        // Initialize repository
        ISquareRepository repository = new SquareRepository(_dbContext);

        // Initialize service
        var service = new SquareService(_dbContext, repository);

        // Initialize controller
        _squareController = new SquareController(service);

        SetupInitialData();
    }

    private void SetupInitialData()
    {
        if (!_dbContext.Squares.Any())
        {
            var reader = new WKTReader();

            _dbContext.Squares.AddRange(
                new Square
                {
                    Id = 1,
                    Name = "Marktplatz",
                    Description = "Main market square with town hall and church",
                    UpdateAt = DateTime.Now,
                    Capacity = 800,
                    Geometry = (Polygon)reader.Read(
                        "POLYGON((8.40393 49.00949, 8.40415 49.00948, 8.40435 49.00947, 8.40455 49.00946, 8.40476 49.00948, 8.40475 49.00935, 8.40474 49.00922, 8.40473 49.00910, 8.40465 49.00911, 8.40445 49.00912, 8.40425 49.00913, 8.40405 49.00914, 8.40392 49.00911, 8.40393 49.00925, 8.40393 49.00938, 8.40393 49.00949))")
                },
                new Square
                {
                    Id = 2,
                    Name = "Schlossplatz",
                    Description = "Palace square with gardens and museums",
                    UpdateAt = DateTime.Now,
                    Capacity = 400,
                    Geometry = (Polygon)reader.Read(
                        "POLYGON((8.40429 49.01339, 8.40445 49.01340, 8.40465 49.01341, 8.40485 49.01342, 8.40505 49.01343, 8.40525 49.01340, 8.40524 49.01325, 8.40523 49.01310, 8.40522 49.01295, 8.40521 49.01290, 8.40505 49.01291, 8.40485 49.01292, 8.40465 49.01293, 8.40445 49.01294, 8.40429 49.01290, 8.40430 49.01305, 8.40431 49.01320, 8.40432 49.01335, 8.40429 49.01339))")
                },
                new Square
                {
                    Id = 3,
                    Name = "Friedrichsplatz",
                    Description = "Square with fountains and cultural events",
                    UpdateAt = DateTime.Now,
                    Capacity = 500,
                    Geometry = (Polygon)reader.Read(
                        "POLYGON((8.40079 49.00859, 8.40100 49.00858, 8.40120 49.00857, 8.40140 49.00856, 8.40165 49.00859, 8.40164 49.00845, 8.40163 49.00832, 8.40162 49.00820, 8.40140 49.00821, 8.40120 49.00822, 8.40100 49.00823, 8.40079 49.00820, 8.40080 49.00830, 8.40081 49.00840, 8.40082 49.00850, 8.40079 49.00859))")
                },
                new Square
                {
                    Id = 4,
                    Name = "Rondellplatz",
                    Description = "Historic circular square in the city center",
                    UpdateAt = DateTime.Now,
                    Capacity = 300,
                    Geometry =
                        (Polygon)reader.Read(
                            "POLYGON((8.40300 49.01100, 8.40312 49.01102, 8.40325 49.01103, 8.40338 49.01104, 8.40350 49.01100, 8.40348 49.01085, 8.40346 49.01070, 8.40344 49.01055, 8.40342 49.01050, 8.40330 49.01051, 8.40315 49.01052, 8.40305 49.01053, 8.40300 49.01050, 8.40301 49.01065, 8.40302 49.01080, 8.40303 49.01095, 8.40300 49.01100))")
                },
                new Square
                {
                    Id = 5,
                    Name = "Ludwigsplatz",
                    Description = "Square with weekly market and surrounding shops",
                    UpdateAt = DateTime.Now,
                    Capacity = 200,
                    Geometry = (Polygon)reader.Read(
                        "POLYGON((8.40600 49.00900, 8.40625 49.00898, 8.40650 49.00896, 8.40675 49.00894, 8.40700 49.00900, 8.40698 49.00880, 8.40696 49.00860, 8.40694 49.00850, 8.40675 49.00852, 8.40650 49.00854, 8.40625 49.00856, 8.40600 49.00850, 8.40602 49.00865, 8.40604 49.00880, 8.40606 49.00895, 8.40600 49.00900))")
                }
            );
            _dbContext.SaveChanges();
        }
    }

    [Fact]
    public async Task GetAll_ShouldReturnAll()
    {
        // Arrange
        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _squareController.GetAll(null, null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<SquareResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
        Assert.Equal(5, pagedResult.Data.Count());
    }

    [Fact]
    public async Task GetAll_WithNameFilter()
    {
        // Arrange
        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _squareController.GetAll("Ludwigsplatz", null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<SquareResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
    }

    [Fact]
    public async Task GetAll_WithDescriptionFilter()
    {
        // Arrange
        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _squareController.GetAll(null, "and", false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<SquareResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
    }

    [Fact]
    public async Task GetAll_WithEmptyDatabase()
    {
        // Arrange
        _dbContext.Squares.RemoveRange(_dbContext.Squares);
        await _dbContext.SaveChangesAsync();

        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _squareController.GetAll(null, null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<SquareResDto>>(okResult.Value);
        Assert.Empty(pagedResult.Data);
    }

    [Fact]
    public async Task GetAll_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<ISquareService>();
        var exceptionMessage = "Database connection failed";

        mockService.Setup(s => s.GetAll(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<Pageable>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new SquareController(mockService.Object);
        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.GetAll(null, null, null, pageable));

        Assert.Equal(exceptionMessage, exception.Message);
    }

    // GET ById Tests
    [Fact]
    public async Task GetById_WithValidId()
    {
        // Arrange
        var id = 1;

        // Execute
        var result = await _squareController.GetById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<SquareResDto>(okResult.Value);
    }

    [Fact]
    public async Task GetById_WithInvalidId()
    {
        // Arrange
        var id = 100;

        // Execute
        var result = await _squareController.GetById(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WithNegativeId()
    {
        // Arrange
        var negativeId = -1;

        // Execute
        var result = await _squareController.GetById(negativeId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<ISquareService>();
        var id = 1;
        var exceptionMessage = "Invalid operation";

        mockService.Setup(s => s.GetById(
            It.IsAny<int>()
        )).ThrowsAsync(new Exception(exceptionMessage));
        var controller = new SquareController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.GetById(id));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // POST Add Square Tests
    [Fact]
    public async Task AddSquare_WithValidData()
    {
        // Arrange
        var coordinates = new[]
        {
            new Coordinate(-70.67, -33.45),
            new Coordinate(-70.68, -33.45),
            new Coordinate(-70.68, -33.46),
            new Coordinate(-70.67, -33.46),
            new Coordinate(-70.67, -33.45)
        };

        var linearRing = new LinearRing(coordinates);

        var dto = new SquareReqDto
        {
            Name = "Test",
            Description = "Test Description",
            Capacity = 100,
            Geometry = new Polygon(linearRing)
        };

        // Execute
        var result = await _squareController.Add(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<SquareResDto>(okResult.Value);
        Assert.Equal("Test", dto.Name);
        Assert.Equal("Test Description", dto.Description);

        // Check if db is updated
        var objInDb = _dbContext.Squares.FirstOrDefault(r => r.Name == "Test");
        Assert.NotNull(objInDb);
    }

    [Fact]
    public async Task AddSquare_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<ISquareService>();
        var dto = new SquareReqDto
        {
            Name = "Test",
            Description = "Test Description"
        };

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Add(
            It.IsAny<SquareReqDto>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new SquareController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.Add(dto));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // PUT Upd Square Tests
    [Fact]
    public async Task UpdSquare_WhenServiceThrowsException()
    {
        // Arrange
        var id = 1;
        var mockService = new Mock<ISquareService>();
        var dto = new SquareReqDto
        {
            Name = "Test",
            Description = "Test Description"
        };

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Upd(
            It.IsAny<SquareReqDto>(),
            It.IsAny<int>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new SquareController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.Upd(dto, id));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // DELETE Del Square Tests
    [Fact]
    public async Task DelSquare_WhenServiceThrowsException()
    {
        // Arrange
        var id = 1;
        var mockService = new Mock<ISquareService>();

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Del(
            It.IsAny<int>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new SquareController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.Del(id));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // DISPOSE
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}