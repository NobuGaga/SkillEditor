using UnityEngine;
using System.Text;
using System.Collections.Generic;
using SkillEditor.LuaStructure;

namespace SkillEditor {

    internal static class LuaAnimClipModel {

        private static List<AnimClipData> m_listAnimClip = new List<AnimClipData>(Config.ModelCount);
        public static List<AnimClipData> AnimClipList => m_listAnimClip;

        private static int m_curIndex;

        public static string CurrentEditModel {
            set {
                string modelName = value;
                ModelName = modelName;
                if (m_curIndex == Config.ErrorIndex)
                    AddNewAnimClipData(modelName);
            }
        }

        private static void AddNewAnimClipData(string modelName) {
            AnimClipData data = new AnimClipData();
            data.modelName = modelName;
            m_listAnimClip.Add(data);
            m_curIndex = m_listAnimClip.Count - 1;
        }

        public static AnimClipData Data {
            set {
                if (m_curIndex == Config.ErrorIndex)
                    ModelName = value.modelName;
                if (m_curIndex == Config.ErrorIndex) {
                    m_listAnimClip.Add(value);
                    m_curIndex = m_listAnimClip.Count - 1;
                    return;
                }
                m_listAnimClip[m_curIndex] = value;
            }
            get {
                if (m_curIndex == Config.ErrorIndex)
                    Debug.LogError("AnimClipModel current index is error index");
                return m_listAnimClip[m_curIndex];
            }
        }

        public static string ModelName {
            set {
                for (int index = 0; index < m_listAnimClip.Count; index++)
                    if (m_listAnimClip[index].modelName == value)
                        m_curIndex = index;
            }
            get {
                return Data.modelName;
            }
        }

        public static string GetWriteFileString(StringBuilder builder) {
            builder.Append(LuaFormat.CurlyBracesPair.start);
            if (m_listAnimClip != null && m_listAnimClip.Count != 0) {
                builder.Append(LuaFormat.LineSymbol);
                for (int index = 0; index < m_listAnimClip.Count; index++)
                    builder.Append(m_listAnimClip[index].ToString());
            }
            builder.Append(LuaFormat.CurlyBracesPair.end);
            return builder.ToString();
        }

        public static void Reset() {
            m_curIndex = Config.ErrorIndex;
        }
    }
}