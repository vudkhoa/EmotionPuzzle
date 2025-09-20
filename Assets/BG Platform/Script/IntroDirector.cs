using System;
using UnityEngine;
using UnityEngine.Playables;

public class IntroDirector : MonoBehaviour
{
    public PlayableDirector director;

    void Awake()
    {
        director.Play();
        director.stopped += OnPlayableDirectorStopped;
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        LoadingManager.instance.LoadScene("Platform 1");
    }
}
