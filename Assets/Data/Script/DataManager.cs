using UnityEngine;
using CustomUtils;

public class DataManager : SingletonMono<DataManager>
{
    [Header(" Level ")]
    public LevelSO LevelData;
    public ItemSO ItemData;
    public BlockSO BlockData;
    public TutorialSO TutorialData;

    [Header(" EESystem ")]
    public ElementSO ElementData;
    public EmotionCoordinationSO EmotionCoordinationData;
    public ElementCoordinateSO ElementCoordinateData;

    [Header(" Boss ")]
    public SadBossSO SadBossData;
    public HappyBossSO HappyBossData;
    public AngryBossSO AngryBossData;

    [Header(" Mini-game Mechanics")]
    public IceStarSO IceStarData;
    public RotateObjectSO RotateObjectData;
}