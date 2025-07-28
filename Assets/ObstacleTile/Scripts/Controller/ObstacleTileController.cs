using CustomUtils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

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
        TileFake obGO = Instantiate(SlideController.Instance.groudTileFakePrefab, SlideController.Instance.groundTilemap.GetCellCenterWorld(oldPos), Quaternion.identity);
        obGO.SetSprite(sp);

        TileBase tile = SlideController.Instance.obstacleTilemap.GetTile(oldPos);
        SlideController.Instance.obstacleTilemap.SetTile(oldPos, null);

        Sequence knockbackSequence = DOTween.Sequence();
        knockbackSequence.Append(obGO.transform.DOMove(SlideController.Instance.groundTilemap.GetCellCenterWorld(newPos), 0.1f).SetEase(Ease.OutQuad));
        knockbackSequence.Append(obGO.transform.DOShakePosition(0.2f, 0.1f, 10, 90));

        knockbackSequence.OnComplete(() =>
        {
            SlideController.Instance.obstacleTilemap.SetTile(newPos, tile);
            Destroy(obGO.gameObject);
        });
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
