using CustomUtils;
using SoundManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ElementController : SingletonMono<ElementController>
{
    public List<Element> ElementList;

    private List<int> elementIdHasJustMove;

    public void Reload()
    {
        foreach (Element e in this.ElementList)
        {
            if (e.ElementType == ElementType.Water)
            {
                e.ReloadElement();
            }
        }
    }

    public void SpawnElement(List<ElementDetail> elementDetails)
    {
        foreach (ElementDetail elementDetail in elementDetails)
        {
            Vector3Int gridPos = new Vector3Int(elementDetail.ElementPos.x, elementDetail.ElementPos.y, 0);
            Vector3 pos = SlideController.Instance.elementTilemap.GetCellCenterWorld(gridPos);
            Element eGO = Instantiate(elementDetail.Element, pos, Quaternion.identity);
            eGO.Setup(elementDetail.EmotionType, elementDetail.ElementPos);
            this.ElementList.Add(eGO);
        }
    }

    public bool CheckCanMoveElement(List<Vector2Int> cellMovePosList, Direction direction, Player player)
    {
        Vector2Int offset = new Vector2Int(0, 0);
        switch (direction)
        {
            case Direction.Left:
                offset = new Vector2Int(-1, 0);
                break;
            case Direction.Right:
                offset = new Vector2Int(1, 0);
                break;
            case Direction.Up:
                offset = new Vector2Int(0, 1);
                break;
            case Direction.Down:
                offset = new Vector2Int(0, -1);
                break;
        }

        foreach (Element e in this.ElementList)
        {
            if (e.EmotionType == EmotionType.Sad || e.EmotionType == EmotionType.Neutral)
            {
                continue;
            }

            bool checkElementMove = false;
            Vector2Int newPos = new Vector2Int(0, 0);
            if (((direction == Direction.Up || direction == Direction.Down) &&
                e.CurrentPos.x == player.GetCurrentPos().x) ||
                ((direction == Direction.Left || direction == Direction.Right) &&
                e.CurrentPos.y == player.GetCurrentPos().y)
                )
            {
                Vector2Int tmp = player.GetCurrentPos() - e.CurrentPos;
                Vector2Int o = new Vector2Int(0, 0);
                if (tmp.x < 0)
                {
                    o.x = -1;
                }
                else if (tmp.x > 0)
                {
                    o.x = 1;
                }
                else if (tmp.y < 0)
                {
                    o.y = -1;
                }
                else if (tmp.y > 0)
                {
                    o.y = 1;
                }

                Vector2Int nextPos = e.CurrentPos;
                for (int i = 0; i <= 100; i++)
                {
                    nextPos += o;
                    if (nextPos == player.GetCurrentPos())
                    {
                        break;
                    }
                    else if (!SlideController.Instance.bgSmallTilemap.HasTile(new Vector3Int(nextPos.x, nextPos.y, 0)))
                    {
                        checkElementMove = true;
                        break;
                    }
                }

                if (checkElementMove ||  e.EmotionType == EmotionType.Neutral || e.EmotionType == EmotionType.Sad)
                {
                    continue;
                }

                if (cellMovePosList[0] == e.CurrentPos)
                {
                    newPos = cellMovePosList[cellMovePosList.Count - 1];
                }
                else
                {
                    newPos = e.CurrentPos + offset;
                }

                if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(newPos.x, newPos.y, 0)) ||
                    this.CheckExistsElement(new Vector3Int(newPos.x, newPos.y, 0))
                    )
                {
                    return false;
                }

                if (SlideController.Instance.IceStarId > 0 &&
                    IceStarController.Instance.CheckExistsSource(new Vector3Int(newPos.x, newPos.y, 0)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void MoveElement(List<Vector2Int> cellsToSlide, Direction direction)
    {
        if (SlideController.Instance.elementId <= 0)
        {
            return;
        }

        int count = cellsToSlide.Count;
        elementIdHasJustMove = new List<int>();
        int id = -1;
        foreach (Element e in this.ElementList)
        {
            id++;
            if (e.EmotionType != EmotionType.Happy && e.EmotionType != EmotionType.Angry)
            {
                continue;
            }
            for (int i = 0; i < count; i++) 
            {
                if (e.CurrentPos == cellsToSlide[i])
                {
                    Vector2Int toGrid = new Vector2Int(0, 0);
                    if (i == 0)
                    {
                        toGrid = cellsToSlide[count - 1];
                    }
                    else
                    {
                        toGrid = cellsToSlide[i - 1];
                    }
                    Vector3 worldPos = SlideController.Instance.elementTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

                    e.MoveTo(toGrid, worldPos);
                    elementIdHasJustMove.Add(id);

                    break;
                }
            }
        }

        Invoke(nameof(CoordinateItem), 0.28f);
        Invoke(nameof(CoordinateElement), 0.28f);
        Invoke(nameof(SadFunction), 0.28f);
    }

    public void RotateElement(Vector2Int pivot, List<Vector2Int> posList)
    {
        for (int i = 0; i < this.ElementList.Count; i++)
        {
            Element e = this.ElementList[i];
            foreach (Vector2Int pos in posList)
            {
                if (e.CurrentPos == pos)
                {
                    Vector2Int newP = RotateObjectController.Instance.RotateAroundPivot(pos, pivot, 90f);
                    Vector3Int newGP = new Vector3Int(newP.x, newP.y, 0);
                    Vector3 p = SlideController.Instance.itemTilemap.GetCellCenterWorld(newGP);

                    //Animation
                    e.Rotate(newP, p);

                    break;
                }
            }
        }
    }

    public bool CheckExistsElement(Vector3Int pos)
    {
        foreach (Element element in this.ElementList)
        {
            if  (element.CurrentPos == new Vector2Int(pos.x, pos.y) && 
                (element.EmotionType == EmotionType.Sad || element.EmotionType == EmotionType.Neutral)) 
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckExistsAllElement(Vector3Int pos)
    {
        foreach (Element element in this.ElementList)
        {
            if (element.CurrentPos == new Vector2Int(pos.x, pos.y))
            {
                return true;
            }
        }
        return false;
    }

    public void SadFunction()
    {
        bool isAbsorb = false;

        foreach (Element e in this.ElementList)
        {
            int index = ItemTileController.Instance.ItemPosList.IndexOf(e.CurrentPos);
            if (index != -1)
            {
                isAbsorb = true;

                Vector2Int posItem = ItemTileController.Instance.ItemPosList[index];
                SlideController.Instance.itemTilemap.SetTile(new Vector3Int(posItem.x, posItem.y, 0), null);
                ItemTileController.Instance.RemoveItem(posItem);
            }
        }

        if (isAbsorb)
        {
            SoundsManager.Instance.PlaySFX(SoundType.Sad);
        }
    }

    private void CoordinateElement()
    {
        foreach (int id in this.elementIdHasJustMove)
        {
            Element e = this.ElementList[id];

            if (e.EmotionType != EmotionType.Happy)
            {
                continue;
            }

            Vector2Int currentPos = e.CurrentPos;
            Vector2Int nearPos = new Vector2Int(0, 0);
            Element er = null;

            nearPos = currentPos + new Vector2Int(1, 0);
            er = this.GetElement(nearPos);
            if (er != null && er.EmotionType == EmotionType.Happy)
            {
                if (e.CoordinateWithElement(er.ElementType, er))
                {
                    int index = this.ElementList.IndexOf(er);
                    this.ElementList[index] = null;
                    this.ElementList[id] = null;
                    continue;
                }
            }

            nearPos = currentPos + new Vector2Int(-1, 0);
            er = this.GetElement(nearPos);
            if (er != null && er.EmotionType == EmotionType.Happy)
            {
                if (e.CoordinateWithElement(er.ElementType, er))
                {
                    int index = this.ElementList.IndexOf(er);
                    this.ElementList[index] = null;
                    this.ElementList[id] = null;
                    continue;
                }
            }

            nearPos = currentPos + new Vector2Int(0, -1);
            er = this.GetElement(nearPos);
            if (er != null && er.EmotionType == EmotionType.Happy)
            {
                if (e.CoordinateWithElement(er.ElementType, er))
                {
                    int index = this.ElementList.IndexOf(er);
                    this.ElementList[index] = null;
                    this.ElementList[id] = null;
                    continue;
                }
            }

            nearPos = currentPos + new Vector2Int(0, 1);
            er = this.GetElement(nearPos);
            if (er != null && er.EmotionType == EmotionType.Happy)
            {
                if (e.CoordinateWithElement(er.ElementType, er))
                {
                    int index = this.ElementList.IndexOf(er);
                    this.ElementList[index] = null;
                    this.ElementList[id] = null;
                    continue;
                }
            }
        }

        this.ElementList.RemoveAll(e => e == null);
    }

    private void CoordinateItem()
    {
        foreach (Element e in ElementList)
        {
            Vector2Int currentPos = e.CurrentPos;
            Vector2Int nearPos = new Vector2Int(0, 0);

            List<Vector2Int> offset4 = Library.Instance.LibOffsets4;

            foreach (Vector2Int offset in offset4)
            {
                nearPos = currentPos + offset;
                if (SlideController.Instance.itemTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
                {
                    if (e.InteractWithItem(ItemTileController.Instance.GetItemType(nearPos), nearPos))
                    {
                        continue;
                    }
                }
            }
        }
    }

    public Element GetElement(Vector2Int pos)
    {
        foreach (Element e in this.ElementList)
        {
            if (e.CurrentPos == pos)
            {
                return e;
            }
        }

        return null;
    }

    public bool IsBlockItem(Vector3Int cell)
    {
        Vector2Int pos = new Vector2Int(cell.x, cell.y);
        foreach (Element e in this.ElementList)
        {
            if (e.CurrentPos == pos && e.EmotionType == EmotionType.Neutral)
            {
                return true;
            }
        }

        return false;
    }

    public void CheckBlocktile()
    {
        foreach (Element e in this.ElementList)
        {
            Vector2Int cell = e.CurrentPos;
            Vector3Int pos = new Vector3Int(cell.x, cell.y, 0);
            if (SlideController.Instance.blockTilemap.HasTile(pos))
            {
                BlockTileController.Instance.AddPosToUnBlockTileList(pos);
            }
        }
    }

    public void ReActivePowerOfElement()
    {
        foreach (Element e in this.ElementList)
        {
            e.ReActivePower();
        }
    }

    public void ReInteractWithItem()
    {
        CoordinateItem();
    }

    public void ReItem()
    {
        foreach (Vector2Int itemPos in ItemTileController.Instance.ItemPosList)
        {
            Vector3Int nearPos3 = new Vector3Int(itemPos.x, itemPos.y, 0);

            if (!SlideController.Instance.itemTilemap.HasTile(nearPos3) && 
                SlideController.Instance.obstacleTilemap.HasTile(nearPos3))
            {
                TileBase obstacleTile = SlideController.Instance.obstacleTilemap.GetTile(nearPos3);
                SlideController.Instance.itemTilemap.SetTile(nearPos3, obstacleTile);
                SlideController.Instance.obstacleTilemap.SetTile(nearPos3, null);
                SlideController.Instance.powerTilemap.SetTile(nearPos3, null);
            }
        }
    }
}
