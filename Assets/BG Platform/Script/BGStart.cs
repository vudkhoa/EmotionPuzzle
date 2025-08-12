using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGStart : MonoBehaviour
{
    public void OnFinishIntro()
    {
        PlayerPrefs.SetInt(Constant.LEVELID, 1);
        LoadingManager.instance.LoadScene("Platform 1");
    }
}
