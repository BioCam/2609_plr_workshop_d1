using System;
using System.Globalization;

namespace PlateReader.Client.Domain;

public readonly record struct WellAddress
{
	public int Row { get; }

	public int Column { get; }

	public int Index => Row * 12 + Column;

	public WellAddress(int row, int column)
	{
		if (row < 0 || row >= 8)
		{
			throw new ArgumentOutOfRangeException("row");
		}
		if (column < 0 || column >= 12)
		{
			throw new ArgumentOutOfRangeException("column");
		}
		Row = row;
		Column = column;
	}

	public static WellAddress FromIndex(int index)
	{
		if (index < 0 || index >= 96)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		return new WellAddress(index / 12, index % 12);
	}

	public static WellAddress Parse(string value)
	{
		if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
		{
			throw new FormatException("Expected a well address such as A1");
		}
		int row = char.ToUpperInvariant(value[0]) - 65;
		if (!int.TryParse(value.AsSpan(1), NumberStyles.None, CultureInfo.InvariantCulture, out var result))
		{
			throw new FormatException("Invalid well column");
		}
		return new WellAddress(row, result - 1);
	}

	public override string ToString()
	{
		return $"{(char)(ushort)(65 + Row)}{Column + 1}";
	}
}
