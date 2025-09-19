using DG.Tweening;
using SoundManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField] private RectTransform title;
    [SerializeField] private RectTransform bg;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private GameObject cheatGO;

    private void Start()
    {
        PlayerPrefs.SetInt(Constant.LEVELID, 1);
        PlayerPrefs.SetInt(Constant.GUIDEID, -100);
        PlayerPrefs.SetInt(Constant.ISRETURNMENU, 0);
        PlayerPrefs.Save();

        if (PlayerPrefs.GetInt(Constant.MAXLEVELID, 0) == 0)
        {
            continueBtn.gameObject.SetActive(false);
            startBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
        }

        AnimateTitle();
        AnimateBackground();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cheatGO.SetActive(true);
        }
    }

    public void StartGame()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        PlayerPrefs.SetInt(Constant.MAXLEVELID, 0);
        LoadingManager.instance.LoadScene("Intro");
    }

    public void ContinueGame()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        LoadingManager.instance.LoadScene("SelectLevelScene");
    }

    public void QuitGame()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        Application.Quit();
    }

    public void PlayLevel(int level)
    {
        PlayerPrefs.SetInt(Constant.LEVELID, level);
        PlayerPrefs.SetInt(Constant.ISRETURNMENU, 1);
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        LoadingManager.instance.LoadScene("Puzzle");
    }

    private void AnimateTitle()
    {
        title.DOScale(1.1f, 1f)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo);

        title.DOAnchorPosY(title.anchoredPosition.y + 10f, 2f)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo);
    }

    void AnimateBackground()
    {
        
    }
}
