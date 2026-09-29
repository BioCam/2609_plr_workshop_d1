using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Configuration;

public sealed class AcquisitionSettings
{
	public int ReplicateCount { get; set; } = 1;

	public int QueueCapacity { get; set; } = 10;

	public int TimeoutSeconds { get; set; } = 5;

	public int PreviewRows { get; set; } = 8;

	public IEnumerable<string> Validate()
	{
		if (ReplicateCount < 1 || ReplicateCount > 20)
		{
			yield return "Acquisition.ReplicateCount must be between 1 and 20";
		}
		if (QueueCapacity < 1 || QueueCapacity > 100)
		{
			yield return "Acquisition.QueueCapacity must be between 1 and 100";
		}
		if (TimeoutSeconds < 1 || TimeoutSeconds > 60)
		{
			yield return "Acquisition.TimeoutSeconds must be between 1 and 60";
		}
		if (PreviewRows < 1 || PreviewRows > 8)
		{
			yield return "Acquisition.PreviewRows must be between 1 and 8";
		}
	}

	public void Reset()
	{
		ReplicateCount = 1;
		QueueCapacity = 10;
		TimeoutSeconds = 5;
		PreviewRows = 8;
	}

	public AcquisitionSettings Clone()
	{
		return new AcquisitionSettings
		{
			ReplicateCount = ReplicateCount,
			QueueCapacity = QueueCapacity,
			TimeoutSeconds = TimeoutSeconds,
			PreviewRows = PreviewRows
		};
	}

	public IReadOnlyDictionary<string, int> ToDictionary()
	{
		return new Dictionary<string, int>
		{
			["ReplicateCount"] = ReplicateCount,
			["QueueCapacity"] = QueueCapacity,
			["TimeoutSeconds"] = TimeoutSeconds,
			["PreviewRows"] = PreviewRows
		};
	}

	public void Apply(IReadOnlyDictionary<string, int> values)
	{
		AcquisitionSettings acquisitionSettings = Clone();
		if (values.TryGetValue("ReplicateCount", out var value))
		{
			acquisitionSettings.ReplicateCount = value;
		}
		if (values.TryGetValue("QueueCapacity", out var value2))
		{
			acquisitionSettings.QueueCapacity = value2;
		}
		if (values.TryGetValue("TimeoutSeconds", out var value3))
		{
			acquisitionSettings.TimeoutSeconds = value3;
		}
		if (values.TryGetValue("PreviewRows", out var value4))
		{
			acquisitionSettings.PreviewRows = value4;
		}
		string[] array = acquisitionSettings.Validate().ToArray();
		if (array.Length != 0)
		{
			throw new ArgumentException(string.Join("; ", array));
		}
		ReplicateCount = acquisitionSettings.ReplicateCount;
		QueueCapacity = acquisitionSettings.QueueCapacity;
		TimeoutSeconds = acquisitionSettings.TimeoutSeconds;
		PreviewRows = acquisitionSettings.PreviewRows;
	}
}
