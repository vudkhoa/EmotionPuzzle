using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

public class TutorialManager : SingletonMono<TutorialManager>
{
    public List<TutorialDetail> tutorialDetailList;
    public List<PopupDetail> popupDetailList;

    public void SetTutorialDetail(List<TutorialDetail> tutorialDetails)
    {
        this.tutorialDetailList = tutorialDetails;
    }
    
    public void SetPopupDetail(List<PopupDetail> popupDetails)
    {
        this.popupDetailList = popupDetails;
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

                    Invoke(nameof(ResetIsTutorial), 0.35f);

                    return;
                }
            }
        }

        foreach (PopupDetail popup in this.popupDetailList)
        {
            if (popup.posInit == pos)
            {
                PlayerPrefs.SetInt(Constant.GUIDEID, popup.guideId);
                //UIManager.Instance.OpenUI<GuideUI>().Init(popup.guideId);
                StartCoroutine(ShowGuide(popup.timeWait, popup.guideId));

                return;
            }
        }
    }

    IEnumerator ShowGuide(float time, int id)
    {
        yield return new WaitForSeconds(time);
        UIManager.Instance.OpenUI<GuideUI>().Init(id);
    }    


    public void ResetIsTutorial()
    {
        SlideController.Instance.isTutorial = false;
    }
}
