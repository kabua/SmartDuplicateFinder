using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace SmartDuplicateFinder.Services;

public class UpdateProgressManager : IProgress, IUpdateProgress, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public UpdateProgressManager(Dispatcher dispatcher, TimeSpan? updateInterval = null)
	{
		_timer = new DispatcherTimer(DispatcherPriority.Send, dispatcher)
		{
			Interval = updateInterval ?? TimeSpan.FromMilliseconds(10),
		};
		_timer.Tick += TimerOnTick;

		Description = "";
		Current = 0;
		Total = 100;
	}

	public string Description { get; private set; }
	public double Current { get; private set; }
	public double Total { get; private set; }

	public void Update(double current, string? description = null, double? total = null)
	{
		_current = current;
		_description = description;
		_total = total;

		_timer.Start();
	}

	private void TimerOnTick(object? sender, EventArgs e)
	{
		_timer.Stop();

		Current = _current;

		if (_description != null)
			Description = _description;

		if (_total != null)
			Total = _total.Value;
	}

	private readonly DispatcherTimer _timer;

	private double _current;
	private string? _description;
	private double? _total;
}