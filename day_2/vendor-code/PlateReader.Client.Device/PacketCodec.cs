using System.IO;

namespace PlateReader.Client.Device;

public static class PacketCodec
{
	public static byte[] BuildCommand(Command command, ushort argument = 0)
	{
		return new byte[8]
		{
			167,
			3,
			(byte)command,
			0,
			(byte)(argument >> 8),
			(byte)(argument & 0xFF),
			0,
			255
		};
	}

	public static int GetResponseLength(Command command, byte status)
	{
		if (status != 0)
		{
			return 8;
		}
		return command switch
		{
			Command.GetState => 12, 
			Command.ReadLuminescence => 198, 
			Command.ReadAbsorbance => 390, 
			_ => 8, 
		};
	}

	public static void ValidateResponse(Command command, byte[] response)
	{
		if (response.Length < 8 || response[0] != 167 || response[1] != 131)
		{
			throw new InvalidDataException("Invalid response from device: header");
		}
		if ((uint)response[2] != (uint)command)
		{
			throw new InvalidDataException("Invalid response from device: command echo");
		}
		if (response.Length != GetResponseLength(command, response[3]))
		{
			throw new InvalidDataException("Invalid response from device: length");
		}
		if (response[^2] != 0 || response[^1] != byte.MaxValue)
		{
			throw new InvalidDataException("Invalid response from device: trailer");
		}
		if (response[3] != 0)
		{
			throw new InstrumentException(command, (ReplyStatus)response[3]);
		}
	}

	public static ushort ReadUInt16(byte[] buffer, int offset)
	{
		if (offset < 0 || offset + 1 >= buffer.Length)
		{
			throw new InvalidDataException("Truncated integer in instrument response");
		}
		return (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
	}

	public static ushort[] ReadArray(byte[] buffer, int offset, int count)
	{
		ushort[] array = new ushort[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = ReadUInt16(buffer, offset + i * 2);
		}
		return array;
	}
}
