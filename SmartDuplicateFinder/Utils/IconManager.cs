using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmartDuplicateFinder.Utils
{
    public enum Icons
    {
        None,
        WindowsShield,
        BlankDocument,
        OpenFolder,
        Blank16x16,
        Blank256x256,
        SearchFolder,
        LinePaper,
        MusicDocument,
        Video,
        FloppyDrive,
        CDRomDrive,
        NetworkConnectedDrive,
        NetworkDisconnectedDrive,
        HardDrive,
        WindowsDrive,
        RemovableDrive,
        RecyclingFull,
        RecyclingEmpty,
        WordDocument,
        TextDocument,
        UnknownDrive,
        Cycle,
        BlueClouds,
        Cloud,
        Redo,
        Undo,
        SearchForward,
        Play,
    }

    public static class IconManager
    {
        public static ImageSource? GetIcon(Icons icon, int pixelSize)
        {
            if (icon == Icons.None)
                return null;

            if (!s_imageCache.TryGetValue(icon, out var value))
            {
                value = LoadImage(icon, pixelSize);
                s_imageCache[icon] = value;
            }

            return value;
        }

        private static ImageSource LoadImage(Icons icon, int pixelSize)
        {
            var (sourceFileIndex, imageIndex) = s_icons[icon];
            var sourceFile = s_sourceFiles[sourceFileIndex];
            var fileName = Environment.ExpandEnvironmentVariables(sourceFile);

            IntPtr hIcon = ExtractIcon(Process.GetCurrentProcess().Handle, fileName, imageIndex);
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromWidthAndHeight(pixelSize, pixelSize);

            ImageSource ret = Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, sizeOptions);
            return ret;
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);


        private static string[] s_sourceFiles = 
        {
            @"%SystemRoot%\System32\imageres.dll",
            @"%SystemRoot%\System32\shell32.dll",
        };

        private static Dictionary<Icons, (int sourceFileIndex, int imageIndex)> s_icons = new()
        {
            {Icons.WindowsShield, (0, 1)},
            {Icons.BlankDocument, (0, 2)},
            {Icons.OpenFolder, (0, 3)},
            {Icons.Blank16x16, (0, 10)},
            {Icons.Blank256x256, (0, 12)},
            {Icons.SearchFolder, (0, 13)},
            {Icons.LinePaper, (0, 14)},
            {Icons.MusicDocument, (0, 17)},
            {Icons.Video, (0, 18)},
            {Icons.FloppyDrive, (0, 23)},
            {Icons.CDRomDrive, (0, 25)},
            {Icons.NetworkConnectedDrive, (0, 28)},
            {Icons.NetworkDisconnectedDrive, (0, 26)},
            {Icons.HardDrive, (0, 27)},
            {Icons.WindowsDrive, (0, 31)},
            {Icons.RemovableDrive, (0, 39)},
            {Icons.RecyclingFull, (0, 49)},
            {Icons.RecyclingEmpty, (0, 50)},
            {Icons.WordDocument, (0, 85)},
            {Icons.TextDocument, (0, 97)},

            {Icons.UnknownDrive, (0, 70)},
            {Icons.Cycle, (0, 228)},
            {Icons.BlueClouds, (0, 220)},
            {Icons.Cloud, (0, 231)},
            {Icons.Redo, (0, 251)},
            {Icons.Undo, (0, 255)},
            {Icons.SearchForward, (0, 271)},
            {Icons.Play, (0, 280)},
            //{Icons.Xxxx, (0, -1)},
        };

        private static Dictionary<Icons, ImageSource> s_imageCache = new ();
    }
}
