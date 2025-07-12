using UnityEngine;
using DG.Tweening;
using TMPro;

public class FeedbackTextAnimator
{
    private readonly TextMeshProUGUI feedbackText; // Ссылка на TMP текст
    private readonly float animationDuration; // Длительность анимации
    private readonly float riseDistance; // На сколько пикселей текст поднимается
    private readonly float textYOffset; // Смещение по Y от позиции клика
    private Sequence feedbackSequence; // Для отслеживания текущей анимации

    public FeedbackTextAnimator(TextMeshProUGUI feedbackText, float animationDuration, float riseDistance, float textYOffset)
    {
        this.feedbackText = feedbackText;
        this.animationDuration = animationDuration;
        this.riseDistance = riseDistance;
        this.textYOffset = textYOffset;

        // Убедимся, что текст изначально выключен
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    public void PlayAnimation(Vector2 mousePos)
    {
        if (feedbackText == null)
        {
            Debug.LogWarning("Feedback Text не назначен!");
            return;
        }

        // Проверяем, активна ли текущая анимация
        if (feedbackSequence != null && feedbackSequence.IsActive())
        {
            return; // Если анимация уже идёт, не запускаем новую
        }

        // Активируем текст
        feedbackText.gameObject.SetActive(true);

        // Сохраняем оригинальные настройки
        Vector2 originalAnchorMin = feedbackText.rectTransform.anchorMin;
        Vector2 originalAnchorMax = feedbackText.rectTransform.anchorMax;
        Vector2 originalPivot = feedbackText.rectTransform.pivot;
        Vector2 originalOffsetMin = feedbackText.rectTransform.offsetMin;
        Vector2 originalOffsetMax = feedbackText.rectTransform.offsetMax;

        // Устанавливаем pivot для точного позиционирования по Y, не трогая X
        feedbackText.rectTransform.pivot = new Vector2(originalPivot.x, 0.5f);

        // Сбрасываем Left/Right (offsetMin.x/offsetMax.x) на 0, чтобы сохранить растяжение
        feedbackText.rectTransform.offsetMin = new Vector2(0, feedbackText.rectTransform.offsetMin.y);
        feedbackText.rectTransform.offsetMax = new Vector2(0, feedbackText.rectTransform.offsetMax.y);

        // Преобразуем экранные координаты мыши в локальные координаты канваса
        Canvas canvas = feedbackText.canvas;
        Vector2 canvasPos;
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, mousePos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out localPoint);

            // Учитываем масштабирование канваса
            canvasPos = localPoint / canvas.scaleFactor;

            // Добавляем смещение по Y
            canvasPos.y += textYOffset;

            // Ограничиваем позицию по Y с учётом высоты текста и канваса
            float textHeight = feedbackText.rectTransform.sizeDelta.y / canvas.scaleFactor;
            float canvasHeight = canvasRect.sizeDelta.y / canvas.scaleFactor;
            canvasPos.y = Mathf.Clamp(canvasPos.y, -canvasHeight / 2 + textHeight / 2, canvasHeight / 2 - textHeight / 2);
        }
        else
        {
            canvasPos = mousePos + new Vector2(0, textYOffset);
        }

        // Устанавливаем позицию только по Y, сохраняя растяжение по X
        feedbackText.rectTransform.anchoredPosition = new Vector2(feedbackText.rectTransform.anchoredPosition.x, canvasPos.y);

        // Сбрасываем начальную прозрачность
        Color textColor = feedbackText.color;
        textColor.a = 0f;
        feedbackText.color = textColor;

        // Создаём последовательность анимации с DOTween
        feedbackSequence = DOTween.Sequence();
        feedbackSequence.Append(feedbackText.DOFade(1f, animationDuration * 0.4f)) // Появление (40% времени)
                       .Join(feedbackText.rectTransform.DOAnchorPosY(canvasPos.y + riseDistance / canvas.scaleFactor, animationDuration * 0.8f).SetEase(Ease.OutQuad)) // Подъём
                       .Append(feedbackText.DOFade(0f, animationDuration * 0.4f)) // Затухание
                       .OnComplete(() =>
                       {
                           feedbackText.gameObject.SetActive(false);
                           // Восстанавливаем оригинальные настройки
                           feedbackText.rectTransform.anchorMin = originalAnchorMin;
                           feedbackText.rectTransform.anchorMax = originalAnchorMax;
                           feedbackText.rectTransform.pivot = originalPivot;
                           feedbackText.rectTransform.offsetMin = originalOffsetMin;
                           feedbackText.rectTransform.offsetMax = originalOffsetMax;
                           feedbackSequence = null; // Сбрасываем ссылку на анимацию
                       }); // Выключаем текст и восстанавливаем настройки
    }
}