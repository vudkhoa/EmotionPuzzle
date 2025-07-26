using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EmotionCoordinationSO", fileName = "EmotionCoordinationData")]
public class EmotionCoordinationSO : ScriptableObject
{
    public List<EmotionCoordinationDetail> EmotionCoordinationDetails;

    public EmotionType GetResult(EmotionType emotionType, ItemType itemType)
    {
        foreach (var item in EmotionCoordinationDetails)
        {
            if (item.EmotionType == emotionType && item.ItemType == itemType)
            {
                return item.Result;
            }
        }

        return EmotionType.None;
    }
}

[Serializable]
public class EmotionCoordinationDetail
{
    public EmotionType EmotionType;
    public ItemType ItemType;
    public EmotionType Result;
}
