using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGStart : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetInt(Constant.LEVELID, 1);
        PlayerPrefs.SetInt(Constant.GUIDEID, 0);
        PlayerPrefs.Save();
    }

    public void OnFinishIntro()
    {
        LoadingManager.instance.LoadScene("Platform 1");
    }
}
