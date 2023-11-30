using Ariston.Entities;
using Ariston.Enums;
using Ariston.Interfaces;
using Ariston.Thermos.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Ariston.Thermos;

public class Thermo(IAriston _ariston, ILogger<Thermo> _logger)
{
    private double _temperature;
    private Mode _mode;
    private TargetTemperature? _targetTemperature;

    public async Task Heartbeat()
    {
        await FetchData();
        await CheckSystem();
    }

    public async Task<double> SetTargetTemperature(double targetTemp)
    {
        _targetTemperature = new TargetTemperature(targetTemp);
        return _targetTemperature.Value;
    }
    
    public async void BoostMode()
    {
        await SetMode(Mode.Boost);
        await SetTargetTemperature(70);
    }
    
    public async void GreenMode()
    {
        await SetMode(Mode.Green);
    }
    
    private async Task CheckSystem()
    {
        CheckIfTemperatureIsReached();

        if (_targetTemperature is not null && _mode != Mode.Boost)
        {
            BoostMode();
        }
        
        CheckTargetIsNeeded();
        
        if (_targetTemperature is null && _mode != Mode.Green)
        {
            GreenMode();
        }
    }

    private void CheckIfTemperatureIsReached()
    {
        if (_targetTemperature is null && _temperature < 70 || _mode == Mode.Green )
        {
            return;
        }
        
        if (_targetTemperature is not null && _temperature < _targetTemperature!.Value)
        {
            return;
        }
        
        _targetTemperature = null;
        GreenMode();
    }

    private async void CheckTargetIsNeeded()
    {
        DateTimeOffset now = DateTimeOffset.Now;

        foreach (BoostSettings setting in ThermoSettings.BoostsSettings)
        {
            if (!setting.DaysOfWeek.Contains(now.DayOfWeek))
            {
                continue;
            }

            if (now.Hour < setting.InitialHour || now.Hour > setting.FinalHour)
            {
                continue;
            }

            await SetTargetTemperature(setting.TargetTemperature.Value);
        }
    }
    
    private async Task FetchData()
    {
        PlantData? plantData = null;
        int counter = 0;
        while (counter <= 3)
        {
            plantData = await _ariston.GetPlantData();
            if (plantData is not null)
            {
                break;
            }
            counter++;
        }
        
        if (plantData is null)
        {
            _logger.LogError("Unable to fetch data");
            return;
        }

        _temperature = plantData.Temp;
        _mode = plantData.Mode;
    }
    
    private async Task SetMode(Mode mode)
    {
        if (mode == _mode)
        {
            _logger.LogInformation("Mode already set");
            return;
        }

        await _ariston.SetMode(mode);
    }
}