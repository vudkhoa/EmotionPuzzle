using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElement : Element
{
    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        this.ElementType = ElementType.Fire;
    }

    public override void Power()
    {
        if (this.EmotionType != EmotionType.Angry)
        {
            return;
        }

        Vector2Int currentPos = this.CurrentPos;

        Vector2Int nearPos = new Vector2Int(0, 0);

        nearPos = currentPos + new Vector2Int(1, 0);
        if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
        {
            ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
        }

        nearPos = currentPos + new Vector2Int(-1, 0);
        if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
        {
            ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
        }

        nearPos = currentPos + new Vector2Int(0, 1);
        if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
        {
            ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
        }

        nearPos = currentPos + new Vector2Int(0, -1);
        if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
        {
            ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
        }

        nearPos = currentPos + new Vector2Int(1, 1);
        if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
        {
            ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
        }

        nearPos = currentPos + new Vector2Int(-1, 1);
        if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
        {
            ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
        }

        nearPos = currentPos + new Vector2Int(1, -1);
        if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
        {
            ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
        }

        nearPos = currentPos + new Vector2Int(-1, -1);
        if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
        {
            ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
        }
    }
}
