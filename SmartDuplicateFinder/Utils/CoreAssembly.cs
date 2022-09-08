using System;
using System.IO;
using System.Reflection;

namespace SmartDuplicateFinder.Utils;

public static class CoreAssembly
{
	public static readonly Assembly Reference = typeof(CoreAssembly).Assembly;
	public static readonly string Folder = Path.GetDirectoryName(Reference.Location) ?? "";
	public static readonly Version Version = Reference.GetName().Version ?? new Version(0,0,0);
}