using System;
using UnityEngine;

public abstract class Element : MonoBehaviour
{
    public ElementType ElementType;
    public EmotionType EmotionType;

    public void InteractWithItem(ItemType itemType)
    {
        EmotionType newEmotionType = DataManager.Instance.EmotionCoordinationData.GetResult(this.EmotionType, itemType);

        this.EmotionType = newEmotionType;
    }

    public abstract void CoordinateWithElement(ElementType elementType);

    public virtual void Power()
    {
        if (this.EmotionType != EmotionType.Angry)
        {
            return;
        }
    }
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
