using System;
using System.Collections.Generic;
using System.Linq;
using PlateReader.Client.Domain;

namespace PlateReader.Client.Application;

public sealed class MethodRepository
{
	private readonly List<MeasurementMethod> methods = new List<MeasurementMethod>();

	public void Save(MeasurementMethod method)
	{
		method.Validate();
		int num = methods.FindIndex((MeasurementMethod item) => item.Name == method.Name);
		if (num >= 0)
		{
			methods[num] = method;
		}
		else
		{
			methods.Add(method);
		}
	}

	public MeasurementMethod Get(string name)
	{
		return methods.Single((MeasurementMethod item) => item.Name == name);
	}

	public IReadOnlyList<MeasurementMethod> List()
	{
		return methods.OrderBy<MeasurementMethod, string>((MeasurementMethod item) => item.Name, StringComparer.Ordinal).ToArray();
	}

	public bool Remove(string name)
	{
		return methods.RemoveAll((MeasurementMethod item) => item.Name == name) > 0;
	}
}
