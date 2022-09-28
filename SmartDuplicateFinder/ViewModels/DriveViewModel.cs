using PropertyChanged;
using SmartDuplicateFinder.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace SmartDuplicateFinder.ViewModels;

[AddINotifyPropertyChangedInterface]
[DebuggerDisplay("{Name}")]

public class DriveViewModel  : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	static DriveViewModel()
	{
		s_systemDrive = Path.GetPathRoot(Environment.SystemDirectory)!;
	}

	public DriveViewModel(DriveInfo driveInfo)
	{
		DriveInfo = driveInfo;

		Icon = DriveInfo.DriveType switch
		{
			DriveType.Fixed => DriveInfo.Name.Equals(s_systemDrive, StringComparison.InvariantCultureIgnoreCase) ? Icons.WindowsDrive : Icons.HardDrive,
			DriveType.Network => DriveInfo.IsReady ? Icons.NetworkConnectedDrive : Icons.NetworkDisconnectedDrive,
			DriveType.CDRom => Icons.CDRomDrive,
			DriveType.Removable => Icons.RemovableDrive,
			_ => Icons.UnknownDrive
		};

		if (DriveInfo.IsReady)
		{
			IsSelectable = true;

			DisplayName = $"{DriveInfo.VolumeLabel} ({DriveInfo.Name[..^1]})";
			Name = DriveInfo.Name[..^1];
		}
		else
		{
			IsSelectable = false;

			DisplayName = Name = $"({DriveInfo.Name[..^1]})";
		}
	
		SubFolders = new ObservableCollection<DirectoryViewModel>
		{
			DirectoryViewModel.UnExpanded
		};
	}

	public Icons Icon { get; private set; }

	public string Name { get; protected set; }

	public string DisplayName { get; protected set; }

	public bool IsSelectable { get; set; }
	public bool IsSelected { get; set; }
	public ObservableCollection<DirectoryViewModel> SubFolders { get; private set; }

	public DriveInfo DriveInfo { get; private set; }

	private static readonly string s_systemDrive;
}