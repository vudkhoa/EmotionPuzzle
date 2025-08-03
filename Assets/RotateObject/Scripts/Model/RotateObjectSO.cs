using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RotateObjectSO", fileName = "RotateObjectData")]
public class RotateObjectSO : ScriptableObject
{
    public List<RotateObjectLevelDetail> RotateObjectLevelDetails;
}

[Serializable]
public class RotateObjectLevelDetail
{
    public int RotateObjectId;
    public List<RotateObjectDetail> RotateObjectDetails;   
}

[Serializable]
public class RotateObjectDetail
{
    public Vector2Int rotatePos;
    public List<Vector2Int> containPosList;
    public RotateObject RotateObjectPrefab;
    public float angle;
}
