using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDuplicateFinder.Services;

public interface IProgress
{
	string Description { get; }
	double Current { get; }
	double Total { get; }
}

public interface IUpdateProgress
{
	void Update(double current, string? description = null, double? total = null);
}