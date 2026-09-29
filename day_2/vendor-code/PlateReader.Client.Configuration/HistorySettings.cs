using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Configuration;

public sealed class HistorySettings
{
	public int PageSize { get; set; } = 25;

	public int MaximumEntries { get; set; } = 1000;

	public int RecentMethodCount { get; set; } = 10;

	public int RefreshSeconds { get; set; } = 30;

	public IEnumerable<string> Validate()
	{
		if (PageSize < 1 || PageSize > 500)
		{
			yield return "History.PageSize must be between 1 and 500";
		}
		if (MaximumEntries < 10 || MaximumEntries > 100000)
		{
			yield return "History.MaximumEntries must be between 10 and 100000";
		}
		if (RecentMethodCount < 1 || RecentMethodCount > 100)
		{
			yield return "History.RecentMethodCount must be between 1 and 100";
		}
		if (RefreshSeconds < 1 || RefreshSeconds > 3600)
		{
			yield return "History.RefreshSeconds must be between 1 and 3600";
		}
	}

	public void Reset()
	{
		PageSize = 25;
		MaximumEntries = 1000;
		RecentMethodCount = 10;
		RefreshSeconds = 30;
	}

	public HistorySettings Clone()
	{
		return new HistorySettings
		{
			PageSize = PageSize,
			MaximumEntries = MaximumEntries,
			RecentMethodCount = RecentMethodCount,
			RefreshSeconds = RefreshSeconds
		};
	}

	public IReadOnlyDictionary<string, int> ToDictionary()
	{
		return new Dictionary<string, int>
		{
			["PageSize"] = PageSize,
			["MaximumEntries"] = MaximumEntries,
			["RecentMethodCount"] = RecentMethodCount,
			["RefreshSeconds"] = RefreshSeconds
		};
	}

	public void Apply(IReadOnlyDictionary<string, int> values)
	{
		HistorySettings historySettings = Clone();
		if (values.TryGetValue("PageSize", out var value))
		{
			historySettings.PageSize = value;
		}
		if (values.TryGetValue("MaximumEntries", out var value2))
		{
			historySettings.MaximumEntries = value2;
		}
		if (values.TryGetValue("RecentMethodCount", out var value3))
		{
			historySettings.RecentMethodCount = value3;
		}
		if (values.TryGetValue("RefreshSeconds", out var value4))
		{
			historySettings.RefreshSeconds = value4;
		}
		string[] array = historySettings.Validate().ToArray();
		if (array.Length != 0)
		{
			throw new ArgumentException(string.Join("; ", array));
		}
		PageSize = historySettings.PageSize;
		MaximumEntries = historySettings.MaximumEntries;
		RecentMethodCount = historySettings.RecentMethodCount;
		RefreshSeconds = historySettings.RefreshSeconds;
	}
}
