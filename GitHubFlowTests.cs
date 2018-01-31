using GitVersion.Tests.config;
using Xunit;

namespace GitVersion.Tests
{
    public class GitHubFlowTests
    {
        /// <summary>
        ///     *---B       feature/f-1
        ///    /     \
        ///   *-------C---- develop
        ///  /         \
        /// A-----------D-- master
        /// </summary>
        [Fact]
        public void FeatureReleaseVersioning()
        {
            var gitVersionConfig = GitVersionConfig.GitHubFlowConfig;

            var repo = GitRepository.Create(gitVersionConfig, "1.0.0");

            //   *-- develop
            //  /
            // A---- master
            repo.CreateBranch("develop", "master");

            var developBeforeCommitVersion = GitVersion.GetVersionInfo(repo); // A

            var featureBranchName = "feature/f-1";

            //     *-- feature/f-1
            //    /
            //   *---- develop
            //  /
            // A------ master
            repo.CreateBranch(featureBranchName, "develop");

            var featureBeforeCommitVersion = GitVersion.GetVersionInfo(repo);

            //     *--B feature/f-1
            //    /
            //   *----- develop
            //  /
            // A------- master
            repo.MakeChange();

            var featureFirstCommitVersion = GitVersion.GetVersionInfo(repo); // B

            //     *--B    feature/f-1
            //    /    \
            //   *------C  develop
            //  /
            // A---------  master
            repo.PullRequestMerge(featureBranchName, "develop", true);

            var developAfterFeatureMergeCommitVersion = GitVersion.GetVersionInfo(repo); // C

            repo.Tag("1.1.0");

            // 	   *---B         feature/f-1
            //    /     \
            //   *-------C----   develop
            //  /         \
            // A-----------D----  master
            repo.PullRequestMerge("develop", "master", false);
            
            var masterAfterDevelopMergeVersion = GitVersion.GetVersionInfo(repo);

            repo.CheckoutBranch("develop");

            // 	   *---B          feature/f-1
            //    /     \
            //   *-------C----E   develop
            //  /         \
            // A-----------D----  master
            repo.MakeChange();

            var developAfterTagAndCommitVersion = GitVersion.GetVersionInfo(repo);

            Assert.Equal("1.0.0", developBeforeCommitVersion.FullSemVer);
            Assert.Equal("1.0.0", featureBeforeCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.1", featureFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.1", developAfterFeatureMergeCommitVersion.FullSemVer);
            Assert.Equal("1.1.0", masterAfterDevelopMergeVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Develop}.1", developAfterTagAndCommitVersion.FullSemVer);
        }
    }
}
