using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemSO", fileName = "ItemData")]
public class ItemSO : ScriptableObject
{
    public List<ItemDetail> ItemDetails;
}

[Serializable]
public class ItemDetail
{
    public int ItemId;
    public List<ItemType> ItemTypes;
    public List<Vector2Int> ItemPos;
}

[Serializable]
public enum ItemType
{
    None = 0,
    MakeAngry = 2,
    MakeHappy = 3,
    MakeSad = 4,
}
