namespace PlateReader.Client.Device;

public enum Command : byte
{
	SetWavelength = 1,
	GetState = 2,
	Reset = 4,
	Open = 6,
	Close = 7,
	ReadLuminescence = 32,
	ReadAbsorbance = 33,
	SetGain = 49
}
