using SoundManager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuideUI : UICanvas
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Transform guideContainer;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] private TextMeshProUGUI guideIdText;

    private int maxGuideId;
    private int curShowGuideId;
    private GameObject guideOb;


    public void Init()
    {
        maxGuideId = PlayerPrefs.GetInt(Constant.GUIDEID, 0);
        curShowGuideId = 1;
        nextBtn.interactable = false;
        ShowGuide();
    }

    public void Init(int id)
    {
        maxGuideId = PlayerPrefs.GetInt(Constant.GUIDEID, 0);
        curShowGuideId = id;
        ShowGuide(id);
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveListener(OnClickCloseBtn);
    }

    private void OnClickCloseBtn()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        GameManager.Instance.State = GameState.Playing;
        UIManager.Instance.CloseUI<GuideUI>();
    }

    public void OnNextGuide()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        curShowGuideId++;
        ShowGuide();
    }

    public void OnPrevGuide()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        curShowGuideId--;
        ShowGuide();
    }

    private void ShowGuide()
    {
        if (maxGuideId == 0)
        {
            if (guideOb != null)
            {
                Destroy(guideOb);
            }
            guideIdText.text = "0/0";
            return;
        }

        guideIdText.text = curShowGuideId.ToString() + "/" + maxGuideId.ToString();

        prevBtn.interactable = true;
        nextBtn.interactable = true;

        if (curShowGuideId == 1)
        {
            prevBtn.interactable = false;
        }

        if (curShowGuideId == maxGuideId)
        {
            nextBtn.interactable = false;
        }

        if (guideOb != null)
        {
            Destroy(guideOb);
        }

        guideOb = Instantiate(Resources.Load<GameObject>("Guide/Guide " + curShowGuideId.ToString()), guideContainer);
    }

    public void ShowGuide(int id)
    {
        curShowGuideId = id;

        guideIdText.text = curShowGuideId.ToString() + "/" + maxGuideId.ToString();

        prevBtn.interactable = true;
        nextBtn.interactable = true;

        if (curShowGuideId == 1)
        {
            prevBtn.interactable = false;
        }

        if (curShowGuideId == maxGuideId)
        {
            nextBtn.interactable = false;
        }

        if (guideOb != null)
        {
            Destroy(guideOb);
        }

        guideOb = Instantiate(Resources.Load<GameObject>("Guide/Guide " + curShowGuideId.ToString()), guideContainer);
    }
}
