using System.Text;
using SkillEditor;

namespace Lua {

    public struct AnimClipData : ITable {
        public string modelName;
        public StateData[] stateList;

        public AnimClipData(string modelName, StateData[] stateList) {
            this.modelName = modelName;
            this.stateList = stateList;
        }

        public ushort GetLayer() => 1;
        public KeyType GetKeyType() => KeyType.String;
        public bool IsNullTable() => modelName == null || modelName == string.Empty || stateList == null;

        private static StringBuilder m_staticBuilder = new StringBuilder(16384);
        public override string ToString() {
            if (stateList == null)
                return string.Empty;
            m_staticBuilder.Clear();
            for (int index = 0; index < stateList.Length; index++)
                m_staticBuilder.Append(stateList[index].ToString());
            string stateListString = m_staticBuilder.ToString();
            string tabString = LuaTable.GetTabString(GetLayer());
            string format = "{0}[\"{1}\"] = {2}\n{3}{0}{4},\n";
            string toString = string.Format(format, tabString,
                                            modelName,
                                            LuaFormat.CurlyBracesPair.start,
                                            stateListString,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
        }
    }
}