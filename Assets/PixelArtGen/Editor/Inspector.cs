using UnityEditor;
using UnityEngine;

namespace PixelArtGen
{
    [CustomEditor(typeof(Generator))]
    [CanEditMultipleObjects]
    public class Inspector : Editor
    {
        SerializedProperty patternStringProperty;
        SerializedProperty isMirrorXProperty;
        SerializedProperty isMirrorYProperty;
        SerializedProperty seedProperty;
        SerializedProperty colorProperty;
        SerializedProperty isRotatedProperty;
        SerializedProperty textureProp;
        GUIStyle textAreaStyle = null;

        public void OnEnable()
        {
            patternStringProperty = serializedObject.FindProperty("patternString");
            isMirrorXProperty = serializedObject.FindProperty("isMirrorX");
            isMirrorYProperty = serializedObject.FindProperty("isMirrorY");
            seedProperty = serializedObject.FindProperty("seed");
            colorProperty = serializedObject.FindProperty("color");
            isRotatedProperty = serializedObject.FindProperty("isRotated");
            textureProp = serializedObject.FindProperty("texture");
        }

        public override void OnInspectorGUI()
        {
            if (textAreaStyle == null)
            {
                textAreaStyle = new GUIStyle(GUI.skin.GetStyle("TextArea"));
                textAreaStyle.font = Resources.Load("Fonts/Fake Receipt", typeof(Font)) as Font;
                textAreaStyle.fontSize = 11;
            }
            serializedObject.Update();
            patternStringProperty.stringValue =
                EditorGUILayout.TextArea(patternStringProperty.stringValue, textAreaStyle);
            isMirrorXProperty.boolValue = EditorGUILayout.Toggle("Mirror X", isMirrorXProperty.boolValue);
            isMirrorYProperty.boolValue = EditorGUILayout.Toggle("Mirror Y", isMirrorYProperty.boolValue);
            seedProperty.intValue = EditorGUILayout.IntField("Seed", seedProperty.intValue);
            colorProperty.colorValue = EditorGUILayout.ColorField("Color", colorProperty.colorValue);
            isRotatedProperty.boolValue = EditorGUILayout.Toggle("Rotated", isRotatedProperty.boolValue);
            Texture2D texture = textureProp.objectReferenceValue as Texture2D;
            if (texture != null)
            {
                GUILayout.Label(texture);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
