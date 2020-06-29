using System;
using System.Text;
using System.Collections.Generic;

namespace Lua.AnimClipData {

    public struct CustomData<T> : IRepeatKeyTable<T> where T : ITable {
        
        public T[] dataList;

        #region ITable Function
        
        public string GetTableName() => "CustomData";
        public ushort GetLayer() => 6;
        public ReadType GetReadType() => ReadType.FixedToRepeat;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => "data";
        public bool IsNullTable() => dataList == null || dataList.Length == 0;
        public void Clear() => dataList = null;

        private static StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 9));
        public override string ToString() {
            var copy = this;
            if (dataList != null && dataList.Length > 0) {
                m_listCache.Clear();
                for (ushort index = 0; index < dataList.Length; index++) {
                    var table = dataList[index];
                    if (table.IsNullTable())
                        continue;
                    table.SetKey(m_listCache.Count + 1);
                    m_listCache.Add(table);
                }
                copy.dataList = m_listCache.ToArray();
            }
            return LuaTable.GetRepeatKeyTableText(m_staticBuilder, copy);
        }
        #endregion
    
        #region IRepeatKeyTable Function

        public Type GetTableListType() => typeof(T);
        private static List<T> m_listCache = new List<T>((ushort)Math.Pow(2, 1));
        public List<T> GetStaticCacheList() => m_listCache;
        public object SetTableList() {
            dataList = m_listCache.ToArray();
            return this;
        }
        public void SetTableListData(ushort index, T data) => dataList[index] = data;
        public T[] GetTableList() => dataList;
        #endregion
    }
}