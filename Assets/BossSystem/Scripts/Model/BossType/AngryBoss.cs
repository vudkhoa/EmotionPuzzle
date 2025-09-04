using SoundManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class AngryBoss : Boss
{
    [Header(" Tile ")]
    [SerializeField] private TileBase FlagAttack;
    [SerializeField] private GameObject CooldownTimePrefab;
    [SerializeField] private Transform BarParent;

    public Slider SliderGr;
    private Vector2Int dangerZonePos;

    public override void Setup(List<float> healths, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos)
    {
        base.Setup(healths, cooldownTimeSkill, totalItems, startPos, endPos);
        this.BossType = BossType.AngryBoss;

        this.dangerZonePos = new Vector2Int(-1, -1);
        this.CurPhase = 1;
        this.CurHealth = this.Healths[CurPhase - 1];
        UIManager.Instance.GetUI<GameplayUI>().SetupBoss("Angry Boss", this.TotalItems);
        this.UpdateHealthUI(this.CurHealth, this.CurHealth);
    }

    public override void ActiveSkill()
    {
        Vector2Int posPlayer = SlideController.Instance.GetPlayerPos();
        Vector3Int worldPos = new Vector3Int(posPlayer.x, posPlayer.y, 0);
        this.dangerZonePos = posPlayer;
        SlideController.Instance.bossTilemap.SetTile(worldPos, this.FlagAttack);

        Vector3 posForCooldownTime = SlideController.Instance.groundTilemap.CellToWorld(worldPos) + SlideController.Instance.groundTilemap.cellSize / 2;
        posForCooldownTime.y -= 0.35f;
        GameObject sl = Instantiate(CooldownTimePrefab, posForCooldownTime, Quaternion.identity, this.BarParent);
        sl.GetComponent<SliderCooldown>().Setup(2f, 1f, true);
        this.SliderGr = sl.GetComponent<Slider>();

        Invoke(nameof(ActingDangerZone), 2f);
    }

    public override void CooldownTimeSkillUI()
    {
        base.CooldownTimeSkillUI();
        if (this.SliderGr == null)
        {
            return;
        }
        //this.SliderGr.value = (this.SliderGr.value * 2 - Time.deltaTime) / 2;
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
        Destroy(this.SliderGr.gameObject);
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
            this.Die();
            SlideController.Instance.LoadNextLevelAfterBoss();
        }
    }

    public override void NextPhase()
    {
        this.CurPhase++;
        base.NextPhase();
    }
}
