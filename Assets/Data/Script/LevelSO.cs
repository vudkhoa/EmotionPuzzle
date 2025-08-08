using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "LevelSO", fileName = "LevelData")]
public class LevelSO : ScriptableObject
{
    public List<LevelDetail> LevelDetails;
}

[Serializable]
public class LevelDetail
{
    public int LevelId;
    public bool IsBoss;
    public Vector2Int PlayerPosition;
    public int TutorialId;
    public int ItemId;
    public int BlockId;
    public int ElementId;
    public int AngryBossId;
    public int SadBossId;
    public int HappyBossId;

    [Header(" Mini-game Mechanics ")]
    public int IceStarId;
    public int RotateObId;

    [Header(" Next Load ")]
    public Vector2Int NextLevelPos;

    [Header(" Power ")]
    public Sprite PowerSprite;
}
