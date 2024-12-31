using CoRSample.Api.Features.WeatherForecast;

namespace CoRSample.Api.Tests.Features.WeatherForecast;

public class WeatherForecastEndpointTests
{
    [Fact]
    public void GetWeatherForecastHandler_ShouldReturnFiveForecasts()
    {
        // Act
        var result = WeatherForecastEndpoint.GetWeatherForecastHandler();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public void GetWeatherForecastHandler_ShouldReturnValidForecasts()
    {
        // Act
        var result = WeatherForecastEndpoint.GetWeatherForecastHandler();

        // Assert
        result.Should().AllSatisfy(forecast =>
        {
            forecast.Date.Should().BeAfter(DateOnly.FromDateTime(DateTime.Now));
            forecast.TemperatureC.Should().BeInRange(-20, 55);
            forecast.Summary.Should().NotBeNullOrEmpty();
        });
    }
}