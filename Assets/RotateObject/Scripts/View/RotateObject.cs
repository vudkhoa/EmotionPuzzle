using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector2Int rotatePos;
    public List<Vector2Int> containPosList;

    public void Setup(Vector2Int rotatePos, List<Vector2Int> containPosList)
    {
        this.rotatePos = rotatePos;
        this.containPosList = containPosList;

        SetGroundTile();
    }

    public void SetGroundTile()
    {
        GroundTileController.Instance.SetGroundTileForRotateObj(this.containPosList);
    }

    public void Rotate()
    {
        GroundTileController.Instance.RemoveGroundTileForRotateObj(this.containPosList);

        float currentZ = transform.eulerAngles.z;
        float targetZ = currentZ + 90f;

        transform.DORotate(new Vector3(0, 0, targetZ), 0.2f, RotateMode.Fast)
                 .SetEase(Ease.OutQuad);

        List<Vector2Int> pL = new List<Vector2Int>();

        foreach (var pos in containPosList)
        {
            Vector2Int p = (RotateObjectController.Instance.RotateAroundPivot(pos, rotatePos, 90f));
            pL.Add(p);
        }

        this.containPosList = pL;

        SetGroundTile();
    }
}
