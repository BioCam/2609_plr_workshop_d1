using System.Collections.Generic;
using System.Text;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Reporting;

public sealed class ReportComposer
{
	private readonly List<IReportSection> sections = new List<IReportSection>();

	public void Add(IReportSection section)
	{
		sections.Add(section);
	}

	public string Compose(PlateResult plate)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
		handler.AppendLiteral("Sample: ");
		handler.AppendFormatted(plate.SampleName);
		stringBuilder3.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder4 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
		handler.AppendLiteral("Mode: ");
		handler.AppendFormatted(plate.Mode);
		stringBuilder4.AppendLine(ref handler);
		foreach (IReportSection section in sections)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(section.Title);
			foreach (string item in section.Render(plate))
			{
				stringBuilder.AppendLine(item);
			}
		}
		return stringBuilder.ToString();
	}
}
