using Ariston.Thermos.ValueObjects;

namespace Ariston.Thermos;

public class ThermoSettings
{
    public static IReadOnlyList<BoostSettings> BoostsSettings { get; } = new List<BoostSettings>()
    {
        new BoostSettings(
            InitialHour: 6,
            FinalHour: 22,
            TargetTemperature: new TargetTemperature(70),
            DaysOfWeek: new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }
            ),
        
        new BoostSettings(
            InitialHour: 8,
            FinalHour: 22,
            TargetTemperature: new TargetTemperature(70),
            DaysOfWeek: new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday }
        ),
    };
}