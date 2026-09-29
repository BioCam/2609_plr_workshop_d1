using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Domain;

public sealed class PlateResult
{
	private readonly double[] values;

	public string SampleName { get; }

	public MeasurementMode Mode { get; }

	public string Unit { get; }

	public int Count => values.Length;

	public double this[int index] => values[index];

	public double this[WellAddress well] => values[well.Index];

	public PlateResult(string sampleName, MeasurementMode mode, string unit, double[] values)
	{
		if (values.Length != 96 || values.Any((double value) => !double.IsFinite(value)))
		{
			throw new ArgumentException("A plate must contain 96 finite readings", "values");
		}
		SampleName = sampleName;
		Mode = mode;
		Unit = unit;
		this.values = (double[])values.Clone();
	}

	public double[] ToArray()
	{
		return (double[])values.Clone();
	}

	public List<List<double>> ToRows()
	{
		List<List<double>> list = new List<List<double>>();
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			List<double> list2 = new List<double>();
			for (int j = 0; j < 12; j++)
			{
				list2.Add(values[num]);
				num++;
			}
			list.Add(list2);
		}
		return list;
	}

	public IEnumerable<(WellAddress Well, double Value)> Wells()
	{
		for (int index = 0; index < values.Length; index++)
		{
			yield return (Well: WellAddress.FromIndex(index), Value: values[index]);
		}
	}

	public double[] GetRow(int row)
	{
		new WellAddress(row, 0);
		return values.Skip(row * 12).Take(12).ToArray();
	}

	public double[] GetColumn(int column)
	{
		new WellAddress(0, column);
		return (from row in Enumerable.Range(0, 8)
			select values[row * 12 + column]).ToArray();
	}
}
