namespace PlateReader.Client.Device;

public sealed record InstrumentSnapshot(ushort? Wavelength, ushort Gain, bool WavelengthConfigured, bool IsOpen = false)
{
	public bool CanReadLuminescence => !IsOpen;

	public bool CanReadAbsorbance
	{
		get
		{
			if (CanReadLuminescence)
			{
				return WavelengthConfigured;
			}
			return false;
		}
	}
}
