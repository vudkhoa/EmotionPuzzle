using SoundManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelUI : MonoBehaviour
{
    [Header(" Level Stage ")]
    public List<LevelStage> LevelStageList;

    private void Start()
    {        
        int maxLevelId = PlayerPrefs.GetInt(Constant.MAXLEVELID, 0);
        int isUnlockLevel = PlayerPrefs.GetInt(Constant.ISUNLOCKLEVEL, 0);

        foreach (LevelStage levelStage in LevelStageList)
        {
            levelStage.Setup();
        }

        if (isUnlockLevel == 1)
        {
            foreach (LevelStage levelStage in LevelStageList)
            {
                if (levelStage.LevelId == maxLevelId)
                {
                    levelStage.PlayUnlockAnimation();

                    break;
                }
            }

            PlayerPrefs.SetInt(Constant.ISUNLOCKLEVEL, 0);
        }
    }

    public void BackHome()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        LoadingManager.instance.LoadScene("Start");
    }
}
