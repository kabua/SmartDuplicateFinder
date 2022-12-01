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
		StepProgress = new DesignTimeProgressManager();
		SummaryProgress = new DesignTimeProgressManager();
	}

	public IProgress StepProgress { get; set; }
	public IProgress SummaryProgress  { get; set; }

	public void ReadAllFiles(ImmutableArray<DirectoryInfo> rootFolders)
	{
	}
}
