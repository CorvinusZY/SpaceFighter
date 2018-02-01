using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

namespace LaireonFramework
{
    public class GUIHelper
    {
        public const float ScrollBarWidth = 25f;//since these can be annoying when calculating placment

        public enum LayoutStyle
        {
            Defualt = 0, Box, Button, TextArea, Window, Textfield,
            HorizontalScrollbar,//Fixed height
            Label,//No Style
            Toggle, //Just puts a non usable CB to the left 
            Toolbar,//Fixed height
            PreToolbar2,
            scrollView
        }

        #region Styles
        public static GUIStyle boldLabel, centeredBoldLabel, centeredLabel;

        public static void InitialiseStyles()
        {
            boldLabel = new GUIStyle(EditorStyles.boldLabel);//not really needed, but cleaner to have all styles in the same place

            centeredBoldLabel = new GUIStyle(EditorStyles.boldLabel);
            centeredBoldLabel.alignment = TextAnchor.MiddleCenter;

            centeredLabel = new GUIStyle(EditorStyles.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
        }
        #endregion

        #region Input
        public static bool IsKeyDown(Event current, KeyCode code)
        {
            return current.type == EventType.KeyDown && current.keyCode == code;
        }
        #endregion

        static bool mouseDragging;

        public static void MoveScrollViewsWithMouseClicks(ref Vector2 scrollRect)
        {
            if(Event.current.type == EventType.MouseDrag && !mouseDragging)
                mouseDragging = true;
            else if(Event.current.type == EventType.MouseUp)
                mouseDragging = false;

            if(mouseDragging)
                scrollRect += Event.current.delta * -1;
        }

        public static bool DrawButtonWithImage(Sprite sprite, Vector2 imageSize, Vector2 buttonSize, GUIContent buttonContent,
            bool drawBox = true)
        {
            bool returnVal = false;

            using(new Horizontal())
            {
                if(GUILayout.Button(buttonContent, GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
                    returnVal = true;

                GUI.color = Color.white;

                DrawSprite(sprite, imageSize.x, imageSize.y, new Vector2(buttonSize.x - (buttonSize.x - imageSize.x) / 2 + 4, (imageSize.y - buttonSize.y) / 2 - 4), drawBox);
                GUILayout.Space(-Mathf.Max(imageSize.x, buttonSize.x));
            }

            return returnVal;
        }

        public static void DrawSprite(Sprite sprite, float width, float height, bool drawBox = true)
        {
            DrawTexture(sprite.texture, GetSpriteTextureRect(sprite.texture, sprite.textureRect), width, height, Vector2.zero, drawBox);
        }

        public static void DrawSprite(Sprite sprite, float width, float height, Vector2 offset, bool drawBox = true)
        {

            DrawTexture(sprite.texture, GetSpriteTextureRect(sprite.texture, sprite.textureRect), width, height, offset, drawBox);
        }

        /// <summary>
        /// Converts a sprites given texture rect to more relatable co-ords
        /// </summary>
        static Rect GetSpriteTextureRect(Texture2D texture, Rect textureRect)
        {
            return new Rect(textureRect.x / texture.width, textureRect.y / texture.height, textureRect.width / texture.width, textureRect.height / texture.height);
        }

        /// <summary>
        /// Draws a sprite in line with the current position of the editor layout
        /// </summary>
        public static Rect DrawTexture(Texture2D texture, Rect sourceRect, float width, float height, Vector2 offset, bool drawBox = true)
        {
            Rect position = GUILayoutUtility.GetRect(width, height);
            position.x -= offset.x;
            position.y -= offset.y;

            if(width > 0)//if there is no width set then just stretch the entire space
                position.width = width;//overrides a weird bug where the image is stretched to fill the space

            if(drawBox)
                GUI.Box(position, "");

            position.x += 2;
            position.y += 2;
            position.width -= 4;
            position.height -= 4;

            if(texture != null)
                GUI.DrawTextureWithTexCoords(position, texture, sourceRect);
            else
                GUI.Box(position, "");

            return position;
        }

        public static Texture2D GammaCorrectTexture(Texture2D texture)
        {
            if(PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                Color[] pixels = texture.GetPixels();

                for(int i = 0; i < pixels.Length; i++)
                {
                    pixels[i].r = Mathf.GammaToLinearSpace(pixels[i].r);
                    pixels[i].g = Mathf.GammaToLinearSpace(pixels[i].g);
                    pixels[i].b = Mathf.GammaToLinearSpace(pixels[i].b);

                    pixels[i].r = Mathf.GammaToLinearSpace(pixels[i].r);
                    pixels[i].g = Mathf.GammaToLinearSpace(pixels[i].g);
                    pixels[i].b = Mathf.GammaToLinearSpace(pixels[i].b);
                }

                texture.SetPixels(pixels);
                texture.Apply();
            }

            return texture;
        }

        public static void ArrayGUI(SerializedObject instance, string name)
        {
            ArrayGUI(instance, instance.FindProperty(name));
        }

        public static void ArrayGUI(SerializedObject instance, SerializedProperty array)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(array, true);

            if(EditorGUI.EndChangeCheck())
                instance.ApplyModifiedProperties();
        }

        public static void DrawToggle(ref AnimBool animation, GUIContent content)
        {
            if(animation == null)
                animation = new AnimBool();

            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.85f);//change the colour of the heading to get it to stand out

            if(!GUILayout.Toggle(true, content, "PreToolbar2", GUILayout.MinWidth(20f)))
                animation.target = !animation.target;//invert

            GUI.contentColor = Color.white;
        }

        public static void DrawCenteredToggle(ref AnimBool animation, GUIContent content)
        {
            using(new Horizontal())
            using(new Centered())
                DrawToggle(ref animation, content);
        }

        /// <summary>
        /// Removes an element from an array and shifts the other values up to replace it
        /// </summary>
        public static void RemoveElementFromArray(SerializedProperty mainProperty, int index)
        {
            for(int i = index; i < mainProperty.arraySize - 1; i++)
                mainProperty.MoveArrayElement(i + 1, i);//shift all other array elements up

            mainProperty.arraySize--;//remove the last element reference and resize
        }
    }

    /// <summary>
    /// A helper to make buttons etc appear disabled
    /// </summary>
    public class DisableUI : IDisposable
    {
        static List<bool> effects = new List<bool>();

        public DisableUI(bool disabled)
        {
            if(effects.Count == 0 || GUI.enabled)//basically if a top level effect has disabled UI, don't overwrite it
                GUI.enabled = !disabled;

            effects.Add(!disabled);
        }

        public void Dispose()
        {
            effects.RemoveAt(effects.Count - 1);

            bool enable = true;

            for(int i = 0; i < effects.Count; i++)//respect heirarchy when disabling effects
                if(!effects[i])
                    enable = false;

            GUI.enabled = enable;

        }
    }

    #region Colour Change
    /// <summary>
    /// A helper to make colour changes easier
    /// </summary>
    public class ColourChange : IDisposable
    {
        Color previousColour = Color.white;

        public ColourChange(Color colour, bool change)
        {
            if(change)
            {
                previousColour = GUI.color;
                GUI.color = colour;
            }
        }

        public ColourChange(Color colour)
        {
            previousColour = GUI.color;
            GUI.color = colour;
        }

        public void Dispose()
        {
            GUI.color = previousColour;
        }
    }
    #endregion

    #region Horizontal
    /// <summary>
    /// A helper to make horizontal groups easier
    /// </summary>
    public class Horizontal : IDisposable
    {
        public Horizontal()
        {
            EditorGUILayout.BeginHorizontal();
        }

        public Horizontal(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
        }

        public Horizontal(GUIHelper.LayoutStyle style)
        {
            EditorGUILayout.BeginHorizontal(style.ToString());
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }
    #endregion

    /// <summary>
    /// A helper to make horizontal groups easier
    /// </summary>
    public class LoopingHorizontal : IDisposable
    {
        int index;
        int maxValue;
        bool addFlexibleSpace;

        public LoopingHorizontal(int index, int loopCount, int maxValue, int spacing)
        {
            Initialise(index, loopCount, maxValue, spacing, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="loopCount"></param>
        /// <param name="maxValue"></param>
        /// <param name="spacing">Set this to -1 to use flexible space!</param>
        public LoopingHorizontal(int index, int loopCount, int maxValue, int spacing, bool addFlexibleSpace)
        {
            Initialise(index, loopCount, maxValue, spacing, addFlexibleSpace);
        }

        public void Initialise(int index, int loopCount, int maxValue, int spacing, bool addFlexibleSpace)
        {
            this.index = index;
            this.maxValue = maxValue;
            this.addFlexibleSpace = addFlexibleSpace;

            loopCount = Mathf.Max(1, loopCount);//don't try to divide by 0!

            if(index == 0)
                EditorGUILayout.BeginHorizontal();
            else if(index % loopCount == 0)
            {
                EditorGUILayout.EndHorizontal();

                if(addFlexibleSpace)
                    GUILayout.FlexibleSpace();

                if(spacing > 0)
                    GUILayout.Space(spacing);

                EditorGUILayout.BeginHorizontal();
            }

        }

        public void Dispose()
        {
            if(index == maxValue)
            {
                EditorGUILayout.EndHorizontal();

                if(addFlexibleSpace)
                    GUILayout.FlexibleSpace();
            }
        }
    }

    /// <summary>
    /// A helper to make Verticle groups easier
    /// </summary>
    public class LoopingVerticle : IDisposable
    {
        int index;
        int maxValue;

        public LoopingVerticle(int index, int loopCount, int maxValue, int spacing)
        {
            this.index = index;
            this.maxValue = maxValue;

            if(index == 0)
                EditorGUILayout.BeginVertical();
            else if(index % loopCount == 0)
            {
                EditorGUILayout.EndVertical();
                GUILayout.Space(spacing);
                EditorGUILayout.BeginVertical();
            }

        }

        public void Dispose()
        {
            if(index == maxValue)
                EditorGUILayout.EndVertical();
        }
    }

    /// <summary>
    /// A helper to make vertical groups easier
    /// </summary>
    public class Vertical : IDisposable
    {
        public Vertical()
        {
            EditorGUILayout.BeginVertical();
        }

        public Vertical(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
        }

        public Vertical(GUIHelper.LayoutStyle style)
        {
            EditorGUILayout.BeginVertical(style.ToString());
        }

        public Vertical(GUIHelper.LayoutStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style.ToString(), options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }

    public class Centered : IDisposable
    {
        public Centered()
        {
            GUILayout.FlexibleSpace();
        }

        public void Dispose()
        {
            GUILayout.FlexibleSpace();
        }
    }


    //FixedWidthLabel class. Extends IDisposable, so that it can be used with the "using" keyword.
    public class FixedWidthLabel : IDisposable
    {
        private readonly ZeroIndent indentReset; //helper class to reset and restore indentation

        public FixedWidthLabel(GUIContent label, GUIStyle style, params GUILayoutOption[] options)//	constructor.
        {//						state changes are applied here.
            EditorGUILayout.BeginHorizontal(options);// create a new horizontal group

            EditorGUILayout.LabelField(label, style,
                GUILayout.Width(GUI.skin.label.CalcSize(label).x +// actual label width
                    9 * EditorGUI.indentLevel));//indentation from the left side. It's 9 pixels per indent level

            indentReset = new ZeroIndent();//helper class to have no indentation after the label
        }

        public FixedWidthLabel(GUIContent label, params GUILayoutOption[] options)//	constructor.
        {//						state changes are applied here.
            EditorGUILayout.BeginHorizontal(options);// create a new horizontal group

            EditorGUILayout.LabelField(label,
                GUILayout.Width(GUI.skin.label.CalcSize(label).x +// actual label width
                    9 * EditorGUI.indentLevel));//indentation from the left side. It's 9 pixels per indent level

            indentReset = new ZeroIndent();//helper class to have no indentation after the label
        }

        public FixedWidthLabel(string label)
            : this(new GUIContent(label))//alternative constructor, if we don't want to deal with GUIContents
        {
        }

        public void Dispose() //restore GUI state
        {
            indentReset.Dispose();//restore indentation
            EditorGUILayout.EndHorizontal();//finish horizontal group
        }
    }

    public class ZeroIndent : IDisposable //helper class to clear indentation
    {
        private readonly int originalIndent;//the original indentation value before we change the GUI state
        public ZeroIndent()
        {
            originalIndent = EditorGUI.indentLevel;//save original indentation
            EditorGUI.indentLevel = 0;//clear indentation
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = originalIndent;//restore original indentation
        }
    }

    #region Space
    /// <summary>
    /// A helper to add space before and after a section in editors
    /// </summary>
    public class Padding : IDisposable
    {
        int space;

        public Padding(int space)
        {
            this.space = space;
            GUILayout.Space(space);
        }

        public void Dispose()
        {
            GUILayout.Space(space);
        }
    }
    #endregion
}