#pragma warning disable ASPIRETERMINAL001
#pragma warning disable ASPIREPIPELINES003
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

List<(string, string?)> SYSEnvironmentVariables = new List<(string, string?)> {
    ("POWERENV_DB_IPADDRESS", Environment.GetEnvironmentVariable("POWERENV_DB_IPADDRESS")),
    ("POWERENV_DB_PASSWORD", Environment.GetEnvironmentVariable("POWERENV_DB_PASSWORD")),
    ("POWERENV_DB_PORT", Environment.GetEnvironmentVariable("POWERENV_DB_PORT")),
    ("POWERENV_HOST_IPADDRESS", Environment.GetEnvironmentVariable("POWERENV_HOST_IPADDRESS"))
};

List<IResourceBuilder<ParameterResource>> environmentVariables = new List<IResourceBuilder<ParameterResource>>();

for(int i = 0; i < SYSEnvironmentVariables.Count; i++)
{
    IResourceBuilder<ParameterResource> newEnvironmentVariable;

    if (!string.IsNullOrEmpty(SYSEnvironmentVariables[i].Item2))
    {
        newEnvironmentVariable = builder.AddParameter(SYSEnvironmentVariables[i].Item1.Replace("_", "-"), value: SYSEnvironmentVariables[i].Item2!);
    }
    else newEnvironmentVariable = builder.AddParameter(SYSEnvironmentVariables[i].Item1.Replace("_", "-"));

    environmentVariables.Add(newEnvironmentVariable);
}

// PROJECTS ORCHESTRATION

IResourceBuilder<ProjectResource> initBootstrap = builder.AddProject<Projects.POWERENV_INIT_BOOTSTRAP>("INIT-BOOTSTRAP")
    .WithArgs(isPublishingMode.ToString())
    .WithTerminal()
    .WithContainerRegistry(registry)
    .WithRemoteImageTag("latest");

IResourceBuilder<RedisResource> redisCache = builder.AddRedis("RedisCache")
    .WithContainerRuntimeArgs("-p", "6379:6379")
    .WithHttpEndpoint(
        port: 6379,
        targetPort: 6379,
        name: "http",
        isProxied: false)
    .WithExternalHttpEndpoints()
    .WaitForCompletion(initBootstrap); // Maps all interfaces (0.0.0.0) by default

builder.AddProject<Projects.POWERENV_BACKEND_API>("MAIN-API")
    .WithReference(redisCache)
    .WithContainerRegistry(registry)
    .WithRemoteImageTag("latest")
    .WithHttpEndpoint(
        port: 5000,
        targetPort: 5000,
        name: "http",
        isProxied: false)
    .WithExternalHttpEndpoints()
    .WithEnvironment("POWERENV_DB_IPADDRESS", environmentVariables[0])
    .WithEnvironment("POWERENV_DB_PASSWORD", environmentVariables[1])
    .WithEnvironment("POWERENV_DB_PORT", environmentVariables[2])
    .WithEnvironment("POWERENV_HOST_IPADDRESS", environmentVariables[3])
    .WaitForCompletion(initBootstrap);

builder.AddProject<Projects.POWERENV_OSCONSOLE_WORKER>("OSCONSOLE-WORKER")
    .WithReference(redisCache)
    .WithContainerRegistry(registry)
    .WithRemoteImageTag("latest")
    .WithEnvironment("POWERENV_DB_IPADDRESS", environmentVariables[0])
    .WithEnvironment("POWERENV_DB_PASSWORD", environmentVariables[1])
    .WithEnvironment("POWERENV_DB_PORT", environmentVariables[2])
    .WithEnvironment("POWERENV_HOST_IPADDRESS", environmentVariables[3])
    .WaitForCompletion(initBootstrap);

builder.Build().Run();