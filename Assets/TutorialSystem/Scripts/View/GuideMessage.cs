using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuideMessage : MonoBehaviour
{
    public RectTransform panelTextTf;
    public TextMeshProUGUI tutorialText;
    public Image fill;

    private bool isShowing = false;
    private float currentTime = 0f;

    public void ShowTutorialText(string text)
    {
        //panelTextTf.gameObject.SetActive(true);
        SetTutorialText(text);

        Invoke(nameof(HideTutorialText), 5f);
    }

    private void Update()
    {
        if (isShowing)
        {
            currentTime += Time.deltaTime;
            fill.fillAmount = 1 - currentTime/5f;
        }
    }

    private void HideTutorialText()
    {
        //panelTextTf.gameObject.SetActive(false);
        panelTextTf.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(this.gameObject);
        });
    }

    private void SetTutorialText(string text)
    {
        tutorialText.text = text;
        //ResizeImageToFitText();

        panelTextTf.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        isShowing = true;
    }

    private void ResizeImageToFitText()
    {
        // Force text to update its layout
        tutorialText.ForceMeshUpdate();

        // Get preferred width and height of the text
        Vector2 textSize = new Vector2(
            tutorialText.preferredWidth,
            tutorialText.preferredHeight
        );

        // Apply padding and set size
        panelTextTf.sizeDelta = textSize + new Vector2(20f, 10f);
    }
}
