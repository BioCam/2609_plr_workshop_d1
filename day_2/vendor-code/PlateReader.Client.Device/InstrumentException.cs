using System;

namespace PlateReader.Client.Device;

public sealed class InstrumentException : Exception
{
	public Command Command { get; }

	public ReplyStatus Status { get; }

	public InstrumentException(Command command, ReplyStatus status)
		: base($"Instrument rejected {command}: {Describe(status)}")
	{
		Command = command;
		Status = status;
	}

	private static string Describe(ReplyStatus status)
	{
		return status switch
		{
			ReplyStatus.BadCommand => "Unknown command", 
			ReplyStatus.BadArgument => "Invalid parameter", 
			ReplyStatus.NotConfigured => "Set wavelength before absorbance", 
			ReplyStatus.ReaderOpen => "Close the plate reader before reading", 
			ReplyStatus.BadFrame => "Invalid request frame", 
			_ => $"Status {(byte)status}", 
		};
	}
}
