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
}

[Serializable]
public class TutorialDetail
{
    public int id;
    public Vector2Int posInit;
    [TextArea(2, 5)]
    public string text;
    public int guideId;
    public float Time;
}
