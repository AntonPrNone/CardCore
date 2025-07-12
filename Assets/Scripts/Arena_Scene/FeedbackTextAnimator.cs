using UnityEngine;
using DG.Tweening;
using TMPro;

public class FeedbackTextAnimator
{
    private readonly TextMeshProUGUI feedbackText;
    private readonly float animationDuration;
    private readonly float riseDistance;
    private readonly float textYOffset;
    private Sequence feedbackSequence;

    public FeedbackTextAnimator(TextMeshProUGUI feedbackText, float animationDuration, float riseDistance, float textYOffset)
    {
        this.feedbackText = feedbackText;
        this.animationDuration = animationDuration;
        this.riseDistance = riseDistance;
        this.textYOffset = textYOffset;

        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    public void PlayAnimation(Vector2 mousePos, string message = "Выберите юнит")
    {
        if (feedbackText == null)
        {
            Debug.LogWarning("Feedback Text не назначен!");
            return;
        }

        if (feedbackSequence != null && feedbackSequence.IsActive())
        {
            return;
        }

        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;

        Vector2 originalAnchorMin = feedbackText.rectTransform.anchorMin;
        Vector2 originalAnchorMax = feedbackText.rectTransform.anchorMax;
        Vector2 originalPivot = feedbackText.rectTransform.pivot;
        Vector2 originalOffsetMin = feedbackText.rectTransform.offsetMin;
        Vector2 originalOffsetMax = feedbackText.rectTransform.offsetMax;

        feedbackText.rectTransform.pivot = new Vector2(originalPivot.x, 0.5f);
        feedbackText.rectTransform.offsetMin = new Vector2(0, feedbackText.rectTransform.offsetMin.y);
        feedbackText.rectTransform.offsetMax = new Vector2(0, feedbackText.rectTransform.offsetMax.y);

        Canvas canvas = feedbackText.canvas;
        Vector2 canvasPos;
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, mousePos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out localPoint);

            canvasPos = localPoint / canvas.scaleFactor;
            canvasPos.y += textYOffset;

            float textHeight = feedbackText.rectTransform.sizeDelta.y / canvas.scaleFactor;
            float canvasHeight = canvasRect.sizeDelta.y / canvas.scaleFactor;
            canvasPos.y = Mathf.Clamp(canvasPos.y, -canvasHeight / 2 + textHeight / 2, canvasHeight / 2 - textHeight / 2);
        }
        else
        {
            canvasPos = mousePos + new Vector2(0, textYOffset);
        }

        feedbackText.rectTransform.anchoredPosition = new Vector2(feedbackText.rectTransform.anchoredPosition.x, canvasPos.y);

        Color textColor = feedbackText.color;
        textColor.a = 0f;
        feedbackText.color = textColor;

        feedbackSequence = DOTween.Sequence();
        feedbackSequence.Append(feedbackText.DOFade(1f, animationDuration * 0.4f))
                       .Join(feedbackText.rectTransform.DOAnchorPosY(canvasPos.y + riseDistance / canvas.scaleFactor, animationDuration * 0.8f).SetEase(Ease.OutQuad))
                       .Append(feedbackText.DOFade(0f, animationDuration * 0.4f))
                       .OnComplete(() =>
                       {
                           feedbackText.gameObject.SetActive(false);
                           feedbackText.rectTransform.anchorMin = originalAnchorMin;
                           feedbackText.rectTransform.anchorMax = originalAnchorMax;
                           feedbackText.rectTransform.pivot = originalPivot;
                           feedbackText.rectTransform.offsetMin = originalOffsetMin;
                           feedbackText.rectTransform.offsetMax = originalOffsetMax;
                           feedbackSequence = null;
                       });
    }
}