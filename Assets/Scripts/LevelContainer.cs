using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot("LevelCollection")]
public class LevelContainer {
    [XmlArray("Levels")]
    [XmlArrayItem("Level")]
    public List<Level> Levels = new List<Level>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(LevelContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static LevelContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(LevelContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as LevelContainer;
        }
    }

    public int GetTotalStars()
    {
        int TotalStars = 0;
        foreach (Level levelData in Levels)
        {
            if (levelData.CompletedStatus)
                TotalStars++;
            if (levelData.CoinStatus)
                TotalStars++;
            if (levelData.TimeStatus)
                TotalStars++;
        }
        return TotalStars;
    }

    public int GetLevelStars(int levelIndex)
    {
        int levelStars = 0;
        if (levelIndex >= 0 && levelIndex < Levels.Count)
        {
            if (Levels[levelIndex].CompletedStatus)
                levelStars++;
            if (Levels[levelIndex].CoinStatus)
                levelStars++;
            if (Levels[levelIndex].TimeStatus)
                levelStars++;
        }
        return levelStars;
    }
}
