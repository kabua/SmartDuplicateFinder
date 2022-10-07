using SmartDuplicateFinder.ViewModels;
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

		if (treeViewItem.DataContext is DirectoryViewModel parent)
		{
			if (!(parent.SubFolders.Count == 1 && parent.SubFolders[0] == DirectoryViewModel.UnExpanded))
				return;

			parent.SubFolders.Clear();

			var options = new EnumerationOptions();
			foreach (var directoryInfo in parent.DirectoryInfo.GetDirectories("*", options))
			{
				parent.SubFolders.Add(new DirectoryViewModel(directoryInfo));
			}
		}
		else if (treeViewItem.DataContext is DriveViewModel drive)
		{
			if (!(drive.SubFolders.Count == 1 && drive.SubFolders[0] == DirectoryViewModel.UnExpanded))
				return;

			drive.SubFolders.Clear();

			var options = new EnumerationOptions();
			foreach (var directoryInfo in drive.DriveInfo.RootDirectory.GetDirectories("*", options))
			{
				drive.SubFolders.Add(new DirectoryViewModel(directoryInfo));
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

	public ObservableCollection<DriveViewModel> Drives { get; set; }


	private void AddCommandBindings()
	{
		CommandBindings.Add(new CommandBinding(AppCommands.Refresh, (sender, args) => OnRefreshDrivers()));

		//CommandBindings.Add(new CommandBinding(AppCommands.Xxxxx, (sender, args) => OnXxxx(args)));
		//CommandBindings.Add(new CommandBinding(AppCommands.Xxxxx, (sender, args) => OnXxxxx()));
	}
}