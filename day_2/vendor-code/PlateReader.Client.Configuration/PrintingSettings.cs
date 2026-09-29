using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Configuration;

public sealed class PrintingSettings
{
	public int MarginLeft { get; set; } = 20;

	public int MarginTop { get; set; } = 20;

	public int RowsPerPage { get; set; } = 40;

	public int Copies { get; set; } = 1;

	public IEnumerable<string> Validate()
	{
		if (MarginLeft < 0 || MarginLeft > 100)
		{
			yield return "Printing.MarginLeft must be between 0 and 100";
		}
		if (MarginTop < 0 || MarginTop > 100)
		{
			yield return "Printing.MarginTop must be between 0 and 100";
		}
		if (RowsPerPage < 1 || RowsPerPage > 100)
		{
			yield return "Printing.RowsPerPage must be between 1 and 100";
		}
		if (Copies < 1 || Copies > 20)
		{
			yield return "Printing.Copies must be between 1 and 20";
		}
	}

	public void Reset()
	{
		MarginLeft = 20;
		MarginTop = 20;
		RowsPerPage = 40;
		Copies = 1;
	}

	public PrintingSettings Clone()
	{
		return new PrintingSettings
		{
			MarginLeft = MarginLeft,
			MarginTop = MarginTop,
			RowsPerPage = RowsPerPage,
			Copies = Copies
		};
	}

	public IReadOnlyDictionary<string, int> ToDictionary()
	{
		return new Dictionary<string, int>
		{
			["MarginLeft"] = MarginLeft,
			["MarginTop"] = MarginTop,
			["RowsPerPage"] = RowsPerPage,
			["Copies"] = Copies
		};
	}

	public void Apply(IReadOnlyDictionary<string, int> values)
	{
		PrintingSettings printingSettings = Clone();
		if (values.TryGetValue("MarginLeft", out var value))
		{
			printingSettings.MarginLeft = value;
		}
		if (values.TryGetValue("MarginTop", out var value2))
		{
			printingSettings.MarginTop = value2;
		}
		if (values.TryGetValue("RowsPerPage", out var value3))
		{
			printingSettings.RowsPerPage = value3;
		}
		if (values.TryGetValue("Copies", out var value4))
		{
			printingSettings.Copies = value4;
		}
		string[] array = printingSettings.Validate().ToArray();
		if (array.Length != 0)
		{
			throw new ArgumentException(string.Join("; ", array));
		}
		MarginLeft = printingSettings.MarginLeft;
		MarginTop = printingSettings.MarginTop;
		RowsPerPage = printingSettings.RowsPerPage;
		Copies = printingSettings.Copies;
	}
}
