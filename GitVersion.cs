using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace GitVersion.Tests
{
    public class GitVersion
    {
        public static GitVersionOutput GetVersionInfo(GitRepository repository)
        {
            var gitVersionPath = Path.GetFullPath("..\\..\\GitVersion_4.0.0-beta0012\\GitVersion.exe");

            var processStartInfo = new ProcessStartInfo(gitVersionPath)
            {
                WorkingDirectory = repository.RepoPath,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var gitVersionProcess = new Process {StartInfo = processStartInfo};
            gitVersionProcess.Start();

            var processOutput = gitVersionProcess.StandardOutput.ReadToEnd();
            gitVersionProcess.WaitForExit();

            var versionInfo = JsonConvert.DeserializeObject<GitVersionOutput>(processOutput);

            return versionInfo;
        }
    }
}
