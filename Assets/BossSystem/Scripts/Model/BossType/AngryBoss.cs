using SoundManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AngryBoss : Boss
{
    [Header(" Tile ")]
    [SerializeField] private TileBase FlagAttack;

    private Vector2Int dangerZonePos;

    public override void Setup(List<float> healths, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos)
    {
        base.Setup(healths, cooldownTimeSkill, totalItems, startPos, endPos);
        this.BossType = BossType.AngryBoss;

        this.dangerZonePos = new Vector2Int(-1, -1);
        this.CurPhase = 1;
        this.CurHealth = this.Healths[CurPhase - 1];
        UIManager.Instance.GetUI<GameplayUI>().SetupBoss("Angry Boss", this.CurHealth, this.TotalItems);
    }

    public override void ActiveSkill()
    {
        Vector2Int posPlayer = SlideController.Instance.GetPlayerPos();
        Vector3Int worldPos = new Vector3Int(posPlayer.x, posPlayer.y, 0);
        this.dangerZonePos = posPlayer;
        SlideController.Instance.bossTilemap.SetTile(worldPos, this.FlagAttack);
        Invoke(nameof(ActingDangerZone), 2f);
    }

    private void ActingDangerZone()
    {
        this.IsActingSkill = true;

        if (CheckPlayerInDangerZone())
        {
            ActingDanger();
            SlideController.Instance.PlayerDie();
            SoundsManager.Instance.PlaySFX(SoundType.LoseBoss);
            GameManager.Instance.State = GameState.GameOver;
            this.BossState = BossState.Dead;
            Invoke(nameof(LoseGameBoss), 0.3f);

        }
        else
        {
            Invoke(nameof(ActingDanger), 0.25f);
            Invoke(nameof(ResetIsActingSkill), 0.35f);
        }
    }

    private void ActingDanger()
    {
        Vector3Int worldPos = new Vector3Int(this.dangerZonePos.x, this.dangerZonePos.y, 0);
        this.dangerZonePos = new Vector2Int(-1, -1);
        SlideController.Instance.bossTilemap.SetTile(worldPos, null);
        SoundsManager.Instance.PlaySFX(SoundType.BossDestroyGround);
        GroundTileController.Instance.RemoveGroundTile(new Vector2Int(worldPos.x, worldPos.y));
    }

    private bool CheckPlayerInDangerZone()
    {
        return SlideController.Instance.GetPlayerPos() == this.dangerZonePos;
    }

    public override void CheckDie()
    {
        if (CurPhase < this.Healths.Count)
        {
            this.NextPhase();
        }
        else
        {
            this.BossState = BossState.Dead;
            SlideController.Instance.BossId = 0;
            Destroy(this.gameObject); // Chưa viết hiệu ứng boss chết
            SlideController.Instance.LoadNextLevelAfterBoss();
        }
    }

    public override void NextPhase()
    {
        this.CurPhase++;
        base.NextPhase();
    }
}
