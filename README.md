# BlogDoFt.RedisToggler.Lib

|                           [![codecov](https://codecov.io/gh/ftathiago/RedisToggler/graph/badge.svg?token=0JFTDIHNAP)](https://codecov.io/gh/ftathiago/RedisToggler)                            |  [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler)   |   [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler)   | [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler) |
| :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: |
| [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler) | [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler) | [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=ftathiago_RedisToggler&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler) |

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
    .AddSingleton(opt =>
    {
        return new CacheEntryConfiguration();
    })
```

Caso você possua mais de uma entidade sendo recuperada do cache, basta que você crie uma classe, herdando de `CacheEntryConfiguration`, e também a adicione como Singleton nas dependências da aplicação.

### Como utilizar?

Adicione ao construtor da classe que utilizará o cache, uma dependência para `IDistributedTypedCache<CacheEntryConfiguration>`. Neste caso, estamos utilizando uma referência para `CacheEntryConfiguration`, mas caso você possua outra configuração, basta substituir a referência de `CacheEntryConfiguration` pela classe de configuração desejada.

Você pode consultar a documentação de [IDistributedTypedCache\<TEntryConfig\>](https://github.com/ftathiago/RedisToggler/blob/main/src/RedisToggler.Lib/Abstractions/IDistributedTypedCache.cs) para mais detalhes. Ou pode ver o [projeto de exemplo](https://github.com/ftathiago/RedisToggler/tree/develop/samples) no github do projeto.

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-orange.svg)](https://sonarcloud.io/summary/new_code?id=ftathiago_RedisToggler)
