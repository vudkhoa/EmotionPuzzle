using CustomUtils;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ElementController : SingletonMono<ElementController>
{
    public List<Element> ElementList;

    private List<int> elementIdHasJustMove;

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
        Vector2Int offset = Vector2Int.zero;
        switch (direction)
        {
            case Direction.Left:
                offset = Vector2Int.left;
                break;
            case Direction.Right:
                offset = Vector2Int.right;
                break;
            case Direction.Up:
                offset = Vector2Int.up;
                break;
            case Direction.Down:
                offset = Vector2Int.down;
                break;
        }

        foreach (Element e in this.ElementList)
        {
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
                    else if (!SlideController.Instance.groundTilemap.HasTile(new Vector3Int(nextPos.x, nextPos.y, 0)))
                    {
                        return true;
                    }
                }

                if (cellMovePosList[0] == e.CurrentPos)
                {
                    newPos = cellMovePosList[cellMovePosList.Count - 1];
                }
                else
                {
                    newPos = e.CurrentPos + offset;
                }
                
                if (SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(newPos.x, newPos.y, 0)))
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
            if (e.EmotionType != EmotionType.Happy && e.EmotionType != EmotionType.Angry)
            {
                continue;
            }

            id++;
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

        Invoke(nameof(CoordinateElement), 0.28f);
        Invoke(nameof(CoordinateItem), 0.28f);
        Invoke(nameof(SadFunction), 0.28f);
    }

    public bool CheckExitsElement(Vector3Int pos)
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

    public void SadFunction()
    {
        foreach (Element e in this.ElementList)
        {
            int index = ItemTileController.Instance.ItemPosList.IndexOf(e.CurrentPos);
            if (index != -1)
            {
                Vector2Int posItem = ItemTileController.Instance.ItemPosList[index];
                SlideController.Instance.itemTilemap.SetTile(new Vector3Int(posItem.x, posItem.y, 0), null);
                ItemTileController.Instance.RemoveItem(posItem);
            }
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

            nearPos = currentPos + new Vector2Int(1, 0);
            if (SlideController.Instance.itemTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
            {
                if (e.InteractWithItem(ItemTileController.Instance.GetItemType(nearPos), nearPos))
                {
                    ItemTileController.Instance.RemoveItem(nearPos);
                    continue;
                }
            }

            nearPos = currentPos + new Vector2Int(-1, 0);
            if (SlideController.Instance.itemTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
            {
                if (e.InteractWithItem(ItemTileController.Instance.GetItemType(nearPos), nearPos))
                {
                    ItemTileController.Instance.RemoveItem(nearPos);
                    continue;
                }
            }

            nearPos = currentPos + new Vector2Int(0, 1);
            if (SlideController.Instance.itemTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
            {
                if (e.InteractWithItem(ItemTileController.Instance.GetItemType(nearPos), nearPos))
                {
                    ItemTileController.Instance.RemoveItem(nearPos);
                    continue;
                }
            }

            nearPos = currentPos + new Vector2Int(0, -1);
            if (SlideController.Instance.itemTilemap.HasTile(new Vector3Int(nearPos.x, nearPos.y, 0)))
            {
                if (e.InteractWithItem(ItemTileController.Instance.GetItemType(nearPos), nearPos))
                {
                    ItemTileController.Instance.RemoveItem(nearPos);
                    continue;
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

    public void SetPowerRingAll()
    {
        foreach (Element e in this.ElementList)
        {
            e.SetPowerRing(e.CurrentPos);
        }
    }
}
