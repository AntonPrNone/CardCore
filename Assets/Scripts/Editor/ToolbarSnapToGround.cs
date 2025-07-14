#if UNITY_2021_1_OR_NEWER
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Snap ↓")]
public class SnapToolbarOverlay : Overlay
{
    enum SnapMode
    {
        Renderer,
        Collider
    }

    private SnapMode currentMode = SnapMode.Renderer;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        root.style.paddingTop = 4;
        root.style.paddingBottom = 4;
        root.style.paddingLeft = 6;
        root.style.paddingRight = 6;
        root.style.minWidth = 120;

        // Переключатель режима
        var modeField = new EnumField("Режим", currentMode);
        modeField.RegisterValueChangedCallback(evt =>
        {
            currentMode = (SnapMode)evt.newValue;
        });
        root.Add(modeField);

        // Кнопка Snap ↓
        var snapButton = new Button(() =>
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (currentMode == SnapMode.Renderer)
                    SnapToGround.Snap(obj, useCollider: false);
                else
                    SnapToGround.Snap(obj, useCollider: true);
            }
        });

        snapButton.text = "Snap ↓";
        snapButton.tooltip = "Привязать к полу (по выбранному режиму)";
        root.Add(snapButton);

        return root;
    }
}
#endif
