using UnityEngine;
using System;
using System.Text;
using System.Diagnostics;
using UGUIEditor;

public static class Shell {

    private static string ShellPath = Tool.GetCacheString(Application.dataPath + 
#if UNITY_EDITOR_WIN
    "\\Editor\\Script\\.Bat\\"
#elif UNITY_EDITOR_OSX
   "/Editor/Script/.Shell/"
#else
    string.Empty
#endif
    );

    private static StringBuilder m_builder = new StringBuilder();

    public static void Run(string shellName, params object[] args) {
        string shellPath = ShellPath + shellName +
#if UNITY_EDITOR_WIN
    ".bat"
#elif UNITY_EDITOR_OSX
   ".sh"
#else
    string.Empty
#endif
        ;
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = shellPath;
        if (args != null && args.Length > 0) {
            m_builder.Clear();
            for (ushort index = 0; index < args.Length; index++) {
                m_builder.Append(args[index]);
                m_builder.Append(',');
            }
            m_builder.Remove(m_builder.Length - 1, 1);
            info.Arguments = m_builder.ToString();
        }
        Process process = new Process();
        process.StartInfo = info;
        try {
            process.Start();
            process.WaitForExit();
        } catch (Exception exception) {
            UnityEngine.Debug.LogError("Shell::Run exception " + exception + "\nshell name " + shellPath);
        } finally {
            process.Close();
        }
    }
}