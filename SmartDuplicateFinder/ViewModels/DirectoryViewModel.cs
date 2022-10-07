using SmartDuplicateFinder.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SmartDuplicateFinder.ViewModels;

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

		IsSelectable = true;
		IsSelected = false;
		IsExpanded = false;
		Icon = Icons.OpenFolder;

		SubFolders = new ObservableCollection<DirectoryViewModel>();
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

	private bool HasSubFolders() => _directoryInfo.EnumerateDirectories("*", s_options).Any();

	private static readonly EnumerationOptions s_options = new ();

	private readonly DirectoryInfo _directoryInfo;
	private readonly DirectoryViewModel? _parent;
}