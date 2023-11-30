namespace Ariston.Thermos.ValueObjects;

public record BoostSettings(
    int InitialHour,
    int FinalHour,
    TargetTemperature TargetTemperature,
    DayOfWeek[] DaysOfWeek
);