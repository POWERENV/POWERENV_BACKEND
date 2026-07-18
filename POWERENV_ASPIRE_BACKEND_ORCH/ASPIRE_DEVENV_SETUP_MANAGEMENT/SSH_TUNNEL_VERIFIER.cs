using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ASPIRE_DEVENV_SETUP_MANAGEMENT
{
    internal record SSHTunnelComplianceCheckArgs();
    internal record SSHTunnelCreateArgs(string targetMachineIPAddress, string targetUserName);

    internal class SSH_TUNNEL_VERIFIER : IDEVENV_SETUP_REQUIREMENT_VERIFIER<SSHTunnelComplianceCheckArgs, SSHTunnelCreateArgs>
    {
        public bool CheckCompliance(SSHTunnelComplianceCheckArgs complianceFilter)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CheckWindowsUsingCli();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return CheckLinuxUsingProc();
            }

            return false;
        }

        static private bool CheckLinuxUsingProc()
        {
            var sshProcesses = Process.GetProcessesByName("ssh");

            foreach (var proc in sshProcesses)
            {
                try
                {
                    string cmdlinePath = $"/proc/{proc.Id}/cmdline";
                    if (!File.Exists(cmdlinePath)) continue;

                    string cmdline = File.ReadAllText(cmdlinePath);
                    string fullCommand = cmdline.Replace('\0', ' ').Trim();

                    if (IsMatch(fullCommand)) return true;
                }
                catch
                {
                    continue; // Skip processes that close mid-check or lack read access
                }
            }
            return false;
        }

        static private bool CheckWindowsUsingCli()
        {
            // Query process arguments natively on Windows using PowerShell's Get-CimInstance
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -Command \"Get-CimInstance Win32_Process -Filter \\\"Name = 'ssh.exe'\\\" | Select-Object -ExpandProperty CommandLine\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using var process = Process.Start(startInfo);
                if (process == null) return false;

                // Read the command lines printed out by PowerShell
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Split lines in case there are multiple ssh.exe instances active
                string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (IsMatch(line)) return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        static private bool IsMatch(string commandLine)
        {
            return commandLine.Contains("-L") &&
                   commandLine.Contains("6379:localhost:6379");
        }

        public bool SetupRequirement(SSHTunnelCreateArgs setupParameters)
        {
            // Sanitize input to protect against shell injection arguments
            if (string.IsNullOrWhiteSpace(setupParameters.targetMachineIPAddress) || setupParameters.targetMachineIPAddress.Any(c => char.IsWhiteSpace(c) || c == ';'))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(setupParameters.targetUserName) || setupParameters.targetUserName.Any(c => char.IsWhiteSpace(c) || c == ';'))
            {
                return false;
            }
            var startInfo = new ProcessStartInfo
            {
                FileName = "ssh",
                Arguments = $"-L 6379:localhost:6379 {setupParameters.targetUserName}@{setupParameters.targetMachineIPAddress}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            try
            {
                var process = Process.Start(startInfo);
                if (process == null) return false;
                Thread.Sleep(2000); // Wait for 2 seconds.
                return CheckCompliance(new SSHTunnelComplianceCheckArgs()); //Check if the process is now running.
            }
            catch
            {
                return false;
            }
        }
    }
}