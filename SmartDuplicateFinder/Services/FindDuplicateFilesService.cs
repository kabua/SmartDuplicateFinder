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

	public long TotalFiles { get; private set; }

	public void ReadAllFiles(ImmutableArray<DirectoryInfo> rootFolders)
	{
		StepUpdater.Update(0, total: rootFolders.Length);

		SummaryUpdater.Update(double.NaN);

		var options = new EnumerationOptions()
		{
			RecurseSubdirectories = true,
			ReturnSpecialDirectories = true,
		};

		var totalFiles = new List<FileInfo[]>(rootFolders.Length);
		for (int index = 0; index < rootFolders.Length; index++)
		{
			DirectoryInfo directoryInfo = rootFolders[index];

			StepUpdater.Update(index, directoryInfo.Name);
			var files = directoryInfo.GetFiles("*", options);
			totalFiles.Add(files);
		}

		TotalFiles = totalFiles.Aggregate(0L, (l, infos) => l + infos.Length);
	}
}
