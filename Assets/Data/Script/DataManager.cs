using UnityEngine;
using CustomUtils;

public class DataManager : SingletonMono<DataManager>
{
    public LevelSO LevelData;
    public ItemSO ItemData;
    public ElementSO ElementData;
    public EmotionCoordinationSO EmotionCoordinationData;
    public ElementCoordinateSO ElementCoordinateData;
    public SadBossSO SadBossData;
    public HappyBossSO HappyBossData;
    public AngryBossSO AngryBossData;

    [Header(" Mini-game Mechanics")]
    public IceStarSO IceStarData;
}