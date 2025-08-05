using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GuideSO", fileName = "GuideData")]
public class GuideSO : ScriptableObject
{
    List<GuideDetail> GuideDetails;

    public GameObject GetGuidePrefab(int guideId)
    {
        return GuideDetails[guideId - 1].guidePrefab;
    }
}

[Serializable]
public class GuideDetail
{
    public int guideId;
    public GameObject guidePrefab;
}
