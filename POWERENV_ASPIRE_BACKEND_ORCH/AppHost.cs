using Aspire.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> redisCache = builder.AddRedis("RedisCache").WithContainerRuntimeArgs("-p", "6379:6379"); // Maps all interfaces (0.0.0.0) by default

builder.AddProject<Projects.POWERENV_BACKEND_API>("MAIN-API").WithReference(redisCache);

builder.AddProject<Projects.POWERENV_OSCONSOLE_WORKER>("OSCONSOLE-WORKER").WithReference(redisCache);

builder.Build().Run();
