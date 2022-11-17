namespace SmartDuplicateFinder.Services;

public class DesignTimeProgressManager : IProgress, IUpdateProgress
{
	public DesignTimeProgressManager()
	{
		Description = "";
		Current = 0;
		Total = 100;
	}

	public string Description { get; private set; }
	public double Current { get; private set; }
	public double Total { get; private set; }

	public void Update(double current, string? description = null, double? total = null)
	{
		Current = current;

		if (description != null)
			Description = description;

		if (total != null)
			Total = total.Value;
	}
}