using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Configuration;

public sealed class DisplaySettings
{
	public int DecimalPlaces { get; set; } = 4;

	public int FontSize { get; set; } = 12;

	public int CellWidth { get; set; } = 80;

	public int CellHeight { get; set; } = 32;

	public IEnumerable<string> Validate()
	{
		if (DecimalPlaces < 0 || DecimalPlaces > 10)
		{
			yield return "Display.DecimalPlaces must be between 0 and 10";
		}
		if (FontSize < 8 || FontSize > 48)
		{
			yield return "Display.FontSize must be between 8 and 48";
		}
		if (CellWidth < 20 || CellWidth > 300)
		{
			yield return "Display.CellWidth must be between 20 and 300";
		}
		if (CellHeight < 16 || CellHeight > 120)
		{
			yield return "Display.CellHeight must be between 16 and 120";
		}
	}

	public void Reset()
	{
		DecimalPlaces = 4;
		FontSize = 12;
		CellWidth = 80;
		CellHeight = 32;
	}

	public DisplaySettings Clone()
	{
		return new DisplaySettings
		{
			DecimalPlaces = DecimalPlaces,
			FontSize = FontSize,
			CellWidth = CellWidth,
			CellHeight = CellHeight
		};
	}

	public IReadOnlyDictionary<string, int> ToDictionary()
	{
		return new Dictionary<string, int>
		{
			["DecimalPlaces"] = DecimalPlaces,
			["FontSize"] = FontSize,
			["CellWidth"] = CellWidth,
			["CellHeight"] = CellHeight
		};
	}

	public void Apply(IReadOnlyDictionary<string, int> values)
	{
		DisplaySettings displaySettings = Clone();
		if (values.TryGetValue("DecimalPlaces", out var value))
		{
			displaySettings.DecimalPlaces = value;
		}
		if (values.TryGetValue("FontSize", out var value2))
		{
			displaySettings.FontSize = value2;
		}
		if (values.TryGetValue("CellWidth", out var value3))
		{
			displaySettings.CellWidth = value3;
		}
		if (values.TryGetValue("CellHeight", out var value4))
		{
			displaySettings.CellHeight = value4;
		}
		string[] array = displaySettings.Validate().ToArray();
		if (array.Length != 0)
		{
			throw new ArgumentException(string.Join("; ", array));
		}
		DecimalPlaces = displaySettings.DecimalPlaces;
		FontSize = displaySettings.FontSize;
		CellWidth = displaySettings.CellWidth;
		CellHeight = displaySettings.CellHeight;
	}
}
