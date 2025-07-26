using DG.Tweening;
using System;
using UnityEngine;

public abstract class Element : MonoBehaviour
{
    public ElementType ElementType;
    public EmotionType EmotionType;
    public Vector2Int CurrentPos;

    public virtual void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        this.EmotionType = emotionType;
        this.CurrentPos = currentPos;
    }

    public void InteractWithItem(ItemType itemType)
    {
        EmotionType newEmotionType = DataManager.Instance.EmotionCoordinationData.GetResult(this.EmotionType, itemType);

        this.EmotionType = newEmotionType;
    }

    public abstract void CoordinateWithElement(ElementType elementType);

    public abstract void Power();

    public void MoveTo(Vector2Int newGridPos, Vector3 worldPos)
    {
        if (this.EmotionType == EmotionType.Neutral)
        {
            return;
        }

        if (Vector2Int.Distance(newGridPos, this.CurrentPos) != 1f)
        {
            // 1. Scale nhỏ dần để ẩn tile
            this.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                // 2. Di chuyển đến đầu hàng
                this.transform.position = worldPos;
                this.CurrentPos = newGridPos;

                // 3. Scale lớn lên để hiện tile lại
                this.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
            });
        }
        else
        {
            CurrentPos = newGridPos;
            transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
        }

        Invoke(nameof(Power), 0.25f);
    }
}
