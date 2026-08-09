using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : UIPanelBase
{
    [SerializeField] private Button closeBtn;

    public override void OnInit(object data)
    {

    }

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instacnce.Hide<RewardPanel>();
        });
    }

    public override void OnShow()
    {
        base.OnShow();
        StartCoroutine(ShowAllAnimation());
    }


    public override void OnHide()
    {
        base.OnHide();
        gameObject.SetActive(false);
    }

    #region 动画相关
    #region  原生动画
    IEnumerator ShowAllAnimation()
    {
        if (isPlayingAnimation) yield return null;
        isPlayingAnimation = true;

        // Coroutine showUpCoroutine = StartCoroutine(ShowUpCoroutine(0.5f));
        Coroutine showScaleCoroutine = StartCoroutine(ShowScaleCoroutine(0.3f));
        Coroutine showFadeCoroutine = StartCoroutine(ShowFadeCoroutine(0.3f));

        // yield return showUpCoroutine;
        yield return showScaleCoroutine;
        yield return showFadeCoroutine;

        isPlayingAnimation = false;
    }


    IEnumerator ShowUpCoroutine(float duration = 0.5f)
    {
        float t = 0;
        float startY = rectTransform.anchoredPosition.y - rectTransform.rect.height / 4;
        float targetY = rectTransform.anchoredPosition.y;
        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startY + (targetY - startY) * t / duration);
            yield return null;
        }
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, targetY);
    }

    IEnumerator ShowScaleCoroutine(float duration = 0.5f)
    {
        float t = 0;
        Vector3 startScale = rectTransform.localScale * 0.65f;
        Vector3 targetScale = Vector3.one * 1.05f;
        //放大
        while (t < (duration / 2))
        {
            t += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t / (duration / 2));
            yield return null;
        }
        rectTransform.localScale = targetScale;

        //缩小
        t = 0;
        startScale = targetScale;
        targetScale = Vector3.one;
        while (t < (duration / 2))
        {
            t += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t / (duration / 2));
            yield return null;
        }
        rectTransform.localScale = targetScale;
    }

    IEnumerator ShowFadeCoroutine(float duration = 0.5f)
    {
        float t = 0;
        canvasGroup.alpha = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
    #endregion

    #region DOTween版本
    private void ShowAllAnimationDOTween()
    {
        Sequence seq = DOTween.Sequence();
        seq.OnKill(() =>
        {
            isPlayingAnimation = false;
        });

        //放大
        seq.Append(rectTransform.DOScale(Vector3.one * 1.05f, 0.3f))
        .OnComplete(() =>
        {
            rectTransform.DOScale(Vector3.one, 0.3f);
        });
        seq.Append(canvasGroup.DOFade(1, 0.3f));



    }
    #endregion
    #endregion
}
