using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chipbar : MonoBehaviour
{

    #region Public Variables

    public float blendInTime = 1.25f;
    public float blendOutTime = 1.25f;

    public Image barImg;
    public Image flashImg;

    #endregion

    #region Private Variables

    private Coroutine blendInRoutine = null;
    private Coroutine blendOutRoutine = null;

    private ChipbarState state = ChipbarState.INITIAL;

    private float initialAmount = 0.0f;
    private float currentAmount = 0.0f;

    #endregion

    private void HideFlash()
    {
        var flashColor = this.flashImg.color;
        flashColor.a = 0.0f;
        this.flashImg.color = flashColor;
    }

    private void Awake()
    {
        this.barImg.fillAmount = 0.0f;
        this.flashImg.fillAmount = 0.0f;

        this.HideFlash();

        this.state = ChipbarState.INITIAL;
        this.initialAmount = 0.0f;
        this.currentAmount = 0.0f;
    }

    public void SetValue(float percent)
    {
        this.barImg.fillAmount = percent;

        this.HideFlash();

        this.state = ChipbarState.INITIAL;
        this.currentAmount = percent;
        this.initialAmount = percent;
    }


    private IEnumerator BlendInFlash()
    {
        var startColor = this.flashImg.color;
        var color = startColor;

        float timer = 0.0f;
        while(timer < this.blendInTime)
        {
            float percent = timer / this.blendInTime;
            color.a = Mathf.Lerp(startColor.a, 1.0f, percent);

            this.flashImg.color = color;

            yield return null;

            timer += Time.deltaTime;
        }

        color.a = 1.0f;
        this.flashImg.color = color;

        this.state = ChipbarState.BLEND_OUT;
        this.blendOutRoutine = this.StartCoroutine(this.BlendOutFlash());
    }

    private IEnumerator BlendOutFlash()
    {
        var startColor = this.flashImg.color;
        var color = startColor;

        float timer = 0.0f;
        while(timer < this.blendOutTime)
        {
            float percent = timer / this.blendOutTime;
            color.a = Mathf.Lerp(startColor.a, 0.0f, percent);

            this.flashImg.color = color;

            yield return null;

            timer += Time.deltaTime;
        }

        color.a = 0.0f;
        this.flashImg.color = color;

        this.state = ChipbarState.INITIAL;
        this.initialAmount = this.currentAmount;
    }


    public void Add(float amount)
    {
        float newAmount = this.currentAmount + amount;
        newAmount = Mathf.Clamp01(newAmount);

        switch(this.state)
        {
            case ChipbarState.INITIAL:
                this.SetValue(newAmount);
                break;
            case ChipbarState.BLEND_IN:
                if(newAmount >= this.initialAmount)
                {
                    this.StopCoroutine(this.blendInRoutine);
                    this.SetValue(newAmount);
                    this.state = ChipbarState.INITIAL;
                }
                else
                {
                    this.currentAmount = newAmount;
                    this.barImg.fillAmount = this.currentAmount;
                }
                break;
            case ChipbarState.BLEND_OUT:
                if(newAmount >= this.initialAmount)
                {
                    this.StopCoroutine(this.blendOutRoutine);
                    this.SetValue(newAmount);
                    this.state = ChipbarState.INITIAL;
                } else
                {
                    this.currentAmount = newAmount;
                    this.barImg.fillAmount = this.currentAmount;
                }
                break;
        }
    }

    public void Remove(float amount)
    {
        float newAmount = this.currentAmount - amount;
        newAmount = Mathf.Clamp01(newAmount);

        this.currentAmount = newAmount;
        this.barImg.fillAmount = this.currentAmount;

        switch (this.state)
        {
            case ChipbarState.INITIAL:

                this.flashImg.fillAmount = this.initialAmount;
                this.state = ChipbarState.BLEND_IN;
                this.blendInRoutine = this.StartCoroutine(this.BlendInFlash());
                break;
            case ChipbarState.BLEND_IN:
                break;
            case ChipbarState.BLEND_OUT:
                this.StopCoroutine(this.blendOutRoutine);

                this.flashImg.fillAmount = this.initialAmount;
                this.state = ChipbarState.BLEND_IN;
                this.blendInRoutine = this.StartCoroutine(this.BlendInFlash());
                break;
        }
    }

}
