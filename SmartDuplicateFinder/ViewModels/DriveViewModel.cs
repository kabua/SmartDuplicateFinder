using PropertyChanged;
using SmartDuplicateFinder.Utils;
using System;
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
		_driveInfo = driveInfo;

		Icon = _driveInfo.DriveType switch
		{
			DriveType.Fixed => _driveInfo.Name.Equals(s_systemDrive, StringComparison.InvariantCultureIgnoreCase) ? Icons.WindowsDrive : Icons.HardDrive,
			DriveType.Network => _driveInfo.IsReady ? Icons.NetworkConnectedDrive : Icons.NetworkDisconnectedDrive,
			DriveType.CDRom => Icons.CDRomDrive,
			DriveType.Removable => Icons.RemovableDrive,
			_ => Icons.UnknownDrive
		};

		if (_driveInfo.IsReady)
		{
			IsSelectable = true;

			DisplayName = $"{_driveInfo.VolumeLabel} ({_driveInfo.Name[..^1]})";
			Name = _driveInfo.Name[..^1];
		}
		else
		{
			IsSelectable = false;

			DisplayName = Name = $"({_driveInfo.Name[..^1]})";
		}
	}

	public Icons Icon { get; private set; }

	public string Name { get; protected set; }

	public string DisplayName { get; protected set; }

	public bool IsSelectable { get; set; }
	public bool IsSelected { get; set; }

	private static readonly string s_systemDrive;

	private readonly DriveInfo _driveInfo;
}