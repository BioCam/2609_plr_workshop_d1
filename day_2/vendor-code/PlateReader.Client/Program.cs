using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using PlateReader.Client.Application;
using PlateReader.Client.Configuration;
using PlateReader.Client.Device;
using PlateReader.Client.Domain;
using PlateReader.Client.Presentation;
using PlateReader.Client.Reporting;

namespace PlateReader.Client;

public static class Program
{
	public static int Main(string[] args)
	{
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		if (args.Length < 2 || args[0] != "demo")
		{
			Console.WriteLine("PlateReader.Client demo <firmware_bridge.py> [output-directory]");
			return 2;
		}
		try
		{
			using ProcessTransport transport = new ProcessTransport("python3", args[1]);
			InstrumentConnection instrumentConnection = new InstrumentConnection(transport);
			PlateReaderDevice device = new PlateReaderDevice(instrumentConnection);
			string fullPath = Path.GetFullPath((args.Length > 2) ? args[2] : "client-output");
			Directory.CreateDirectory(fullPath);
			if (new ApplicationSettings().Validate().Count != 0)
			{
				throw new InvalidOperationException("Invalid application settings");
			}
			MethodRepository methodRepository = new MethodRepository();
			methodRepository.Save(new MeasurementMethod("Luminescence overview", MeasurementMode.Luminescence, 600, 1, 1));
			methodRepository.Save(new MeasurementMethod("Absorbance 600 nm", MeasurementMode.Absorbance, 600, 1, 1));
			AuditTrail auditTrail = new AuditTrail();
			AcquisitionScreen acquisitionScreen = new AcquisitionScreen(new MeasurementCoordinator(device, auditTrail));
			ReportComposer reportComposer = ReportCatalog.CreateDefault();
			foreach (MeasurementMethod item in methodRepository.List())
			{
				acquisitionScreen.Editor.Load(item);
				PlateResult plateResult = acquisitionScreen.ReadPlateButtonClick("Workshop sample")[0];
				acquisitionScreen.Grid.SelectRow(0);
				string text = Path.Combine(fullPath, plateResult.Mode.ToString());
				File.WriteAllText(text + ".csv", new PlateCsvWriter().Write(plateResult));
				File.WriteAllText(text + ".json", new PlateJsonWriter().Write(plateResult));
				File.WriteAllText(text + ".txt", new PlateTextWriter().Write(plateResult));
				File.WriteAllText(text + ".report.txt", reportComposer.Compose(plateResult));
				Console.WriteLine($"{plateResult.Mode}: {plateResult.Count} wells, A1={plateResult[0]:G17}, H12={plateResult[95]:G17}");
			}
			File.WriteAllText(Path.Combine(fullPath, "traffic.json"), JsonSerializer.Serialize(instrumentConnection.Traffic.Select((TrafficEntry entry) => new { entry.Sequence, entry.Direction, entry.Hex }), new JsonSerializerOptions
			{
				WriteIndented = true
			}));
			File.WriteAllText(Path.Combine(fullPath, "audit.json"), JsonSerializer.Serialize(auditTrail.Entries, new JsonSerializerOptions
			{
				WriteIndented = true
			}));
			Console.WriteLine("Reports and raw traffic saved to " + fullPath);
			return 0;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex.Message);
			return 1;
		}
	}
}
