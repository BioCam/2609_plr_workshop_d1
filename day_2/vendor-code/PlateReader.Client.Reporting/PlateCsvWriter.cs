using System.Globalization;
using System.Text;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Reporting;

public sealed class PlateCsvWriter
{
	public string Write(PlateResult plate)
	{
		StringBuilder stringBuilder = new StringBuilder("Well,Value,Unit\n");
		foreach (var item in plate.Wells())
		{
			stringBuilder.Append(item.Well);
			stringBuilder.Append(',');
			stringBuilder.Append(item.Value.ToString("G17", CultureInfo.InvariantCulture));
			stringBuilder.Append(',');
			stringBuilder.AppendLine(plate.Unit);
		}
		return stringBuilder.ToString();
	}
}
