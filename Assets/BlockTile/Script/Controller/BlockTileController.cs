using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using UnityEngine.Tilemaps;

public class BlockTileController : SingletonMono<BlockTileController>
{
    public TileBase blockTile;
    public TileBase unBlockTile;

    public List<Block> blocks;
    public List<Vector3Int> unBlockTileList = new List<Vector3Int>();

    private List<bool> isUnblock = new List<bool>();

    public bool IsInSave(Vector2Int pos)
    {
        if (SavePointController.Instance.startSavePoint.x <= pos.x
            && SavePointController.Instance.startSavePoint.y <= pos.y
            && SavePointController.Instance.endSavePoint.x >= pos.x
            && SavePointController.Instance.endSavePoint.y >= pos.y)
        {
            return true;
        }

        return false;
    }

    public void ResetInitData()
    {
        for (int i= 0; i < blocks.Count; i++)
        {
            bool isUnblock = true;

            Block block = blocks[i];
            foreach (Vector2Int cell in block.BlockPosList)
            {
                Vector3Int pos = new Vector3Int(cell.x, cell.y, 0);
                if (!this.unBlockTileList.Contains(pos))
                {
                    isUnblock = false;
                    break;
                }
            }

            this.isUnblock[i] = isUnblock;
        }
    }

    public void Reload()
    {
        //foreach (Block block in blocks)
        //{
        //    if (IsInSave(block.BlockPosList[0]))
        //    {
        //        foreach (Vector2Int groundPos in block.groundPosList)
        //        {
        //            SlideController.Instance.groundTilemap.SetTile(new Vector3Int(groundPos.x, groundPos.y, 0), null);

        //        }

        //        foreach (Vector2Int bPos in block.BlockPosList)
        //        {
        //            SlideController.Instance.blockTilemap.SetTile(new Vector3Int(bPos.x, bPos.y, 0), blockTile);
        //        }
        //    }
        //}

        foreach (Block block in blocks)
        {
            int index = blocks.IndexOf(block);
            if (!isUnblock[index])
            {
                foreach (Vector2Int groundPos in block.groundPosList)
                {
                    SlideController.Instance.groundTilemap.SetTile(new Vector3Int(groundPos.x, groundPos.y, 0), null);

                }

                foreach (Vector2Int bPos in block.BlockPosList)
                {
                    SlideController.Instance.blockTilemap.SetTile(new Vector3Int(bPos.x, bPos.y, 0), blockTile);
                }
            }
            else
            {
                foreach (Vector2Int groundPos in block.groundPosList)
                {
                    SlideController.Instance.groundTilemap.SetTile(new Vector3Int(groundPos.x, groundPos.y, 0), block.groundTileList[0]);
                }

                foreach (Vector2Int bPos in block.BlockPosList)
                {
                    SlideController.Instance.blockTilemap.SetTile(new Vector3Int(bPos.x, bPos.y, 0), unBlockTile);
                }
            }
        }
    }

    public void Setup(List<Block> blocks)
    {
        this.blocks = new List<Block>(blocks);
        for (int i = 0; i < blocks.Count; i++)
        {
            isUnblock.Add(false);
        }
    }

    public void UnBlockTile()
    {
        //Reset
        foreach (Vector3Int tile in unBlockTileList)
        {
            SlideController.Instance.blockTilemap.SetTile(tile, blockTile);
        }

        unBlockTileList = new List<Vector3Int>();

        foreach (Block block in blocks)
        {
            for (int i = 0; i < block.groundPosList.Count; i++)
            {
                Vector2Int cell1 = block.groundPosList[i];
                Vector3Int pos1 = new Vector3Int(cell1.x, cell1.y, 0);
                SlideController.Instance.groundTilemap.SetTile(pos1, null);
            }
        }

        //Check player
        Vector2Int cell = SlideController.Instance.GetPlayerPos();
        Vector3Int pos = new Vector3Int(cell.x, cell.y, 0);
        if (SlideController.Instance.blockTilemap.HasTile(pos))
        {
            SlideController.Instance.blockTilemap.SetTile(pos, unBlockTile);
            unBlockTileList.Add(pos);
        }

        //Check obstacle
        foreach (Block block in blocks)
        {
            foreach (Vector2Int bPos in block.BlockPosList)
            {
                pos = new Vector3Int(bPos.x, bPos.y, 0);    
                if (SlideController.Instance.obstacleTilemap.HasTile(pos))
                {
                    SlideController.Instance.blockTilemap.SetTile(pos, unBlockTile);
                    unBlockTileList.Add(pos);
                }
            }
        }

        //Check item
        ItemTileController.Instance.CheckBlockTile();

        //Check element
        ElementController.Instance.CheckBlocktile();

        OpenGroundTile();
    }

    public void AddPosToUnBlockTileList(Vector3Int pos)
    {
        SlideController.Instance.blockTilemap.SetTile(pos, unBlockTile);
        unBlockTileList.Add(pos);
    }

    private void OpenGroundTile()
    {
        foreach (Block block in blocks)
        {
            bool isUnBlock = true;
            foreach (Vector2Int cell in block.BlockPosList)
            {
                Vector3Int pos = new Vector3Int(cell.x, cell.y, 0);
                if (!this.unBlockTileList.Contains(pos))
                {
                    isUnBlock = false;
                    break;
                }
            }
            if (isUnBlock)
            {
                for (int i = 0; i < block.groundPosList.Count; i++)
                {
                    Vector2Int cell = block.groundPosList[i];
                    Vector3Int pos = new Vector3Int(cell.x, cell.y, 0);
                    SlideController.Instance.groundTilemap.SetTile(pos, block.groundTileList[i]);
                }
            }
        }
    }
}
