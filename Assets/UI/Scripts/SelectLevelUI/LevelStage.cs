using DG.Tweening;
using SoundManager;
using UnityEngine;
using UnityEngine.UI;

public class LevelStage : MonoBehaviour
{
    [Header(" Data ")]
    public int LevelId;

    [Header(" UI ")]
    public Button LevelBtn;
    public RectTransform lockImage;
    public CanvasGroup stageGroup;
    public RectTransform imageRect;
    public CanvasGroup textGroup;

    private void OnEnable()
    {
        LevelBtn.onClick.AddListener(OnClickLevelBtn);
    }

    private void OnDisable()
    {
        LevelBtn.onClick.RemoveListener(OnClickLevelBtn);
    }

    private void OnClickLevelBtn()
    {
        PlayerPrefs.SetInt(Constant.LEVELID, LevelId);
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        LoadingManager.instance.LoadScene("Platform " + LevelId.ToString());
    }

    public void Setup()
    {
        int maxLevelId = PlayerPrefs.GetInt(Constant.MAXLEVELID, 0);
        if (this.LevelId <= maxLevelId)
        {
            LevelBtn.interactable = true;
            lockImage.gameObject.SetActive(false);
            imageRect.gameObject.SetActive(true);
            textGroup.gameObject.SetActive(true);
        }
        else
        {
            LevelBtn.interactable = false;
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
