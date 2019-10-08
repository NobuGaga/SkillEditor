using UnityEditor;
using System;
using System.Reflection;

public static class SkillEditorTool {

    private static MethodInfo m_clearConsoleMethod = null;

    public static void ClearConsole() {
        if (m_clearConsoleMethod == null) {
            Type logClass = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
            m_clearConsoleMethod = logClass.GetMethod("Clear");
        }
        m_clearConsoleMethod.Invoke(null, null);
    }

    public static string FileWithExtension(string fileName, string extension) {
        return string.Format("{0}.{1}", fileName, extension);
    }

    public static string FullPathToProjectPath(string fullPath) {
        int subIndex = fullPath.IndexOf("Assets/", StringComparison.Ordinal);
        return fullPath.Substring(subIndex);
    }
}