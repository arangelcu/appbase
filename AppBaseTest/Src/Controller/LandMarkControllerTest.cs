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

public class LandMarkControllerTest
{
    private readonly ApiDbContext _dbContext;
    private readonly LandMarkController _landMarkController;

    public LandMarkControllerTest()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApiDbContext(options);

        // Initialize repository
        ILandMarkRepository repository = new LandMarkRepository(_dbContext);

        // Initialize service
        var service = new LandMarkService(_dbContext, repository);

        // Initialize controller
        _landMarkController = new LandMarkController(service);

        SetupInitialData();
    }

    private void SetupInitialData()
    {
        if (!_dbContext.LandMarks.Any())
        {
            var reader = new WKTReader();

            _dbContext.LandMarks.AddRange(
                new LandMark
                {
                    Id = 1,
                    Name = "Karlsruhe Palace",
                    Description = "Historic palace built in 1715, now houses the Baden State Museum",
                    UpdateAt = DateTime.Now,
                    Geometry = (Point)reader.Read(
                        "POINT(8.40428 49.01389)")
                },
                new LandMark
                {
                    Id = 2,
                    Name = "Karlsruhe Pyramid",
                    Description = "Landmark pyramid in the market square marking the tomb of city founder",
                    UpdateAt = DateTime.Now,
                    Geometry = (Point)reader.Read(
                        "POINT(8.40435 49.00935)")
                },
                new LandMark
                {
                    Id = 3,
                    Name = "Karlsruhe Hauptbahnhof",
                    Description = "Main railway station with historic building and modern shopping center",
                    UpdateAt = DateTime.Now,
                    Geometry = (Point)reader.Read(
                        "POINT(8.40083 49.00972)")
                },
                new LandMark
                {
                    Id = 4,
                    Name = "Baden State Theater",
                    Description = "Premiere theater for opera, ballet, and drama performances",
                    UpdateAt = DateTime.Now,
                    Geometry =
                        (Point)reader.Read(
                            "POINT(8.40389 49.00889)")
                },
                new LandMark
                {
                    Id = 5,
                    Name = "ZKM Center for Art and Media",
                    Description = "World-renowned museum for interactive and media arts",
                    UpdateAt = DateTime.Now,
                    Geometry = (Point)reader.Read(
                        "POINT(8.38583 49.00972)")
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
        var result = await _landMarkController.GetAll(null, null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<LandMarkResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
        Assert.Equal(5, pagedResult.Data.Count());
    }

    [Fact]
    public async Task GetAll_WithNameFilter()
    {
        // Arrange
        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _landMarkController.GetAll("Karlsruhe", null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<LandMarkResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
    }

    [Fact]
    public async Task GetAll_WithDescriptionFilter()
    {
        // Arrange
        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _landMarkController.GetAll(null, "opera", false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<LandMarkResDto>>(okResult.Value);
        Assert.True(pagedResult.Data.Count > 0);
    }

    [Fact]
    public async Task GetAll_WithEmptyDatabase()
    {
        // Arrange
        _dbContext.LandMarks.RemoveRange(_dbContext.LandMarks);
        await _dbContext.SaveChangesAsync();

        var pageable = new Pageable { Page = 0, PageSize = 10 };

        // Execute
        var result = await _landMarkController.GetAll(null, null, false, pageable);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var pagedResult = Assert.IsType<PagedResult<LandMarkResDto>>(okResult.Value);
        Assert.Empty(pagedResult.Data);
    }

    [Fact]
    public async Task GetAll_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<ILandMarkService>();
        var exceptionMessage = "Database connection failed";

        mockService.Setup(s => s.GetAll(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<Pageable>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new LandMarkController(mockService.Object);
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
        var result = await _landMarkController.GetById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<LandMarkResDto>(okResult.Value);
    }

    [Fact]
    public async Task GetById_WithInvalidId()
    {
        // Arrange
        var id = 100;

        // Execute
        var result = await _landMarkController.GetById(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WithNegativeId()
    {
        // Arrange
        var negativeId = -1;

        // Execute
        var result = await _landMarkController.GetById(negativeId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<ILandMarkService>();
        var id = 1;
        var exceptionMessage = "Invalid operation";

        mockService.Setup(s => s.GetById(
            It.IsAny<int>()
        )).ThrowsAsync(new Exception(exceptionMessage));
        var controller = new LandMarkController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.GetById(id));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // POST Add LandMark Tests
    [Fact]
    public async Task AddLandMark_WithValidData()
    {
        // Arrange
        var dto = new LandMarkReqDto
        {
            Name = "Test",
            Description = "Test Description",
            Geometry = new Point(new Coordinate(-70.69, -33.47))
        };

        // Execute
        var result = await _landMarkController.Add(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<LandMarkResDto>(okResult.Value);
        Assert.Equal("Test", dto.Name);
        Assert.Equal("Test Description", dto.Description);

        // Check if db is updated
        var objInDb = _dbContext.LandMarks.FirstOrDefault(r => r.Name == "Test");
        Assert.NotNull(objInDb);
    }

    [Fact]
    public async Task AddLandMark_WhenServiceThrowsException()
    {
        // Arrange
        var mockService = new Mock<ILandMarkService>();
        var dto = new LandMarkReqDto
        {
            Name = "Test",
            Description = "Test Description"
        };

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Add(
            It.IsAny<LandMarkReqDto>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new LandMarkController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.Add(dto));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // PUT Update LandMark Tests
    [Fact]
    public async Task UpdLandMark_WhenServiceThrowsException()
    {
        // Arrange
        var id = 1;
        var mockService = new Mock<ILandMarkService>();
        var dto = new LandMarkReqDto
        {
            Name = "Test",
            Description = "Test Description"
        };

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Update(
            It.IsAny<int>(),
            It.IsAny<LandMarkReqDto>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new LandMarkController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.Update(id, dto));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // DELETE Delete LandMark Tests
    [Fact]
    public async Task DelLandMark_WhenServiceThrowsException()
    {
        // Arrange
        var id = 1;
        var mockService = new Mock<ILandMarkService>();

        var exceptionMessage = "Invalid operation";
        mockService.Setup(s => s.Delete(
            It.IsAny<int>()
        )).ThrowsAsync(new Exception(exceptionMessage));

        var controller = new LandMarkController(mockService.Object);

        // Execution
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            controller.Delete(id));

        // Assert
        Assert.Equal(exceptionMessage, exception.Message);
    }

    // DISPOSE
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}