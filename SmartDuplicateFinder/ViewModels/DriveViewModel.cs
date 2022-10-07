using System;
using System.Diagnostics;
using System.IO;
using PropertyChanged;
using SmartDuplicateFinder.Utils;

namespace SmartDuplicateFinder.ViewModels;

[AddINotifyPropertyChangedInterface]
[DebuggerDisplay("{DisplayName}")]
public class DriveViewModel : DirectoryViewModel
{
	static DriveViewModel()
	{
		s_systemDrive = Path.GetPathRoot(Environment.SystemDirectory)!;
	}

	public DriveViewModel(DriveInfo driveInfo)
		: base(driveInfo.RootDirectory, null!)
	{
		Icon = driveInfo.DriveType switch
		{
			DriveType.Fixed => driveInfo.Name.Equals(s_systemDrive, StringComparison.InvariantCultureIgnoreCase) ? Icons.WindowsDrive : Icons.HardDrive,
			DriveType.Network => driveInfo.IsReady ? Icons.NetworkConnectedDrive : Icons.NetworkDisconnectedDrive,
			DriveType.CDRom => Icons.CDRomDrive,
			DriveType.Removable => Icons.RemovableDrive,
			_ => Icons.UnknownDrive
		};

		if (driveInfo.IsReady)
		{
			IsSelectable = true;

			DisplayName = $"{driveInfo.VolumeLabel} ({driveInfo.Name[..^1]})";
			Name = driveInfo.Name[..^1];
		}
		else
		{
			IsSelectable = false;

			DisplayName = Name = $"({driveInfo.Name[..^1]})";
		}
	}

	private static readonly string s_systemDrive;
}