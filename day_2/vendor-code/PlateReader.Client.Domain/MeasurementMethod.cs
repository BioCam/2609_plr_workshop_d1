using System;

namespace PlateReader.Client.Domain;

public sealed record MeasurementMethod(string Name, MeasurementMode Mode, ushort Wavelength, ushort Gain, int Replicates)
{
	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			throw new ArgumentException("Method name is required");
		}
		if (Gain < 1 || Gain > 16)
		{
			throw new ArgumentException("Gain must be between 1 and 16");
		}
		if (Mode == MeasurementMode.Absorbance && (Wavelength < 340 || Wavelength > 850))
		{
			throw new ArgumentException("Wavelength must be between 340 and 850 nm");
		}
		if (Replicates < 1 || Replicates > 20)
		{
			throw new ArgumentException("Replicates must be between 1 and 20");
		}
	}
}
