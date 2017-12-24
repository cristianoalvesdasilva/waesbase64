using System;
using System.Data.Entity;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WAES.Cris.DataAccess;
using WAES.Cris.Model;
using WAES.Cris.Services;

namespace WAES.Cris.UnitTests
{
  [TestClass]
  public class BinDataServiceTests
  {
    private readonly Mock<IDataEntitiesFactory> dataEntitiesFactoryMock;
    private readonly Mock<IDataEntities> dataEntitiesMock;
    private readonly BinDataService service;
    private Mock<IDbSet<BinData>> binDataSetMock;

    public BinDataServiceTests()
    {
      this.dataEntitiesMock = new Mock<IDataEntities>();
      this.binDataSetMock = MockHelper.MockDbSet<BinData>();
      this.dataEntitiesMock.Setup(_ => _.BinDataSet)
        .Returns(this.binDataSetMock.Object);

      this.dataEntitiesMock.Setup(_ => _.Set<BinData>())
       .Returns(this.binDataSetMock.Object);

      this.dataEntitiesFactoryMock = new Mock<IDataEntitiesFactory>();
      this.dataEntitiesFactoryMock.Setup(_ => _.Create())
        .Returns(this.dataEntitiesMock.Object);

      this.service = new BinDataService(this.dataEntitiesFactoryMock.Object);
    }

    [TestMethod]
    public void UpsertAsync_Should_Throw_Exception_When_LeftContent_And_RightContent_Are_Null()
    {
      // Arrange
      var data = new BinData
      {
        Id = 1,
        LeftContent = null,
        RightContent = null
      };

      // Act & Assert
      this.Invoking(_ => this.service.UpsertAsync(data).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<ArgumentException>()
        .WithInnerMessage("*Either 'left' or 'right' properties must be provided.*");
    }

    [TestMethod]
    public void UpsertAsync_Should_Throw_Exception_When_LeftContent_And_RightContent_Are_Empty()
    {
      // Arrange
      var data = new BinData
      {
        Id = 1,
        LeftContent = string.Empty,
        RightContent = string.Empty
      };

      // Act & Assert
      this.Invoking(_ => this.service.UpsertAsync(data).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<ArgumentException>()
        .WithInnerMessage("*Either 'left' or 'right' properties must be provided.*");
    }

    [TestMethod]
    public void UpsertAsync_Should_Throw_Exception_When_LeftContent_And_RightContent_Are_White_Spaces()
    {
      // Arrange
      var data = new BinData
      {
        Id = 1,
        LeftContent = "  ",
        RightContent = "      "
      };

      // Act & Assert
      this.Invoking(_ => this.service.UpsertAsync(data).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<ArgumentException>()
        .WithInnerMessage("*Either 'left' or 'right' properties must be provided.*");
    }

    [TestMethod]
    public void UpsertAsync_Should_Throw_Exception_When_LeftContent_Is_Not_Base64()
    {
      // Arrange
      var data = new BinData
      {
        Id = 1,
        LeftContent = "123"
      };

      // Act & Assert
      this.Invoking(_ => this.service.UpsertAsync(data).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<ArgumentException>()
        .WithInnerMessage("Invalid base64 string.");
    }

    [TestMethod]
    public void UpsertAsync_Should_Throw_Exception_When_RightContent_Is_Not_Base64()
    {
      // Arrange
      var data = new BinData
      {
        Id = 1,
        RightContent = "123"
      };

      // Act & Assert
      this.Invoking(_ => this.service.UpsertAsync(data).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<ArgumentException>()
        .WithInnerMessage("Invalid base64 string.");
    }

    [TestMethod]
    public void UpsertAsync_Should_Throw_Exception_When_Id_Is_Lower_Than_One()
    {
      // Arrange
      var data = new BinData
      {
        Id = 0,
        LeftContent = "aaaaa",
        RightContent = "aaaaa"
      };

      // Act & Assert
      this.Invoking(_ => this.service.UpsertAsync(data).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<ArgumentOutOfRangeException>()
        .WithInnerMessage("*'Id' must be greater than zero.*");
    }

    [TestMethod]
    public void UpsertAsync_Should_Throw_Exception_When_Data_Is_Null()
    {
      // Arrange
      BinData data = null;

      // Act & Assert
      this.Invoking(_ => this.service.UpsertAsync(data).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<ArgumentNullException>();
    }

    [TestMethod]
    public void UpsertAsync_Should_Create_Record_When_Id_Does_Not_Exist()
    {
      // Arrange
      // By default the BinDataSet is empty, so it should force the data below to be 'added'.
      var binData = new BinData { Id = 1, LeftContent = "aaaa" };

      // Act
      this.service.UpsertAsync(binData).Wait();

      // Assert
      this.binDataSetMock.Verify(_ => _.Add(binData), Times.Once);
      this.dataEntitiesMock.Verify(_ => _.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public void UpsertAsync_Should_Update_Existing_Record_When_Id_Exists()
    {
      // Arrange
      var existingData = new BinData { Id = 1, LeftContent = "aaaaaa", RightContent = "bbbb" };
      var mockSet = MockHelper.MockDbSet(existingData);
      this.dataEntitiesMock.Setup(_ => _.BinDataSet)
        .Returns(mockSet.Object);

      var binData = new BinData { Id = 1, LeftContent = "bGVlZWVlZmZmZnR0dA==", RightContent = "cmlnaHR0dHR0dHR0" };

      // Act
      this.service.UpsertAsync(binData).Wait();

      // Assert
      existingData.LeftContent.Should().Be(binData.LeftContent);
      existingData.RightContent.Should().Be(binData.RightContent);

      this.binDataSetMock.Verify(_ => _.Add(binData), Times.Never);
      this.dataEntitiesMock.Verify(_ => _.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public void CompareLeftAndRightAsync_Should_Throw_Exception_When_No_Record_Found()
    {
      // Act & Assert
      this.Invoking(_ => this.service.CompareLeftAndRightAsync(1).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<InvalidOperationException>()
        .WithInnerMessage("No record was found for id 1.");
    }

    [TestMethod]
    public void CompareLeftAndRightAsync_Should_Throw_Exception_When_LeftContent_Is_Missing()
    {
      // Arrange
      var existingData = new BinData { Id = 1, LeftContent = null, RightContent = "bbbb" };
      var mockSet = MockHelper.MockDbSet(existingData);
      this.dataEntitiesMock.Setup(_ => _.Set<BinData>())
        .Returns(mockSet.Object);

      // Act & Assert
      this.Invoking(_ => this.service.CompareLeftAndRightAsync(1).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<InvalidOperationException>()
        .WithInnerMessage("'Left' content is missing.");
    }

    [TestMethod]
    public void CompareLeftAndRightAsync_Should_Throw_Exception_When_RightContent_Is_Missing()
    {
      // Arrange
      var existingData = new BinData { Id = 1, LeftContent = "aaaaa", RightContent = null };
      var mockSet = MockHelper.MockDbSet(existingData);
      this.dataEntitiesMock.Setup(_ => _.Set<BinData>())
        .Returns(mockSet.Object);

      // Act & Assert
      this.Invoking(_ => this.service.CompareLeftAndRightAsync(1).Wait())
        .ShouldThrow<AggregateException>()
        .WithInnerException<InvalidOperationException>()
        .WithInnerMessage("'Right' content is missing.");
    }

    [TestMethod]
    public void CompareLeftAndRightAsync_Should_Return_Message_When_Left_And_Right_Are_The_Same()
    {
      // Arrange
      var existingData = new BinData { Id = 1, LeftContent = "SEVMTE8gV09STEQh", RightContent = "SEVMTE8gV09STEQh" };
      var mockSet = MockHelper.MockDbSet(existingData);
      this.dataEntitiesMock.Setup(_ => _.Set<BinData>())
        .Returns(mockSet.Object);

      // Act
      var result = this.service.CompareLeftAndRightAsync(1).Result;

      // Assert
      result.DiffOffsets.Should().BeNull();
      result.Length.Should().BeNull();
      result.Message.Should().Be("Left and Right are the same.");
    }

    [TestMethod]
    public void CompareLeftAndRightAsync_Should_Return_Message_When_Left_And_Right_Have_Different_Lengths()
    {
      // Arrange
      var existingData = new BinData { Id = 1, LeftContent = "SEVMTE8gV09STEQ=", RightContent = "SEVMTE8gV09STEQh" };
      var mockSet = MockHelper.MockDbSet(existingData);
      this.dataEntitiesMock.Setup(_ => _.Set<BinData>())
        .Returns(mockSet.Object);

      // Act
      var result = this.service.CompareLeftAndRightAsync(1).Result;

      // Assert
      result.DiffOffsets.Should().BeNull();
      result.Length.Should().BeNull();
      result.Message.Should().Be("Left and Right have different sizes.");
    }

    [TestMethod]
    public void CompareLeftAndRightAsync_Should_Return_Message_And_Offsets_When_Left_And_Right_Have_Same_Lengths()
    {
      // Arrange
      var existingData = new BinData { Id = 1, LeftContent = "aGVsbG8gd29ybGQh", RightContent = "SEVMTE8gV09STEQh" };
      var mockSet = MockHelper.MockDbSet(existingData);
      this.dataEntitiesMock.Setup(_ => _.Set<BinData>())
        .Returns(mockSet.Object);

      // Act
      var result = this.service.CompareLeftAndRightAsync(1).Result;

      // Assert
      result.Message.Should().Be("Left and Right have same size, but with different content.");
      result.DiffOffsets.Should().NotBeNullOrEmpty();
      result.Length.Should().NotBeNull();
    }
  }
}