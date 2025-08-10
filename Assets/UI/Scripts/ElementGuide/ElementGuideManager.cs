using CustomUtils;
using System.Collections.Generic;
using UnityEngine;

public class ElementGuideManager : SingletonMono<ElementGuideManager>
{
    [Header(" Infor ")]
    public bool haveFire;
    public bool haveWater;
    public bool haveIce;
    public bool haveWind;

    public Vector2Int PosForFire;
    public Vector2Int PosForWater;
    public Vector2Int PosForIce;
    public Vector2Int PosForWind;

    public List<bool> isShowBtn;

    public void SetUp(int elementGuideId)
    {
        ElementGuideLevelDetail data = DataManager.Instance.ElementGuideData.ElementGuideDetails[elementGuideId - 1];

        this.haveFire = data.Detail.haveFire;
        this.haveWater = data.Detail.haveWater;
        this.haveIce = data.Detail.haveIce;
        this.haveWind = data.Detail.haveWind;

        this.PosForFire = data.Detail.PosForFire;
        this.PosForWater = data.Detail.PosForWater;
        this.PosForIce = data.Detail.PosForIce;
        this.PosForWind = data.Detail.PosForWind;

        this.isShowBtn = new List<bool>();
        for (int i = 0; i < 4; ++i)
        {
            this.isShowBtn.Add(false);
        }
    }

    public void ShowElementGuide(Vector2Int playerPos)
    {
        if (haveFire && playerPos == PosForFire)
        {
            UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideUI(true, false, false, false);
            if (!this.isShowBtn[1])
            {
                UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideBtn(true, false, false, false);
            }
        }

        if (haveWater && playerPos == PosForWater)
        {
            UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideUI(false, true, false, false);
            if (!this.isShowBtn[2])
                UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideBtn(false, true, false, false);
        }

        if (haveWind && playerPos == PosForWind)
        {
            UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideUI(false, false, true, false);
            if (!this.isShowBtn[3])
                UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideBtn(false, false, true, false);
        }

        if (haveIce && playerPos == PosForIce)
        {
            UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideUI(false, false, false, true);
            if (!this.isShowBtn[4])
                UIManager.Instance.GetUI<GameplayUI>().ShowElementGuideBtn(false, false, false, true);
        }
    }


}