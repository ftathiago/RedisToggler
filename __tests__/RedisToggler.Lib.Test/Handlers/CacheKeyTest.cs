using RedisToggler.Lib.Configurations;
using RedisToggler.Lib.Handlers;
using System.Globalization;

namespace RedisToggler.Lib.Test.Handlers;

public class CacheKeyTest
{
    private readonly CacheEntryConfiguration config = new();

    [Fact]
    public void Should_CorrectlyNameAType_When_IsASingleGenericType()
    {
        var keyValue = Guid.NewGuid().ToString();
        var expectedValue =
            $"RedisToggler.Lib.Test.Fixtures.SingleGeneric<System.String>:{keyValue}";
        var key = new CacheKey<SingleGeneric<string>>(config, keyValue);

        var value = key.Value;

        value.Should().BeEquivalentTo(expectedValue);
    }

    [Fact]
    public void Should_CorrectlyNameAType_When_IsADoubleGenericType()
    {
        var keyValue = Guid.NewGuid().ToString();
        var expectedValue =
            $"RedisToggler.Lib.Test.Fixtures.DoubleGeneric<System.String, System.Int32>:{keyValue}";
        var key = new CacheKey<DoubleGeneric<string, int>>(config, keyValue);

        var value = key.Value;

        value.Should().BeEquivalentTo(expectedValue);
    }

    [Fact]
    public void Should_CorrectlyNameAType_When_IsALargeGenericType()
    {
        var keyValue = Guid.NewGuid().ToString();
        var expectedValue =
            $"RedisToggler.Lib.Test.Fixtures.LargeGeneric<System.Int32, System.Int32, System.Int32, System.Int32, System.Int32, System.Int32, System.Int32, System.Int32, System.Int32, System.Int32>:{keyValue}";
        var key = new CacheKey<LargeGeneric<int, int, int, int, int, int, int, int, int, int>>(config, keyValue);

        var value = key.Value;

        value.Should().BeEquivalentTo(expectedValue);
    }

    [Fact]
    public void Should_CorrectlyNameAType_When_IsAChildGenericClass()
    {
        var keyValue = Guid.NewGuid().ToString();
        var expectedValue =
            $"RedisToggler.Lib.Test.Fixtures.ChildGenericClass:{keyValue}";
        var key = new CacheKey<ChildGenericClass>(config, keyValue);

        var value = key.Value;

        value.Should().BeEquivalentTo(expectedValue);
    }

    [Fact]
    public void Should_IncludeCurrentThreadCulture_When_StoreLanguageIsTrue()
    {
        // Given
        Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
        var keyValue = Guid.NewGuid().ToString();
        var expectedValue =
            $"RedisToggler.Lib.Test.Fixtures.ChildGenericClass:pt-BR:{keyValue}";
        config.StoreLanguage = true;
        var key = new CacheKey<ChildGenericClass>(config, keyValue);

        // When
        var value = key.Value;

        // Then
        value.Should().BeEquivalentTo(expectedValue);
    }

    [Fact]
    public void Should_IncludePrefix_When_PrefixIsConfigured()
    {
        // Given
        Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
        var keyValue = Guid.NewGuid().ToString();
        config.KeyPrefix = "test";
        var expectedValue =
            $"{config.KeyPrefix}:RedisToggler.Lib.Test.Fixtures.ChildGenericClass:{keyValue}";
        var key = new CacheKey<ChildGenericClass>(config, keyValue);

        // When
        var value = key.Value;

        // Then
        value.Should().BeEquivalentTo(expectedValue);
    }
}
