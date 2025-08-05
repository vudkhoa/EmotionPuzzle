using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using UnityEngine.Tilemaps;

public class BlockTileController : SingletonMono<BlockTileController>
{
    public TileBase blockTile;
    public TileBase unBlockTile;

    private List<Block> blocks;
    private List<Vector3Int> unBlockTileList = new List<Vector3Int>();

    public void Setup(List<Block> blocks)
    {
        this.blocks = new List<Block>(blocks);
    }

    public void UnBlockTile()
    {
        foreach (Vector3Int tile in unBlockTileList)
        {
            SlideController.Instance.blockTilemap.SetTile(tile, blockTile);
        }

        unBlockTileList = new List<Vector3Int>();

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
    }

    public void AddPosToUnBlockTileList(Vector3Int pos)
    {
        SlideController.Instance.blockTilemap.SetTile(pos, unBlockTile);
        unBlockTileList.Add(pos);
    }
}
