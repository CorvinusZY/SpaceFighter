using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using LaireonFramework;

[CustomEditor(typeof(IconProgressBar))]
public class IconProgressBarEditor : Editor
{
    SerializedProperty backingImage, mainImage;
    protected IconProgressBar current;

    protected void OnEnable()
    {
        backingImage = serializedObject.FindProperty("backingImage");
        mainImage = serializedObject.FindProperty("mainImage");
    }

    public override void OnInspectorGUI()
    {
        #region Initialise
        serializedObject.Update();
        current = (IconProgressBar)target;
        #endregion

        DrawUI(false);

        if(GUI.changed)
            EditorUtility.SetDirty(target);

        serializedObject.ApplyModifiedProperties();// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
    }

    protected void DrawUI(bool disableFactions)
    {
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(backingImage, new GUIContent("Backing Image"));

        EditorGUILayout.PropertyField(mainImage, new GUIContent("Main Image"));

        if(mainImage.objectReferenceValue != null && backingImage.objectReferenceValue != null)
        {
            current.MaxIcons = EditorGUILayout.IntField(new GUIContent("Max Icons"), current.MaxIcons);
            current.Padding = EditorGUILayout.FloatField(new GUIContent("Padding"), current.Padding);

            GUILayout.Space(10);

            current.maxValue = EditorGUILayout.FloatField(new GUIContent("Max Value"), current.maxValue);
            current.currentValue = EditorGUILayout.FloatField(new GUIContent("Current Value"), current.currentValue);

            GUILayout.Space(10);

            using(new DisableUI(disableFactions))
                current.showFractions = EditorGUILayout.Toggle(new GUIContent("Show Fractions", "If true then you will see the last icon being clipped. Otherwise only shows full icons"), current.showFractions);

            if(Application.isEditor)
                if(GUILayout.Button("Clear Un-Used Images"))
                    current.ClearInactive();
        }
    }
}
