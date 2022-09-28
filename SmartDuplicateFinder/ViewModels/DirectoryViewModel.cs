using SmartDuplicateFinder.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace SmartDuplicateFinder.ViewModels;

public class DirectoryViewModel : INotifyPropertyChanged
{
	public static readonly DirectoryViewModel UnExpanded = new ();

	public event PropertyChangedEventHandler? PropertyChanged;

	internal DirectoryViewModel(DirectoryInfo directoryInfo)
		: this()
	{
		DirectoryInfo = directoryInfo;

		DisplayName = DirectoryInfo.Name;
		Name = DirectoryInfo.Name;
	}

	private DirectoryViewModel()
	{
		DirectoryInfo = null!;
		DisplayName = "Loading...";
		Name = "";

		IsSelectable = true;
		IsSelected = false;
		IsExpanded = false;
		Icon = Icons.OpenFolder;

		SubFolders = new ObservableCollection<DirectoryViewModel>
		{
			UnExpanded
		};
	}

	public Icons Icon { get; private set; }

	public string DisplayName { get; protected set; }
	public string Name { get; protected set; }

	public bool IsSelectable { get; set; }
	public bool? IsSelected { get; set; }
	public bool IsExpanded { get; set; }

	public DirectoryInfo DirectoryInfo { get; private set; }

	public ObservableCollection<DirectoryViewModel> SubFolders { get; private set; }
}