# BlogDoFt.RedisToggler.Lib

|                           [![codecov](https://codecov.io/gh/ftathiago/RedisToggler/graph/badge.svg?token=0JFTDIHNAP)](https://codecov.io/gh/ftathiago/RedisToggler)                            |  [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler)   |   [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler)   | [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler) |
| :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: |
| [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler) | [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler) | [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler) |

- [Versão em Português](#português)

- [English Version](#english)

## Português

Esta biblioteca é um wrapper para a [Microsoft.Extensions.Caching.StackExchangeRedis](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis), adicionando a capacidade de se auto-desligar quando perceber uma queda do servidor Redis conectado. Desta forma, as requisições não ficarão lentas, aguardando pelo time-out do request ao Redis.

Ainda, quando o Redis retornar on-line, a biblioteca irá se religar automaticamente, tornando as funcionalidades de cache disponíveis outra vez.

Outra funcionalidade que esta biblioteca adiciona é a padronização de chaves. Desta forma, quando necessitar de compartilhar o cache entre aplicações diferentes, o padrão poderá garantir que as aplicações acessem sempre o mesmo conteúdo.

### Configurando o cache

Ao configurar as dependências da sua aplicação, adicione uma chamada a `.AddCacheWrapper()`. Este método configura a conexão e o tipo de cache a ser utilizado.

```csharp

builder.Services
    .AddCacheWrapper(opt =>
    {
        opt.ConnectionString = "localhost:6379,asyncTimeout=1000,connectTimeout=1000,password=<your-redis-password>,abortConnect=false";
        opt.CacheType = CacheType.Redis;
    });
```

O método configura uma instância da classe [RedisToggler.Lib.Configurations.CacheConfig](https://github.com/ftathiago/RedisToggler/blob/main/src/RedisToggler.Lib/Configurations/CacheConfig.cs), responsável por configurar o cache. Os tipos suportados são:

- NoCache: Nenhuma funcionalidade de cache estará ativa;
- Redis: A aplicação irá utilizar o Redis como repositório de cache;
- Memory: A aplicação irá utilizar a memória como repositório de cache;

Veja a documentação em: [CacheType.cs](https://github.com/ftathiago/RedisToggler/blob/main/src/RedisToggler.Lib/Abstractions/CacheType.cs).

### Configurando a entidade para cache

Como as entidades cacheadas podem seguir padrões de *storage* diferentes, a biblioteca possui a capacidade de adicionar configurações para cada entidade cacheada.

Para isso, basta adicionar uma instância de `CacheEntryConfiguration` como Singleton como dependência da aplicação.

```csharp
builder.Services
    .AddSingleton(opt => new CacheEntryConfiguration()
    {
        StoreLanguage = true,        
    })
```

Caso você possua mais de uma entidade sendo recuperada do cache, basta que você crie uma classe, herdando de `CacheEntryConfiguration`, e também a adicione como Singleton nas dependências da aplicação.

A propriedade `StoreLanguage` informa se é necessário - ou não - adicionar as informações de cultura da Thread na chave do registro no cache.

### Como utilizar?

Adicione ao construtor da classe que utilizará o cache, uma dependência para `IDistributedTypedCache<CacheEntryConfiguration>`. Neste caso, estamos utilizando uma referência para `CacheEntryConfiguration`, mas caso você possua outra configuração, basta substituir a referência de `CacheEntryConfiguration` pela classe de configuração desejada.

Você pode consultar a documentação de [IDistributedTypedCache\<TEntryConfig\>](https://github.com/ftathiago/RedisToggler/blob/main/src/RedisToggler.Lib/Abstractions/IDistributedTypedCache.cs) para mais detalhes. Ou pode ver o [projeto de exemplo](https://github.com/ftathiago/RedisToggler/tree/develop/samples) no github do projeto.

## English

This library is a wrapper to [Microsoft.Extensions.Caching.StackExchangeRedis](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis), adding the capability to auto turn-off it's self when Redis server has a breakdown. This way, the request will not become slow, waiting for Redis connection time-out.

When Redis become on-line, the library will turn-on it's self automatically, enabling the cache capabilities again.

Another feature that this library add is the opiniative pattern of Cache Keys. This way, when will needed to share the cache data with different applications, the pattern could grant that the applications always access the same content.

### Configuring cache

When you are configuring the application dependencies, add a call to `.AddCacheWrapper()`. This extension method configures the connection and the cache type to be used.

```csharp

builder.Services
    .AddCacheWrapper(opt =>
    {
        opt.ConnectionString = "localhost:6379,asyncTimeout=1000,connectTimeout=1000,password=<your-redis-password>,abortConnect=false";
        opt.CacheType = CacheType.Redis;
    });
```

This extension method configures a class instance of [RedisToggler.Lib.Configurations.CacheConfig](https://github.com/ftathiago/RedisToggler/blob/main/src/RedisToggler.Lib/Configurations/CacheConfig.cs), in charge of cache configuration. The supported types are:

- NoCache: None cache capability are active;
- Redis: The application will use Redis as cache storage;
- Memory: Application will use his container memory as cache storage;

See documentation at: [CacheType.cs](https://github.com/ftathiago/RedisToggler/blob/main/src/RedisToggler.Lib/Abstractions/CacheType.cs).

### Setup an Entity to be cached

As the entities could be cached by different ways, this library has the ability to add custom configuration to each cacheable entity.

To this, you must add a `CacheEntryConfiguration` instance as a singleton into IServiceCollection.

```csharp
builder.Services
    .AddSingleton(opt =>
    {
        return new CacheEntryConfiguration();
    })
```

In case that you have more then one entity being restore from cache, you must write a classe that inherits from `CacheEntryConfiguration`, and also add an instance of created class, as singleton, into IServiceCollection.

The `StoreLanguage` property set if is necessary - or not - to add Culture information, present at Current Thread, to cache key.

### How to use?

At class constructor, that will use the cached entity, add a dependency to `IDistributedTypedCache<CacheEntryConfiguration>`. In this specific case, we are using a reference to `CacheEntryConfiguration`, but if you are using other (or multiple) configurations, you may change the `CacheEntryConfiguration` by the desired configuration class.

You may consult the documentation of [IDistributedTypedCache\<TEntryConfig\>](https://github.com/ftathiago/RedisToggler/blob/main/src/RedisToggler.Lib/Abstractions/IDistributedTypedCache.cs) for more details. Or you can read the [sample project](https://github.com/ftathiago/RedisToggler/tree/develop/samples) into github project.

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-orange.svg)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler)
