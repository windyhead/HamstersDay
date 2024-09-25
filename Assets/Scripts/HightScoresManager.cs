using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class HighscoresManager : SingletonBehaviour<HighscoresManager>
{
    private string fileAddress;

    private List<HighscoresEntry> entries;

    private void Awake()
    {
        fileAddress =  Application.persistentDataPath + "/save.dat";
        entries = new List<HighscoresEntry>();
        TryCreateScoresFile();
    }

    public void AddScore(string name)
    {
        entries.Add(new HighscoresEntry(name,GameController.PlayersFat, GameController.CurrentStage));
        entries = new List<HighscoresEntry>(entries.OrderByDescending(x => x.Score).ThenByDescending(x=>x.Fat).ThenByDescending(x=>x.Stage));
        SaveFile();
    }

    private void TryCreateScoresFile()
    {
        if (File.Exists(fileAddress))
        {
            //File.Delete(fileAddress);
           LoadFile();
        }
        else 
            SaveFile();
    }
    
    private void SaveFile()
    {
        FileStream file;
        if (File.Exists(fileAddress))
            file = File.OpenWrite(fileAddress);
        else 
            file = File.Create(fileAddress);
        
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, new Highscores(entries:entries.ToArray()));
        file.Close();
    }

    public List<HighscoresEntry> GetHighScores()
    {
        return entries;
    }

    private void LoadFile()
    {
        FileStream file;
        if(File.Exists(fileAddress)) file = File.OpenRead(fileAddress);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        var data = (Highscores) bf.Deserialize(file);
        file.Close();

        entries = data.Entries.ToList();
    }
    
    [Serializable]
    public class HighscoresEntry
    {
        public HighscoresEntry(string name, int fat, int stage)
        {
            Name = name;
            Score = fat + stage;
            Fat = fat;
            Stage = stage;
        }

        public string Name {get; private set;}
        
        public int Score {get; private set;}
        public int Fat {get; private set;}
        public int Stage {get; private set;}
    }
    
    [Serializable]
    private class Highscores
    {
        public HighscoresEntry[] Entries;

        public Highscores(HighscoresEntry[] entries)
        {
            Entries = entries;
        }
    }
}
