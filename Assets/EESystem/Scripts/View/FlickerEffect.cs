using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlickerEffect : MonoBehaviour
{
    public SpriteRenderer targetImage;   
    private float minAlpha = 0.85f; 
    private float maxAlpha = 1f;   
    private float duration = 0.5f; 

    private Tween flickerTween;

    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponentInChildren<SpriteRenderer>();

        StartFlicker();
    }

    void StartFlicker()
    {
        // Nhấp nháy liên tục giữa minAlpha và maxAlpha
        flickerTween = targetImage.DOFade(minAlpha, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void OnDestroy()
    {
        flickerTween?.Kill();
    }
}
