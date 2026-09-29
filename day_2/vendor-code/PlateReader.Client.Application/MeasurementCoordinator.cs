using System;
using System.Collections.Generic;
using PlateReader.Client.Device;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Application;

public sealed class MeasurementCoordinator
{
	private readonly PlateReaderDevice device;

	private readonly AuditTrail audit;

	public event Action<string>? Progress;

	public MeasurementCoordinator(PlateReaderDevice device, AuditTrail audit)
	{
		this.device = device;
		this.audit = audit;
	}

	public IReadOnlyList<PlateResult> Run(MeasurementMethod method, string sampleName)
	{
		method.Validate();
		if (device.GetState().IsOpen)
		{
			throw new InvalidOperationException("Close the plate reader before reading");
		}
		audit.Append("Method selected", method.Name);
		this.Progress?.Invoke("Preparing plate reader...");
		device.Reset();
		device.SetGain(method.Gain);
		if (method.Mode == MeasurementMode.Absorbance)
		{
			this.Progress?.Invoke("Set wavelength");
			device.SetWavelength(method.Wavelength);
		}
		InstrumentSnapshot state = device.GetState();
		if (method.Mode == MeasurementMode.Absorbance && !state.CanReadAbsorbance)
		{
			throw new InvalidOperationException("Instrument configuration is incomplete");
		}
		List<PlateResult> list = new List<PlateResult>();
		for (int i = 0; i < method.Replicates; i++)
		{
			this.Progress?.Invoke($"Reading plate... {i + 1}/{method.Replicates}");
			PlateResult plateResult = ((method.Mode == MeasurementMode.Absorbance) ? device.ReadAbsorbance(sampleName) : device.ReadLuminescence(sampleName));
			list.Add(plateResult);
			audit.Append("Measurement completed", $"{method.Mode}: {plateResult.Count} wells");
		}
		this.Progress?.Invoke("Ready");
		return list;
	}
}
