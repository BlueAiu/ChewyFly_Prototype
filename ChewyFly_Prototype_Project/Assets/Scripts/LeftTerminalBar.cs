using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar
{
    public float baseX;
    public float maxWidth;
    public float maxScore;
    public RectTransform rectTransform;

    public Bar(float maxScore, RectTransform rectTransform)
    {
        this.maxScore = maxScore;
        this.rectTransform = rectTransform;

        this.baseX = rectTransform.localPosition.x - rectTransform.sizeDelta.x / 2;
        this.maxWidth = rectTransform.sizeDelta.x;
    }

    public void Set(float score)
    {
        score = Mathf.Clamp(score, 0, maxScore);
        var width = maxWidth * (score / maxScore);

        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        rectTransform.localPosition = new Vector2(baseX + width / 2, rectTransform.localPosition.y);
    }
}
