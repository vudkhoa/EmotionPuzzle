using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "SavePointSO", fileName = "SavePointData")]
public class SavePointSO : ScriptableObject
{
    public List<SavePointDetail> SavePointDetails;
}

[Serializable]
public class SavePointDetail
{
    public int SavePointId;
    public List<PointDetail> Points;
    public TileBase TileSavePoint;
}

[Serializable]
public class PointDetail
{
    public Vector2Int SavePoint;
    public Vector2Int StartPoint;
    public Vector2Int EndPoint;
}