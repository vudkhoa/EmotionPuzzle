using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AngryBossSO", fileName = "AngryBossData")]
public class AngryBossSO : ScriptableObject
{
    public List<AngryBossDetail> BossList;
}

[Serializable]
public class AngryBossDetail
{
    public int AngryBossId;
    public float Health;
    public float CooldownTimeSkill;
    public int TotalItems;
    public Vector2Int StartPos;
    public Vector2Int EndPos;
    public AngryBoss BossPrefab;
    public int TotalPhases;
}
