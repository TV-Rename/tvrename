//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TVRename
{
    public class Release
    {
        public Version VersionNumber { get; }

        public enum VersionType
        {
            semantic,
            friendly
        }

        public string Prerelease { get; }
        public string Build { get; }

        public Release([NotNull] string version, VersionType type)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("The provided version string is invalid.", nameof(version));
            }

            string matchString = type == VersionType.semantic
                ? @"^v?(?<major>[0-9]+)((\.(?<minor>[0-9]+))(\.(?<patch>[0-9]+))?)?([ \-](?<pre>[0-9A-Za-z\- \.]+|[*]))?(\+(?<build>[0-9A-Za-z\-\.]+|[*]))?$"
                : @"^v?(?<major>[0-9]+)((\.(?<minor>[0-9]+))(\.(?<patch>[0-9]+))?)?([ \-](?<pre>[0-9A-Za-z\- \.]+))?$";

            Regex regex = new Regex(matchString, RegexOptions.ExplicitCapture);
            Match match = regex.Match(version);

            if (!match.Success || !match.Groups["major"].Success || !match.Groups["minor"].Success)
            {
                throw new ArgumentException("The provided version string is invalid.", nameof(version));
            }

            if (type == VersionType.semantic && !match.Groups["patch"].Success)
            {
                throw new ArgumentException("The provided version string is invalid semantic version.",
                    nameof(version));
            }

            VersionNumber = new Version(int.Parse(match.Groups["major"].Value),
                int.Parse(match.Groups["minor"].Value),
                match.Groups["patch"].Success ? int.Parse(match.Groups["patch"].Value) : 0);

            Prerelease = match.Groups["pre"].Value.Replace(" ", string.Empty);
            Build = match.Groups["build"].Value;
        }

        private int CompareTo(object? obj)
        {
            //Returns 1 if this > object, 0 if this=object and -1 if this< object
            if (obj is null)
            {
                return 1;
            }

            if (!(obj is Release otherUpdateVersion))
            {
                throw new ArgumentException("Object is not a UpdateVersion");
            }

            //Extract Version Numbers and then compare them
            if (VersionNumber.CompareTo(otherUpdateVersion.VersionNumber) != 0)
            {
                return VersionNumber.CompareTo(otherUpdateVersion.VersionNumber);
            }

            //We have the same version - now we have to get tricky and look at the extension (rc1, beta2 etc)
            //if both have no extension then they are the same
            if (string.IsNullOrWhiteSpace(Prerelease) && string.IsNullOrWhiteSpace(otherUpdateVersion.Prerelease))
            {
                return 0;
            }

            //If either are not present then you can assume they are FINAL versions and trump any rx1 verisons
            if (string.IsNullOrWhiteSpace(Prerelease))
            {
                return 1;
            }

            if (string.IsNullOrWhiteSpace(otherUpdateVersion.Prerelease))
            {
                return -1;
            }

            //We have 2 suffixes
            //Compare alphabetically alpha1 < alpha2 < beta1 < beta2 < rc1 < rc2 etc
            return string.Compare(Prerelease, otherUpdateVersion.Prerelease, StringComparison.OrdinalIgnoreCase);
        }

        public bool NewerThan(Release? compare) => CompareTo(compare) > 0;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(VersionNumber);
            if (!string.IsNullOrWhiteSpace(Prerelease))
            {
                sb.Append("-" + Prerelease);
            }

            if (!string.IsNullOrWhiteSpace(Build))
            {
                sb.Append("-(" + Build + ")");
            }

            return sb.ToString();
        }
    }
}
