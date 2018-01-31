namespace GitVersion.Tests
{
    public class GitVersionOutput
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string PreReleaseTag { get; set; }
        public string PreReleaseTagWithDash { get; set; }
        public string PreReleaseLabel { get; set; }
        public int? PreReleaseNumber { get; set; }
        public string MajorMinorPatch { get; set; }
        public string SemVer { get; set; }
        public string LegacySemVer { get; set; }
        public string LegacySemVerPadded { get; set; }
        public string AssemblySemVer { get; set; }
        public string AssemblySemFileVer { get; set; }
        public string FullSemVer { get; set; }
        public string BranchName { get; set; }
        public string NuGetVersionV2 { get; set; }
        public string NuGetVersion { get; set; }
        public string NuGetPreReleaseTagV2 { get; set; }
        public string NuGetPreReleaseTag { get; set; }
        public string CommitsSinceVersionSource { get; set; }
        public string CommitsSinceVersionSourcePadded { get; set; }
        public string CommitDate { get; set; }
    }
}
