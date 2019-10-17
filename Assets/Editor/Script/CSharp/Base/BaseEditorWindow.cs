using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public class BaseEditorWindow : EditorWindow {

    private static Dictionary<Style, GUIStyle> m_dicGUIStyle = new Dictionary<Style, GUIStyle>(8);
    private static Dictionary<string, ButtonData> m_dicButtonData = new Dictionary<string, ButtonData>(8);
    private const float DefaultSpace = 10;

    protected static void Open<T>(string title) where T : BaseEditorWindow {
        GetWindow<T>(title).Show();
    }

    protected static void CloseWindow<T>() where T : BaseEditorWindow {
        m_dicButtonData.Clear();
        GetWindow<T>().Close();
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

    protected static void AddButtonData(string name, Style style) {
        if (m_dicButtonData.ContainsKey(name))
            return;
        ButtonData data = new ButtonData(name, GetGUIStyle(style));
        m_dicButtonData.Add(data.Key, data);
    }

    protected bool Button(string buttonName) {
        if (!m_dicButtonData.ContainsKey(buttonName)) {
            Debug.LogError("BaseEditorWindow::Button button name not exit");
            return false;
        }
        ButtonData data = m_dicButtonData[buttonName];
        return GUILayout.Button(data.name, data.style);
    }

    protected void FlexibleSpace() {
        GUILayout.FlexibleSpace();
    }

    protected void Space(float space = DefaultSpace) {
        GUILayout.Space(space);
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

    protected struct ButtonData {
        public string name;
        public GUIStyle style;

        public string Key => name;

        public ButtonData(string name, GUIStyle style) {
            this.name = name;
            this.style = style;
        }
    }
}