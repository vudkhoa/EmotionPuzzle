using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElement : Element
{
    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        InitOffsetList();
        base.Setup(emotionType, currentPos);
        this.ElementType = ElementType.Fire;
    }

    private void InitOffsetList()
    {
        this._offsetList = new List<Vector2Int>();
        this.ActivePowerList = new List<bool>();
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
                this.ActivePowerList.Add(false);
            }
        }
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
        int count = -1;
        foreach (Vector2Int offset in this._offsetList)
        {
            count++;
            nearPos = currentPos + offset;
            if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)) &&
                this.ActivePowerList[count] == true)
            {
                this.ActivePowerList[count] = false;
                ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
            }
        }
    }
}
