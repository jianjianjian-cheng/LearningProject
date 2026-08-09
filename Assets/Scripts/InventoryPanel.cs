using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : UIPanelBase
{
    private bool isPlayingAnimation = false;
    private List<ItemData> itemDatasList = new List<ItemData>();
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemParent;

    private float originalY;
    private float startY;

    protected override void Awake()
    {
        base.Awake();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        originalY = rectTransform.anchoredPosition.y;
        startY = -Screen.height / 4;
    }

    void Start()
    {
        // 初始化数据
        itemDatasList.Add(new ItemData { name = "物品1", iconColor = Color.red });
        itemDatasList.Add(new ItemData { name = "物品2", iconColor = Color.green });
        itemDatasList.Add(new ItemData { name = "物品3", iconColor = Color.blue });
        itemDatasList.Add(new ItemData { name = "物品4", iconColor = Color.yellow });
        itemDatasList.Add(new ItemData { name = "物品5", iconColor = Color.magenta });
        itemDatasList.Add(new ItemData { name = "物品6", iconColor = Color.cyan });
        itemDatasList.Add(new ItemData { name = "物品7", iconColor = Color.white });
        RefreshItem();
    }

    public override void OnShow()
    {
        base.OnShow();
        gameObject.SetActive(true);
        ShowAnimation();
    }

    public override void OnHide()
    {
        base.OnHide();
        HideAnimation();
        // gameObject.SetActive(false);
    }

    private void RefreshItem()
    {
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }
        foreach (ItemData item in itemDatasList)
        {
            GameObject newItem = Instantiate(itemPrefab, itemParent);
            Item itemComponent = newItem.GetComponent<Item>();
            itemComponent.SetItemData(item);
        }
        if (gameObject.activeInHierarchy)
            StartCoroutine(RefreshBuild());
    }


    //刷新构建
    IEnumerator RefreshBuild()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemParent.GetComponent<RectTransform>());
    }

    public override void OnInit(object data)
    {

    }


    #region 面板显示隐藏时的动画
    private void ShowAnimation()
    {
        // StartCoroutine(AllShowAnimations());
        ShowAnimDOTween();
    }

    private void HideAnimation()
    {
        // StartCoroutine(AllHideAnimations());
        HIdeAnimDOTween();
    }

    #region  原生
    IEnumerator AllShowAnimations()
    {
        if (isPlayingAnimation) yield break;
        isPlayingAnimation = true;

        Coroutine fadeCor = StartCoroutine(FadeIn(0.5f));
        Coroutine slideCor = StartCoroutine(SlideUp(0.1f));

        yield return fadeCor;
        yield return slideCor;

        isPlayingAnimation = false;
    }

    IEnumerator AllHideAnimations()
    {
        if (isPlayingAnimation) yield break;
        isPlayingAnimation = true;

        Coroutine fadeCor = StartCoroutine(FadeOut(0.5f));
        Coroutine slideCor = StartCoroutine(SlideDown(0.5f));

        yield return fadeCor;
        yield return slideCor;

        isPlayingAnimation = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 淡入
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator FadeIn(float duration)
    {
        canvasGroup.alpha = 0;
        float originalAlpha = canvasGroup.alpha;
        float targetAlpha = 1;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(originalAlpha, targetAlpha, t / duration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    IEnumerator FadeOut(float duration)
    {
        canvasGroup.alpha = 1;
        float originalAlpha = canvasGroup.alpha;
        float targetAlpha = 0;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(originalAlpha, targetAlpha, t / duration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }


    IEnumerator SlideUp(float duration)
    {
        float startY = rectTransform.anchoredPosition.y - Screen.height / 4;
        float targetY = rectTransform.anchoredPosition.y;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startY);

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Lerp(startY, targetY, t / duration));
            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, targetY);
    }

    IEnumerator SlideDown(float duration)
    {
        float startY = rectTransform.anchoredPosition.y;
        float targetY = rectTransform.anchoredPosition.y - Screen.height / 4;
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startY);

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Lerp(startY, targetY, t / duration));
            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startY);
    }
    #endregion
    #region  DOTween
    private void ShowAnimDOTween()
    {

        if (isPlayingAnimation) return;//布尔锁
        isPlayingAnimation = true;
        gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        //添加淡入动画
        seq.Append(canvasGroup.DOFade(1, 0.2f));
        //添加上滑动画
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startY);
        seq.Join(rectTransform.DOAnchorPosY(originalY, 0.2f).SetEase(Ease.OutBack));

        seq.OnComplete(() =>
        {
            isPlayingAnimation = false;
        });

        seq.OnKill(() =>
        {
            isPlayingAnimation = false;
        });
    }

    private void HIdeAnimDOTween()
    {
        if (isPlayingAnimation) return;

        isPlayingAnimation = true;
        Sequence seq = DOTween.Sequence();
        //添加淡出动画
        seq.Append(canvasGroup.DOFade(0, 0.2f));
        //添加下滑动画
        seq.Join(rectTransform.DOAnchorPosY(startY, 0.2f).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            isPlayingAnimation = false;
            gameObject.SetActive(false);
        });

        seq.OnKill(() =>
        {
            isPlayingAnimation = false;
        });
    }
    #endregion

    #endregion

}
