using GitVersion.Tests.config;
using Xunit;

namespace GitVersion.Tests
{
    public class DevelopmentTests
    {
        /// <summary>
        /// 	*---*     feature/f-1
        ///    /     \
        ///   *-------*-- develop
        ///  /
        /// *------------ master
        /// </summary>
        [Fact]
        public void SimpleFeatureBranchVersioning()
        {
            var gitVersionConfig = GitVersionConfig.GitFlowConfig;

            var repo = GitRepository.Create(gitVersionConfig);
            repo.CreateBranch(StaticBranches.Develop, StaticBranches.Master);

            var masterFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange();

            var developFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            var featureBranchName = "feature/f-1";

            repo.CreateBranch(featureBranchName, StaticBranches.Develop);
            repo.MakeChange();

            var featureFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange();

            var featureSecondCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.PullRequestMerge(featureBranchName, StaticBranches.Develop, true);

            var developAfterFeatureMergeCommitVersion = GitVersion.GetVersionInfo(repo);
            
            Assert.Equal("1.0.0", masterFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.1", developFirstCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.2", featureFirstCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.3", featureSecondCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.3", developAfterFeatureMergeCommitVersion.FullSemVer);
        }

        /// <summary>
        /// 	*---*     feature/f-1
        ///    /     \
        ///   *---*---*-- develop
        ///  /
        /// *------------ master
        /// </summary>
        [Fact]
        public void FeatureBranchWithCommitOnDevelopVersioning()
        {
            var gitVersionConfig = GitVersionConfig.GitFlowConfig;

            var repo = GitRepository.Create(gitVersionConfig);
            repo.CreateBranch(StaticBranches.Develop, StaticBranches.Master);

            var masterFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange();

            var developFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            var featureBranchName = "feature/f-1";

            repo.CreateBranch(featureBranchName, StaticBranches.Develop);
            repo.MakeChange();

            var featureFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.CheckoutBranch(StaticBranches.Develop);
            repo.MakeChange();

            var developSecondCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.CheckoutBranch(featureBranchName);
            repo.MakeChange();

            var featureSecondCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MergeBranch(StaticBranches.Develop, featureBranchName);

            repo.PullRequestMerge(featureBranchName, StaticBranches.Develop, true);

            var developAfterFeatureMergeCommitVersion = GitVersion.GetVersionInfo(repo);

            Assert.Equal("1.0.0", masterFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.1", developFirstCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.2", featureFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.2", developSecondCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.3", featureSecondCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.5", developAfterFeatureMergeCommitVersion.FullSemVer);
        }

        /// <summary>
        /// 	  *---*          feature/f-1
        ///      /     \
        ///     *---*---*---------* develop
        ///    /         \       /
        ///   /           *--*--*   release
        ///  /
        /// *------------ master
        /// </summary>
        [Fact]
        public void FeatureBranchWithReleaseVersioning()
        {
            var gitVersionConfig = GitVersionConfig.GitFlowConfig;

            var repo = GitRepository.Create(gitVersionConfig);
            repo.CreateBranch(StaticBranches.Develop, StaticBranches.Master);

            var masterFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange();

            var developFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            var featureBranchName = "feature/f-1";

            repo.CreateBranch(featureBranchName, StaticBranches.Develop);
            repo.MakeChange();

            var featureFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.CheckoutBranch(StaticBranches.Develop);
            repo.MakeChange();

            var developSecondCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.CheckoutBranch(featureBranchName);
            repo.MakeChange();

            var featureSecondCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MergeBranch(StaticBranches.Develop, featureBranchName);

            repo.PullRequestMerge(featureBranchName, StaticBranches.Develop, true);

            var developAfterFeatureMergeCommitVersion = GitVersion.GetVersionInfo(repo);

            var releaseBranchName = "release/1.1.0";

            repo.CreateBranch(releaseBranchName, StaticBranches.Develop);

            var releaseBeforeCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange();
            repo.Tag($"1.1.0-{PreReleaseTags.Release}.1");

            var releaseFirstCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.PullRequestMerge(releaseBranchName, StaticBranches.Develop, true);

            var developAfterReleaseMergeVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange();

            var developAfterReleaseMergeAndCommitVersion = GitVersion.GetVersionInfo(repo);

            Assert.Equal("1.0.0", masterFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.1", developFirstCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.2", featureFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.2", developSecondCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.3", featureSecondCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.5", developAfterFeatureMergeCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.0", releaseBeforeCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.1", releaseFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.1", developAfterReleaseMergeVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Develop}.1", developAfterReleaseMergeAndCommitVersion.FullSemVer);
        }

        /// <summary>
        /// 	  *---B   *------F1-----F2      feature/*
        ///      /     \ /                \
        ///     *-------C-----C1-------E---G    develop
        ///    /         \            /
        ///   /           *--D1--D2--+          release
        ///  /
        /// A------------ master
        /// </summary>
        [Fact]
        public void FeatureReleaseVersioning()
        {
            var gitVersionConfig = GitVersionConfig.GitFlowConfig;

            var repo = GitRepository.Create(gitVersionConfig);

            //   *-- develop
            //  /
            // A---- master
            repo.CreateBranch(StaticBranches.Develop, StaticBranches.Master);

            var developBeforeCommitVersion = GitVersion.GetVersionInfo(repo); // A

            var featureBranchName = "feature/f-1";

            //     *-- feature/f-1
            //    /
            //   *---- develop
            //  /
            // A------ master
            repo.CreateBranch(featureBranchName, StaticBranches.Develop);

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
            repo.PullRequestMerge(featureBranchName, StaticBranches.Develop, true);

            var developAfterFeatureMergeCommitVersion = GitVersion.GetVersionInfo(repo); // C

            var releaseBranchName = "release/1.1.0";

            //   	 *---B        feature/*
            //      /     \
            //     *-------C----  develop
            //    /         \
            //   /           *--  release
            //  /
            // A----------------  master
            repo.CreateBranch(releaseBranchName, StaticBranches.Develop);

            var releaseBeforeCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange(); // release/1.1.0

            var releaseFirstCommitVersion = GitVersion.GetVersionInfo(repo); // D1

            repo.MakeChange();

            var releaseSecondCommitVersion = GitVersion.GetVersionInfo(repo); // D2

            repo.Tag($"1.1.0-{PreReleaseTags.Release}.2");

            var releaseAfterTagVersion = GitVersion.GetVersionInfo(repo);

            var featureBranch2Name = "feature/f-2";

            repo.CreateBranch(featureBranch2Name, StaticBranches.Develop);

            var feature2BeforeCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange();

            var feature2FirstCommitVersion = GitVersion.GetVersionInfo(repo); // F1

            repo.MakeChange();

            var feature2SecondCommitVersion = GitVersion.GetVersionInfo(repo); // F2

            repo.CheckoutBranch(StaticBranches.Develop);
            repo.MakeChange();

            var developCommitBeforeReleaseMerge = GitVersion.GetVersionInfo(repo); // C1

            repo.PullRequestMerge(releaseBranchName, StaticBranches.Develop, true);

            var developAfterReleaseMergeVersion = GitVersion.GetVersionInfo(repo);  // E

            repo.PullRequestMerge(featureBranch2Name, StaticBranches.Develop, true);

            var developAfterFeature2MergeCommitVersion = GitVersion.GetVersionInfo(repo); // G

            Assert.Equal("1.0.0", developBeforeCommitVersion.FullSemVer);
            Assert.Equal("1.0.0", featureBeforeCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.1", featureFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.1", developAfterFeatureMergeCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.0", releaseBeforeCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.1", releaseFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.2", releaseSecondCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.2", releaseAfterTagVersion.FullSemVer);
            Assert.Equal("1.0.0-f-2.1", feature2BeforeCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-2.2", feature2FirstCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-2.3", feature2SecondCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.2", developCommitBeforeReleaseMerge.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Develop}.0", developAfterReleaseMergeVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Develop}.3", developAfterFeature2MergeCommitVersion.FullSemVer);
        }

        /// <summary>
        /// 	  *---B   *--F1--F2--F3      feature/*
        ///      /     \ /            \
        ///     *-------C-------C1-----G---Y--    develop
        ///    /         \                /
        ///   /           *--D1--D2--+   /       release
        ///  /                        \ /
        /// A--------------------------X------ master
        /// </summary>
        [Fact]
        public void DevelopFeaturesWithReleaseAndBackToDevelop()
        {
            var gitVersionConfig = GitVersionConfig.GitFlowConfig;

            var repo = GitRepository.Create(gitVersionConfig);

            //   *-- develop
            //  /
            // A---- master
            repo.CreateBranch(StaticBranches.Develop, StaticBranches.Master);

            var developBeforeCommitVersion = GitVersion.GetVersionInfo(repo); // A

            var featureBranchName = "feature/f-1";

            //     *-- feature/f-1
            //    /
            //   *---- develop
            //  /
            // A------ master
            repo.CreateBranch(featureBranchName, StaticBranches.Develop);

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
            repo.PullRequestMerge(featureBranchName, StaticBranches.Develop, true);

            var developAfterFeatureMergeCommitVersion = GitVersion.GetVersionInfo(repo); // C

            var releaseBranchName = "release/1.1.0";

            //   	 *---B        feature/*
            //      /     \
            //     *-------C----  develop
            //    /         \
            //   /           *--  release
            //  /
            // A----------------  master
            repo.CreateBranch(releaseBranchName, StaticBranches.Develop);

            var releaseBeforeCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange(); // release/1.1.0

            var releaseFirstCommitVersion = GitVersion.GetVersionInfo(repo); // D1

            repo.MakeChange();

            var releaseSecondCommitVersion = GitVersion.GetVersionInfo(repo); // D2

            repo.Tag("1.1.0");

            var releaseAfterTagVersion = GitVersion.GetVersionInfo(repo);

            repo.PullRequestMerge(releaseBranchName, StaticBranches.Master, true);

            var masterAfterReleaseMergeVersion = GitVersion.GetVersionInfo(repo);
            
            var featureBranch2Name = "feature/f-2";

            repo.CreateBranch(featureBranch2Name, StaticBranches.Develop);

            var feature2BeforeCommitVersion = GitVersion.GetVersionInfo(repo);

            repo.MakeChange();

            var feature2FirstCommitVersion = GitVersion.GetVersionInfo(repo); // F1

            repo.MakeChange();

            var feature2SecondCommitVersion = GitVersion.GetVersionInfo(repo); // F2

            repo.MakeChange();

            var feature2ThirdCommitVersion = GitVersion.GetVersionInfo(repo); // F3

            repo.MakeChange(StaticBranches.Develop);

            var developCommitBeforeFeature2Merge = GitVersion.GetVersionInfo(repo); // C1

            repo.PullRequestMerge(featureBranch2Name, StaticBranches.Develop, true);

            var developAfterfeature2MergeCommitVersion = GitVersion.GetVersionInfo(repo); // G

            repo.PullRequestMerge(StaticBranches.Master, StaticBranches.Develop, false);

            var developAfterMasterMergeVersion = GitVersion.GetVersionInfo(repo);

            Assert.Equal("1.0.0", developBeforeCommitVersion.FullSemVer);
            Assert.Equal("1.0.0", featureBeforeCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-1.1", featureFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.1", developAfterFeatureMergeCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.0", releaseBeforeCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.1", releaseFirstCommitVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Release}.2", releaseSecondCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-2.1", feature2BeforeCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-2.2", feature2FirstCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-2.3", feature2SecondCommitVersion.FullSemVer);
            Assert.Equal("1.0.0-f-2.4", feature2ThirdCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.2", developCommitBeforeFeature2Merge.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.6", developAfterfeature2MergeCommitVersion.FullSemVer);
            Assert.Equal("1.1.0", releaseAfterTagVersion.FullSemVer);
            Assert.Equal("1.1.0", masterAfterReleaseMergeVersion.FullSemVer);
            Assert.Equal($"1.1.0-{PreReleaseTags.Develop}.6", developAfterMasterMergeVersion.FullSemVer);
        }

        [Fact]
        public void HotfixBranchVersioning()
        {
            var gitVersionConfig = GitVersionConfig.GitFlowConfig;

            var repo = GitRepository.Create(gitVersionConfig);

            //   *-- develop
            //  /
            // A---- master
            repo.CreateBranch(StaticBranches.Develop, StaticBranches.Master);

            var developBeforeCommitVersion = GitVersion.GetVersionInfo(repo); // A

            var hotfixBranchName = "hotfix/1.0.1";

            //   *-- develop
            //  /
            // A---- master
            //  \
            //   *-- hotfix/1.0.1
            repo.CreateBranch(hotfixBranchName, StaticBranches.Master);

            //   *----  develop
            //  /
            // A------  master
            //  \
            //   *--B-  hotfix/1.0.1
            repo.MakeChange(hotfixBranchName);

            var hotfixFirstCommitVersion = GitVersion.GetVersionInfo(repo); // B

            repo.Tag("1.0.1");

            //   *------  develop
            //  /
            // A------C-  master
            //  \    /
            //   *--B  hotfix/1.0.1
            repo.PullRequestMerge(hotfixBranchName, StaticBranches.Master, true);

            var masterAfterHotfixMergeCommitVersion = GitVersion.GetVersionInfo(repo); // C

            //   *--D---  develop
            //  /
            // A------C-  master
            //  \    /
            //   *--B  hotfix/1.0.1
            repo.MakeChange(StaticBranches.Develop);

            var developBeforeHotfixMasterMergeCommitVersion = GitVersion.GetVersionInfo(repo); // D

            //   *--D---E  develop
            //  /      /
            // A------C-  master
            //  \    /
            //   *--B  hotfix/1.0.1
            repo.PullRequestMerge(StaticBranches.Master, StaticBranches.Develop, false);

            var developAfterHotfixMasterMergeCommitVersion = GitVersion.GetVersionInfo(repo); // E

            Assert.Equal("1.0.0", developBeforeCommitVersion.FullSemVer);
            Assert.Equal($"1.0.1-{PreReleaseTags.Hotfix}.1", hotfixFirstCommitVersion.FullSemVer);
            Assert.Equal("1.0.1", masterAfterHotfixMergeCommitVersion.FullSemVer);
            Assert.Equal($"1.0.0-{PreReleaseTags.Develop}.1", developBeforeHotfixMasterMergeCommitVersion.FullSemVer);
            Assert.Equal($"1.0.1-{PreReleaseTags.Develop}.2", developAfterHotfixMasterMergeCommitVersion.FullSemVer);
        }
    }
}
