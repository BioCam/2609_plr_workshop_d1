using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PlateReader.Client.Device;

public sealed class ProcessTransport : IByteTransport, IDisposable
{
	private readonly Process process;

	private readonly Stream input;

	private readonly Stream output;

	private readonly Task<string> stderr;

	public ProcessTransport(string python, string bridge)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo(python)
		{
			RedirectStandardInput = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false
		};
		processStartInfo.ArgumentList.Add("-u");
		processStartInfo.ArgumentList.Add(Path.GetFullPath(bridge));
		process = Process.Start(processStartInfo) ?? throw new IOException("Unable to start firmware test process");
		input = process.StandardInput.BaseStream;
		output = process.StandardOutput.BaseStream;
		stderr = process.StandardError.ReadToEndAsync();
	}

	public void Write(byte[] data)
	{
		input.Write(data);
		input.Flush();
	}

	public byte[] ReadExactly(int count)
	{
		byte[] array = new byte[count];
		int i = 0;
		using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5L));
		try
		{
			int result;
			for (; i < count; i += result)
			{
				result = output.ReadAsync(array.AsMemory(i), cancellationTokenSource.Token).AsTask().GetAwaiter()
					.GetResult();
				if (result == 0)
				{
					throw new EndOfStreamException("Firmware transport closed mid-response");
				}
			}
		}
		catch (OperationCanceledException innerException)
		{
			throw new TimeoutException("Timed out waiting for plate reader response", innerException);
		}
		return array;
	}

	public void Dispose()
	{
		input.Dispose();
		if (!process.WaitForExit(2000))
		{
			process.Kill(entireProcessTree: true);
			process.WaitForExit();
		}
		string result = stderr.GetAwaiter().GetResult();
		if (!string.IsNullOrWhiteSpace(result))
		{
			Console.Error.WriteLine(result);
		}
		output.Dispose();
		process.Dispose();
	}
}
