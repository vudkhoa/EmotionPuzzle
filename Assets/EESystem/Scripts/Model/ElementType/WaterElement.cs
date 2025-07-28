using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterElement : Element
{
    private List<Vector2Int> _offsetList;
    [SerializeField] private Tile WaterTile;

    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        this.ElementType = ElementType.Water;
        InitOffsetList();
    }

    private void InitOffsetList()
    {
        this._offsetList = new List<Vector2Int>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                if ((i == 0 && j == 0) || (i != 0 && j != 0))
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

            if (SlideController.Instance.bgWaterTilemap.HasTile(nearPos3) &&
            !SlideController.Instance.waterTilemap.HasTile(nearPos3))
            {
                FillWater(nearPos3, offset);
                SlideController.Instance.SpawnRaft(nearPos2);
            }
        }
    }

    private void FillWater(Vector3Int nearPos3, Vector2Int offset)
    {
        Vector3Int waterPos = new Vector3Int(0, 0);
        for (int i = 0; i <= 100 ; ++i)
        {
            waterPos = nearPos3;
            if (offset.x < 0)
            {
                waterPos.x -= i;
            }
            else if (offset.x > 0)
            {
                waterPos.x += i;
            }
            else if (offset.y > 0)
            {
                waterPos.y += i;
            }
            else if (offset.y < 0)
            {
                waterPos.y -= i;
            }
            

            // Lên
            for (int j = 0; j <= 100; ++j)
            {
                if (offset.x != 0)
                {
                    waterPos.y += j;
                }
                else
                {
                    waterPos.x += j;
                }
                //Debug.Log($"Water pos: {waterPos}");
                if (SlideController.Instance.bgWaterTilemap.HasTile(waterPos) &&
                !SlideController.Instance.waterTilemap.HasTile(waterPos))
                {
                    SlideController.Instance.waterTilemap.SetTile(waterPos, WaterTile);
                }
                else if (!SlideController.Instance.bgWaterTilemap.HasTile(waterPos))
                {
                    break;
                }
            }
            
            if (offset.x != 0)
            {
                waterPos.y = nearPos3.y;
            }
            else
            {
                waterPos.x = nearPos3.x;
            }
            for (int j = 0; j <= 100; ++j)
            {
                if (offset.x != 0)
                {
                    waterPos.y -= j;
                }
                else
                {
                    waterPos.x -= j;
                }
                if (SlideController.Instance.bgWaterTilemap.HasTile(waterPos) &&
                !SlideController.Instance.waterTilemap.HasTile(waterPos))
                {
                    SlideController.Instance.waterTilemap.SetTile(waterPos, WaterTile);
                }
                else if (!SlideController.Instance.bgWaterTilemap.HasTile(waterPos))
                {
                    break;
                }
            }
        }
    }
}
