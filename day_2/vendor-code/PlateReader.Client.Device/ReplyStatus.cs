namespace PlateReader.Client.Device;

public enum ReplyStatus : byte
{
	Ok = 0,
	BadCommand = 1,
	BadArgument = 2,
	NotConfigured = 5,
	BadFrame = 6,
	ReaderOpen = 7
}
