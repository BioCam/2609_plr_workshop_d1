using System.Collections.Generic;

namespace PlateReader.Client.Configuration;

public sealed class ApplicationSettings
{
	public DisplaySettings Display { get; } = new DisplaySettings();

	public ExportSettings Export { get; } = new ExportSettings();

	public HistorySettings History { get; } = new HistorySettings();

	public QualitySettings Quality { get; } = new QualitySettings();

	public PrintingSettings Printing { get; } = new PrintingSettings();

	public AcquisitionSettings Acquisition { get; } = new AcquisitionSettings();

	public IReadOnlyList<string> Validate()
	{
		List<string> list = new List<string>();
		list.AddRange(Display.Validate());
		list.AddRange(Export.Validate());
		list.AddRange(History.Validate());
		list.AddRange(Quality.Validate());
		list.AddRange(Printing.Validate());
		list.AddRange(Acquisition.Validate());
		return list;
	}

	public void Reset()
	{
		Display.Reset();
		Export.Reset();
		History.Reset();
		Quality.Reset();
		Printing.Reset();
		Acquisition.Reset();
	}
}
