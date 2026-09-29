using System;
using System.IO;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Device;

public sealed class PlateReaderDevice
{
	private readonly InstrumentConnection connection;

	public PlateReaderDevice(InstrumentConnection connection)
	{
		this.connection = connection;
	}

	public void Open()
	{
		connection.Exchange(Command.Open, 0);
	}

	public void Close()
	{
		connection.Exchange(Command.Close, 0);
	}

	public void Reset()
	{
		connection.Exchange(Command.Reset, 0);
	}

	public void SetWavelength(ushort nanometers)
	{
		if (nanometers < 340 || nanometers > 850)
		{
			throw new ArgumentOutOfRangeException("nanometers");
		}
		if (PacketCodec.ReadUInt16(connection.Exchange(Command.SetWavelength, nanometers), 4) != nanometers)
		{
			throw new InvalidDataException("Wavelength acknowledgement mismatch");
		}
	}

	public void SetGain(ushort gain)
	{
		if (gain < 1 || gain > 16)
		{
			throw new ArgumentOutOfRangeException("gain");
		}
		if (PacketCodec.ReadUInt16(connection.Exchange(Command.SetGain, gain), 4) != gain)
		{
			throw new InvalidDataException("Gain acknowledgement mismatch");
		}
	}

	public InstrumentSnapshot GetState()
	{
		byte[] array = connection.Exchange(Command.GetState, 0);
		ushort num = PacketCodec.ReadUInt16(array, 4);
		return new InstrumentSnapshot((num == 0) ? ((ushort?)null) : new ushort?(num), PacketCodec.ReadUInt16(array, 6), (array[8] & 1) != 0, array[9] == 1);
	}

	public PlateResult ReadLuminescence(string sampleName)
	{
		ushort[] array = PacketCodec.ReadArray(connection.Exchange(Command.ReadLuminescence, 0), 4, 96);
		double[] array2 = new double[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = (int)array[i];
		}
		return new PlateResult(sampleName, MeasurementMode.Luminescence, "RLU", array2);
	}

	public PlateResult ReadAbsorbance(string sampleName)
	{
		byte[] buffer = connection.Exchange(Command.ReadAbsorbance, 0);
		ushort[] array = PacketCodec.ReadArray(buffer, 4, 96);
		ushort[] array2 = PacketCodec.ReadArray(buffer, 196, 96);
		double[] array3 = new double[96];
		for (int i = 0; i < array3.Length; i++)
		{
			if (array2[i] == 0)
			{
				throw new InvalidDataException($"Zero reference at well {WellAddress.FromIndex(i)}");
			}
			array3[i] = (double)(int)array[i] / (double)(int)array2[i];
		}
		return new PlateResult(sampleName, MeasurementMode.Absorbance, "ratio", array3);
	}
}
