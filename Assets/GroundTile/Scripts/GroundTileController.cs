using UnityEngine;
using CustomUtils;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GroundTileController : SingletonMono<GroundTileController>
{
    public void MoveGroundTile(List<Vector2Int> cellsToSlides, Direction direction)
    {
        List<Vector2Int> cellsToSlide = new List<Vector2Int>(cellsToSlides);

        for (int i = cellsToSlide.Count - 1; i >= 0; i--)
        {
            Vector2Int cell = cellsToSlide[i];
            Vector3Int gridPos = new Vector3Int(cell.x, cell.y, 0);
            TileBase tile = SlideController.Instance.groundTilemap.GetTile(gridPos);
            if (SlideController.Instance.GetSpriteFromTile(tile) == null)
            {
                cellsToSlide.Remove(cell);
            }
        }

        //Spawn các tile động theo thứ tự cells
        List<TileFake> clones = new List<TileFake>();
        List<TileBase> tileOrder = new List<TileBase>();

        foreach (Vector2Int cellPos in cellsToSlide)
        {
            Vector3Int cell = new Vector3Int(cellPos.x, cellPos.y, 0);
            TileBase tile = SlideController.Instance.groundTilemap.GetTile(cell);
            if (tile == null) continue;

            // Lưu thứ tự tile để xử lý logic wrap-around
            tileOrder.Add(tile);

            // Tạo GameObject clone để tween
            TileFake obj = Instantiate(SlideController.Instance.groudTileFakePrefab, SlideController.Instance.groundTilemap.GetCellCenterWorld(cell), Quaternion.identity);
            Sprite sprite = SlideController.Instance.GetSpriteFromTile(tile);
            if (sprite != null)
                obj.SetSprite(sprite);

            obj.gridPos = cellPos;
            clones.Add(obj);

            SlideController.Instance.groundTilemap.SetTile(cell, null);
        }

        //Di chuyển các tile
        int count = cellsToSlide.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2Int toGrid = new Vector2Int(0, 0);
            if (i == 0)
            {
                toGrid = cellsToSlide[count-1];
            }
            else
            {
                toGrid = cellsToSlide[i - 1];
            }
            Vector3 worldPos = SlideController.Instance.groundTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

            clones[i].MoveTo(toGrid, worldPos);
        }

        //Bước 3: Sau khi tween xong → cập nhật lại Tilemap và xóa clone
        DOVirtual.DelayedCall(0.25f, () =>
        {
            for (int i = 0; i < count; i++)
            {
                int fromIndex = (i + 1) % count;
                Vector2Int toCell = cellsToSlide[i];
                SlideController.Instance.groundTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
            }

            foreach (var obj in clones)
                Destroy(obj.gameObject);
        });
    }

    public void SetGroundTileForRaft(Vector2Int posRaft)
    {
        Vector3Int p = new Vector3Int(posRaft.x, posRaft.y, 0);
        if (!SlideController.Instance.groundTilemap.HasTile(p))
        {
            SlideController.Instance.groundTilemap.SetTile(p, SlideController.Instance.groundNoneSprite);
        }
    }

    public void SetGroundTileForRotateObj(List<Vector2Int> posList)
    {
        foreach (Vector2Int pos in posList)
        {
            Vector3Int p = new Vector3Int(pos.x, pos.y, 0);
            if (!SlideController.Instance.groundTilemap.HasTile(p))
            {
                SlideController.Instance.groundTilemap.SetTile(p, SlideController.Instance.groundNoneSprite);
                SlideController.Instance.bgSmallTilemap.SetTile(p, SlideController.Instance.groundNoneSprite);
            }
        }
    }

    public void RemoveGroundTileForRotateObj(List<Vector2Int> posList)
    {
        foreach (Vector2Int pos in posList)
        {
            if (!this.HasSpriteInTile(pos))
            {
                Vector3Int p = new Vector3Int(pos.x, pos.y, 0);
                SlideController.Instance.groundTilemap.SetTile(p, null);
                SlideController.Instance.bgSmallTilemap.SetTile(p, null);
            }
        }
    }

    private bool HasSpriteInTile(Vector2Int pos)
    {
        Vector3Int p = new Vector3Int(pos.x, pos.y, 0);
        TileBase tile = SlideController.Instance.groundTilemap.GetTile(p);
        if (SlideController.Instance.GetSpriteFromTile(tile) == null)
        {
            return false;
        }

        return true;
    }
}
