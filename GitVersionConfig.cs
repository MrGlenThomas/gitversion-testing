using System.IO;
using System.Reflection;

namespace GitVersion.Tests
{
    public class GitVersionConfig
    {
        private static string _gitFlowConfig;

        public static string GitFlowConfig => _gitFlowConfig ?? (_gitFlowConfig = Load("GitVersion.Tests.config.GitFlow.txt"));

        private static string _gitHubFlowConfig;

        public static string GitHubFlowConfig => _gitHubFlowConfig ?? (_gitHubFlowConfig = Load("GitVersion.Tests.config.GitHubFlow.txt"));

        public static string Load(string configResourceFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(configResourceFileName))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();

                return result;
            }
        }
    }
}
