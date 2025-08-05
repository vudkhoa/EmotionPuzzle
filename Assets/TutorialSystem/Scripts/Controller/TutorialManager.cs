using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

public class TutorialManager : SingletonMono<TutorialManager>
{
    public List<TutorialDetail> tutorialDetailList;

    public void SetTutorialDetail(List<TutorialDetail> tutorialDetails)
    {
        this.tutorialDetailList = tutorialDetails;
    }

    public void ShowTutorial(Vector2Int pos)
    {
        foreach (TutorialDetail td in this.tutorialDetailList)
        {
            if (td.posInit == pos)
            {
                int id = td.id;
                int curTutoId = PlayerPrefs.GetInt(Constant.TUTORIALID, 0);

                if (curTutoId < id)
                {
                    SlideController.Instance.isTutorial = true;

                    PlayerPrefs.SetInt(Constant.TUTORIALID, id);
                    UIManager.Instance.GetUI<GameplayUI>().ShowTutorialText(td.text, td.Time);

                    //Set guide
                    int guideId = td.guideId;
                    if (guideId != 0)
                    {
                        int curGuideId = PlayerPrefs.GetInt(Constant.GUIDEID, 0);
                        if (curGuideId < guideId)
                        {
                            PlayerPrefs.SetInt(Constant.GUIDEID, guideId);
                        }
                    }

                    Invoke(nameof(ResetIsTutorial), 0.35f);

                    return;
                }
            }
        }
    }


    public void ResetIsTutorial()
    {
        SlideController.Instance.isTutorial = false;
    }
}
