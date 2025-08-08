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

    private void Start()
    {
        maxGuideId = PlayerPrefs.GetInt(Constant.GUIDEID, 0);
        curShowGuideId = 1;
        nextBtn.interactable = false;
        ShowGuide();
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
        GameManager.Instance.State = GameState.Playing;
        UIManager.Instance.CloseUI<GuideUI>();
    }

    public void OnNextGuide()
    {
        curShowGuideId++;
        ShowGuide();
    }

    public void OnPrevGuide()
    {
        curShowGuideId--;
        ShowGuide();
    }

    private void ShowGuide()
    {
        if (curShowGuideId == 0)
        {
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
}
