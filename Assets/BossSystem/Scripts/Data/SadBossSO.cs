using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SadBossSO", fileName = "SadBossData")]
public class SadBossSO : ScriptableObject
{
    public List<SadBossDetail> BossList;
}

[Serializable]
public class SadBossDetail
{
    public int SadBossId;
    public List<float> Healths;
    public float CooldownTimeSkill;
    public int TotalItems;
    public Vector2Int StartPos;
    public Vector2Int EndPos;
    public SadBoss BossPrefab;
}
