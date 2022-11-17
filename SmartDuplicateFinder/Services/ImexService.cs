using System;
using System.IO;
using Newtonsoft.Json;

namespace SmartDuplicateFinder.Services;

public class ImexService
{
    public ImexService()
    {
    }

    public void Save(string fileName, string[] rootFolders)
    {
        File.WriteAllText(fileName, JsonConvert.SerializeObject(rootFolders));
    }

    public string[] Load(string fileName)
    {
        var rootFolders = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(fileName));

        return rootFolders ?? Array.Empty<string>();
    }
}
