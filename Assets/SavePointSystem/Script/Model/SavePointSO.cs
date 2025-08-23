using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SavePointSO", fileName = "SavePointData")]
public class SavePointSO : ScriptableObject
{
    public int SavePointId;
    public List<Vector2Int> Points;
}
