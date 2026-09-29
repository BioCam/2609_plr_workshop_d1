using System.Collections.Generic;
using System.Globalization;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Reporting;

public sealed class MedianAbsoluteDeviationSection : IReportSection
{
	public string Title => "MedianAbsoluteDeviation";

	public IEnumerable<string> Render(PlateResult plate)
	{
		yield return "Whole plate: " + Format(Evaluate(plate.ToArray()));
		for (int row = 0; row < 8; row++)
		{
			double value = Evaluate(plate.GetRow(row));
			yield return $"Row {(char)(ushort)(65 + row)}: {Format(value)}";
		}
		for (int row = 0; row < 12; row++)
		{
			double value2 = Evaluate(plate.GetColumn(row));
			yield return $"Column {row + 1}: {Format(value2)}";
		}
	}

	private static double Evaluate(double[] values)
	{
		return Statistics.MedianAbsoluteDeviation(values);
	}

	private static string Format(double value)
	{
		return value.ToString("F6", CultureInfo.InvariantCulture);
	}
}
