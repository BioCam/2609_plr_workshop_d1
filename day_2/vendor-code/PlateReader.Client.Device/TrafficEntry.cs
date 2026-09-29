using System;

namespace PlateReader.Client.Device;

public sealed record TrafficEntry(int Sequence, string Direction, byte[] Data)
{
	public string Hex => Convert.ToHexString(Data);
}
