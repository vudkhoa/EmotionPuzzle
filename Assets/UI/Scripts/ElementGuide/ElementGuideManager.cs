using CustomUtils;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElementGuideManager : SingletonMono<ElementGuideManager>
{
    [Header(" Btn ")]
    public bool haveFire;
    public bool haveWater;
    public bool haveIce;
    public bool haveWind;

    [Header(" Popup ")]
    public bool haveFirePopup;
    public bool haveWaterPopup;
    public bool haveIcePopup;
    public bool haveWindPopup;

    [Header(" Position ")]
    public Vector2Int PosForFire;
    public Vector2Int PosForWater;
    public Vector2Int PosForIce;
    public Vector2Int PosForWind;

    [Header(" Time ")]
    public float timeFire;
    public float timeWater;
    public float timeIce;
    public float timeWind;

    public List<bool> isShowBtn;

    public void SetUp(int elementGuideId)
    {
        ElementGuideLevelDetail data = DataManager.Instance.ElementGuideData.ElementGuideDetails[elementGuideId - 1];

        this.haveFire = data.Detail.haveFire;
        this.haveWater = data.Detail.haveWater;
        this.haveIce = data.Detail.haveIce;
        this.haveWind = data.Detail.haveWind;

        this.haveFirePopup = data.Detail.haveFirePopup;
        this.haveWaterPopup = data.Detail.haveWaterPopup;
        this.haveIcePopup = data.Detail.haveIcePopup;
        this.haveWindPopup = data.Detail.haveWindPopup;

        this.PosForFire = data.Detail.PosForFire;
        this.PosForWater = data.Detail.PosForWater;
        this.PosForIce = data.Detail.PosForIce;
        this.PosForWind = data.Detail.PosForWind;

        this.timeFire = data.Detail.fireTime;
        this.timeWater = data.Detail.waterTime;
        this.timeIce = data.Detail.iceTime;
        this.timeWind = data.Detail.windTime;

        this.isShowBtn = new List<bool>();
        for (int i = 0; i < 4; ++i)
        {
            this.isShowBtn.Add(false);
        }
    }

    public void ShowElementGuide(Vector2Int playerPos)
    {
        if (haveFire && playerPos == PosForFire && !this.isShowBtn[0])
        {
            this.isShowBtn[0] = true;
            UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideBtn(true, false, false, false);

            if (haveFirePopup)
            {
                StartCoroutine(UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideUI(true, false, false, false, timeFire));
            }
        }

        if (haveWater && playerPos == PosForWater && !this.isShowBtn[1])
        {
            this.isShowBtn[1] = true;
            UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideBtn(false, true, false, false);
            if (haveWaterPopup)
            {
                StartCoroutine(UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideUI(false, true, false, false, timeWater));
            }
        }

        if (haveIce && playerPos == PosForIce && !this.isShowBtn[2])
        {
            this.isShowBtn[2] = true;
            UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideBtn(false, false, false, true);
            if (haveIcePopup)
            {
                StartCoroutine(UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideUI(false, false, true, false, timeWind));
            }
        }

        if (haveWind && playerPos == PosForWind && !this.isShowBtn[3])
        {
            this.isShowBtn[3] = true;
            UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideBtn(false, false, true, false);
            if (haveWindPopup)
            {
                StartCoroutine(UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideUI(false, false, false, true, timeIce));
            }
        }

        //Button Number 
        UIManager.Instance.GetUI<GameplayUI>().UpdateElementButtonNumberGuide();
    }

    public void ResetIsElementGuide()
    {
        SlideController.Instance.isElementGuideUI = false;
    }
}