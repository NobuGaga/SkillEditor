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

    protected void SetTextColor(Color color) => GUI.contentColor = color;

    protected void Label(string text) => GUILayout.Label(text);

    protected void SpaceWithLabel(string text, float space = DefaultSpace) {
        Space(space);
        Label(text);
    }

    protected short TextField(short shortNumber) {
        string shortNumberString = TextField(shortNumber.ToString());
        if (short.TryParse(shortNumberString, out short result))
            return result;
        return shortNumber;
    }

    protected int TextField(int interge) {
        string intString = TextField(interge.ToString());
        if (int.TryParse(intString, out int result))
            return result;
        return interge;
    }

    protected uint TextField(uint interge) {
        string intString = TextField(interge.ToString());
        if (uint.TryParse(intString, out uint result))
            return result;
        return interge;
    }

    protected float TextField(float number) {
        string floatString = TextField(number.ToString());
        if (float.TryParse(floatString, out float result))
            return result;
        return number;
    }

    protected string TextField(string text) => EditorGUILayout.TextField(text);

    protected bool Button(string buttonName, Style style = Style.PreButton) => GUILayout.Button(buttonName, GetGUIStyle(style));

    protected bool SpaceWithButton(string buttonName, Style style = Style.PreButton, float space = DefaultSpace) {
        Space(space);
        return Button(buttonName, style);
    }

    protected int IntPopup(int selectIndex, string[] arrayText, int[] arrayIndex, Style style = Style.PreDropDown) =>
                                        EditorGUILayout.IntPopup(selectIndex, arrayText, arrayIndex, GetGUIStyle(style));

    protected Enum EnumPopup(Enum enumType, Style style = Style.PreDropDown) => EditorGUILayout.EnumPopup(enumType, GetGUIStyle(style));

    protected void FlexibleSpace() => GUILayout.FlexibleSpace();

    protected void Space(float space = DefaultSpace) => GUILayout.Space(space);

    protected float Slider(float current, float maxValue) => EditorGUILayout.Slider(current, 0, maxValue);

    protected void FadeLayoutUI(Action uiFunction, float value) {
        EditorGUILayout.BeginFadeGroup(value);
        uiFunction();
        EditorGUILayout.EndFadeGroup();
    }

    protected void BeginHorizontal(Layout layout) {
        EditorGUILayout.BeginHorizontal();
        if (layout != Layout.Left)
            FlexibleSpace();
    }

    protected void EndHorizontal(Layout layout) {
        if (layout != Layout.Right)
            FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    protected void HorizontalLayoutUI(Action uiFunction, Layout layout = Layout.Left) {
        BeginHorizontal(layout);
        uiFunction();
        EndHorizontal(layout);
    }

    protected void HorizontalLayoutUI(Action<int> uiFunction, int data, Layout layout = Layout.Left) {
        BeginHorizontal(layout);
        uiFunction(data);
        EndHorizontal(layout);
    }

    protected bool HorizontalLayoutUI(Func<int, bool> uiFunction, int data, Layout layout = Layout.Left) {
        BeginHorizontal(layout);
        bool result = uiFunction(data);
        EndHorizontal(layout);
        return result;
    }

    protected bool HorizontalLayoutUI(Func<int, object, bool> uiFunction, int index, object data, Layout layout = Layout.Left) {
        BeginHorizontal(layout);
        bool result = uiFunction(index, data);
        EndHorizontal(layout);
        return result;
    }

    protected void BeginVertical(Layout layout = Layout.Top) {
        EditorGUILayout.BeginVertical();
        if (layout != Layout.Top)
            FlexibleSpace();
    }

    protected void EndVertical(Layout layout = Layout.Top) {
        if (layout != Layout.Bottom)
            FlexibleSpace();
        EditorGUILayout.EndVertical();
    }

    protected Vector2 BeginVerticalScrollView(Vector2 pos) => EditorGUILayout.BeginScrollView(pos, false, false);

    protected void EndVerticalScrollView() {
        EditorGUILayout.EndScrollView();
    }

    protected void VerticalLayoutUI(Action uiFunction, Layout layout = Layout.Top) {
        BeginVertical(layout);
        uiFunction();
        EndVertical(layout);
    }

    protected void VerticalLayoutUI(Action<int> uiFunction, int index, Layout layout = Layout.Top) {
        BeginVertical(layout);
        uiFunction(index);
        EndVertical(layout);
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