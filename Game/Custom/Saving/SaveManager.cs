using System.Collections.Generic;
using System.IO;

namespace Game.Custom.Saving;

public class SaveManager
{
    private string saveDirectory = "Saves/";

    public SaveManager()
    {
        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);
    }

    // Get all saves
    public List<string> GetSaves()
    {
        List<string> saveFiles = [];

        string[] files = Directory.GetFiles(saveDirectory, "*.db");
        foreach (string file in files)
        {
            saveFiles.Add(Path.GetFileNameWithoutExtension(file)); // Get filename only
        }

        return saveFiles;
    }

    // Get full path of a save
    public string GetSavePath(string saveName)
    {
        return Path.Combine(saveDirectory, saveName + ".db");
    }

    // Create a new save file
    public string CreateNewSave()
    {
        int saveIndex = Directory.GetFiles(saveDirectory, "*.db").Length + 1;
        string saveFile = $"save_{saveIndex:D3}.db"; // Format: save_001.db, save_002.db
        string savePath = GetSavePath(saveFile);

        File.Create(savePath).Close(); // Create the DB file
        return saveFile;
    }
}
