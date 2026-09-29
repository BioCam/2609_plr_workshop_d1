using System.Linq;
using System.Text.Json;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Reporting;

public sealed class PlateJsonWriter
{
	public string Write(PlateResult plate)
	{
		return JsonSerializer.Serialize(new
		{
			SampleName = plate.SampleName,
			Mode = plate.Mode.ToString(),
			Unit = plate.Unit,
			Wells = from item in plate.Wells()
				select new
				{
					Address = item.Well.ToString(),
					Value = item.Value
				}
		}, new JsonSerializerOptions
		{
			WriteIndented = true
		});
	}
}
