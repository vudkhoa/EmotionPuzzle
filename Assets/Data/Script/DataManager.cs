using UnityEngine;
using CustomUtils;

public class DataManager : SingletonMono<DataManager>
{
    public LevelSO LevelData;
    public ItemSO ItemData;
    public ElementSO ElementData;
    public EmotionCoordinationSO EmotionCoordinationData;
    public ElementCoordinateSO ElementCoordinateData;
}
