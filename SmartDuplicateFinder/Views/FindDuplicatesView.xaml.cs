using Ninject;
using Ookii.Dialogs.Wpf;
using SmartDuplicateFinder.Services;
using SmartDuplicateFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartDuplicateFinder.Views;

/// <summary>
/// Interaction logic for FindDuplicatesView.xaml
/// </summary>
public partial class FindDuplicatesView : UserControl, INotifyPropertyChanged
{
	[Inject]
	public FindDuplicatesView(ImexService imexService) : this()
	{
		_imexService = imexService;
	}

	public FindDuplicatesView()
	{
		InitializeComponent();
		AddCommandBindings();

		if (App.InDesignMode())
		{
			_imexService = new ImexService();
		}
		Drives = new ObservableCollection<DriveViewModel>();
		ScanVerb = Constants.ScanVerbName;
		OnRefreshDrivers();

		DataContext = this;
	}

	public string ScanVerb { get; set; }
	public bool IsScanning { get; private set; }

	public ObservableCollection<DriveViewModel> Drives { get; private set; }

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
		ScanVerb = IsScanning ? Constants.ScanningVerbName : Constants.ScanVerbName;
	}


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

		try
		{
			var rootFolderNames = GetRootFolders().Select(r => r.FullPath).ToArray();
			_imexService.Save(dialog.FileName, rootFolderNames);
		}
		catch (Exception e)
		{
			MessageBox.Show(Window.GetWindow(this)!, e.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
		}
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

		var rootFolderNames = _imexService.Load(dialog.FileName);
		foreach (var fullFolderName in rootFolderNames)
		{
			var folders = new Queue<string>(fullFolderName.Split(Path.DirectorySeparatorChar));
			var driveName = folders.Dequeue();

			var folderVm = Drives.FirstOrDefault(d => d.Name.Equals(driveName, StringComparison.CurrentCultureIgnoreCase)) as DirectoryViewModel;
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

	private void OnRefreshDrivers()
	{
		Drives.Clear();

		IEnumerable<DriveViewModel> drives = DriveInfo.GetDrives().Where(d => d.IsReady).Select(d => new DriveViewModel(d));
		foreach (DriveViewModel driver in drives)
		{
			Drives.Add(driver);
		}
	}

	private void OnClearAll()
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


	private async void ScanAsync()
	{
		try
		{
			IsScanning = true;

			//var rootFolders = GetRootFolders();
			await Task.Delay(TimeSpan.FromSeconds(10));
		}
		finally
		{
			IsScanning = false;
		}
	}

	private void AddCommandBindings()
	{
		CommandBindings.Add(new CommandBinding(AppCommands.Refresh, (sender, args) => OnRefreshDrivers()));
		CommandBindings.Add(new CommandBinding(AppCommands.ClearAll, (sender, args) => OnClearAll()));

		CommandBindings.Add(new CommandBinding(AppCommands.Scan, (sender, args) => ScanAsync()));

		//CommandBindings.Add(new CommandBinding(AppCommands.Xxxxx, (sender, args) => OnXxxx(args)));
		//CommandBindings.Add(new CommandBinding(AppCommands.Xxxxx, (sender, args) => OnXxxxx()));
	}

	private readonly ImexService _imexService;
}