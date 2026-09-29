using System;
using System.Collections.Generic;
using System.Linq;

namespace PlateReader.Client.Application;

public sealed class AuditTrail
{
	private readonly List<AuditEntry> entries = new List<AuditEntry>();

	public IReadOnlyList<AuditEntry> Entries => entries.AsReadOnly();

	public void Append(string action, string detail)
	{
		entries.Add(new AuditEntry(entries.Count + 1, action, detail));
	}

	public IEnumerable<AuditEntry> Find(string term)
	{
		return entries.Where((AuditEntry entry) => entry.Action.Contains(term, StringComparison.OrdinalIgnoreCase) || entry.Detail.Contains(term, StringComparison.OrdinalIgnoreCase));
	}
}
