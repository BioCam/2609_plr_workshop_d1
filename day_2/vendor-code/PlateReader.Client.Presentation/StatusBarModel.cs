namespace PlateReader.Client.Presentation;

public sealed class StatusBarModel : ObservableModel
{
	private string message = "Disconnected";

	private bool busy;

	private int completed;

	public string Message
	{
		get
		{
			return message;
		}
		set
		{
			Set(ref message, value, "Message");
		}
	}

	public bool Busy
	{
		get
		{
			return busy;
		}
		set
		{
			Set(ref busy, value, "Busy");
		}
	}

	public int Completed
	{
		get
		{
			return completed;
		}
		set
		{
			Set(ref completed, value, "Completed");
		}
	}
}
