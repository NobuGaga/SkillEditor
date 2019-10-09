using UnityEngine;
using System.Collections.Generic;

namespace SkillEditor {

    internal static class KeyFrameModel {

        private static List<KeyFrameData> m_curFrameDataList = new List<KeyFrameData>();

        private static int m_curIndex;
        private const short ErrorIndex = -1;

        public static void Init(string modelName){

        }

        public static KeyFrameData Data {
            set {
                if (m_curIndex == ErrorIndex)
                    ModelName = value.modelName;
                if (m_curIndex == ErrorIndex) {
                    m_curFrameDataList.Add(value);
                    m_curIndex = m_curFrameDataList.Count - 1;
                    return;
                }
                m_curFrameDataList[m_curIndex] = value;
            }
            get {
                if (m_curIndex == ErrorIndex)
                    Debug.LogError("KeyFrameModel current index is error index");
                return m_curFrameDataList[m_curIndex];
            }
        }

        public static string ModelName {
            set {
                for (int index = 0; index < m_curFrameDataList.Count; index++)
                    if (m_curFrameDataList[index].modelName == value)
                        m_curIndex = index;
            }
            get {
                return Data.modelName;
            }
        }

        public static void Reset() {
            m_curIndex = ErrorIndex;
        }
    }
}