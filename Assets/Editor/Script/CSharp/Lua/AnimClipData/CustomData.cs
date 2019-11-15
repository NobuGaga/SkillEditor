using System;
using System.Text;
using System.Collections.Generic;

namespace Lua.AnimClipData {

    public struct CustomData<T> : IRepeatKeyTable<T> where T : ITable {
        
        public T[] dataList;

        public CustomData(T[] dataList) {
            this.dataList = dataList;
        }

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
        public override string ToString() => LuaTable.GetRepeatKeyTableText(m_staticBuilder, this);
        #endregion
    
        #region IRepeatKeyTable Function
        public Type GetTableListType() => typeof(T);
        private static List<T> m_listCache = new List<T>((ushort)Math.Pow(2, 1));
        public List<T> GetStaticCacheList() => m_listCache;
        public void SetTableList() => dataList = m_listCache.ToArray();
        public T[] GetTableList() => dataList;
        #endregion
    }
}