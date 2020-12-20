using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public struct SemVersion : IComparable, IComparable<SemVersion>, IEquatable<SemVersion>
{
    public static SemVersion Zero { get; } = new SemVersion(0,0,0, null, null, "0.0.0");

    static readonly Regex SemVerRegex =
        new Regex (
            @"(?<Major>0|(?:[1-9]\d*))(?:\.(?<Minor>0|(?:[1-9]\d*))(?:\.(?<Patch>0|(?:[1-9]\d*)))?(?:\-(?<PreRelease>[0-9A-Z\.-]+))?(?:\+(?<Meta>[0-9A-Z\.-]+))?)?",
            RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase
        );

    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }
    public string PreRelease { get; }
    public string Meta { get; }
    public bool IsPreRelease { get; }
    public bool HasMeta { get; }
    public string VersionString { get; }

    public SemVersion (int major, int minor, int patch, string preRelease = null, string meta = null) :
        this (major, minor, patch, preRelease, meta, null)
    {
    }

    private SemVersion (int major, int minor, int patch, string preRelease, string meta, string versionString)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        IsPreRelease = !string.IsNullOrEmpty (preRelease);
        HasMeta = !string.IsNullOrEmpty (meta);
        PreRelease = IsPreRelease ? preRelease : null;
        Meta = HasMeta ? meta : null;

        if (!string.IsNullOrEmpty (versionString)) {
            VersionString = versionString;
        } else {
            var sb = new StringBuilder ();
            sb.AppendFormat (CultureInfo.InvariantCulture, "{0}.{1}.{2}", Major, Minor, Patch);

            if (IsPreRelease) {
                sb.AppendFormat (CultureInfo.InvariantCulture, "-{0}", PreRelease);
            }

            if (HasMeta) {
                sb.AppendFormat (CultureInfo.InvariantCulture, "+{0}", Meta);
            }

            VersionString = sb.ToString ();
        }
    }

    public static bool TryParse (string version, out SemVersion semVersion)
    {
        semVersion = Zero;

        if (string.IsNullOrEmpty(version)) {
            return false;
        }

        var match = SemVerRegex.Match (version);
        if (!match.Success) {
            return false;
        }

        if (!int.TryParse (
                match.Groups["Major"].Value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var major) ||
            !int.TryParse (
                match.Groups["Minor"].Value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var minor) ||
            !int.TryParse (
                match.Groups["Patch"].Value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var patch)) {
            return false;
        }

        semVersion = new SemVersion (
            major,
            minor,
            patch,
            match.Groups["PreRelease"]?.Value,
            match.Groups["Meta"]?.Value,
            version);

        return true;
    }



    public bool Equals (SemVersion other)
    {
        return Major == other.Major
                && Minor == other.Minor
                && Patch == other.Patch
                && string.Equals(PreRelease, other.PreRelease, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Meta, other.Meta, StringComparison.OrdinalIgnoreCase);
    }

    public int CompareTo (SemVersion other)
    {
        if (Equals(other))
        {
            return 0;
        }

        if (Major > other.Major) {
            return 1;
        }

        if (Major < other.Major) {
            return -1;
        }

        if (Minor > other.Minor) {
            return 1;
        }

        if (Minor < other.Minor) {
            return -1;
        }

        if (Patch > other.Patch) {
            return 1;
        }

        if (Patch < other.Patch) {
            return -1;
        }

        switch(StringComparer.InvariantCultureIgnoreCase.Compare(PreRelease, other.PreRelease)) {
            case 1:
                return 1;

            case -1:
                return -1;

            default:
                return StringComparer.InvariantCultureIgnoreCase.Compare (Meta, other.Meta);
        }
    }

    public int CompareTo (object obj)
    {
        return (obj is SemVersion semVersion)
            ? CompareTo (semVersion)
            : -1;
    }

    public override bool Equals (object obj)
    {
        return (obj is SemVersion semVersion)
                && Equals (semVersion);
    }

    public override int GetHashCode ()
    {
        unchecked {
            var hashCode = Major;
            hashCode = (hashCode * 397) ^ Minor;
            hashCode = (hashCode * 397) ^ Patch;
            hashCode = (hashCode * 397) ^ (PreRelease != null ? StringComparer.OrdinalIgnoreCase.GetHashCode (PreRelease) : 0);
            hashCode = (hashCode * 397) ^ (Meta != null ? StringComparer.OrdinalIgnoreCase.GetHashCode (Meta) : 0);
            return hashCode;
        }
    }

    public override string ToString ()
        => VersionString;

    // Define the is greater than operator.
    public static bool operator > (SemVersion operand1, SemVersion operand2)
        => operand1.CompareTo (operand2) == 1;

    // Define the is less than operator.
    public static bool operator < (SemVersion operand1, SemVersion operand2)
        => operand1.CompareTo (operand2) == -1;

    // Define the is greater than or equal to operator.
    public static bool operator >= (SemVersion operand1, SemVersion operand2)
        => operand1.CompareTo (operand2) >= 0;

    // Define the is less than or equal to operator.
    public static bool operator <= (SemVersion operand1, SemVersion operand2)
        => operand1.CompareTo (operand2) <= 0;
}