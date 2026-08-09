using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 面板层级


public abstract class UIPanelBase : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    public UILayer UILayer { get; set; }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        OnShow();
        // canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void Hide()
    {
        // gameObject.SetActive(false);
        OnHide();
    }





    //------子类去实现-------
    public abstract void OnInit(object data);

    public virtual void OnShow() { }
    public virtual void OnHide() { }
}
