using DG.Tweening;
using UnityEngine;

public class Raft : MonoBehaviour
{
    private Vector2Int currentPos;

    public Vector2Int GetCurrentPos()
    {
        return currentPos;
    }

    public void SetCurrentPos(Vector2Int newPos)
    {
        currentPos = newPos;
    }

    public void MoveTo(Vector2Int newGridPos, Vector3 worldPos)
    {
        currentPos = newGridPos;
        transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
    }
}
