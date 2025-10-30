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

public class StreetControllerTest
{
    private readonly ApiDbContext _dbContext;
    private readonly StreetController _streetController;

    public StreetControllerTest()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApiDbContext(options);

        // Initialize repository
        IStreetRepository repository = new StreetRepository(_dbContext);

        // Initialize service
        var service = new StreetService(_dbContext, repository);

        // Initialize controller
        _streetController = new StreetController(service);

        SetupInitialData();
    }

    private void SetupInitialData()
    {
        if (!_dbContext.Streets.Any())
        {
            var reader = new WKTReader();

            _dbContext.Streets.AddRange(
                new Street
                {
                    Id = 1,
                    Name = "Kaiserstraße",
                    Description = "Main commercial and pedestrian street in the city center",
                    UpdateAt = DateTime.Now,
                    Capacity = 800,
                    Geometry = (LineString)reader.Read(
                        "LINESTRING(8.40379 49.00939, 8.40410 49.00950, 8.40445 49.00962, 8.40486 49.00974)")
                },
                new Street
                {
                    Id = 2,
                    Name = "Karl-Friedrich-Straße",
                    Description = "Historic street near the palace",
                    UpdateAt = DateTime.Now,
                    Capacity = 400,
                    Geometry = (LineString)reader.Read(
                        "LINESTRING(8.40345 49.00871, 8.40365 49.00882, 8.40385 49.00893, 8.40405 49.00904, 8.40420 49.00915)")
                },
                new Street
                {
                    Id = 3,
                    Name = "Waldstraße",
                    Description = "Street with traditional architecture and shops",
                    UpdateAt = DateTime.Now,
                    Capacity = 500,
                    Geometry = (LineString)reader.Read(
                        "LINESTRING(8.40628 49.01021, 8.40655 49.01032, 8.40682 49.01043, 8.40710 49.01054, 8.40750 49.01065)")
                },
                new Street
                {
                    Id = 4,
                    Name = "Sophienstraße",
                    Description = "Quiet street with residential atmosphere",
                    UpdateAt = DateTime.Now,
                    Capacity = 300,
                    Geometry =
                        (LineString)reader.Read(
                            "LINESTRING(8.40752 49.01066, 8.40775 49.01075, 8.40798 49.01082, 8.40825 49.01092, 8.40861 49.01105)")
                },
                new Street
                {
                    Id = 5,
                    Name = "Douglasstraße",
                    Description = "Commercial street near main train station",
                    UpdateAt = DateTime.Now,
                    Capacity = 200,
                    Geometry = (LineString)reader.Read(
                        "LINESTRING(8.40079 49.00970, 8.40105 49.00978, 8.40132 49.00986, 8.40158 49.00996, 8.40189 49.01010)")
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
        var result = await _streetController.GetAll(null, null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<StreetResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
        Assert.Equal(5, pagedResult.Data.Count());
    }

    [Fact]
    public async Task GetAll_WithNameFilter()
    {
        // Arrange
        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _streetController.GetAll("Douglas", null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<StreetResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
    }

    [Fact]
    public async Task GetAll_WithDescriptionFilter()
    {
        // Arrange
        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _streetController.GetAll(null, "with", false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<StreetResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
    }

    [Fact]
    public async Task GetAll_WithEmptyDatabase()
    {
        // Arrange
        _dbContext.Streets.RemoveRange(_dbContext.Streets);
        await _dbContext.SaveChangesAsync();

        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _streetController.GetAll(null, null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<StreetResDto>>(okResult.Value);
        Assert.Empty(pagedResult.Data);
    }

    [Fact]
    public async Task GetAll_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<IStreetService>();
        var exceptionMessage = "Database connection failed";

        mockService.Setup(s => s.GetAll(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<Pageable>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new StreetController(mockService.Object);
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
        var result = await _streetController.GetById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<StreetResDto>(okResult.Value);
    }

    [Fact]
    public async Task GetById_WithInvalidId()
    {
        // Arrange
        var id = 100;

        // Execute
        var result = await _streetController.GetById(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WithNegativeId()
    {
        // Arrange
        var negativeId = -1;

        // Execute
        var result = await _streetController.GetById(negativeId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<IStreetService>();
        var id = 1;
        var exceptionMessage = "Invalid operation";

        mockService.Setup(s => s.GetById(
            It.IsAny<int>()
        )).ThrowsAsync(new Exception(exceptionMessage));
        var controller = new StreetController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.GetById(id));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // POST Add Street Tests
    [Fact]
    public async Task AddStreet_WithValidData()
    {
        // Arrange
        var coordinates = new[]
        {
            new Coordinate(-70.67, -33.45),
            new Coordinate(-70.68, -33.46),
            new Coordinate(-70.69, -33.47)
        };

        var dto = new StreetReqDto
        {
            Name = "Test",
            Description = "Test Description",
            Capacity = 100,
            Geometry = new LineString(coordinates)
        };

        // Execute
        var result = await _streetController.Add(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<StreetResDto>(okResult.Value);
        Assert.Equal("Test", dto.Name);
        Assert.Equal("Test Description", dto.Description);

        // Check if db is updated
        var objInDb = _dbContext.Streets.FirstOrDefault(r => r.Name == "Test");
        Assert.NotNull(objInDb);
    }

    [Fact]
    public async Task AddStreet_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<IStreetService>();
        var dto = new StreetReqDto
        {
            Name = "Test",
            Description = "Test Description"
        };

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Add(
            It.IsAny<StreetReqDto>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new StreetController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.Add(dto));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // PUT Upd Street Tests
    [Fact]
    public async Task UpdStreet_WhenServiceThrowsException()
    {
        // Arrange
        var id = 1;
        var mockService = new Mock<IStreetService>();
        var dto = new StreetReqDto
        {
            Name = "Test",
            Description = "Test Description"
        };

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Upd(
            It.IsAny<StreetReqDto>(),
            It.IsAny<int>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new StreetController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.Upd(dto, id));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // DELETE Del Street Tests
    [Fact]
    public async Task DelStreet_WhenServiceThrowsException()
    {
        // Arrange
        var id = 1;
        var mockService = new Mock<IStreetService>();

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Del(
            It.IsAny<int>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new StreetController(mockService.Object);

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