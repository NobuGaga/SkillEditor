using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public class BaseEditorWindow : EditorWindow {

    private static Dictionary<Style, GUIStyle> m_dicGUIStyle = new Dictionary<Style, GUIStyle>(8);
    private const float DefaultSpace = 10;

    protected static void Open<T>(string title) where T : BaseEditorWindow {
        GetWindow<T>(title).Show();
    }

    protected static GUIStyle GetGUIStyle(Style style) {
        if (m_dicGUIStyle.ContainsKey(style))
            return m_dicGUIStyle[style];
        GUIStyle guiStyle = new GUIStyle(style.ToString());
        m_dicGUIStyle.Add(style, guiStyle);
        return guiStyle;
    }

    protected void Label(string text) {
        GUILayout.Label(text);
    }

    protected void SpaceWithLabel(string text, float space = DefaultSpace) {
        Space(space);
        Label(text);
    }

    protected short TextField(short shortNumber) {
        string shortNumberString = EditorGUILayout.TextField(shortNumber.ToString());
        if (short.TryParse(shortNumberString, out short result))
            return result;
        return shortNumber;
    }

    protected int TextField(int interge) {
        string intString = EditorGUILayout.TextField(interge.ToString());
        if (int.TryParse(intString, out int result))
            return result;
        return interge;
    }

    protected float TextField(float number) {
        string floatString = EditorGUILayout.TextField(number.ToString());
        if (float.TryParse(floatString, out float result))
            return result;
        return number;
    }

    protected bool Button(string buttonName, Style style = Style.PreButton) {
        return GUILayout.Button(buttonName, GetGUIStyle(style));
    }

    protected bool SpaceWithButton(string buttonName, Style style = Style.PreButton, float space = DefaultSpace) {
        Space(space);
        return Button(buttonName, style);
    }

    protected int IntPopup(int selectIndex, string[] arrayText, int[] arrayIndex, Style style = Style.PreDropDown) {
        return EditorGUILayout.IntPopup(selectIndex, arrayText, arrayIndex, GetGUIStyle(style));
    }

    protected Enum EnumPopup(Enum enumType, Style style = Style.PreDropDown) {
        return EditorGUILayout.EnumPopup(enumType, GetGUIStyle(style));
    }

    protected void FlexibleSpace() {
        GUILayout.FlexibleSpace();
    }

    protected void Space(float space = DefaultSpace) {
        GUILayout.Space(space);
    }

    protected void FadeLayoutUI(Action uiFunction, float value) {
        EditorGUILayout.BeginFadeGroup(value);
        uiFunction();
        EditorGUILayout.EndFadeGroup();
    }

    protected void HorizontalLayoutUI(Action uiFunction, Layout layout = Layout.Left) {
        EditorGUILayout.BeginHorizontal();
        if (layout != Layout.Left)
            FlexibleSpace();
        uiFunction();
        if (layout != Layout.Right)
            FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    protected void HorizontalLayoutUI(Action<int> uiFunction, int index, Layout layout = Layout.Left) {
        EditorGUILayout.BeginHorizontal();
        if (layout != Layout.Left)
            FlexibleSpace();
        uiFunction(index);
        if (layout != Layout.Right)
            FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    protected object HorizontalLayoutUI(Func<object, object> uiFunction, object data, Layout layout = Layout.Left) {
        EditorGUILayout.BeginHorizontal();
        if (layout != Layout.Left)
            FlexibleSpace();
        object result = uiFunction(data);
        if (layout != Layout.Right)
            FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        return result;
    }

    protected void VerticalLayoutUI(Action uiFunction, Layout layout = Layout.Top) {
        EditorGUILayout.BeginVertical();
        if (layout != Layout.Top)
            FlexibleSpace();
        uiFunction();
        if (layout != Layout.Bottom)
            FlexibleSpace();
        EditorGUILayout.EndVertical();
    }

    protected void VerticalLayoutUI(Action<object> uiFunction, object data, Layout layout = Layout.Left) {
        EditorGUILayout.BeginVertical();
        if (layout != Layout.Left)
            FlexibleSpace();
        uiFunction(data);
        if (layout != Layout.Right)
            FlexibleSpace();
        EditorGUILayout.EndVertical();
    }

    protected void CenterLayoutUI(Action uiFunction) {
        EditorGUILayout.BeginHorizontal();
        FlexibleSpace();
        EditorGUILayout.BeginVertical();
        FlexibleSpace();
        uiFunction();
        FlexibleSpace();
        EditorGUILayout.EndVertical();
        FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    protected enum Layout {
        Left,
        Right,
        Center,
        Top,
        Bottom,
    }

    protected enum Style {
        PreButton,
        PreDropDown,
    }
}