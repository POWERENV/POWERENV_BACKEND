using ASPIRE_DEVENV_SETUP_MANAGEMENT;

#region REQUIREMENTS_CHECKUP

string? executionMode = Environment.GetEnvironmentVariable("POWERENV_EXECUTION_MODE");

switch (executionMode)
{
    case null:
        throw new Exception("POWERENV_EXECUTION_MODE environment variable is not set. Please set the environment variable and restart the application.");

    case "DEVELOPMENT":
        string? targetMachineIPAddress = null;
        string? targetUserName = null;

        DOCKER_CONTEXT_VERIFIER dockerContextVerifier = new DOCKER_CONTEXT_VERIFIER();
        SSH_TUNNEL_VERIFIER sshTunnelVerifier = new SSH_TUNNEL_VERIFIER();

        if (!dockerContextVerifier.CheckCompliance(new DockerContextComplianceCheckArgs("vmware-docker")))
        {
            Console.Write("Please provide the target machine IP address: ");

            while (targetMachineIPAddress == null)
            {
                targetMachineIPAddress = Console.ReadLine();
            }

            Console.Write("Please provide the target machine username: ");

            while (targetUserName == null)
            {
                targetUserName = Console.ReadLine();
            }

            dockerContextVerifier.SetupRequirement(new DockerContextCreateArgs("vmware-docker", targetMachineIPAddress, targetUserName));
        }

        if (!sshTunnelVerifier.CheckCompliance(new SSHTunnelComplianceCheckArgs()))
        {
            if (targetMachineIPAddress == null && targetUserName == null)
            {
                Console.Write("Please provide the target machine IP address: ");

                while (targetMachineIPAddress == null)
                {
                    targetMachineIPAddress = Console.ReadLine();
                }

                Console.Write("Please provide the target machine username: ");

                while (targetUserName == null)
                {
                    targetUserName = Console.ReadLine();
                }
            }

            sshTunnelVerifier.SetupRequirement(new SSHTunnelCreateArgs(targetMachineIPAddress, targetUserName));
        }

        break;

    case "PRODUCTION":
        break;

    default:
        throw new Exception($"Invalid POWERENV_EXECUTION_MODE value: {executionMode}. Please set it to either 'DEVELOPMENT' or 'PRODUCTION'.");
}

#endregion

#region APP_ORCHESTRATION

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> redisCache = builder.AddRedis("RedisCache").WithContainerRuntimeArgs("-p", "6379:6379"); // Maps all interfaces (0.0.0.0) by default

builder.AddProject<Projects.POWERENV_BACKEND_API>("MAIN-API").WithReference(redisCache);

builder.AddProject<Projects.POWERENV_OSCONSOLE_WORKER>("OSCONSOLE-WORKER").WithReference(redisCache);

builder.Build().Run();

#endregion