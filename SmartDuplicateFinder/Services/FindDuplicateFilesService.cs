using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDuplicateFinder.Services;
public class FindDuplicateFilesService
{
	public FindDuplicateFilesService()
	{
		StepUpdater = new DesignTimeProgressManager();
		SummaryUpdater = new DesignTimeProgressManager();
	}

	public IUpdateProgress StepUpdater { get; set; }
	public IUpdateProgress SummaryUpdater  { get; set; }

	public void ReadAllFiles(ImmutableArray<DirectoryInfo> rootFolders)
	{
		StepUpdater.Update(double.NaN);

		SummaryUpdater.Update(0, "Reading files", rootFolders.Length);

		var options = new EnumerationOptions()
		{
			RecurseSubdirectories = true,
			ReturnSpecialDirectories = true,
		};

		var current = 0;
		foreach (var directoryInfo in rootFolders)
		{
			var files = directoryInfo.GetFileSystemInfos("*", options);
			SummaryUpdater.Update(++current);
		}
	}
}
