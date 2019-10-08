using UnityEditor;
using UnityEngine;
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

    public static string ProjectPathToFullPath(string projectPath) {
        return string.Format("{0}{1}", SkillEditorConfig.ProjectPath, projectPath);
    }

    public static string CombinePath(string path1, string path2) {
        if (path1 == null || path1.Length == 0 || path2 == null || path2.Length == 0) {
            Debug.LogError("SkillEditorConfig::CombinePath argument error");
            return string.Empty;
        }
        string format;
        char flag = '/';
        bool isPath1LastFormat = path1[path1.Length - 1] == flag;
        bool isPath2StartFormat = path2[0] == flag;
        if (isPath1LastFormat && isPath2StartFormat)
            return string.Format("{0}{1}", path1.Substring(0, path1.Length - 2), path2);
        else if (!isPath1LastFormat && !isPath2StartFormat)
            format = "{0}/{1}";
        else
            format = "{0}{1}";
        return string.Format(format, path1, path2);
    }

    public static string CombineFilePath(string path, string fileName, string extension = null) {
        if (extension == null)
            return CombinePath(path, fileName);
        return CombinePath(path, FileWithExtension(fileName, extension));
    }
}