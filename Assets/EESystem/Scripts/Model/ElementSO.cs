using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ElementSO", fileName = "ElementData")]
public class ElementSO : ScriptableObject
{
    public List<ElementLevelDetail> ElementLevelDetails;
}

[Serializable]
public class ElementLevelDetail
{
    public int ElementId;
    public List<ElementDetail> ElementDetails;
}

[Serializable]
public class ElementDetail
{
    public Element Element;
    public EmotionType EmotionType;
    public Vector2Int ElementPos;
}

[Serializable]
public enum ElementType
{
    None = 0,
    Fire = 1,
    Wind = 2,
    Water = 3,
    Ice = 4,
}
