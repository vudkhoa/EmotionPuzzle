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
    public Vector2Int PosForFire;
    public bool haveFire;
    public bool haveFirePopup;
    public float fireTime;

    public Vector2Int PosForWater;
    public bool haveWater;
    public bool haveWaterPopup;
    public float waterTime;

    public Vector2Int PosForIce;
    public bool haveIce;
    public bool haveIcePopup;
    public float iceTime;

    public Vector2Int PosForWind;
    public bool haveWind;
    public bool haveWindPopup;
    public float windTime;
}