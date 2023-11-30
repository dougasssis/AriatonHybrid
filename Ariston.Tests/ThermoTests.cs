using Ariston.Entities;

namespace Ariston.Tests;

public class ThermoTests
{
    private readonly Mock<IAriston> _aristonMock;
    private readonly Thermo _thermo;

    public ThermoTests()
    {
        _aristonMock = new Mock<IAriston>();
        Mock<ILogger<Thermo>> loggerMock = new();
        _thermo = new Thermo(_ariston: _aristonMock.Object, _logger: loggerMock.Object);
    }

    [Fact]
    public async Task Heartbeat_ShouldFetchDataAndCheckSystem()
    {
        // Arrange
        PlantData plantData = new PlantData(
            Temp: 30,
            Mode: Mode.Green,
            On: true,
            ProcReqTemp: 50,
            ReqTemp: 50,
            BoostReqTemp: 30,
            AntiLeg: false,
            HeatReq: true,
            AvShw: 0,
            Gw: "gw"
        );
        
        _aristonMock.Setup(a => a.GetPlantData()).ReturnsAsync(plantData);

        await _thermo.SetTargetTemperature(50);

        // Act
        await _thermo.Heartbeat();

        // Assert
        _aristonMock.Verify(expression: a => a.GetPlantData(), times: Times.Once);
        _aristonMock.Verify(expression: a => a.SetMode(Mode.Boost), times: Times.Once);
    }

    [Fact]
    public async Task TargetTemperature_WithValidTemp_ShouldTargetTemperature()
    {
        // Arrange
        const double targetTemp = 50;
        Thermo thermo = new(_ariston: _aristonMock.Object, _logger: Mock.Of<ILogger<Thermo>>());
        
        // Act
        double response = await thermo.SetTargetTemperature(targetTemp: targetTemp);
        
        // Assert
        response.Should().Be(targetTemp);
    }
    
    [Fact]
    public async Task BoostMode_ShouldSetModeToBoost()
    {
        // Arrange
        Thermo thermo = new(_ariston: _aristonMock.Object, _logger: Mock.Of<ILogger<Thermo>>());
        
        // Act
        thermo.BoostMode();
        
        // Assert
        _aristonMock.Verify(expression: a => a.SetMode(Mode.Boost), times: Times.Once);
    }
    
    [Fact]
    public async Task TemperatureReachesTarget_ShouldSetModeToGreen()
    {
        // Arrange
        await _thermo.SetTargetTemperature(50);
        _aristonMock.Setup(a => a.GetPlantData()).ReturnsAsync(new PlantData(
            Temp: 50, Mode: Mode.Boost, On: true,
            ProcReqTemp: 50,
            ReqTemp: 50,
            BoostReqTemp: 30,
            AntiLeg: false,
            HeatReq: true,
            AvShw: 0,
            Gw: "gw"
        ));

        // Act
        await _thermo.Heartbeat();

        // Assert
        _aristonMock.Verify(expression: a => a.SetMode(Mode.Green), times: Times.Once);
    }

    [Fact]
    public async Task TemperatureBelowTarget_ShouldSetModeToBoost()
    {
        // Arrange
        await _thermo.SetTargetTemperature(50);
        _aristonMock.Setup(a => a.GetPlantData()).ReturnsAsync(new PlantData(
            Temp: 40, 
            Mode: Mode.Green,
            On: true,
            ProcReqTemp: 50,
            ReqTemp: 50,
            BoostReqTemp: 30,
            AntiLeg: false,
            HeatReq: true,
            AvShw: 0,
            Gw: "gw"
        ));

        // Act
        await _thermo.Heartbeat();

        // Assert
        _aristonMock.Verify(expression: a => a.SetMode(Mode.Boost), times: Times.Once);
    }
    
    [Fact]
    public async Task TargetTemperatureAndModeNotBoost_ShouldSetModeToBoost()
    {
        // Arrange
        await _thermo.SetTargetTemperature(50);
        _aristonMock.Setup(a => a.GetPlantData()).ReturnsAsync(new PlantData(
            Temp: 40, 
            Mode: Mode.Green, 
            On: true,
            ProcReqTemp: 50,
            ReqTemp: 50,
            BoostReqTemp: 30,
            AntiLeg: false,
            HeatReq: true,
            AvShw: 0,
            Gw: "gw"));

        // Act
        await _thermo.Heartbeat();

        // Assert
        _aristonMock.Verify(expression: a => a.SetMode(Mode.Boost), times: Times.Once);
    }

    [Fact]
    public async Task NoTargetTemperatureAndModeNotGreenAndTemperatureEqual70_ShouldSetModeToGreen()
    {
        // Arrange
        _aristonMock.Setup(a => a.GetPlantData()).ReturnsAsync(new PlantData(
            Temp: 70, 
            Mode: Mode.Boost, 
            On: true,
            ProcReqTemp: 50,
            ReqTemp: 50,
            BoostReqTemp: 30,
            AntiLeg: false,
            HeatReq: true,
            AvShw: 0,
            Gw: "gw"
        ));
        
        // Act
        await _thermo.Heartbeat();

        // Assert
        _aristonMock.Verify(expression: a => a.SetMode(Mode.Green), times: Times.Once);
    }

    [Fact]
    public async Task ModeAlreadySet_ShouldNotCallSetMode()
    {
        // Arrange
        _aristonMock.Setup(a => a.GetPlantData()).ReturnsAsync(new PlantData(
            Temp: 40, 
            Mode: Mode.Green, 
            On: true,
            ProcReqTemp: 50,
            ReqTemp: 50,
            BoostReqTemp: 30,
            AntiLeg: false,
            HeatReq: true,
            AvShw: 0,
            Gw: "gw"
        ));
        
        // Act
        await _thermo.Heartbeat();

        // Assert
        _aristonMock.Verify(expression: a => a.SetMode(It.IsAny<Mode>()), times: Times.Never);
    }

    [Fact]
    public async Task SetTargetTemperature_ShouldSetTargetTemperature()
    {
        // Arrange
        const double targetTemp = 50;

        // Act
        double response = await _thermo.SetTargetTemperature(targetTemp);

        // Assert
        response.Should().Be(targetTemp);
    }

    [Fact]
    public async Task Heartbeat_ShouldCallFetchDataAndCheckSystem()
    {
        // Arrange
        _aristonMock.Setup(a => a.GetPlantData()).ReturnsAsync(new PlantData(
            Temp: 40, 
            Mode: Mode.Green, 
            On: true,
            ProcReqTemp: 50,
            ReqTemp: 50,
            BoostReqTemp: 30,
            AntiLeg: false,
            HeatReq: true,
            AvShw: 0,
            Gw: "gw"
        ));
        // Act
        await _thermo.Heartbeat();

        // Assert
        _aristonMock.Verify(expression: a => a.GetPlantData(), times: Times.Once);
    }

    [Fact]
    public async Task Heartbeat_WithOverrideBoost_ShouldNotSetModeToGreen()
    {
        // Arrange
        _aristonMock.Setup(a => a.GetPlantData()).ReturnsAsync(new PlantData(
            Temp: 60, 
            Mode: Mode.Green, 
            On: true,
            ProcReqTemp: 50,
            ReqTemp: 50,
            BoostReqTemp: 30,
            AntiLeg: false,
            HeatReq: true,
            AvShw: 0,
            Gw: "gw"
        ));
        
        Thermo thermo = new(_ariston: _aristonMock.Object, _logger: Mock.Of<ILogger<Thermo>>());
        await thermo.SetTargetTemperature(60);
        thermo.BoostMode();

        // Act
        await thermo.Heartbeat();

        // Assert
        _aristonMock.Verify(expression: a => a.SetMode(Mode.Green), times: Times.Never);
    }
}