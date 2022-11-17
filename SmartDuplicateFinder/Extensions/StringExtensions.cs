using System;
using System.IO;
using System.Linq;

namespace SmartDuplicateFinder.Extensions;

public static class StringExtensions
{
    public static string ShortenPathname(this string? pathname, int maxLength = 75)
    {
        if (string.IsNullOrWhiteSpace(pathname))
            return "";

        pathname = Path.GetFullPath(pathname);

        var root = Path.GetPathRoot(pathname) ?? "";
        if (root.Length > MaxDriveLetterRootSize)
        {
            // Is Unc Path, include...
            root += Path.DirectorySeparatorChar;
        }

        var curPathLength = root.Length + Dots.Length;
        maxLength = Math.Max(maxLength, curPathLength + 1);

        if (pathname.Length <= maxLength)
            return pathname;

        var segments = pathname.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Where(s => s != "").ToArray();
        var segmentCount = segments.Length;
        var frontSegment = -1;
        var endSegment = segmentCount;
        var frontEnd = true;

        if (curPathLength + 1 + segments[^1].Length > maxLength)
        {
            // rootPath + lastSegment is larger then maxLength
            // Trim the start of the last segment down.
            //
            var take = maxLength - curPathLength;
            var shortenPathName = root + Dots + segments[^1][^take..^1];
            return shortenPathName;
        }
        else
        {
            //
            // Try to keep as much of the front and end of the path as possible.
            //

            curPathLength = Dots.Length;

            var toggle = true;
            while (frontSegment < endSegment)
            {
                var segmentId = -1;
                if (frontEnd)
                {
                    frontSegment++;
                    segmentId = frontSegment;
                }
                else
                {
                    endSegment--;
                    segmentId = endSegment;
                }

                curPathLength += segments[segmentId].Length + 1;
                if (curPathLength > maxLength)
                {
                    if (frontEnd)
                    {
                        curPathLength -= segments[segmentId].Length + 1;
                        frontSegment--;
                    }
                    else
                    {
                        // Keep the last segment
                        //
                        if (endSegment + 1 < segmentCount)
                        {
                            curPathLength -= segments[segmentId].Length + 1;
                            endSegment++;
                        }
                    }

                    if (toggle)
                    {
                        frontEnd = !frontEnd;
                        toggle = false;
                    }
                    else
                    {
                        break;
                    }
                }

                if (toggle)
                {
                    frontEnd = !frontEnd;
                }
            }

            var shortenPathName = string.Join("" + Path.DirectorySeparatorChar, segments.Take(frontSegment + 1)) 
              + Path.DirectorySeparatorChar + Dots + Path.DirectorySeparatorChar 
              + string.Join("" + Path.DirectorySeparatorChar, segments.Skip(endSegment));

            return shortenPathName;
        }
    }

    private const int MaxDriveLetterRootSize = 3;
    private const int MinPathLength = MaxDriveLetterRootSize + 8 + 3;
    private const string Dots = "...";
}
