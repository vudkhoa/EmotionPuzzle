using SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WindElement : Element
{
    //private void Awake()
    //{
    //    InitOffsetList();
    //}

    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        InitOffsetList();
        this.ElementType = ElementType.Wind;
        if (this.EmotionType == EmotionType.Angry)
        {
            this.SetActivatePower();
        }
    }

    public override void InitOffsetList()
    {
        this.OffsetList = new List<Vector2Int>();
        this.ActivePowerList = new List<bool>();
        this.PowerRingList = new List<GameObject>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                Vector2Int pos = new Vector2Int(i, j);
                this.OffsetList.Add(pos);
                this.ActivePowerList.Add(false);
                GameObject go = Instantiate(
                    this.PowerRingPrefab,
                    SlideController.Instance.bgSmallTilemap.GetCellCenterWorld(new Vector3Int(CurrentPos.x + pos.x, CurrentPos.y + pos.y, 0)),
                    Quaternion.identity,
                    this.transform
                    );
                go.gameObject.SetActive(false);
                this.PowerRingList.Add(go);
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
        int count = -1;

        bool isPower = false;

        foreach (Vector2Int offset in this.OffsetList)
        {
            nearPos2 = offset + this.CurrentPos;
            nearPos3 = new Vector3Int(nearPos2.x, nearPos2.y, 0);

            count++;
            if (SlideController.Instance.obstacleTilemap.HasTile(nearPos3) && this.ActivePowerList[count] == true)
            {
                if (SlideController.Instance.IceStarId > 0 &&
                    IceStarController.Instance.CheckExistsBlock(nearPos3))
                {
                    continue;
                }

                Vector3Int newObstaclePos = nearPos3 + (nearPos3 - new Vector3Int(this.CurrentPos.x, this.CurrentPos.y));
                if (SlideController.Instance.IceStarId > 0 &&
                        IceStarController.Instance.CheckExistsSource(newObstaclePos))
                {
                    continue;
                }

                isPower = true;

                this.ActivePowerList[count] = false;
                if (!SlideController.Instance.bgSmallTilemap.HasTile(newObstaclePos))
                {
                    ObstacleTileController.Instance.ThrowObstacleTile(nearPos3, newObstaclePos);
                }
                else if (!SlideController.Instance.obstacleTilemap.HasTile(newObstaclePos) &&
                    !ElementController.Instance.CheckExistsElement(newObstaclePos))
                {
                    ObstacleTileController.Instance.MoveObsatcleTile(nearPos3, newObstaclePos);
                }
            }
        }

        if (isPower)
        {
            SoundsManager.Instance.PlaySFX(SoundType.WindPower);
        }
    }

    public override void ReloadElement()
    {
    }
}
