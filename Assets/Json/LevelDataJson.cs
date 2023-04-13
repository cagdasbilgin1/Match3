using System.Collections.Generic;

[System.Serializable]
public class LevelItem
{
    public int colorIndex;
}

[System.Serializable]
public class LevelDataJson
{
    public List<LevelItem> levelItems;
}