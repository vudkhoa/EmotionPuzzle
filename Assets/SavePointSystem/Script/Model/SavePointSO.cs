using System;
using System.Collections.Generic;
using UnityEngine;

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
}

[Serializable]
public class PointDetail
{
    public Vector2Int SavePoint;
    public Vector2Int StartPoint;
    public Vector2Int EndPoint;
}