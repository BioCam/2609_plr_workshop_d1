using System.Collections.Generic;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Reporting;

public interface IReportSection
{
	string Title { get; }

	IEnumerable<string> Render(PlateResult plate);
}
