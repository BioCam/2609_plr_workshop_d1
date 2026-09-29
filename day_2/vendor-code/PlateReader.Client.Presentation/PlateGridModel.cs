using System;
using System.Collections.Generic;
using System.Linq;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Presentation;

public sealed class PlateGridModel
{
	private readonly HashSet<int> selected = new HashSet<int>();

	private PlateResult? plate;

	public void Load(PlateResult result)
	{
		plate = result;
		selected.Clear();
	}

	public void Select(string address)
	{
		selected.Add(WellAddress.Parse(address).Index);
	}

	public void SelectRow(int row)
	{
		for (int i = 0; i < 12; i++)
		{
			selected.Add(new WellAddress(row, i).Index);
		}
	}

	public void ClearSelection()
	{
		selected.Clear();
	}

	public IEnumerable<WellCell> Cells()
	{
		if (plate == null)
		{
			yield break;
		}
		foreach (var item in plate.Wells())
		{
			yield return new WellCell(item.Well.ToString(), item.Value, selected.Contains(item.Well.Index));
		}
	}

	public double[] SelectedValues()
	{
		if (plate == null)
		{
			return Array.Empty<double>();
		}
		return (from index in selected.Order()
			select plate[index]).ToArray();
	}
}
