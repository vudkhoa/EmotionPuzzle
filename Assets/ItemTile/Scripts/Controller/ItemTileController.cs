using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using DG.Tweening;
using UnityEngine.Tilemaps;

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
                ItemTileController.Instance.UpdateItemPosList(cellsToSlide[fromIndex], toCell); //Update Item Pos List
                SlideController.Instance.itemTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
            }

            foreach (var obj in clones)
            {
                if (obj != null)
                    Destroy(obj.gameObject);
            }

            foreach (Element e in ElementController.Instance.ElementList)
            {
                if (e.ElementType == ElementType.Ice)
                {
                    e.Power();
                }
            }    
        });
    }

}
