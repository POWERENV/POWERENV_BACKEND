#pragma warning disable ASPIRETERMINAL001
#pragma warning disable ASPIRECOMPUTE003

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

bool isPublishingMode = args.Contains("--operation") && args.Contains("publish");

// DOCKER SETUP

var registry = builder.AddContainerRegistry(
    "ghcr",
    "ghcr.io",
    "powerenv/powerenv_backend"
);

builder.AddDockerComposeEnvironment("powerenv-docker-compose");

// PROJECTS ORCHESTRATION

IResourceBuilder<ProjectResource> initBootstrap = builder.AddProject<Projects.POWERENV_INIT_BOOTSTRAP>("INIT-BOOTSTRAP").WithArgs(isPublishingMode).WithTerminal().WithContainerRegistry(registry);

IResourceBuilder<RedisResource> redisCache = builder.AddRedis("RedisCache").WithContainerRuntimeArgs("-p", "6379:6379").WaitForCompletion(initBootstrap); // Maps all interfaces (0.0.0.0) by default

builder.AddProject<Projects.POWERENV_BACKEND_API>("MAIN-API").WithReference(redisCache).WithContainerRegistry(registry).WaitForCompletion(initBootstrap);

builder.AddProject<Projects.POWERENV_OSCONSOLE_WORKER>("OSCONSOLE-WORKER").WithReference(redisCache).WithContainerRegistry(registry).WaitForCompletion(initBootstrap);

builder.Build().Run();