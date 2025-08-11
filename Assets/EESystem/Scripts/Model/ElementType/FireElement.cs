using SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElement : Element
{
    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        InitOffsetList();
        this.ElementType = ElementType.Fire;
        if (this.EmotionType == EmotionType.Angry)
        {
            this.SetActivatePower();
        }
    }

    private void InitOffsetList()
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

        Vector2Int currentPos = this.CurrentPos;

        Vector2Int nearPos = new Vector2Int(0, 0);

        nearPos = currentPos + new Vector2Int(1, 0);
        int count = -1;

        bool isPower = false;

        foreach (Vector2Int offset in this.OffsetList)
        {
            count++;
            nearPos = currentPos + offset;
            if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)) &&
                this.ActivePowerList[count] == true)
            {
                isPower = true;

                this.ActivePowerList[count] = false;
                ObstacleTileController.Instance.RemoveObstacleTile(nearPos);
            }
        }

        if (isPower)
        {
            SoundsManager.Instance.PlaySFX(SoundType.FirePower);
        }
    }
}
