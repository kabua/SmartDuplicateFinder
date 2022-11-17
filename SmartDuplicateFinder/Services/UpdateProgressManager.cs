using System.ComponentModel;
using System.Windows.Threading;

namespace SmartDuplicateFinder.Services;

public class UpdateProgressManager : IProgress, IUpdateProgress, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public UpdateProgressManager(Dispatcher dispatcher)
	{
		_dispatcher = dispatcher;

		Description = "";
		Current = 0;
		Total = 100;
	}

	public string Description { get; private set; }
	public double Current { get; private set; }
	public double Total { get; private set; }

	public void Update(double current, string? description = null, double? total = null)
	{
		static void DoUpdate(UpdateProgressManager self, double current, string? description = null, double? total = null)
		{
			self.Current = current;

			if (description != null)
				self.Description = description;

			if (total != null)
				self.Total = total.Value;
		}

		if (_dispatcher.CheckAccess())
		{
			DoUpdate(this, current, description, total);
		}
		else
		{
			_dispatcher.Invoke(() => DoUpdate(this, current, description, total));
		}
	}

	private readonly Dispatcher _dispatcher;
}