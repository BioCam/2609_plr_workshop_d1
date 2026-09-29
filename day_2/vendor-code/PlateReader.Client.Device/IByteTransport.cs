using System;

namespace PlateReader.Client.Device;

public interface IByteTransport : IDisposable
{
	void Write(byte[] data);

	byte[] ReadExactly(int count);
}
