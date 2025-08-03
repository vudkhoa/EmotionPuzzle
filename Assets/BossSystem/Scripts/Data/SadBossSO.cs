using System;
using System.Collections;
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
    public float Health;
    public float CooldownTimeSkill;
    public int TotalItems;
    public Vector2Int StartPos;
    public Vector2Int EndPos;
    public SadBoss BossPrefab;
    public int TotalPhases;
}
