using DG.Tweening;
using UnityEngine;

public class LevelStage : MonoBehaviour
{
    [Header(" Data ")]
    public int LevelId;

    [Header(" UI ")]
    public RectTransform lockImage;
    public CanvasGroup stageGroup;
    public RectTransform imageRect;
    public CanvasGroup textGroup;

    public void Setup()
    {
        int maxLevelId = PlayerPrefs.GetInt(Constant.MAXLEVELID, 0);
        if (this.LevelId <= maxLevelId)
        {
            lockImage.gameObject.SetActive(false);
            imageRect.gameObject.SetActive(true);
            textGroup.gameObject.SetActive(true);
        }
        else
        {
            lockImage.gameObject.SetActive(true);
            imageRect.gameObject.SetActive(false);
            textGroup.gameObject.SetActive(false);
        }
    }

    public void PlayUnlockAnimation()
    {
        stageGroup.alpha = 0f;
        if (imageRect != null) imageRect.localScale = Vector3.zero;
        if (textGroup != null) textGroup.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        seq.Append(stageGroup.DOFade(1f, 0.5f));

        if (imageRect != null)
            seq.Append(imageRect.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        if (textGroup != null)
            seq.Append(textGroup.DOFade(1f, 0.4f));

        seq.Play();
    }
}
