namespace TVRename
{
    public class FileSystemProperties
    {
        public  FileSystemProperties(long? totalBytes, long? freeBytes, long? availableBytes)
        {
            TotalBytes = totalBytes;
            FreeBytes = freeBytes;
            AvailableBytes = availableBytes;
        }

        /// <summary>
        /// Gets the total number of bytes on the drive.
        /// </summary>
        public long? TotalBytes { get; }

        /// <summary>
        /// Gets the number of bytes free on the drive.
        /// </summary>
        public long? FreeBytes { get; }

        /// <summary>
        /// Gets the number of bytes available on the drive (counts disk quotas).
        /// </summary>
        public long? AvailableBytes { get; }
    }
}
