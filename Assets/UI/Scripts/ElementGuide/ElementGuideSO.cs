using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ElementGuideSO", fileName = "ElementGuideData")]
public class ElementGuideSO : ScriptableObject
{
    public List<ElementGuideLevelDetail> ElementGuideDetails;
}

[Serializable]
public class ElementGuideLevelDetail
{
    public int Id;
    public ElementGuideDetail Detail;
}

[Serializable]
public class ElementGuideDetail
{
    public Vector2Int PosForFire;
    public bool haveFire;
    public Vector2Int PosForWater;
    public bool haveWater;
    public Vector2Int PosForIce;
    public bool haveIce;
    public Vector2Int PosForWind;
    public bool haveWind;
}