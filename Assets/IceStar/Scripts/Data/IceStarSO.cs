using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IceStarDetail
{ 
    public Vector2Int Position;
    public Direction Direction;
    public Vector2Int LockPosition;
}

[Serializable]
public class IceStarLevel
{
    public int IceStarID;
    public List<IceStarDetail> IceStarList;
}

[CreateAssetMenu(fileName = "IceStarSO", menuName = "IceStarData")]
public class IceStarSO : ScriptableObject
{
    public Sprite LightSprite;
    public GameObject CrossingLightPrefab;
    public Sprite LockSprite;
    public List<IceStarLevel> IceStarLevelList;
}
