namespace Ariston.Thermos.ValueObjects;

public record TargetTemperature
{
    public double Value { get; }
    
    public TargetTemperature(double value)
    {
        if (value is < 40 or > 70)
        {
            throw new ArgumentOutOfRangeException($"Target temperature must be between 40 and 70");
        }
        
        Value = value;
    }
}