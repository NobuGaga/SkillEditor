using System;
using System.Text;

namespace Lua.AnimClipData {

    public struct CustomData<T> : IRepeatKeyTable<T> where T : ITable {
        
        public T[] dataList;

        public CustomData(T[] dataList) {
            this.dataList = dataList;
        }

        #region ITable Function
        public string GetTableName() => "CustomData";
        public ushort GetLayer() => 6;
        public KeyType GetKeyType() => KeyType.Reference;
        public void SetKey(object key) { }
        public string GetKey() => "data";
        public bool IsNullTable() => dataList == null || dataList.Length == 0;
        public void Clear() => dataList = null;

        private static StringBuilder m_staticBuilder = new StringBuilder((UInt16)Math.Pow(2, 9));
        public override string ToString() => LuaTable.GetRepeatKeyTableText(m_staticBuilder, this);
        #endregion
    
        #region IRepeatKeyTable Function
        public T[] GetTableList() => dataList;
        #endregion
    }
}