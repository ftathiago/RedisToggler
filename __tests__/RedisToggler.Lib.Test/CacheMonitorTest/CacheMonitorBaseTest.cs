namespace RedisToggler.Lib.Test.CacheMonitorTest;

public class CacheMonitorBaseTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Should_UpdateCache(bool active)
    {
        // Given
        var monitor = new CacheMonitor();

        // When
        monitor.UpdateCache(active);

        // Then
        monitor.Active.Should().Be(active);
    }
}
