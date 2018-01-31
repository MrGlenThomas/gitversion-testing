using System;
using System.Diagnostics;
using System.IO;

namespace GitVersion.Tests
{
    public class GitRepository
    {
        public static GitRepository Create(string gitVersionConfig = null, string initialVersion = "1.0.0")
        {
            return new GitRepository(gitVersionConfig, initialVersion);
        }

        public GitRepository(string gitVersionConfig, string initialVersion)
        {
            RepoName = Guid.NewGuid().ToString();
            var repoDirectory = Directory.CreateDirectory($"{Path.GetTempPath()}\\GitVersionTests\\{RepoName}");
            RepoPath = repoDirectory.FullName;
            FileName = $"{Guid.NewGuid().ToString()}.txt";
            FilePath = $"{RepoPath}\\{FileName}";
            File.WriteAllText(FilePath, "Line1");

            if (!string.IsNullOrEmpty(gitVersionConfig))
            {
                var gitVersionConfigFilePath = $"{RepoPath}\\GitVersion.yml";

                File.WriteAllText(gitVersionConfigFilePath, gitVersionConfig);
            }

            InvokeGitCommand("init");
            InvokeGitCommand("add .");
            InvokeGitCommand("commit -m \"initial commit\"");
            InvokeGitCommand($"tag \"v{initialVersion}\"");
        }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string RepoPath { get; set; }

        public string RepoName { get; set; }

        private void InvokeGitCommand(string arguments)
        {
            var processStartInfo = new ProcessStartInfo("git", arguments)
            {
                WorkingDirectory = RepoPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                ErrorDialog = true
            };

            var gitProcess = Process.Start(processStartInfo);

            var processOutput = gitProcess.StandardOutput.ReadToEnd();
            var processError = gitProcess.StandardError.ReadToEnd();
            gitProcess.WaitForExit();

            Console.WriteLine(processOutput);
            Console.WriteLine(processError);
        }

        public void Tag(string tagName)
        {
            InvokeGitCommand($"tag {tagName}");
        }

        public void CreateBranch(string branchName, string fromBranch)
        {
            InvokeGitCommand($"checkout {fromBranch} -b {branchName}");
        }

        public void MakeChange(string branchName = null)
        {
            if (!string.IsNullOrWhiteSpace(branchName))
            {
                CheckoutBranch(branchName);
            }

            File.AppendAllText(FilePath, "Change");
            InvokeGitCommand("add .");
            InvokeGitCommand("commit -m \"text file change\"");
        }

        public void PullRequestMerge(string fromBranch, string toBranch, bool deleteAfterMerge, string commitMessage = null)
        {
            MergeBranch(fromBranch, toBranch);
            if (deleteAfterMerge)
            {
                DeleteBranch(fromBranch);
            }
        }

        public void MergeBranch(string fromBranch, string toBranch, string commitMessage = null)
        {
            CheckoutBranch(toBranch);
            InvokeGitCommand($"merge {fromBranch}");
            InvokeGitCommand("add .");
            string message = string.IsNullOrWhiteSpace(commitMessage)
                ? "\"merge from {fromBranch} to {toBranch}\""
                : commitMessage;
            InvokeGitCommand($"commit -m {message}");
        }

        public void CheckoutBranch(string branchName)
        {
            InvokeGitCommand($"checkout {branchName}");
        }

        public void DeleteBranch(string branchName)
        {
            InvokeGitCommand($"branch -d {branchName}");
        }
    }
}
