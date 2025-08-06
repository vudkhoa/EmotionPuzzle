using CustomUtils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObstacleTileController : SingletonMono<ObstacleTileController>
{
    [SerializeField] private ParticleSystem burnDownEffect;

    public void RemoveObstacleTile(Vector2Int pos)
    {
        Instantiate(burnDownEffect, SlideController.Instance.obstacleTilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0)), Quaternion.identity);
        SlideController.Instance.obstacleTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
    }

    public void MoveObsatcleTile(Vector3Int oldPos, Vector3Int newPos)
    {
        Sprite sp = SlideController.Instance.GetSpriteFromTile(SlideController.Instance.obstacleTilemap.GetTile(oldPos));
        TileFake obGO = Instantiate(SlideController.Instance.obstacleTileFakePrefab, SlideController.Instance.groundTilemap.GetCellCenterWorld(oldPos), Quaternion.identity);
        obGO.SetSprite(sp);

        TileBase tile = SlideController.Instance.obstacleTilemap.GetTile(oldPos);
        SlideController.Instance.obstacleTilemap.SetTile(oldPos, null);

        Sequence knockbackSequence = DOTween.Sequence();
        knockbackSequence.Append(obGO.transform.DOMove(SlideController.Instance.obstacleTilemap.GetCellCenterWorld(newPos), 0.1f).SetEase(Ease.OutQuad));
        knockbackSequence.Append(obGO.transform.DOShakePosition(0.15f, 0.1f, 10, 90));

        knockbackSequence.OnComplete(() =>
        {
            SlideController.Instance.obstacleTilemap.SetTile(newPos, tile);

            if (SlideController.Instance.IceStarId > 0)
            {
                IceStarController.Instance.SetIceStars();
            }

            Destroy(obGO.gameObject);
        });
    }

    public void RotateObstacleTile(Vector2Int pivot, List<Vector2Int> posList)
    {
        foreach (Vector2Int pos in posList)
        {
            Vector3Int gridPos = new Vector3Int(pos.x, pos.y, 0);
            if (SlideController.Instance.obstacleTilemap.HasTile(gridPos))
            {
                Vector2Int newP = RotateObjectController.Instance.RotateAroundPivot(pos, pivot, 90f);
                Vector3Int newGP = new Vector3Int(newP.x, newP.y, 0);
                Vector3 p = SlideController.Instance.obstacleTilemap.GetCellCenterWorld(newGP);

                //Animation
                TileBase tile = SlideController.Instance.obstacleTilemap.GetTile(gridPos);

                TileFake obj = Instantiate(SlideController.Instance.obstacleTileFakePrefab, SlideController.Instance.obstacleTilemap.GetCellCenterWorld(gridPos), Quaternion.identity);
                Sprite sprite = SlideController.Instance.GetSpriteFromTile(tile);
                if (sprite != null)
                    obj.SetSprite(sprite);

                obj.gridPos = pos;

                SlideController.Instance.obstacleTilemap.SetTile(gridPos, null);

                obj.transform.DOMove(p, 0.2f).SetEase(Ease.OutQuad);

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    Destroy(obj.gameObject);
                    SlideController.Instance.obstacleTilemap.SetTile(newGP, tile);
                });
            }
        }
    }

    public void ThrowObstacleTile(Vector3Int oldPos, Vector3Int newPos)
    {
        Sprite sp = SlideController.Instance.GetSpriteFromTile(SlideController.Instance.obstacleTilemap.GetTile(oldPos));
        TileFake obGO = Instantiate(SlideController.Instance.groudTileFakePrefab, SlideController.Instance.groundTilemap.GetCellCenterWorld(oldPos), Quaternion.identity);
        obGO.SetSprite(sp);

        SlideController.Instance.obstacleTilemap.SetTile(oldPos, null);

        Vector3 startPos = SlideController.Instance.groundTilemap.GetCellCenterWorld(oldPos);
        Vector3 endPos = SlideController.Instance.groundTilemap.GetCellCenterWorld(newPos);
        Vector3 upOffset = new Vector3(0, 1f, 0);

        Sequence throwSequence = DOTween.Sequence();
        throwSequence.Append(obGO.transform
        .DOJump(endPos, 1.2f, 1, 0.15f)
        .SetEase(Ease.OutQuad));

        throwSequence.Join(obGO.transform
            .DOScale(Vector3.zero, 0.10f) 
            .SetEase(Ease.InQuad)
            .SetDelay(0.05f));

        throwSequence.OnComplete(() =>
        {
            SlideController.Instance.obstacleTilemap.SetTile(newPos, null);
            Destroy(obGO.gameObject);
        });
    }
}
