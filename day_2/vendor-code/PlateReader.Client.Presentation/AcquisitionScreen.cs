using System;
using System.Collections.Generic;
using PlateReader.Client.Application;
using PlateReader.Client.Device;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Presentation;

public sealed class AcquisitionScreen
{
	private readonly MeasurementCoordinator coordinator;

	public StatusBarModel Status { get; } = new StatusBarModel();

	public MethodEditorModel Editor { get; } = new MethodEditorModel();

	public PlateGridModel Grid { get; } = new PlateGridModel();

	public string? LastError { get; private set; }

	public AcquisitionScreen(MeasurementCoordinator coordinator)
	{
		this.coordinator = coordinator;
		coordinator.Progress += delegate(string message)
		{
			Status.Message = message;
		};
	}

	public IReadOnlyList<PlateResult> ReadPlateButtonClick(string sampleName)
	{
		if (Status.Busy)
		{
			throw new InvalidOperationException("An acquisition is already running");
		}
		Status.Busy = true;
		LastError = null;
		try
		{
			MeasurementMethod method = Editor.BuildMethod();
			IReadOnlyList<PlateResult> readOnlyList = coordinator.Run(method, sampleName);
			Grid.Load(readOnlyList[readOnlyList.Count - 1]);
			Status.Completed += readOnlyList.Count;
			return readOnlyList;
		}
		catch (InstrumentException ex)
		{
			LastError = ex.Message;
			Status.Message = "Instrument error";
			throw;
		}
		catch (Exception ex2)
		{
			LastError = ex2.Message;
			Status.Message = "Acquisition failed";
			throw;
		}
		finally
		{
			Status.Busy = false;
		}
	}
}
