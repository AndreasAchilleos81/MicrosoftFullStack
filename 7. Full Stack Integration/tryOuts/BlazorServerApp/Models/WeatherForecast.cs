namespace BlazorServerApp.Models;

internal class WeatherData
{
    public Location location { get; set; }
    public Current current { get; set; }

    public override string ToString()
    {
        return $"{location.name}, {location.country}: {current.Temp_C}°C, {current.Condition.text}";
    }
}

internal class Location
{
    public required string name { get; set; }
    public required string country { get; set; }
}

internal class Current
{
    public required double Temp_C { get; set; }
    public required Condition Condition { get; set; }
}

internal class Condition
{
    public required string text { get; set; }
    public required string icon { get; set; }
}
