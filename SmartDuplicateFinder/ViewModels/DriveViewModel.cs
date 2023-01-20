using SmartDuplicateFinder.Utils;
using System;
using System.IO;

namespace SmartDuplicateFinder.ViewModels;

public class DriveViewModel  : DirectoryViewModel
{
	static DriveViewModel()
	{
		s_systemDrive = Path.GetPathRoot(Environment.SystemDirectory)!;
	}

	public DriveViewModel(DriveInfo driveInfo)
		: base (driveInfo.RootDirectory, null!)
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
	}

	protected override void UpdateIcon()
	{
		// Do not update current icon.
	}

	public DriveInfo DriveInfo { get; private set; }

	private static readonly string s_systemDrive;
}