using System;
using System.IO;

namespace API_HUNT.Models
{
    public static class FileSecurityHelper
    {
        /// <summary>
        /// Sanitizes a user-supplied file name and resolves it against an allowed base path.
        /// Strips all directory components and verifies the result stays within the base directory.
        /// </summary>
        public static string GetSafePath(string fileName, string allowedBasePath)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.");

            string sanitized = Path.GetFileName(fileName);

            if (string.IsNullOrWhiteSpace(sanitized))
                throw new ArgumentException("Invalid file name.");

            string fullPath = Path.GetFullPath(Path.Combine(allowedBasePath, sanitized));
            string normalizedBase = Path.GetFullPath(allowedBasePath);

            if (!normalizedBase.EndsWith(Path.DirectorySeparatorChar.ToString()))
                normalizedBase += Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Access denied.");

            return fullPath;
        }
    }
}
