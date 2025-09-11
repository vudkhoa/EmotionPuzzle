using UnityEngine;
using DG.Tweening;

public class ElementIdle : MonoBehaviour
{
    [Header("Idle Settings")]
    public float moveDistance = 0.1f;   // độ cao nhún
    public float scaleAmount = 0.1f;    // biên độ scale
    public float duration = 1f;         // thời gian 1 nhịp

    private Vector3 startLocalPos;
    private Vector3 startScale;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startScale = transform.localScale;

        PlayIdle();
    }

    void PlayIdle()
    {
        // Nhún lên xuống theo localPosition (luôn relative với cha)
        transform.DOLocalMoveY(startLocalPos.y + moveDistance, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Scale nhún
        transform.DOScale(startScale * (1 + scaleAmount), duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
