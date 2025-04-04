using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Microsoft.Xna.Framework;

namespace Game.Custom.Saving
{
    public class SaveManager
    {
        private string dbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves", "GameSaves.db");

        public SaveManager()
        {
            string saveDir = Path.GetDirectoryName(dbPath)!;
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                InitializeDatabase();
            }
        }

        public string GetSavePath() => dbPath;

        // Initializes the database schema
        public void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();

                string createGameSavesTable = @"
                    CREATE TABLE IF NOT EXISTS GameSaves (
                        GameId INTEGER PRIMARY KEY AUTOINCREMENT,
                        SaveName TEXT NOT NULL
                    );";

                string createItemsTable = @"
                    CREATE TABLE IF NOT EXISTS Items (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        GameId INTEGER NOT NULL,
                        Name TEXT NOT NULL,
                        Quantity INTEGER NOT NULL,
                        Place TEXT NOT NULL,
                        FOREIGN KEY (GameId) REFERENCES GameSaves(GameId)
                    );";

                string createEntitiesTable = @"
                    CREATE TABLE IF NOT EXISTS Entities (
                        EntityId INTEGER PRIMARY KEY AUTOINCREMENT,
                        GameId INTEGER NOT NULL,
                        PositionX REAL NOT NULL,
                        PositionY REAL NOT NULL,
                        Type TEXT NOT NULL,
                        HP INTEGER NOT NULL,
                        FOREIGN KEY (GameId) REFERENCES GameSaves(GameId)
                    );";

                new SQLiteCommand(createGameSavesTable, connection).ExecuteNonQuery();
                new SQLiteCommand(createItemsTable, connection).ExecuteNonQuery();
                new SQLiteCommand(createEntitiesTable, connection).ExecuteNonQuery();
            }
        }

        // Creates a new game save entry in the database
        public int CreateNewSave(string saveName)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();

                string insertSave = "INSERT INTO GameSaves (SaveName) VALUES (@name);";

                using (var command = new SQLiteCommand(insertSave, connection))
                {
                    command.Parameters.AddWithValue("@name", saveName);
                    command.ExecuteNonQuery();
                }

                return (int)connection.LastInsertRowId;
            }
        }

        // Retrieves all saved games
        public List<GameSave> GetAllGameSaves()
        {
            var saves = new List<GameSave>();

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT GameId, SaveName FROM GameSaves ORDER BY GameId ASC;";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        saves.Add(new GameSave
                        {
                            GameId = reader.GetInt32(0),
                            SaveName = reader.GetString(1)
                        });
                    }
                }
            }

            return saves;
        }

        // Adds an item to a specific game save
        public void AddItem(int gameId, string name, int quantity, string place)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO Items (GameId, Name, Quantity, Place) VALUES (@gid, @name, @quantity, @place);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@gid", gameId);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@place", place);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Adds an entity to a specific game save
        public void AddEntity(int gameId, Vector2 position, string type, int hp)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO Entities (GameId, PositionX, PositionY, Type, HP) VALUES (@gid, @x, @y, @type, @hp);";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@gid", gameId);
                    command.Parameters.AddWithValue("@x", position.X);
                    command.Parameters.AddWithValue("@y", position.Y);
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@hp", hp);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Retrieves all items for a specific game save
        public List<(int ItemId, string Name, int Quantity, string Place)> GetItems(int gameId)
        {
            var items = new List<(int, string, int, string)>();

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Id, Name, Quantity, Place FROM Items WHERE GameId = @gid;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@gid", gameId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add((
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetInt32(2),
                                reader.GetString(3)
                            ));
                        }
                    }
                }
            }

            return items;
        }

        // Retrieves all entities for a specific game save
        public List<(int EntityId, Vector2 Position, string Type, int HP)> GetEntities(int gameId)
        {
            var entities = new List<(int, Vector2, string, int)>();

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT EntityId, PositionX, PositionY, Type, HP FROM Entities WHERE GameId = @gid;";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@gid", gameId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            entities.Add((
                                reader.GetInt32(0),
                                new Vector2(reader.GetFloat(1), reader.GetFloat(2)),
                                reader.GetString(3),
                                reader.GetInt32(4)
                            ));
                        }
                    }
                }
            }

            return entities;
        }

        // Starts the game from a specific save
        public void StartFromSave(int gameId)
        {
            List<(int EntityId, Vector2 Position, string Type, int HP)> existingEntities = GetEntities(gameId);
            List<(int ItemId, string Name, int Quantity, string Place)> existingItems = GetItems(gameId);

            // Process entities
            foreach (var entity in existingEntities)
            {
                switch (entity.Type)
                {
                    case "Player":
                        Factories.EntityFactory.CreatePlayerAt(entity.Position);
                        break;
                    case "Slime":
                        Factories.EntityFactory.CreateSlimeAt(entity.Position);
                        break;
                    case "Sword":
                        Factories.EntityFactory.CreateSwordAt(entity.Position);
                        break;
                    // Add more entity types as needed
                }
            }

            // Process items
            foreach (var item in existingItems)
            {
                // TODO: Add your item handling logic here
                // Example:
                // ItemFactory.CreateItem(item.Name, item.Place, item.Quantity);
            }
        }

        // Structure to represent a game save
        public struct  GameSave 
        {
            public int GameId;
            public string SaveName;
        }
    }
}
