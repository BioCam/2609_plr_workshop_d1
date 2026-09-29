using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Configuration;

public sealed class QualitySettings
{
	public int MinimumSignal { get; set; }

	public int MaximumSignal { get; set; } = 65535;

	public int MaximumCvPercent { get; set; } = 20;

	public int OutlierThreshold { get; set; } = 3;

	public IEnumerable<string> Validate()
	{
		if (MinimumSignal < 0 || MinimumSignal > 65535)
		{
			yield return "Quality.MinimumSignal must be between 0 and 65535";
		}
		if (MaximumSignal < 1 || MaximumSignal > 65535)
		{
			yield return "Quality.MaximumSignal must be between 1 and 65535";
		}
		if (MaximumCvPercent < 1 || MaximumCvPercent > 100)
		{
			yield return "Quality.MaximumCvPercent must be between 1 and 100";
		}
		if (OutlierThreshold < 1 || OutlierThreshold > 10)
		{
			yield return "Quality.OutlierThreshold must be between 1 and 10";
		}
	}

	public void Reset()
	{
		MinimumSignal = 0;
		MaximumSignal = 65535;
		MaximumCvPercent = 20;
		OutlierThreshold = 3;
	}

	public QualitySettings Clone()
	{
		return new QualitySettings
		{
			MinimumSignal = MinimumSignal,
			MaximumSignal = MaximumSignal,
			MaximumCvPercent = MaximumCvPercent,
			OutlierThreshold = OutlierThreshold
		};
	}

	public IReadOnlyDictionary<string, int> ToDictionary()
	{
		return new Dictionary<string, int>
		{
			["MinimumSignal"] = MinimumSignal,
			["MaximumSignal"] = MaximumSignal,
			["MaximumCvPercent"] = MaximumCvPercent,
			["OutlierThreshold"] = OutlierThreshold
		};
	}

	public void Apply(IReadOnlyDictionary<string, int> values)
	{
		QualitySettings qualitySettings = Clone();
		if (values.TryGetValue("MinimumSignal", out var value))
		{
			qualitySettings.MinimumSignal = value;
		}
		if (values.TryGetValue("MaximumSignal", out var value2))
		{
			qualitySettings.MaximumSignal = value2;
		}
		if (values.TryGetValue("MaximumCvPercent", out var value3))
		{
			qualitySettings.MaximumCvPercent = value3;
		}
		if (values.TryGetValue("OutlierThreshold", out var value4))
		{
			qualitySettings.OutlierThreshold = value4;
		}
		string[] array = qualitySettings.Validate().ToArray();
		if (array.Length != 0)
		{
			throw new ArgumentException(string.Join("; ", array));
		}
		MinimumSignal = qualitySettings.MinimumSignal;
		MaximumSignal = qualitySettings.MaximumSignal;
		MaximumCvPercent = qualitySettings.MaximumCvPercent;
		OutlierThreshold = qualitySettings.OutlierThreshold;
	}
}
