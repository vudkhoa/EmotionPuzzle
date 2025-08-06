using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Element : MonoBehaviour
{
    [Header(" Sprite ")]
    public Sprite neutralSprite;
    public Sprite angrySprite;
    public Sprite happySprtie;
    public Sprite sadSprtie;
    public SpriteRenderer spr;

    [Header(" Info ")]
    public ElementType ElementType;
    public EmotionType EmotionType;
    public void SetEmotionType(EmotionType emotionType)
    {
        this.EmotionType = emotionType;
        ChangeEmotionAni(emotionType);
    }

    public Vector2Int CurrentPos;

    [Header(" Tile ")]
    public GameObject PowerRingPrefab;
    public List<bool> ActivePowerList;
    public List<Vector2Int> OffsetList;
    public List<GameObject> PowerRingList;

    public virtual void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        this.EmotionType = emotionType;
        ChangeEmotion(emotionType);
        this.CurrentPos = currentPos;
    }

    public virtual bool InteractWithItem(ItemType itemType, Vector2Int itemPos)
    {
        EmotionType newEmotionType = DataManager.Instance.EmotionCoordinationData.GetResult(this.EmotionType, itemType);
        if (newEmotionType == EmotionType.None)
        {
            return false;
        }

        //this.EmotionType = newEmotionType;
        this.SetEmotionType(newEmotionType);

        return true;
    }

    public virtual bool CoordinateWithElement(ElementType elementType, Element eGO)
    {
        Element resultPrefab = DataManager.Instance.ElementCoordinateData.GetResult(this.ElementType, elementType);
        if (resultPrefab == null)
        {
            return false;
        }

        Vector3Int gridPos = new Vector3Int(this.CurrentPos.x, this.CurrentPos.y, 0);
        Element eOb = Instantiate(resultPrefab, SlideController.Instance.elementTilemap.GetCellCenterWorld(gridPos), Quaternion.identity);
        eOb.Setup(EmotionType.Neutral, this.CurrentPos);
        ElementController.Instance.ElementList.Add(eOb);

        eGO.transform.DOMove(this.transform.position, 0.1f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                eGO.gameObject.SetActive(false);
                this.transform.localScale = Vector3.zero;

                //mergeParticle.Play();

                // Pop
                eOb.transform.localScale = Vector3.zero;
                eOb.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    Destroy(eGO.gameObject);
                    Destroy(this.gameObject);
                });
            });

        return true;
    }

    public void ChangeEmotionAni(EmotionType emotionType)
    {
        this.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutBack);
        if (emotionType == EmotionType.Neutral)
        {
            this.spr.sprite = neutralSprite;
        }
        else if (emotionType == EmotionType.Angry)
        {
            this.spr.sprite = angrySprite;
        }
        else if (emotionType == EmotionType.Sad)
        {
            this.spr.sprite = sadSprtie;
        }
        else if (emotionType == EmotionType.Happy)
        {
            this.spr.sprite = happySprtie;
        }
        this.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
    }

    public void ChangeEmotion(EmotionType emotionType)
    {
        if (emotionType == EmotionType.Neutral)
        {
            this.spr.sprite = neutralSprite;
        }
        else if (emotionType == EmotionType.Angry)
        {
            this.spr.sprite = angrySprite;
        }
        else if (emotionType == EmotionType.Sad)
        {
            this.spr.sprite = sadSprtie;
        }
        else if (emotionType == EmotionType.Happy)
        {
            this.spr.sprite = happySprtie;
        }
    }

    public abstract void Power();

    public void MoveTo(Vector2Int newGridPos, Vector3 worldPos)
    {
        Vector2Int oldGridPos = this.CurrentPos;
        if (Vector2Int.Distance(newGridPos, this.CurrentPos) != 1f)
        {
            this.CurrentPos = newGridPos;
            // 1. Scale nhỏ dần để ẩn tile
            this.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                // 2. Di chuyển đến đầu hàng
                this.transform.position = worldPos;


                // 3. Scale lớn lên để hiện tile lại
                this.transform.DOScale(Vector3.one, 0.15f);
            });
        }
        else
        {
            CurrentPos = newGridPos;
            transform.DOMove(worldPos, 0.25f);
        }
        Power();
        SetPowerRing(oldGridPos);
        //ElementController.Instance.SetPowerRingAll();
    }

    public void Rotate(Vector2Int newGridPos, Vector3 worldPos)
    {
        Vector2Int oldGridPos = this.CurrentPos;
        this.CurrentPos = newGridPos;
        this.transform.DOMove(worldPos, 0.2f).SetEase(Ease.OutQuad);

        Power();
        SetPowerRing(oldGridPos);
        ElementController.Instance.SetPowerRingAll();
    }

    public void SetActivePower()
    {
        for (int i = 0; i < this.ActivePowerList.Count; ++i)
        {
            this.ActivePowerList[i] = true;
        }

        //Fix: Nếu Element đã có Power thì thi triển ngay khả năng.
        //Invoke(nameof(Power), 0.28f);
        this.Power();
        this.SetPowerRing(this.CurrentPos);
    }

    public void SetPowerRing(Vector2Int elementOldPos)
    {
        for (int i = 0; i < this.ActivePowerList.Count; ++i)
        {
            if (this.ActivePowerList[i] != false)
            {
                this.PowerRingList[i].SetActive(true);
            }
            else
            {
                this.PowerRingList[i].SetActive(false);
            }
            

            //Vector3Int pos = new Vector3Int(elementOldPos.x + this.OffsetList[i].x, elementOldPos.y + this.OffsetList[i].y, 0);
            //if (limitationTilemap.HasTile(pos))
            //{
            //    Sprite sp = SlideController.Instance.GetSpriteFromTile(limitationTilemap.GetTile(pos));
            //    TileFake tempGO = GameObject.Instantiate(
            //        SlideController.Instance.groudTileFakePrefab,
            //        limitationTilemap.GetCellCenterWorld(pos),
            //        Quaternion.identity
            //    );
            //    tempGO.SetSprite(sp);
            //    limitationTilemap.SetTile(pos, null);
            //    tileFakes.Add(tempGO);
            //}
            //else
            //{
            //    TileFake tempGO = GameObject.Instantiate(
            //        SlideController.Instance.groudTileFakePrefab,
            //        limitationTilemap.GetCellCenterWorld(pos),
            //        Quaternion.identity
            //    );
            //    tileFakes.Add(tempGO);
            //}
        }

        //for (int i = 0; i < this.ActivePowerList.Count; ++i)
        //{
        //    Vector3Int pos = new Vector3Int(this.CurrentPos.x + this.OffsetList[i].x, this.CurrentPos.y + this.OffsetList[i].y, 0);
        //    if (elementOldPos != this.CurrentPos)
        //    {
        //        if (this.ActivePowerList[i] == true)
        //        {
        //            int index = i;
        //            //TileBase tileBase = limitationTilemap.GetTile(pos);
        //            tileFakes[i].transform.DOMove(limitationTilemap.GetCellCenterWorld(pos), 0.2f)
        //                .SetEase(Ease.OutQuad).OnComplete(() =>
        //            {
        //                limitationTilemap.SetTile(pos, this.ElementPowerTile);
        //                Destroy(tileFakes[index].gameObject);
        //            });
        //        }
        //        else
        //        {
        //            tileFakes[i].gameObject.SetActive(false);
        //            Destroy(tileFakes[i].gameObject);
        //        }
        //    }
        //    else
        //    {
        //        if (this.ActivePowerList[i] == true)
        //        {
        //            limitationTilemap.SetTile(pos, this.ElementPowerTile);
        //        } 
        //    }
        //}
    }
}