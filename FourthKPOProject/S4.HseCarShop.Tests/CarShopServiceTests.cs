using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Bogus;
using Moq;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using S4.HseCarShop.Models;
using S4.HseCarShop.Models.Abstractions;
using S4.HseCarShop.Models.HandCar;
using S4.HseCarShop.Models.HybridCar;
using S4.HseCarShop.Models.PedalCar;
using S4.HseCarShop.Services;
using S4.HseCarShop.Services.Abstractions;
using S4.HseCarShop.Services.HandCars;
using S4.HseCarShop.Services.HybridCars;
using S4.HseCarShop.Services.PedalCars;
using System.ComponentModel.DataAnnotations;

using static Bogus.DataSets.Name;

namespace S4.HseCarShop.Tests;

public class CarShopServiceTests
{
    private readonly IFixture _fixture;

    private readonly Mock<ICarProvider> _carProviderMock;

    private readonly Mock<ICustomerStorage> _customersProviderMock;

    public CarShopServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _carProviderMock = _fixture.Freeze<Mock<ICarProvider>>();
        _customersProviderMock = _fixture.Freeze<Mock<ICustomerStorage>>();
    }

    #region Tests

    [Fact]
    public void SellCars_CustomerAlreadyHasCar_SkipCustomer()
    {
        // Arrange
        var customer = _fixture.Build<Customer>().Create();
        var carNumer = customer.Car!.Number;

        _customersProviderMock.Setup(x => x.GetCustomers()).Returns([customer]);

        // Act
        var service = _fixture.Create<CarShopService>();
        service.SellCars();

        // Assert
        Assert.Equal(carNumer, customer.Car.Number);

        _carProviderMock.Verify(cp => cp.GetCar(It.IsAny<IEnumerable<CarType>>()), Times.Never);
    }

    [Theory, AutoMockData]
    internal void SellCars_CustomerAlreadyHasCar_SkipCustomer2(
    Customer customer,
    [Frozen] Mock<ICarProvider> carStorageMock,
    [Frozen] Mock<ICustomerStorage> customerStorageMock,
    CarShopService carShop)
    {
        // Arrange
        customerStorageMock.Setup(cp => cp.GetCustomers()).Returns([customer]);

        // Act
        carShop.SellCars();

        // Assert
        carStorageMock.Verify(cp => cp.GetCar(It.IsAny<IEnumerable<CarType>>()), Times.Never);
    }

    [Theory]
    [InlineAutoData(6, 0, CarType.Pedal)]
    [InlineAutoData(0, 6, CarType.Hand)]
    [InlineAutoData(1, 1, CarType.Hybrid)]
    [InlineAutoData(6, 6)]
    public void SellCars_HasSuitableCar_AssignCar(
        uint legStrength,
        uint handStrength,
        CarType expectedType,
        [Range(1, 100)] uint pedalSize)
    {
        var faker = new Faker();

        // Arrange
        var customer = _fixture.Build<Customer>()
            .FromFactory(() => new Customer(faker.Name.FirstName(Gender.Male), legStrength, handStrength))
            .Without(c => c.Car)
            .Create();

        IEngine engine = expectedType switch
        {
            CarType.Pedal => new PedalEngine(pedalSize),
            CarType.Hand => new HandEngine(GripsType.Silicone),
            CarType.Hybrid => new HybridEngine(GripsType.Silicone, pedalSize),
            _ => throw new NotImplementedException(),
        };

        ICar expectedCar;

        if (expectedType == CarType.Hand)
        {
            expectedCar = _fixture.Build<HandCar>()
                .FromFactory(() => new HandCar(_fixture.Create<Guid>(), (HandEngine)engine!))
                .Create();
        }
        else if (expectedType == CarType.Pedal)
        {
            expectedCar = _fixture.Build<PedalCar>()
                            .FromFactory(() => new PedalCar(_fixture.Create<Guid>(), (PedalEngine)engine!))
                            .Create();
        }
        else
        {
            expectedCar = _fixture.Build<HybridCar>()
                .FromFactory(() => new HybridCar(_fixture.Create<Guid>(), (HybridEngine)engine!))
                .Create();
        }

        _customersProviderMock.Setup(x => x.GetCustomers()).Returns([customer]);
        _carProviderMock.Setup(x => x.GetCar(It.Is<IEnumerable<CarType>>(e => e.Contains(expectedType)))).Returns(expectedCar);

        // Act
        var service = new CarShopService(_carProviderMock.Object, _customersProviderMock.Object, new CarAvailabilityService(
            [new HandCarAvailabilityCheck(), new PedalCarAvailabilityCheck(), new HybridCarAvailabilityCheck()]));
        service.SellCars();

        // Assert
        Assert.NotNull(customer.Car);
        Assert.Equal(expectedCar, customer.Car);

        _carProviderMock.Verify(x => x.GetCar(It.IsAny<IEnumerable<CarType>>()), Times.Once);
    }

    [Theory]
    [InlineAutoMockData]
    [InlineAutoMockData(3, 0)]
    [InlineAutoMockData(0, 3)]
    [InlineAutoMockData(0, 0)]
    internal void SellCars_HasNoSuitableCar_DoNotAssignCar(
        [Range(0, 4)] uint legStrength,
        [Range(0, 4)] uint handStrength,
        [Frozen] Mock<ICarProvider> carStorageMock,
        [Frozen] Mock<ICustomerStorage> customerStorageMock,
        CarShopService carShop)
    {
        // Arrange
        var customer = _fixture.Build<Customer>()
            .FromFactory(() => new Customer(_fixture.Create<string>(), legStrength, handStrength))
            .Without(c => c.Car)
            .Create();

        customerStorageMock.Setup(cs => cs.GetCustomers()).Returns([customer]);

        // Act
        carShop.SellCars();

        // Assert
        Assert.Null(customer.Car);

        carStorageMock.Verify(x => x.GetCar(It.IsAny<IEnumerable<CarType>>()), Times.Never);
    }

    [Theory]
    [AutoMockData]
    internal void SellCars_MultipleSuitableCustomers_ProcessAll(
        [CustomizeCustomer(0, 6)] Customer customer1,
        [CustomizeCustomer(6, 0)] Customer customer2,
        [CustomizeCustomer(1, 1)] Customer customer3,
        [Frozen] Mock<ICarProvider> carStorageMock,
        [Frozen] Mock<ICustomerStorage> customerStorageMock
        )
    {
        // Arrange
        var customers = new[]
        {
            customer1,
            customer2,
            customer3
        };

        var expectedCars = new ICar[]
        {
                _fixture.Build<PedalCar>().FromFactory(() => new PedalCar(_fixture.Create<Guid>(), new PedalEngine(42))).Create(),
                _fixture.Build<HandCar>().FromFactory(() => new HandCar(_fixture.Create<Guid>(), new HandEngine(GripsType.Rubber))).Create(),
                _fixture.Build<HybridCar>().FromFactory(() => new HybridCar(_fixture.Create<Guid>(), new HybridEngine(GripsType.Silicone, 42))).Create(),
        };

        customerStorageMock.Setup(x => x.GetCustomers()).Returns(customers);

        carStorageMock.SetupSequence(cp => cp.GetCar(It.IsAny<IEnumerable<CarType>>()))
            .Returns(expectedCars[0])
            .Returns(expectedCars[1])
            .Returns(expectedCars[2]);

        CarShopService carShop = new(carStorageMock.Object, customerStorageMock.Object, new CarAvailabilityService(
            [new HandCarAvailabilityCheck(), new PedalCarAvailabilityCheck(), new HybridCarAvailabilityCheck()]));

        // Act
        carShop.SellCars();

        // Assert
        Assert.Equal(expectedCars[0], customers[0].Car);
        Assert.Equal(expectedCars[1], customers[1].Car);
        Assert.Equal(expectedCars[2], customers[2].Car);

        carStorageMock.Verify(x => x.GetCar(It.IsAny<IEnumerable<CarType>>()), Times.Exactly(3));
    }

    [Theory]
    [InlineAutoMockData(5, 0)]
    [InlineAutoMockData(0, 5)]
    [InlineAutoData(5, 5)]
    internal void SellCars_BorderlineStrengthValues_AssignCorrectly(
        uint legStrength,
        uint handStrength,
        [Frozen] Mock<ICarProvider> carStorageMock,
        [Frozen] Mock<ICustomerStorage> customerStorageMock)
    {
        // Arrange
        var customer = _fixture.Build<Customer>()
            .FromFactory(() => new Customer(_fixture.Create<string>(), legStrength, handStrength))
            .Without(c => c.Car)
            .Create();

        customerStorageMock.Setup(cs => cs.GetCustomers()).Returns([customer]);

        var expectedCars = new ICar[]
        {
                _fixture.Build<PedalCar>().FromFactory(() => new PedalCar(_fixture.Create<Guid>(), new PedalEngine(42))).Create(),
                _fixture.Build<HandCar>().FromFactory(() => new HandCar(_fixture.Create<Guid>(), new HandEngine(GripsType.Rubber))).Create(),
                _fixture.Build<HybridCar>().FromFactory(() => new HybridCar(_fixture.Create<Guid>(), new HybridEngine(GripsType.Silicone, 42))).Create(),
        };

        carStorageMock.SetupSequence(x => x.GetCar(It.IsAny<IEnumerable<CarType>>()))
            .Returns(expectedCars[0])
            .Returns(expectedCars[1]);

        CarShopService carShop = new(carStorageMock.Object, customerStorageMock.Object, new CarAvailabilityService(
            [new HandCarAvailabilityCheck(), new PedalCarAvailabilityCheck(), new HybridCarAvailabilityCheck()]));

        // Act
        carShop.SellCars();

        // Assert
        carStorageMock.Verify(cp => cp.GetCar(It.IsAny<IEnumerable<CarType>>()), Times.Never);
    }

    #endregion Tests
}
