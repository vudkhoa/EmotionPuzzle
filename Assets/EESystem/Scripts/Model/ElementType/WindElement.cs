using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WindElement : Element
{
    private List<Vector2Int> _offsetList;

    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        this.ElementType = ElementType.Wind;
        InitOffsetList();
    }

    private void InitOffsetList()
    {
        this._offsetList = new List<Vector2Int>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                Vector2Int pos = new Vector2Int(i, j);
                this._offsetList.Add(pos);
            }
        }
    }

    public override void Power()
    {
        if (this.EmotionType != EmotionType.Angry)
        {
            return;
        }

        Vector3Int nearPos3 = new Vector3Int(0, 0, 0);
        Vector2Int nearPos2 = new Vector2Int(0, 0);
        foreach (Vector2Int offset in this._offsetList)
        {
            nearPos2 = offset + this.CurrentPos;
            nearPos3 = new Vector3Int(nearPos2.x, nearPos2.y, 0);


            if (SlideController.Instance.obstacleTilemap.HasTile(nearPos3))
            {
                Vector3Int newObstaclePos = nearPos3 + (nearPos3 - new Vector3Int(this.CurrentPos.x, this.CurrentPos.y));
                if (!SlideController.Instance.groundTilemap.HasTile(newObstaclePos))
                {
                    ObstacleTileController.Instance.ThrowObstacleTile(nearPos3, newObstaclePos);
                }
                if (!SlideController.Instance.obstacleTilemap.HasTile(newObstaclePos))
                {
                    ObstacleTileController.Instance.MoveObsatcleTile(nearPos3, newObstaclePos);
                }
            }
        }
    }
}
