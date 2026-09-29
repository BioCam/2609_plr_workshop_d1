using System;
using System.Collections.Generic;
using System.IO;

namespace PlateReader.Client.Device;

public sealed class InstrumentConnection
{
	private readonly IByteTransport transport;

	private readonly List<TrafficEntry> traffic = new List<TrafficEntry>();

	private readonly object gate = new object();

	private int sequence;

	private bool faulted;

	public IReadOnlyList<TrafficEntry> Traffic => traffic.AsReadOnly();

	public event Action<TrafficEntry>? TrafficRecorded;

	private void RecordTraffic(string direction, byte[] data)
	{
		TrafficEntry trafficEntry = new TrafficEntry(++sequence, direction, data);
		traffic.Add(trafficEntry);
		this.TrafficRecorded?.Invoke(trafficEntry);
	}

	public InstrumentConnection(IByteTransport transport)
	{
		this.transport = transport;
	}

	public byte[] Exchange(Command command, ushort argument = 0)
	{
		lock (gate)
		{
			if (faulted)
			{
				throw new IOException("Reconnect after an incomplete or invalid exchange");
			}
			byte[] data = PacketCodec.BuildCommand(command, argument);
			try
			{
				transport.Write(data);
				RecordTraffic("TX", data);
				byte[] array = transport.ReadExactly(4);
				if (array[0] != 167 || array[1] != 131 || (uint)array[2] != (uint)command)
				{
					throw new InvalidDataException("Unexpected response header");
				}
				int responseLength = PacketCodec.GetResponseLength(command, array[3]);
				byte[] array2 = transport.ReadExactly(responseLength - 4);
				byte[] array3 = new byte[responseLength];
				Array.Copy(array, array3, 4);
				Array.Copy(array2, 0, array3, 4, array2.Length);
				RecordTraffic("RX", array3);
				PacketCodec.ValidateResponse(command, array3);
				return array3;
			}
			catch (InstrumentException)
			{
				throw;
			}
			catch
			{
				faulted = true;
				throw;
			}
		}
	}
}
