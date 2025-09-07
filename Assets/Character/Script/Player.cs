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

    public void ErrorMove(Direction direction)
    {
        Vector3 curPos = this.transform.position;
        Vector3 offset = Vector3.zero;
        switch (direction)
        {
            case Direction.Up:
                offset = new Vector3(0, 1f, 0);
                break;
            case Direction.Down:
                offset = new Vector3(0, -1f, 0);
                break;
            case Direction.Left:
                offset = new Vector3(-1f, 0, 0);
                break;
            case Direction.Right:
                offset = new Vector3(1f, 0, 0);
                break;
        }
        this.transform.DOMove(curPos + offset * 0.17f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            this.transform.DOMove(curPos, 0.15f).SetEase(Ease.OutBack, 2f);
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
        // Reset F Tutorial
        if (RotateObjectController.Instance.IsShowTutorial(currentPos))
        {
            ShowFTutorial();
        }
        else
        {
            HideFTutorial();
        }

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
        Vector3 pos = this.transform.position;

        this.transform.DOShakePosition(
            duration: 0.1f,
            strength: new Vector3(0.5f, 0.5f, 0),
            vibrato: 1000,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );

        this.transform.DOMove(pos, 0.1f).SetEase(Ease.InOutBack);
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
