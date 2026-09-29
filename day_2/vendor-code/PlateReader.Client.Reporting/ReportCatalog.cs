namespace PlateReader.Client.Reporting;

public static class ReportCatalog
{
	public static ReportComposer CreateDefault()
	{
		ReportComposer reportComposer = new ReportComposer();
		reportComposer.Add(new MeanSection());
		reportComposer.Add(new MedianSection());
		reportComposer.Add(new MinimumSection());
		reportComposer.Add(new MaximumSection());
		reportComposer.Add(new StandardDeviationSection());
		reportComposer.Add(new CoefficientOfVariationSection());
		reportComposer.Add(new InterquartileRangeSection());
		reportComposer.Add(new MedianAbsoluteDeviationSection());
		reportComposer.Add(new RootMeanSquareSection());
		reportComposer.Add(new TrimmedMeanSection());
		return reportComposer;
	}
}
