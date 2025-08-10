using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TutorialSO", fileName = "TutorialData")]
public class TutorialSO : ScriptableObject
{
    public List<TutorialLevelDetail> TutorialLevelDetails;
}

[Serializable]
public class TutorialLevelDetail
{
    public int tutorialId;
    public List<TutorialDetail> TutorialDetails;
    public List<PopupDetail> PopupDetails;
}

[Serializable]
public class TutorialDetail
{
    public int id;
    public Vector2Int posInit;
    [TextArea(2, 5)]
    public string text;
    public float Time;
}

[Serializable]
public class PopupDetail
{
    public Vector2Int posInit;
    public int guideId;
    public float timeWait;
}
