using SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button quitBtn;

    private void Start()
    {
        PlayerPrefs.SetInt(Constant.LEVELID, 1);
        PlayerPrefs.SetInt(Constant.GUIDEID, 0);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        LoadingManager.instance.LoadScene("BG Start");
    }

    public void QuitGame()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        Application.Quit();
    }
}
