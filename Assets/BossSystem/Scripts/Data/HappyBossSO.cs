using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HappyBossSO", fileName = "HappyBossData")]
public class HappyBossSO : ScriptableObject
{
    public List<HappyBossDetail> BossList;
}

[Serializable]
public class HappyBossDetail
{
    public int HappyBossId;
    public float Health;
    public float CooldownTimeSkill;
    public int TotalItems;
    public Vector2Int StartPos;
    public Vector2Int EndPos;
    public HappyBoss BossPrefab;
    public int TotalPhases;
}
