using PropertyChanged;
using SmartDuplicateFinder.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SmartDuplicateFinder.ViewModels;

[AddINotifyPropertyChangedInterface]
[DebuggerDisplay("{DisplayName}")]
public class DirectoryViewModel : INotifyPropertyChanged
{
	public static readonly DirectoryViewModel UnExpanded = new ();

	public event PropertyChangedEventHandler? PropertyChanged;

	internal DirectoryViewModel(DirectoryInfo directoryInfo, DirectoryViewModel parent)
		: this()
	{
		_directoryInfo = directoryInfo!;
		_parent = parent;

		DisplayName = _directoryInfo.Name;
		Name = _directoryInfo.Name;

		if (HasSubFolders())
		{
			SubFolders.Add(UnExpanded);
		}
	}

	private DirectoryViewModel()
	{
		_directoryInfo = null!;
		DisplayName = "Loading...";
		Name = "";
		Icon = Icons.OpenFolder;

		SubFolders = new ObservableCollection<DirectoryViewModel>();

		// Must come last, due to OnXxxxChanged() Events.
		//
		IsSelectable = true;
		IsSelected = false;
		IsExpanded = false;
	}

	public Icons Icon { get; protected set; }

	public string DisplayName { get; protected set; }
	public string Name { get; protected set; }

	public bool IsSelectable { get; set; }
	public bool? IsSelected { get; set; }
	public bool IsExpanded { get; set; }

	public ObservableCollection<DirectoryViewModel> SubFolders { get; private set; }

	public void LoadSubFolders()
	{
		SubFolders.Clear();

		foreach (var directoryInfo in _directoryInfo.GetDirectories("*", s_options))
		{
			SubFolders.Add(new DirectoryViewModel(directoryInfo, this));
		}
	}

	// Is called by PropertyChanged.Fody
	// ReSharper disable once UnusedMember.Global
	protected virtual void OnIsSelectedChanged()
	{
		UpdateIcon();

		if (_parent != null)
		{
			var allSiblings = _parent.SubFolders.All(i => i.IsSelected != false);
			var noSiblings = _parent.SubFolders.All(i => i.IsSelected == false);

			_parent.IsSelected = allSiblings ? true : noSiblings ? false : null;
		}

		if (IsSelected == false)
		{
			foreach (var subFolder in SubFolders)
			{
				subFolder.IsSelected = false;
			}
		}
	}

	private void UpdateIcon()
	{
		if (IsSelected == true)
		{
			Icon = IsExpanded ? Icons.SearchOpenFolder : Icons.SearchCloseFolder;
		}
		else
		{
			Icon = IsExpanded ? Icons.OpenFolder : Icons.CloseFolder;
		}
	}

	private bool HasSubFolders() => _directoryInfo.EnumerateDirectories("*", s_options).Any();

	private static readonly EnumerationOptions s_options = new ();

	private readonly DirectoryInfo _directoryInfo;
	private readonly DirectoryViewModel? _parent;
}