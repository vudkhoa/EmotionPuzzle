using System;
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
    [Header(" Fire ")]
    public Vector2Int PosForFire;
    public bool haveFire;
    public bool haveFirePopup;
    public float fireTime;

    [Header(" Water ")]
    public Vector2Int PosForWater;
    public bool haveWater;
    public bool haveWaterPopup;
    public float waterTime;

    [Header(" Ice ")]
    public Vector2Int PosForIce;
    public bool haveIce;
    public bool haveIcePopup;
    public float iceTime;

    [Header(" Wind ")]
    public Vector2Int PosForWind;
    public bool haveWind;
    public bool haveWindPopup;
    public float windTime;
}