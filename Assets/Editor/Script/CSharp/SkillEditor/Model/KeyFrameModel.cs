using UnityEngine;
using System.Collections.Generic;
using SkillEditor.Structure;

namespace SkillEditor {

    internal static class KeyFrameModel {

        private static List<KeyFrameData> m_curFrameDataList;

        private static int m_curIndex;

        public static void Init(List<KeyFrameData> list, string modelName){
            if (m_curFrameDataList == null)
                m_curFrameDataList = list;
            ModelName = modelName;
            if (m_curIndex == Config.ErrorIndex)
                AddNewKeyFrameData(modelName);
        }

        private static void AddNewKeyFrameData(string modelName) {
            KeyFrameData data = new KeyFrameData();
            data.modelName = modelName;
            m_curFrameDataList.Add(data);
            m_curIndex = m_curFrameDataList.Count - 1;
        }

        public static KeyFrameData Data {
            set {
                if (m_curIndex == Config.ErrorIndex)
                    ModelName = value.modelName;
                if (m_curIndex == Config.ErrorIndex) {
                    m_curFrameDataList.Add(value);
                    m_curIndex = m_curFrameDataList.Count - 1;
                    return;
                }
                m_curFrameDataList[m_curIndex] = value;
            }
            get {
                if (m_curIndex == Config.ErrorIndex)
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
            m_curIndex = Config.ErrorIndex;
        }
    }
}