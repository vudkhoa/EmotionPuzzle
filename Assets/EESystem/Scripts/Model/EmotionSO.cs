using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EmotionSO", fileName = "EmotionData")]
public class EmotionSO : ScriptableObject
{
    public List<EmotionDetail> EmotionDetails;

    public Sprite GetSpriteEmotionForElement(ElementType elementType, EmotionType emotionType)
    {
        foreach (EmotionDetail detail in EmotionDetails)
        {
            if (detail.ElementType == elementType)
            {
                if (emotionType == EmotionType.Neutral)
                {
                    return detail.NeutralIcon;
                }
                else if (emotionType == EmotionType.Angry)
                {
                    return detail.AngryIcon;
                }
                else if (emotionType == EmotionType.Happy)
                {
                    return detail.HappyIcon;
                }
                else 
                {
                    return detail.SadIcon;
                }
            }
        }

        return null;
    }
}

[Serializable]
public class EmotionDetail
{
    public ElementType ElementType;
    public Sprite NeutralIcon;
    public Sprite AngryIcon;
    public Sprite HappyIcon;
    public Sprite SadIcon;
}

[Serializable]
public enum EmotionType
{
    None = 0,
    Neutral = 1,
    Angry = 2,
    Happy = 3,
    Sad = 4,
}