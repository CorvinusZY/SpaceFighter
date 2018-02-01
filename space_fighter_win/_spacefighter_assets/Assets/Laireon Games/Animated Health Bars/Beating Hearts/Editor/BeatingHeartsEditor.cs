using UnityEngine;
using System.Collections;
using UnityEditor;

namespace LaireonFramework
{
    [CustomEditor(typeof(BeatingHealthBar))]
    public class BeatingHeartsEditor : IconProgressBarEditor
    {
        SerializedProperty minHeartSize, maxHeartSize,
            minBeatingTime, maxBeatingTime,
            yPivot, heartPrefab;

        new void OnEnable()
        {
            base.OnEnable();

            minHeartSize = serializedObject.FindProperty("minHeartSize");
            maxHeartSize = serializedObject.FindProperty("maxHeartSize");

            minBeatingTime = serializedObject.FindProperty("minBeatingTime");
            maxBeatingTime = serializedObject.FindProperty("maxBeatingTime");

            yPivot = serializedObject.FindProperty("yPivot");

            heartPrefab = serializedObject.FindProperty("heartPrefab");
        }

        public override void OnInspectorGUI()
        {
            #region Initialise
            serializedObject.Update();
            current = (BeatingHealthBar)target;
            #endregion

            EditorGUILayout.PropertyField(heartPrefab, new GUIContent("Heart Prefab", "The prefab for each heart icon"));

            if(heartPrefab.objectReferenceValue != null)
            {
                DrawUI(true);//draw the base
                current.showFractions = true;//force showing fractions. Doesn't actually do anything but easier to explain to users when viewing the other inspector 

                EditorGUILayout.PropertyField(minHeartSize, new GUIContent("Min Heart Size", "This is how small a heart will shrink to. It will generally shrink according to its current value"));
                EditorGUILayout.PropertyField(maxHeartSize, new GUIContent("Max Heart Size", "This is the max size of the heart relative to the current value. Think of this as the size the heart beats to when the current value is 0"));

                EditorGUILayout.PropertyField(minBeatingTime, new GUIContent("Min Beating Time", "The slowest speed the heart will beat at, seen when the current value is close to 1 "));
                EditorGUILayout.PropertyField(maxBeatingTime, new GUIContent("Max Beating Time", "The fastest speed the heart will beat at, seen when the current value is close to 0"));

                #region Pivot
                EditorGUILayout.PropertyField(yPivot, new GUIContent("Y Pivot", "This helps offset for any small transparent gaps at the bottom if your image"));

                ((BeatingHealthBar)current).UpdatePivot();//update the images with any new pivot values
                #endregion
            }

            if(GUI.changed)
                EditorUtility.SetDirty(target);

            serializedObject.ApplyModifiedProperties();// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        }
    }
}
