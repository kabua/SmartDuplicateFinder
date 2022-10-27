using SmartDuplicateFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartDuplicateFinder.Views;

/// <summary>
/// Interaction logic for FindDuplicatesView.xaml
/// </summary>
public partial class FindDuplicatesView : UserControl
{
	public FindDuplicatesView()
	{
		InitializeComponent();
		AddCommandBindings();

		Drives = new ObservableCollection<DriveViewModel>();
		OnRefreshDrivers();

		DataContext = this;
	}

	private void TreeViewItem_OnExpanded(object sender, RoutedEventArgs e)
	{
		var treeViewItem = (TreeViewItem)e.OriginalSource;
		var parent = (DirectoryViewModel) treeViewItem.DataContext;

		parent.LoadSubFolders();
	}
	public ObservableCollection<DriveViewModel> Drives { get; set; }

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

	private void AddCommandBindings()
	{
		CommandBindings.Add(new CommandBinding(AppCommands.Refresh, (sender, args) => OnRefreshDrivers()));
		CommandBindings.Add(new CommandBinding(AppCommands.ClearAll, (sender, args) => OnClearAll()));

		//CommandBindings.Add(new CommandBinding(AppCommands.Xxxxx, (sender, args) => OnXxxx(args)));
		//CommandBindings.Add(new CommandBinding(AppCommands.Xxxxx, (sender, args) => OnXxxxx()));
	}
}