using CardMatch.CardMatch;
using UnityEditor;
using UnityEngine;

namespace CardMatch.Levels.Editor
{
    [CustomPropertyDrawer(typeof(LevelConfig))]
    public class LevelConfigDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty layout = property.FindPropertyRelative("Layout");
            SerializedProperty scoring = property.FindPropertyRelative("Scoring");

            float height = 0f;
            if (layout != null)
                height += EditorGUI.GetPropertyHeight(layout, true);
            if (layout != null && scoring != null)
                height += Spacing;
            if (scoring != null)
                height += EditorGUI.GetPropertyHeight(scoring, true);

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty layout = property.FindPropertyRelative("Layout");
            SerializedProperty scoring = property.FindPropertyRelative("Scoring");

            Rect rect = position;
            if (layout != null)
            {
                float h = EditorGUI.GetPropertyHeight(layout, true);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, h), layout, true);
                rect.y += h + Spacing;
            }
            if (scoring != null)
            {
                float h = EditorGUI.GetPropertyHeight(scoring, true);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, h), scoring, true);
            }
        }
    }
}
