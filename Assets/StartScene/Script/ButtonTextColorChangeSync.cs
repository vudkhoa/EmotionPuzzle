using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonTextColorChangeSync : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TMP_Text tmpText;

    private Button button;
    private ColorBlock colors;
    private bool isHovered = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        colors = button.colors;

        if (tmpText == null)
            tmpText = GetComponentInChildren<TMP_Text>();

        tmpText.color = colors.normalColor;
    }

    private void Update()
    {
        if (!button.interactable)
        {
            tmpText.color = colors.disabledColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            tmpText.color = colors.highlightedColor;
            isHovered = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            tmpText.color = colors.normalColor;
            isHovered = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
            tmpText.color = colors.pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button.interactable)
            tmpText.color = isHovered ? colors.highlightedColor : colors.normalColor;
    }
}
