using PlateReader.Client.Domain;

namespace PlateReader.Client.Presentation;

public sealed class MethodEditorModel : ObservableModel
{
	private string name = "Untitled method";

	private ushort wavelength = 600;

	private ushort gain = 1;

	private int replicates = 1;

	private MeasurementMode mode;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			Set(ref name, value, "Name");
		}
	}

	public ushort Wavelength
	{
		get
		{
			return wavelength;
		}
		set
		{
			Set(ref wavelength, value, "Wavelength");
		}
	}

	public ushort Gain
	{
		get
		{
			return gain;
		}
		set
		{
			Set(ref gain, value, "Gain");
		}
	}

	public int Replicates
	{
		get
		{
			return replicates;
		}
		set
		{
			Set(ref replicates, value, "Replicates");
		}
	}

	public MeasurementMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			Set(ref mode, value, "Mode");
		}
	}

	public MeasurementMethod BuildMethod()
	{
		MeasurementMethod measurementMethod = new MeasurementMethod(Name, Mode, Wavelength, Gain, Replicates);
		measurementMethod.Validate();
		return measurementMethod;
	}

	public void Load(MeasurementMethod method)
	{
		Name = method.Name;
		Mode = method.Mode;
		Wavelength = method.Wavelength;
		Gain = method.Gain;
		Replicates = method.Replicates;
	}
}
