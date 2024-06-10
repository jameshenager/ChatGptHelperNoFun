namespace Helper.Core;

public class InternetMessage
{
    public int InternetMessageId { get; set; }
    public required string MasterCode { get; set; } //to retrieve all messages in bulk or deleting all messages, the master code is required
    public required string ShortCode { get; set; } //to retrieve the message from the webAPI, the short code is required
    public required string Message { get; set; }
    public byte[]? File { get; set; }
    public string? FileName { get; set; }
    public DateTime TimeStamp { get; set; }
    public bool IsDeleted { get; set; }
}

public class InternetMessageRetrieval //the master code shouldn't be exposed to the client
{
    public int InternetMessageId { get; set; }
    public required string ShortCode { get; set; }
    public required string Message { get; set; }
    public byte[]? File { get; set; }
    public string? FileName { get; set; }
    public DateTime TimeStamp { get; set; }
    public bool IsDeleted { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}