using DG.Tweening;
using SoundManager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

public abstract class Element : MonoBehaviour
{
    [Header(" Sprite ")]
    public Sprite neutralSprite;
    public Sprite angrySprite;
    public Sprite happySprtie;
    public Sprite sadSprtie;
    public SpriteRenderer spr;
    public bool isIceMove = false;

    [Header(" Info ")]
    public ElementType ElementType;
    public EmotionType EmotionType;
    private EmotionType initEmotionType;
    private Vector2Int initPosition;

    [Header(" VFX ")]
    public ParticleSystem absorbParticle;
    public VisualEffect emotionChangeVFX;

    //private List<Vector2Int> powerTilemapList;

    public void SetEmotionType(EmotionType emotionType, bool isReActive = true)
    {
        this.EmotionType = emotionType;

        if (isReActive)
        {
            if (this.EmotionType == EmotionType.Angry)
            {
                this.SetActivatePower();
            }
            else
            {
                this.SetDeactivePower();
            }
        }

        ChangeEmotionAni(emotionType);
        this.SetupLock();
    }

    [Header(" Other ")]

    public Vector2Int CurrentPos;

    [Header(" Tile ")]
    public GameObject PowerRingPrefab;
    public List<bool> ActivePowerList;
    public List<Vector2Int> OffsetList;
    public List<GameObject> PowerRingList;

    private List<bool> tmpPowerRingList;

    public void SetInitInfo(EmotionType emotionType, Vector2Int currentPos)
    {
        initEmotionType = emotionType;
        initPosition = currentPos;
    }

    public void ResetInitData()
    {
        initEmotionType = this.EmotionType;
        initPosition = this.CurrentPos;

        this.tmpPowerRingList = new List<bool>(this.ActivePowerList);
    }

    public ElementData GetInitData()
    {
        ElementData data = new ElementData();
        data.ElementType = this.ElementType;
        data.EmotionType = this.EmotionType;
        data.Position = this.CurrentPos;
        data.ActivePowerList = new List<bool>(this.ActivePowerList);
        return data;
    }

    public virtual void Reload(ElementData elementData)
    {
        this.ElementType = elementData.ElementType;
        Vector3Int gridPos = new Vector3Int(elementData.Position.x, elementData.Position.y, 0);
        this.transform.position = SlideController.Instance.elementTilemap.GetCellCenterWorld(gridPos);
        this.CurrentPos = elementData.Position;
        this.SetEmotionType(elementData.EmotionType, false);
        if (this.EmotionType == EmotionType.Angry)
        {
            int count = -1;
            foreach (GameObject go in this.PowerRingList)
            {
                count++;
                if (elementData.ActivePowerList[count])
                {
                    this.ActivePowerList[count] = true;
                    go.SetActive(true);
                    Vector3Int worldPos = new Vector3Int(this.OffsetList[count].x + this.CurrentPos.x, this.OffsetList[count].y + this.CurrentPos.y, 0);
                    int curLevelId = SlideController.Instance.curLevelId;
                    if (SlideController.Instance.bgSmallTilemap.HasTile(worldPos))
                    {
                        this.PowerRingList[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].PowerSprite;
                    }
                    else
                    {
                        this.PowerRingList[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
                    }
                }
                else
                {
                    this.ActivePowerList[count] = false;
                    go.SetActive(false);
                }
            }


        }
        else
        {
            this.SetDeactivePower();
        }
    }

    public virtual void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        this.EmotionType = emotionType;
        ChangeEmotion(emotionType);
        if (this.EmotionType == EmotionType.Angry)
        {
            this.SetActivatePower();
        }
        else
        {
            this.SetDeactivePower();
        }
        this.CurrentPos = currentPos;
        this.SetupLock();
    }



    public virtual bool InteractWithItem(ItemType itemType, Vector2Int itemPos)
    {
        EmotionType newEmotionType = DataManager.Instance.EmotionCoordinationData.GetResult(this.EmotionType, itemType);
        if (newEmotionType == EmotionType.None)
        {
            return false;
        }

        Vector3Int gridPos = new Vector3Int(this.CurrentPos.x, this.CurrentPos.y, 0);
        Vector3 eWorldPos = SlideController.Instance.elementTilemap.GetCellCenterWorld(gridPos);
        ItemTileController.Instance.InteractWithElement(itemPos, eWorldPos);

        //VFX
        //emotionChangeVFX.Play();

        //Sound
        SoundsManager.Instance.PlaySFX(SoundType.EmotionChange);

        this.SetEmotionType(newEmotionType);

        return true;
    }

    public virtual bool CoordinateWithElement(ElementType elementType, Element eGO)
    {
        Element resultPrefab = DataManager.Instance.ElementCoordinateData.GetResult(this.ElementType, elementType);
        //Debug.Log(resultPrefab);
        if (resultPrefab == null)
        {
            return false;
        }

        SoundsManager.Instance.PlaySFX(SoundType.Coordinate);

        Vector3Int gridPos = new Vector3Int(this.CurrentPos.x, this.CurrentPos.y, 0);
        Element eOb = Instantiate(resultPrefab, SlideController.Instance.elementTilemap.GetCellCenterWorld(gridPos), Quaternion.identity);
        eOb.Setup(EmotionType.Neutral, this.CurrentPos);
        ElementController.Instance.ElementList.Add(eOb);

        //VFX
        eOb.emotionChangeVFX.Play();

        eGO.transform.DOMove(this.transform.position, 0.1f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                eGO.gameObject.SetActive(false);
                this.transform.localScale = Vector3.zero;

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
        if (this.ElementType == ElementType.Ice && this.EmotionType == EmotionType.Angry)
        {
            isIceMove = true;
        }
        else
        {
            isIceMove = false;
        }

        Power();
        SetPowerRing(this.CurrentPos);
    }

    public void Rotate(Vector2Int newGridPos, Vector3 worldPos)
    {
        Vector3Int gridOldPos = new Vector3Int(this.CurrentPos.x, this.CurrentPos.y, 0);
        this.CurrentPos = newGridPos;
        this.transform.DOMove(worldPos, 0.2f).SetEase(Ease.OutQuad);
        Power();
        SetPowerRing(this.CurrentPos);
        //Debug.Log(gridOldPos);
        //if (SlideController.Instance.blockTilemap.HasTile(gridOldPos))
        //{
        //    Debug.Log("Delete");
        //    LockController.Instance.RemoveLock(gridOldPos);
        //}

        //}
        //Vector3Int gridNewPos = new Vector3Int(this.CurrentPos.x, this.CurrentPos.y, 0);
        //LockController.Instance.SetLock(gridNewPos);


    }

    public void SetActivatePower()
    {
        for (int i = 0; i < this.ActivePowerList.Count; ++i)
        {
            this.ActivePowerList[i] = true;
        }

        //Nếu Element đã có Power thì thi triển ngay khả năng.
        this.Power();
        this.SetPowerRing(this.CurrentPos);
    }

    public void SetDeactivePower()
    {
        for (int i = 0; i < this.ActivePowerList.Count; ++i)
        {
            this.ActivePowerList[i] = false;
        }
        this.SetPowerRing(this.CurrentPos);
    }

    public void SetPowerRing(Vector2Int elementPos)
    {
        for (int i = 0; i < this.ActivePowerList.Count; ++i)
        {
            Vector3Int worldPos = new Vector3Int(this.OffsetList[i].x + elementPos.x, this.OffsetList[i].y + elementPos.y, 0);
            if (this.ActivePowerList[i])
            {
                this.PowerRingList[i].SetActive(true);
                int curLevelId = SlideController.Instance.curLevelId;
                if (SlideController.Instance.bgSmallTilemap.HasTile(worldPos))
                {
                    this.PowerRingList[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].PowerSprite;
                }
                else
                {
                    this.PowerRingList[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
                }
            }
            else
            {
                this.PowerRingList[i].SetActive(false);
            }
        }
    }

    public void ReActivePower()
    {
        Power();
        SetPowerRing(this.CurrentPos);
    }

    public abstract void ReloadElement();

    private void SetupLock()
    {
        if (this.EmotionType == EmotionType.Sad || this.EmotionType == EmotionType.Neutral)
        {   
            LockController.Instance.SetLock(new Vector3Int(this.CurrentPos.x, this.CurrentPos.y, 0));
        }
        else
        {
            LockController.Instance.RemoveLock(new Vector3Int(this.CurrentPos.x, this.CurrentPos.y, 0));
        }
    }
}

[Serializable]
public class ElementData
{
    public ElementType ElementType;
    public EmotionType EmotionType;
    public Vector2Int Position;
    public List<bool> ActivePowerList;
}