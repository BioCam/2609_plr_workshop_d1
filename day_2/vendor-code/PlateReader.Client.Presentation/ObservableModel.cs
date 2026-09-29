using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlateReader.Client.Presentation;

public abstract class ObservableModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected bool Set<T>(ref T field, T value, [CallerMemberName] string? property = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
		{
			return false;
		}
		field = value;
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		return true;
	}
}
