using DG.Tweening;
using SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject FTutorial;

    private Vector2Int currentPos;

    public Vector2Int GetCurrentPos()
    {
        return currentPos;
    }

    public void SetCurrentPos(Vector2Int newPos)
    {
        currentPos = newPos;
    }

    public void MoveTo(Vector2Int newGridPos, Vector3 worldPos)
    {
        //Sound
        SoundsManager.Instance.PlaySFX(SoundType.Slide);

        currentPos = newGridPos;
        transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            SavePointController.Instance.SetCheckPoint(currentPos);

            if (RotateObjectController.Instance.IsShowTutorial(this.currentPos))
            {
                ShowFTutorial();
            }
            else
            {
                HideFTutorial();
            }
            SlideController.Instance.LoadNextLevel();
        });
    }

    public void Teleport(Vector2Int newGridPos, Vector3 worldPos)
    {
        //Sound
        SoundsManager.Instance.PlaySFX(SoundType.Teleport);

        // 1. Scale nhỏ dần để ẩn 
        this.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            // 2. Di chuyển đến đầu hàng
            this.transform.position = worldPos;
            this.currentPos = newGridPos;

            // 3. Scale lớn lên để hiện  lại
            this.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack).OnComplete(() => 
            {
                SavePointController.Instance.SetCheckPoint(currentPos);

                if (RotateObjectController.Instance.IsShowTutorial(this.currentPos))
                {
                    ShowFTutorial();
                }
                else
                {
                    HideFTutorial();
                }
                SlideController.Instance.LoadNextLevel();
            });
        });
    }

    public void SetPos(Vector2Int pos)
    {
        currentPos = pos;
        this.transform.position = SlideController.Instance.groundTilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
    }

    public void Shake()
    {
        //Sound
        SoundsManager.Instance.PlaySFX(SoundType.Error);

        this.transform.localRotation = Quaternion.identity;

        this.transform.DOShakeRotation(
            duration: 0.4f,       // Thời gian lắc
            strength: new Vector3(0, 0, 15f), // Xoay quanh trục Z
            vibrato: 10,          // Số lần lắc
            randomness: 90f       // Ngẫu nhiên góc
        ).SetEase(Ease.OutQuad)
        .OnKill(() =>
        {
            this.transform.localRotation = Quaternion.identity;
        });
    }

    public void TakeDamage()
    {
        this.transform.DOShakePosition(
            duration: 0.1f,
            strength: new Vector3(0.2f, 0.2f, 0),
            vibrato: 1000,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );
    }

    public void ShowFTutorial()
    {
        FTutorial.SetActive(true);
    }

    public void HideFTutorial()
    {
        FTutorial.SetActive(false);
    }
}
