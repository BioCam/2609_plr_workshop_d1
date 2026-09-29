using System.Globalization;
using System.Text;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Reporting;

public sealed class PlateTextWriter
{
	public string Write(PlateResult plate)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 8; i++)
		{
			stringBuilder.Append((char)(65 + i));
			double[] row = plate.GetRow(i);
			foreach (double num in row)
			{
				stringBuilder.Append(' ');
				stringBuilder.Append(num.ToString("F4", CultureInfo.InvariantCulture).PadLeft(10));
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}
}
