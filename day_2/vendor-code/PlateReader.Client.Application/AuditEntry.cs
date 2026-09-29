namespace PlateReader.Client.Application;

public sealed record AuditEntry(int Number, string Action, string Detail);
