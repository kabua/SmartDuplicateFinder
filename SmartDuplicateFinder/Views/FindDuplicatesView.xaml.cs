using Ninject;
using Ookii.Dialogs.Wpf;
using SmartDuplicateFinder.Extensions;
using SmartDuplicateFinder.Services;
using SmartDuplicateFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmartDuplicateFinder.Views;

/// <summary>
/// Interaction logic for FindDuplicatesView.xaml
/// </summary>
public partial class FindDuplicatesView : UserControl, INotifyPropertyChanged
{
	// Called by the GUI designer
	//
	public FindDuplicatesView()
		: this(new ImexService(), new FindDuplicateFilesService())
	{
	}

#pragma warning disable CS8618 // Non-nullable _imexService but is set if in design mode.
	[Inject]
	public FindDuplicatesView(ImexService imexService, FindDuplicateFilesService duplicateFilesService)
#pragma warning restore CS8618
	{
		InitializeComponent();
		AddCommandBindings();

		_imexService = imexService;
		_duplicateFilesService = duplicateFilesService;
		ElapsedTime = TimeSpan.Zero;

		Drives = new ObservableCollection<DriveViewModel>();
		ScanVerb = Constants.ScanVerbName;

		if (App.InDesignMode())
		{
			StepProgress = new DesignTimeProgressManager();
			SummaryProgress = new DesignTimeProgressManager();
		}
		else
		{
			StepProgress = new UpdateProgressManager(App.Current.Dispatcher);
			SummaryProgress = new UpdateProgressManager(App.Current.Dispatcher);
		}

		_duplicateFilesService.StepUpdater = (IUpdateProgress) StepProgress;
		_duplicateFilesService.SummaryUpdater = (IUpdateProgress) SummaryProgress;
		_stopwatch = new Stopwatch();
		_timer = new DispatcherTimer(DispatcherPriority.Send, Dispatcher.CurrentDispatcher)
		{
			Interval = TimeSpan.FromSeconds(1),
		};
		_timer.Tick += TimerOnTick;

		RefreshDrivers();

		DataContext = this;

		if (App.InDesignMode())
		{
			//IsScanning = true;

			var fullFileName = @"D:\Dev.old\Repos\GainsCapitalRateDownLoader\packages\HtmlAgilityPack.1.4.9.5\lib\portable-net45+netcore45+wp8+MonoAndroid+MonoTouch\\HtmlAgilityPack.dll";
			fullFileName = fullFileName.ShortenPathname();

			((IUpdateProgress)StepProgress).Update(65, fullFileName, 100);
			((IUpdateProgress)SummaryProgress).Update(double.NaN, "Step 1 of 3", 0);

			//ElapsedTime = new TimeSpan(5, 43, 21);
		}
	}

	public string ScanVerb { get; set; }
	public bool IsScanning { get; private set; }

	public ObservableCollection<DriveViewModel> Drives { get; private set; }

	public IProgress StepProgress { get; private set; }
	public IProgress SummaryProgress { get; private set; }
	public TimeSpan ElapsedTime { get; private set; }


	private void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
	{
		var treeViewItem = (TreeViewItem)e.OriginalSource;
		var parent = (DirectoryViewModel) treeViewItem.DataContext;

		parent.LoadSubFolders();
	}

	private void WalkTree(Func<DirectoryViewModel, bool> process, Action<DirectoryViewModel> action)
	{
		var treeNodes = new Stack<DirectoryViewModel>();

		foreach (DriveViewModel drive in Drives.Where(d => d != DirectoryViewModel.UnExpanded).Reverse())
		{
			if (process(drive))
			{
				treeNodes.Push(drive);
			}
		}

		while(treeNodes.TryPop(out var folder))
		{
			foreach (DirectoryViewModel item in folder.SubFolders.Where(d => d != DirectoryViewModel.UnExpanded).Reverse())
			{
				if (process(item))
				{
					treeNodes.Push(item);
				}
			}

			action(folder);
		}
	}

#pragma warning disable IDE0051 // Called by generated code.
	// ReSharper disable once UnusedMember.Local
	private void OnIsScanningChanged()
#pragma warning restore IDE0051
	{
		if (IsScanning)
		{
			ScanVerb = Constants.ScanningVerbName;
			_stopwatch.Reset();
			_stopwatch.Restart();
			_timer.Start();
		}
		else
		{
			ScanVerb = Constants.ScanVerbName;
			_timer.Stop();
			_stopwatch.Stop();
			_stopwatch.Reset();
		}
	}

	private void RefreshDrivers()
	{
		Drives.Clear();

		IEnumerable<DriveViewModel> drives = DriveInfo.GetDrives().Where(d => d.IsReady).Select(d => new DriveViewModel(d));
		foreach (DriveViewModel driver in drives)
		{
			Drives.Add(driver);
		}
	}

	private void SaveSelection(string fileName)
	{
		try
		{
			var rootFolderNames = GetRootFolders().Select(r => r.FullPath).ToArray();
			_imexService.Save(fileName, rootFolderNames);
		}
		catch (Exception e)
		{
			MessageBox.Show(Window.GetWindow(this)!, e.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	private void LoadSelection(VistaOpenFileDialog dialog)
	{
		var rootFolderNames = _imexService.Load(dialog.FileName);
		foreach (var fullFolderName in rootFolderNames)
		{
			var folders = new Queue<string>(fullFolderName.Split(Path.DirectorySeparatorChar).Where(d => !string.IsNullOrWhiteSpace(d)));
			var driveName = folders.Dequeue();

			var folderVm = Drives.FirstOrDefault(d => d.Name.Equals(driveName, StringComparison.CurrentCultureIgnoreCase)) as DirectoryViewModel;

			if (folders.Count == 0 && folderVm != null)
			{
				folderVm.IsSelected = true;
			}

			while (folders.Count > 0 && folderVm != null)
			{
				folderVm.IsExpanded = true;
				folderVm.LoadSubFolders();

				var folderName = folders.Dequeue();
				folderVm = folderVm.SubFolders.FirstOrDefault(d => d.Name.Equals(folderName, StringComparison.CurrentCultureIgnoreCase));

				if (folders.Count == 0 && folderVm != null)
				{
					folderVm.IsSelected = true;
				}
			}
		}
	}


	private void TimerOnTick(object? sender, EventArgs e) => ElapsedTime = _stopwatch.Elapsed;

	private void OnSaveSelection()
	{
		var dialog = new VistaSaveFileDialog
		{
			Title = $"{App.Name} - Save Selected Folders",
			AddExtension = true,
			CheckPathExists = true,
			OverwritePrompt = true,
			DefaultExt = Constants.FileExtension,
			Filter = $"{App.Name} (*.{Constants.FileExtension})|*.{Constants.FileExtension}|All files (*.*)|*.*",
			FilterIndex = 0
		};

		if (dialog.ShowDialog(Window.GetWindow(this)) != true)
			return;

		SaveSelection(dialog.FileName);
	}

	private void OnLoadSelection()
	{
		var dialog = new VistaOpenFileDialog
		{
			Title = $"{App.Name} - Load Selected Folders",
			CheckFileExists = true,
			CheckPathExists = true,
			RestoreDirectory = true,
			Multiselect = false,
			DefaultExt = Constants.FileExtension,
			Filter = $"{App.Name} (*.{Constants.FileExtension})|*.{Constants.FileExtension}|All files (*.*)|*.*",
			FilterIndex = 0
		};

		if (dialog.ShowDialog(Window.GetWindow(this)) != true)
			return;

		LoadSelection(dialog);
	}


	private void OnRefreshDrivers() => RefreshDrivers();

	private void OnClearAll() => ClearAll();
	private void ClearAll()
	{
		WalkTree(d => d.IsSelected != false, d =>
		{
			if (d.IsSelected == true)
			{
				d.IsSelected = false;
			}
		});
	}

	private IList<DirectoryViewModel> GetRootFolders()
	{
		var rootFolders = new List<DirectoryViewModel>();

		WalkTree(d => d.IsSelected != false, d =>
		{
			if (d.IsSelected == true)
			{
				rootFolders.Add(d);
			}
		});

		return rootFolders;
	}


	private async void OnScanAsync()
	{
		await Task.Run(async () =>
		{
			try
			{
				IsScanning = true;

				var rootFolders = GetRootFolders().Select(vm => vm.DirectoryInfo).ToImmutableArray();
				_duplicateFilesService.ReadAllFiles(rootFolders);
			}
			finally
			{
				IsScanning = false;
			}
		});
	}

	private void AddCommandBindings()
	{
		App.Current.MainWindow!.CommandBindings.Add(new CommandBinding(AppCommands.Save, (sender, args) => OnSaveSelection()));
		App.Current.MainWindow!.CommandBindings.Add(new CommandBinding(AppCommands.Load, (sender, args) => OnLoadSelection()));

		CommandBindings.Add(new CommandBinding(AppCommands.Refresh, (sender, args) => OnRefreshDrivers()));
		CommandBindings.Add(new CommandBinding(AppCommands.ClearAll, (sender, args) => OnClearAll()));

		CommandBindings.Add(new CommandBinding(AppCommands.Scan, (sender, args) => OnScanAsync()));

		//CommandBindings.Add(new CommandBinding(AppCommands.Xxxxx, (sender, args) => OnXxxx(args)));
		//CommandBindings.Add(new CommandBinding(AppCommands.Xxxxx, (sender, args) => OnXxxxx()));
	}

	private readonly ImexService _imexService;
	private readonly FindDuplicateFilesService _duplicateFilesService;

	private readonly Stopwatch _stopwatch;
	private readonly DispatcherTimer _timer;
}