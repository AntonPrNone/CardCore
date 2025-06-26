#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNamePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => Path.GetFileNameWithoutExtension(s.path))
            .ToArray();

        if (scenes.Length == 0)
        {
            EditorGUI.LabelField(position, "Нет сцен в Build Settings");
            return;
        }

        int currentIndex = Mathf.Max(0, System.Array.IndexOf(scenes, property.stringValue));
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, scenes);

        property.stringValue = scenes[selectedIndex];
    }
}
#endif
