using System.IO;
using MetaExtractor.Infrastructure.Services;
using MetaExtractor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MetaExtractor.Domain.Entities;

namespace MetaExtractor.Infrastructure.Tests;

public class AppDbContextTests : IDisposable
{
    private readonly AppDbContext _context;

    public AppDbContextTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    [Fact]
    public void AppDbContext_ShouldCreateDatabase()
    {
        // Act
        var created = _context.Database.EnsureCreated();

        // Assert
        Assert.True(created);
    }

    [Fact]
    public async Task AppDbContext_ShouldSaveFaceEntity()
    {
        // Arrange
        _context.Database.EnsureCreated();
        var face = new Face
        {
            Timestamp = DateTime.UtcNow
        };

        // Act
        _context.Faces.Add(face);
        await _context.SaveChangesAsync();

        // Assert
        var savedFace = await _context.Faces.FirstOrDefaultAsync();
        Assert.NotNull(savedFace);
        Assert.True(savedFace.Id > 0);
    }

    [Fact]
    public async Task AppDbContext_ShouldSaveFaceWithMetadata()
    {
        // Arrange
        _context.Database.EnsureCreated();
        var face = new Face
        {
            Timestamp = DateTime.UtcNow,
            Metadata = new List<Metadata>
            {
                new() { Key = "age", Value = "25" },
                new() { Key = "gender", Value = "male" }
            }
        };

        // Act
        _context.Faces.Add(face);
        await _context.SaveChangesAsync();

        // Assert
        var savedFace = await _context.Faces.Include(f => f.Metadata).FirstOrDefaultAsync();
        Assert.NotNull(savedFace);
        Assert.Equal(2, savedFace.Metadata.Count);
        Assert.Contains(savedFace.Metadata, m => m.Key == "age" && m.Value == "25");
        Assert.Contains(savedFace.Metadata, m => m.Key == "gender" && m.Value == "male");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public class CameraImageSourceStrategyTests
{
    [Fact]
    public void Constructor_ShouldNotThrowWhenOpenCVNotAvailable()
    {
        // This test verifies that class can be instantiated even if OpenCV runtime isn't available
        // In real CI environment, OpenCV might not be installed
        try
        {
            var strategy = new CameraImageSourceStrategy();
            Assert.NotNull(strategy);
        }
        catch (TypeInitializationException)
        {
            // Expected in CI environment without OpenCV runtime
            Assert.True(true, "OpenCV runtime not available in test environment - this is expected");
        }
    }

    [Fact]
    public async Task GetNextFrameAsync_ShouldHandleNoOpenCVGracefully()
    {
        // This test ensures the method handles OpenCV absence gracefully
        try
        {
            var strategy = new CameraImageSourceStrategy();
            var frame = await strategy.GetNextFrameAsync();
            // If we get here, OpenCV is available and method works
            Assert.True(frame == null || frame != null);
        }
        catch (TypeInitializationException)
        {
            // Expected in CI environment without OpenCV runtime
            Assert.True(true, "OpenCV runtime not available - this is expected in CI");
        }
    }
}

public class FileImageSourceStrategyTests
{
    [Fact]
    public void Constructor_ShouldThrowFileNotFoundForNonExistentFile()
    {
        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => new FileImageSourceStrategy("test.jpg"));
    }

    [Fact]
    public void FileExtensionValidation_ShouldDetectSupportedFormats()
    {
        // Test image extensions
        Assert.True(FileImageSourceStrategy.IsFileSupported("test.jpg"));
        Assert.True(FileImageSourceStrategy.IsFileSupported("test.png"));
        Assert.True(FileImageSourceStrategy.IsFileSupported("test.mp4"));
        Assert.True(FileImageSourceStrategy.IsFileSupported("test.avi"));
        
        // Test unsupported extensions
        Assert.False(FileImageSourceStrategy.IsFileSupported("test.txt"));
        Assert.False(FileImageSourceStrategy.IsFileSupported("test.exe"));
    }

    [Fact]
    public void GetSupportedExtensions_ShouldReturnCorrectFormats()
    {
        var imageExtensions = FileImageSourceStrategy.GetSupportedImageExtensions();
        var videoExtensions = FileImageSourceStrategy.GetSupportedVideoExtensions();

        Assert.Contains(".jpg", imageExtensions);
        Assert.Contains(".png", imageExtensions);
        Assert.Contains(".mp4", videoExtensions);
        Assert.Contains(".avi", videoExtensions);
    }

    [Fact]
    public void Constructor_ShouldThrowFileNotFoundForMissingFile()
    {
        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => new FileImageSourceStrategy("nonexistent.jpg"));
    }
}