using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Configuration;

public sealed class ExportSettings
{
	public int Precision { get; set; } = 8;

	public int BatchSize { get; set; } = 96;

	public int MaximumFileCount { get; set; } = 100;

	public int RetentionDays { get; set; } = 30;

	public IEnumerable<string> Validate()
	{
		if (Precision < 1 || Precision > 17)
		{
			yield return "Export.Precision must be between 1 and 17";
		}
		if (BatchSize < 1 || BatchSize > 1000)
		{
			yield return "Export.BatchSize must be between 1 and 1000";
		}
		if (MaximumFileCount < 1 || MaximumFileCount > 10000)
		{
			yield return "Export.MaximumFileCount must be between 1 and 10000";
		}
		if (RetentionDays < 1 || RetentionDays > 3650)
		{
			yield return "Export.RetentionDays must be between 1 and 3650";
		}
	}

	public void Reset()
	{
		Precision = 8;
		BatchSize = 96;
		MaximumFileCount = 100;
		RetentionDays = 30;
	}

	public ExportSettings Clone()
	{
		return new ExportSettings
		{
			Precision = Precision,
			BatchSize = BatchSize,
			MaximumFileCount = MaximumFileCount,
			RetentionDays = RetentionDays
		};
	}

	public IReadOnlyDictionary<string, int> ToDictionary()
	{
		return new Dictionary<string, int>
		{
			["Precision"] = Precision,
			["BatchSize"] = BatchSize,
			["MaximumFileCount"] = MaximumFileCount,
			["RetentionDays"] = RetentionDays
		};
	}

	public void Apply(IReadOnlyDictionary<string, int> values)
	{
		ExportSettings exportSettings = Clone();
		if (values.TryGetValue("Precision", out var value))
		{
			exportSettings.Precision = value;
		}
		if (values.TryGetValue("BatchSize", out var value2))
		{
			exportSettings.BatchSize = value2;
		}
		if (values.TryGetValue("MaximumFileCount", out var value3))
		{
			exportSettings.MaximumFileCount = value3;
		}
		if (values.TryGetValue("RetentionDays", out var value4))
		{
			exportSettings.RetentionDays = value4;
		}
		string[] array = exportSettings.Validate().ToArray();
		if (array.Length != 0)
		{
			throw new ArgumentException(string.Join("; ", array));
		}
		Precision = exportSettings.Precision;
		BatchSize = exportSettings.BatchSize;
		MaximumFileCount = exportSettings.MaximumFileCount;
		RetentionDays = exportSettings.RetentionDays;
	}
}
