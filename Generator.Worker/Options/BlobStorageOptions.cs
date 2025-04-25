namespace Generator.Worker.Options;

/// <summary>
/// Configuration options for the blob storage service
/// </summary>
public record BlobStorageOptions
{
    /// <summary>
    /// Config file section
    /// </summary>
    public static readonly string Section = "BlobStorage";

    /// <summary>
    /// Endpoint for blob storage
    /// </summary>
    /// <remarks>
    /// Not required if a connection string is used
    /// </remarks>
    public Uri? Endpoint { get; init; } = null;

    /// <summary>
    /// Blob container to store files
    /// </summary>
    public string Container { get; init; } = string.Empty;

    /// <summary>
    /// Folder to store files
    /// </summary>
    public string Folder { get; init; } = string.Empty;
}
