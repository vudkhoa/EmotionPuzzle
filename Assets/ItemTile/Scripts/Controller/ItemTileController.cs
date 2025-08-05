using CustomUtils;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class ItemTileController : SingletonMono<ItemTileController>
{
    public List<Vector2Int> ItemPosList;
    public List<ItemType> ItemTypeList;

    public void SetItemPosList(List<Vector2Int> itemPosList)
    {
        this.ItemPosList = new List<Vector2Int>(itemPosList);
    }

    public void SetItemTypeList(List<ItemType> itemTypeList)
    {
        this.ItemTypeList = new List<ItemType>(itemTypeList);
    }

    public void UpdateItemPosList(Vector2Int oldPos, Vector2Int newPos)
    {
        if (this.ItemPosList.Contains(oldPos))
        {
            //Update KeyType List
            int i = this.ItemPosList.IndexOf(oldPos);
            ItemType itemType = this.ItemTypeList[i];
            this.ItemTypeList.RemoveAt(i);
            this.ItemTypeList.Add(itemType);

            this.ItemPosList.Remove(oldPos);
            this.ItemPosList.Add(newPos);
        }
    }

    public void MoveItemTile(List<Vector2Int> cellsToSlide, Direction direction)
    {
        if (SlideController.Instance.itemId == 0)
        {
            return;
        }

        //Spawn các tile động theo thứ tự cells
        List<TileFake> clones = new List<TileFake>();
        List<TileBase> tileOrder = new List<TileBase>();

        foreach (Vector2Int cellPos in cellsToSlide)
        {
            Vector3Int cell = new Vector3Int(cellPos.x, cellPos.y, 0);
            
            TileBase tile = SlideController.Instance.itemTilemap.GetTile(cell);
            if (tile == null)
            {
                clones.Add(null);
                tileOrder.Add(null);

                continue;
            }

            if (tile != null && !this.ItemPosList.Contains(new Vector2Int(cell.x, cell.y)))
            {
                SlideController.Instance.itemTilemap.SetTile(new Vector3Int(cell.x, cell.y, 0), null);
                clones.Add(null);
                tileOrder.Add(null);
                continue;
            }

            // Lưu thứ tự tile để xử lý logic wrap-around
            tileOrder.Add(tile);

            // Tạo GameObject clone để tween
            TileFake obj = Instantiate(SlideController.Instance.itemTileFakePrefab, SlideController.Instance.itemTilemap.GetCellCenterWorld(cell), Quaternion.identity);
            Sprite sprite = SlideController.Instance.GetSpriteFromTile(tile);
            if (sprite != null)
                obj.SetSprite(sprite);

            obj.gridPos = cellPos;
            clones.Add(obj);

            SlideController.Instance.itemTilemap.SetTile(cell, null);
        }

        //Di chuyển các tile
        int count = cellsToSlide.Count;
        for (int i = 0; i < count; i++)
        {
            if (clones[i] == null)
            {
                continue;
            }

            Vector2Int toGrid = new Vector2Int(0, 0);
            if (i == 0)
            {
                toGrid = cellsToSlide[count - 1];
            }
            else
            {
                toGrid = cellsToSlide[i - 1];
            }
            Vector3 worldPos = SlideController.Instance.itemTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

            clones[i].MoveTo(toGrid, worldPos);
        }

        //Bước 3: Sau khi tween xong → cập nhật lại Tilemap và xóa clone
        DOVirtual.DelayedCall(0.25f, () =>
        {
            for (int i = 0; i < count; i++)
            {
                int fromIndex = (i + 1) % count;

                if (tileOrder[fromIndex] == null)
                {
                    continue;
                }

                Vector2Int toCell = cellsToSlide[i];
                this.UpdateItemPosList(cellsToSlide[fromIndex], toCell); //Update Item Pos List
                SlideController.Instance.itemTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
                
                if (SlideController.Instance.BossId > 0)
                {
                    int id = BossController.Instance.Boss.ItemList.IndexOf(cellsToSlide[fromIndex]);
                    if (id >= 0)
                    {
                        BossController.Instance.Boss.ItemList[id] = toCell;
                    }
                }
            }

            foreach (var obj in clones)
            {
                if (obj != null)
                    Destroy(obj.gameObject);
            }

            if (SlideController.Instance.elementId > 0)
            {
                foreach (Element e in ElementController.Instance.ElementList)
                {
                    if (e.ElementType == ElementType.Ice)
                    {
                        e.Power();
                    }
                }
            }
            
            if (SlideController.Instance.BossId > 0)
            {
                BossController.Instance.TakeDamage();
            }

            if (SlideController.Instance.IceStarId > 0)
            {
                IceStarController.Instance.SetIceStars();
            }
        });
    }

    public void RotateItemTile(Vector2Int pivot, List<Vector2Int> posList)
    {
        for (int i=0; i<this.ItemPosList.Count; i++)
        {
            Vector2Int itemPos = this.ItemPosList[i];
            foreach (Vector2Int pos in posList)
            {
                if (itemPos == pos)
                {
                    Vector2Int newP = RotateObjectController.Instance.RotateAroundPivot(pos, pivot, 90f);
                    Vector3Int newGP = new Vector3Int(newP.x, newP.y, 0);
                    Vector3 p = SlideController.Instance.itemTilemap.GetCellCenterWorld(newGP);

                    //Update Pos List
                    this.ItemPosList[i] = newP;

                    //Animation
                    Vector3Int cell = new Vector3Int(itemPos.x, itemPos.y, 0);

                    TileBase tile = SlideController.Instance.itemTilemap.GetTile(cell);

                    TileFake obj = Instantiate(SlideController.Instance.itemTileFakePrefab, SlideController.Instance.itemTilemap.GetCellCenterWorld(cell), Quaternion.identity);
                    Sprite sprite = SlideController.Instance.GetSpriteFromTile(tile);
                    if (sprite != null)
                        obj.SetSprite(sprite);

                    obj.gridPos = itemPos;

                    SlideController.Instance.itemTilemap.SetTile(cell, null);

                    obj.transform.DOMove(p, 0.2f).SetEase(Ease.OutQuad);

                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        Destroy(obj.gameObject);
                        SlideController.Instance.itemTilemap.SetTile(newGP, tile);
                    });

                    break;
                }
            }
        }
    }

    public ItemType GetItemType(Vector2Int itemPos)
    {
        for (int i=0; i<ItemPosList.Count; i++)
        {
            if (itemPos == ItemPosList[i])
            {
                return ItemTypeList[i];
            }
        }

        return ItemType.None;
    }

    public void RemoveItem(Vector2Int itemPos)
    {
        Vector3Int gridPos = new Vector3Int(itemPos.x, itemPos.y, 0);
        Sprite sp = SlideController.Instance.GetSpriteFromTile(SlideController.Instance.itemTilemap.GetTile(gridPos));
        int index = this.ItemPosList.IndexOf(itemPos);
       
        SlideController.Instance.itemTilemap.SetTile(new Vector3Int(this.ItemPosList[index].x, this.ItemPosList[index].y, 0), null);

        TileFake ob = Instantiate(SlideController.Instance.itemTileFakePrefab, SlideController.Instance.itemTilemap.GetCellCenterWorld(gridPos), Quaternion.identity);
        ob.SetSprite(sp);

        
        this.ItemPosList.RemoveAt(index);
        this.ItemTypeList.RemoveAt(index);

        //effect
        ob.transform.DORotate(new Vector3(0, 0, 360f), 0.15f, RotateMode.FastBeyond360);
        ob.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(ob.gameObject);
        });
    }

    public void InteractWithElement(Vector2Int itemPos, Vector3 eWorldPos)
    {
        Vector3Int gridPos = new Vector3Int(itemPos.x, itemPos.y, 0);
        Sprite sp = SlideController.Instance.GetSpriteFromTile(SlideController.Instance.itemTilemap.GetTile(gridPos));
        int index = this.ItemPosList.IndexOf(itemPos);

        SlideController.Instance.itemTilemap.SetTile(new Vector3Int(this.ItemPosList[index].x, this.ItemPosList[index].y, 0), null);

        TileFake ob = Instantiate(SlideController.Instance.itemTileFakePrefab, SlideController.Instance.itemTilemap.GetCellCenterWorld(gridPos), Quaternion.identity);
        ob.SetSprite(sp);


        this.ItemPosList.RemoveAt(index);
        this.ItemTypeList.RemoveAt(index);

        //effect
        ob.transform.DOMove(eWorldPos, 0.1f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                ob.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    Destroy(ob.gameObject);
                });
            });
    }

    public List<Vector2Int> FindItemCluster()
    {
        List<Vector2Int> resultList = new List<Vector2Int>();
        List<Vector2Int> posList = new List<Vector2Int>(this.ItemPosList);
        List<Vector2Int> offsetList = new List<Vector2Int>();

        offsetList = Library.Instance.LibOffsets8;

        for (int i = 0; i < this.ItemPosList.Count; i++)
        {
            List<Vector2Int> tempList = new List<Vector2Int>();
            tempList.Add(this.ItemPosList[i]);
            posList.Remove(this.ItemPosList[i]);
            Vector2Int itemPos = FindItemNear(tempList, posList, offsetList);

            while (itemPos != new Vector2Int(-1000, -1000))
            {
                tempList.Add(itemPos);
                posList.Remove(itemPos);
                itemPos = FindItemNear(tempList, posList, offsetList);
            }

            if (tempList.Count > resultList.Count)
            {
                resultList = new List<Vector2Int>(tempList);
            }
            posList = new List<Vector2Int>(this.ItemPosList);
        }

        if (resultList.Count == 1)
        {
            resultList = this.FindItemAbsMin();
        }

        return resultList;
    }

    public Vector2Int FindItemNear(List<Vector2Int> tempList, List<Vector2Int> posList, List<Vector2Int> offsetList)
    {
        foreach (Vector2Int posItem in posList)
        {
            foreach (Vector2Int offset in offsetList)
            {
                Vector2Int checkPos = posItem + offset;
                if (tempList.Contains(checkPos))
                {
                    return posItem;
                }
            }
        }
        return new Vector2Int(-1000, -1000);
    }

    public List<Vector2Int> FindItemAbsMin()
    {
        List<Vector2Int> resultList = new List<Vector2Int>();
        
        Vector2Int minPos = new Vector2Int(-1000, -1000);
        float minDistance = float.MaxValue; 
        foreach (Vector2Int pos in this.ItemPosList) 
        {
            float distance = 0;
            distance = CaculateDistance(pos, SlideController.Instance.GetPlayerPos());
            Vector2Int midPos = new Vector2Int(7, 7);
            distance += CaculateDistance(pos, midPos);
            if (minDistance > distance)
            {
                minDistance = distance;
                minPos = pos;
            }
        }

        List<Vector2Int> offsetList = new List<Vector2Int>();
        List<Vector2Int> posList = new List<Vector2Int>(this.ItemPosList);

        offsetList = Library.Instance.LibOffsets8;

        List<Vector2Int> tempList = new List<Vector2Int>();

        tempList.Add(minPos);
        posList.Remove(minPos);
        Vector2Int itemPos = FindItemNear(tempList, posList, offsetList);

        while (itemPos != new Vector2Int(-1000, -1000))
        {
            tempList.Add(itemPos);
            posList.Remove(itemPos);
            itemPos = FindItemNear(tempList, posList, offsetList);
        }
        
        resultList = new List<Vector2Int>(tempList);
        return resultList;
    }

    public float CaculateDistance(Vector2Int pos1, Vector2Int pos2)
    {
        float distance = 0;
        distance = Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
        return distance;
    }

    private void Update()
    {
        if (SlideController.Instance.itemId <= 0 || SlideController.Instance.BossId <= 0)
        {
            return;
        }
        this.ResetTile();
    }

    public void ResetTile()
    {
        for (int i = 0; i < 100; ++i)
        {
            for (int j = 0; j < 100; ++j)
            {
                Vector3Int cell = new Vector3Int(i, j, 0);
                if (SlideController.Instance.itemTilemap.HasTile(cell) &&
                    !this.ItemPosList.Contains(new Vector2Int(cell.x, cell.y)))
                {
                    SlideController.Instance.itemTilemap.SetTile(cell, null);
                }
            }
        }
    }

    public void CheckBlockTile()
    {
        foreach (Vector2Int cell in this.ItemPosList)
        {
            Vector3Int pos = new Vector3Int(cell.x, cell.y, 0);
            if (SlideController.Instance.blockTilemap.HasTile(pos))
            {
                BlockTileController.Instance.AddPosToUnBlockTileList(pos);
            }
        }
    }
}
