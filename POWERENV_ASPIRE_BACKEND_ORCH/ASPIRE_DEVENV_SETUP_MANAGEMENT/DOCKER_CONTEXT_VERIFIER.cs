using System.Diagnostics;

namespace ASPIRE_DEVENV_SETUP_MANAGEMENT
{
    internal record DockerContextComplianceCheckArgs(string contextName);
    internal record DockerContextCreateArgs(string contextName, string targetMachineIPAddress, string targetUserName);

    internal class DOCKER_CONTEXT_VERIFIER : IDEVENV_SETUP_REQUIREMENT_VERIFIER<DockerContextComplianceCheckArgs, DockerContextCreateArgs>
    {
        public bool CheckCompliance(DockerContextComplianceCheckArgs complianceFilter)
        {
            // Sanitize input to protect against shell injection arguments
            if (string.IsNullOrWhiteSpace(complianceFilter.contextName) || complianceFilter.contextName.Any(c => char.IsWhiteSpace(c) || c == ';'))
            {
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"context inspect {complianceFilter.contextName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = Process.Start(startInfo);
                if (process == null) return false;

                // Wait for the command to finish execution
                process.WaitForExit();

                // Exit code 0 means the context exists and was successfully inspected
                return process.ExitCode == 0;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Thrown if the "docker" command-line tool is not installed on the host
                throw new Exception("The Docker CLI tool is not installed or accessible in the environment path.");
            }
        }

        public bool SetupRequirement(DockerContextCreateArgs setupParameters)
        {
            // Sanitize input to protect against shell injection arguments
            if (string.IsNullOrWhiteSpace(setupParameters.contextName) || setupParameters.contextName.Any(c => char.IsWhiteSpace(c) || c == ';'))
            {
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"context create {setupParameters.contextName} --description \"DEV WORKSPACE\" --docker \"host=ssh://{setupParameters.targetUserName}@{setupParameters.targetMachineIPAddress}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = Process.Start(startInfo);
                if (process == null) return false;

                // Read errors to help debug failed creations
                string errorOutput = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    // Common errors include context already existing or invalid flags
                    throw new Exception($"Docker command failed with code {process.ExitCode}: {errorOutput}");
                }

                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Thrown if the "docker" command-line tool is not installed on the host
                throw new Exception("The Docker CLI tool is not installed or accessible in the environment path.");
            }
        }
    }
}