using System.Xml;
using System.Xml.Serialization;

public class Level {
    [XmlAttribute("index")]
    public int Index = 0;

    public int StarsToUnlock = 0;
    public bool CompletedStatus = false;
    public bool CoinStatus = false;
    public bool TimeStatus = false;

    public Level()
    {
        Index = 0;
        StarsToUnlock = 0;
        CompletedStatus = false;
        CoinStatus = false;
        TimeStatus = false;
    }

    public Level(int index, int stars)
    {
        Index = index;
        StarsToUnlock = stars;
        CompletedStatus = false;
        CoinStatus = false;
        TimeStatus = false;
    }
}
