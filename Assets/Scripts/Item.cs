using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    private RectTransform rectTransform;
    private RectTransform currentRectTransform;
    private Transform itemParent;

    private Vector3 startPosition;
    private Vector2 offset;
    private Canvas canvas;

    private ItemData itemData;
    [SerializeField] private TextMeshProUGUI itemNameText;

    private UnityEngine.GameObject placeholder;//占位符
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetItemData(ItemData itemData)
    {
        this.itemData = itemData;
        itemNameText.text = itemData.name;
        itemNameText.color = itemData.iconColor;
    }



    #region  拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        placeholder = new UnityEngine.GameObject("Placeholder");
        RectTransform plRT = placeholder.AddComponent<RectTransform>();
        plRT.sizeDelta = rectTransform.sizeDelta;
        placeholder.transform.SetParent(rectTransform.parent);
        int index = rectTransform.GetSiblingIndex();
        placeholder.transform.SetSiblingIndex(index);

        currentRectTransform = rectTransform;
        offset = (Vector2)rectTransform.position - eventData.position;
        startPosition = rectTransform.position;
        itemParent = transform.parent;
        transform.SetParent(canvas.transform, false);
        transform.SetAsLastSibling();

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position + offset;
    }

    public void OnDrop(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPosition;
        transform.SetParent(itemParent);

        int index = placeholder.transform.GetSiblingIndex();
        rectTransform.SetSiblingIndex(index);
        Destroy(placeholder);
    }
    #endregion
}
