using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Reporting;

public static class Statistics
{
	public static double Mean(IReadOnlyList<double> values)
	{
		RequireValues(values);
		double num = 0.0;
		foreach (double value in values)
		{
			num += value;
		}
		return num / (double)values.Count;
	}

	public static double Variance(IReadOnlyList<double> values)
	{
		RequireValues(values);
		if (values.Count < 2)
		{
			return 0.0;
		}
		double num = Mean(values);
		double num2 = 0.0;
		foreach (double value in values)
		{
			double num3 = value - num;
			num2 += num3 * num3;
		}
		return num2 / (double)(values.Count - 1);
	}

	public static double StandardDeviation(IReadOnlyList<double> values)
	{
		return Math.Sqrt(Variance(values));
	}

	public static double CoefficientOfVariation(IReadOnlyList<double> values)
	{
		double num = Mean(values);
		if (num != 0.0)
		{
			return StandardDeviation(values) / Math.Abs(num) * 100.0;
		}
		return 0.0;
	}

	public static double Quantile(IReadOnlyList<double> values, double probability)
	{
		RequireValues(values);
		if (probability < 0.0 || probability > 1.0)
		{
			throw new ArgumentOutOfRangeException("probability");
		}
		double[] array = values.Order().ToArray();
		double num = probability * (double)(array.Length - 1);
		int num2 = (int)Math.Floor(num);
		int num3 = (int)Math.Ceiling(num);
		return array[num2] + (array[num3] - array[num2]) * (num - (double)num2);
	}

	public static double Median(IReadOnlyList<double> values)
	{
		return Quantile(values, 0.5);
	}

	public static double MedianAbsoluteDeviation(IReadOnlyList<double> values)
	{
		double median = Median(values);
		return Median(values.Select((double value) => Math.Abs(value - median)).ToArray());
	}

	public static double RootMeanSquare(IReadOnlyList<double> values)
	{
		RequireValues(values);
		return Math.Sqrt(values.Sum((double value) => value * value) / (double)values.Count);
	}

	public static double TrimmedMean(IReadOnlyList<double> values, double fraction)
	{
		RequireValues(values);
		if (fraction < 0.0 || fraction >= 0.5)
		{
			throw new ArgumentOutOfRangeException("fraction");
		}
		double[] array = values.Order().ToArray();
		int num = (int)((double)array.Length * fraction);
		return Mean(array.Skip(num).Take(array.Length - num * 2).ToArray());
	}

	private static void RequireValues(IReadOnlyList<double> values)
	{
		if (values.Count == 0 || values.Any((double value) => !double.IsFinite(value)))
		{
			throw new ArgumentException("Statistics require finite observations");
		}
	}
}
