namespace BlobBot.Shared.Interfaces
{
    public enum Status
    {
        New,
        Downloading,
        Processing,
        Succeeded,
        ProcessFailed,
        DownloadFailed
    }

    public interface IBlobInfo
    {
        string Container { get; }
        string Name { get; }
        DateTime DateCreated { get; }
        long Length { get; }
        string ContentType { get; }
        Status Status { get; set; }
        string ProcessedBy { get; set; }
        DateTime? StatusDateTime { get; set; }
        string ErrorMessage { get; set; }
    }
}
