using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public TileBase GroundTile;
    public List<IceStarLevel> IceStarLevelList;
}
