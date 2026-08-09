using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] private Image HPFill;

    [SerializeField] private float maxHP = 100f;
    float currentHP = 100f;
    [SerializeField] private TextMeshProUGUI HPText;
    private RectTransform rectTransform;

    void Start()
    {
        HPFill.fillAmount = 1;
        UpdateHPText();
        rectTransform = GetComponent<RectTransform>();
    }


    public void SetHP(float value)
    {
        StartCoroutine(ReduceHP(value));
    }


    //插值平滑过渡HP
    IEnumerator ReduceHP(float value, float duration = 0.5f)
    {
        currentHP -= value;
        UpdateHPText();
        StartCoroutine(HitAnim(0.2f));
        float hpRatio = currentHP / maxHP;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float valueClamp = Mathf.Clamp01(t / duration);
            HPFill.fillAmount = Mathf.Lerp(HPFill.fillAmount, hpRatio, valueClamp);
            yield return null;
        }
        HPFill.fillAmount = hpRatio;
    }

    //更新HP文本
    private void UpdateHPText()
    {
        float hpRatio = Mathf.Clamp01(currentHP / maxHP);
        HPText.text = $"{currentHP}/{maxHP}";

        if (hpRatio <= 0.3f)
        {
            LowHONoticeAnim();
        }
    }


    //受击提示动画(协程版本)
    IEnumerator HitAnim(float duration = 0.5f)
    {
        float i = 0;
        Vector3 startScale = rectTransform.localScale;
        Vector3 targetScale = rectTransform.localScale * 1.2f;
        while (i < duration / 2)
        {
            i += Time.deltaTime;
            float p = i / (duration / 2);
            rectTransform.localScale = Vector3.Lerp(startScale,
            targetScale, p);
            yield return null;
        }
        rectTransform.localScale = startScale;
        //变回原本大小
        i = 0;
        while (i < duration / 2)
        {
            i += Time.deltaTime;
            float p = i / (duration / 2);
            rectTransform.localScale = Vector3.Lerp(targetScale,
            startScale, p);
            yield return null;
        }
        rectTransform.localScale = startScale;
    }

    ////受击提示动画(DOTween版本)
    private void HitAnimDOTween()
    {

    }


    //低血条提示
    private void LowHONoticeAnim()
    {
        HPText.color = Color.red;
    }
}


