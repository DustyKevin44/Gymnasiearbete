using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.SQLite;
using System.IO;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using MonoGameGum.Forms.Controls;

namespace Game.Custom.Saving;

public class SaveManager
{
    private string saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");

    //private string saveDirectory = "Saves/";

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
    public string CreateNewSave(string saveName)
    {
        int saveIndex = Directory.GetFiles(saveDirectory, "*.db").Length + 1;
        string saveFile = $"save_{saveName:D3}.db"; // Format: save_001.db, save_002.db
        string savePath = GetSavePath(saveFile);

        File.Create(savePath).Close(); // Create the DB file

        return saveFile;
    }
    public void InitializeDatabase(string dbPath)
    {
        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();

            string createItemsTable = @"
                CREATE TABLE IF NOT EXISTS Items (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Place TEXT NOT NULL
                );";

            string createEntitiesTable = @"
                CREATE TABLE IF NOT EXISTS Entities (
                    EntityId INTEGER PRIMARY KEY AUTOINCREMENT,
                    PositionX REAL NOT NULL,
                    PositionY REAL NOT NULL,
                    Type TEXT NOT NULL,
                    HP INTEGER NOT NULL
                );";

            using (var command = new SQLiteCommand(createItemsTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(createEntitiesTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
    public void AddItem(string dbPath, string name, int quantity, string place)
    {
        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();
            string query = "INSERT INTO Items (Name, Quantity, Place) VALUES (@name, @quantity, @place);";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@place", place);
                command.ExecuteNonQuery();
            }
        }
    }
    public void AddEntity(string dbPath, Vector2 position, string type, int hp)
    {
        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();
            string query = "INSERT INTO Entities (PositionX, PositionY, Type, HP) VALUES (@x, @y, @type, @hp);";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@x", position.X);
                command.Parameters.AddWithValue("@y", position.Y);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@hp", hp);
                command.ExecuteNonQuery();
            }
        }
    }
    public List<(int ItemId, string Name, int Quantity, string Place)> GetItems(string dbPath)
    {
        var items = new List<(int, string, int, string)>();

        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();
            string query = "SELECT * FROM Items;";

            using (var command = new SQLiteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add((
                        reader.GetInt32(0), // Id
                        reader.GetString(1), // Name
                        reader.GetInt32(2), // Quantity
                        reader.GetString(3) // Place
                    ));
                }
            }
        }

        return items;
    }
    public List<(int EntityId, Vector2 Position, string Type, int HP)> GetEntities(string dbPath)
    {
        var entities = new List<(int, Vector2, string, int)>();

        using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
        {
            connection.Open();
            string query = "SELECT * FROM Entities;";

            using (var command = new SQLiteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    entities.Add((
                        reader.GetInt32(0), // EntityId
                        new Vector2(reader.GetFloat(1), reader.GetFloat(2)), // Position (X, Y)
                        reader.GetString(3), // Type
                        reader.GetInt32(4) // HP
                    ));
                }
            }
        }

        return entities;
    }
    public void StartFromSaveFile(string dbPath)
    {
        List<(int EntityId, Vector2 Position, string Type, int HP)> existingEntities = GetEntities(dbPath);
        List<(int ItemId, string Name, int Quantity, string Place)> existingItems = GetItems(dbPath);
        foreach (var entity in existingEntities)
        {
            if (entity.Type == "Player")
            {
                Factories.EntityFactory.CreatePlayerAt(entity.Position);
            }
            if (entity.Type == "Slime")
            {
                Factories.EntityFactory.CreateSlimeAt(entity.Position);
            }
            if (entity.Type == "Sword")
            {
                Factories.EntityFactory.CreateSwordAt(entity.Position);
            }

        }

        foreach (var item in existingItems)
        {
            
        }
        
    }
}
