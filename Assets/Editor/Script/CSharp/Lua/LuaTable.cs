using UnityEngine;
using System.Text;
using SkillEditor;

namespace Lua {

    internal static class LuaTable {

        public static string GetFieldKeyTableText(StringBuilder builder, IFieldKeyTable table) {
            if (table.IsNullTable())
                return string.Empty;
            builder.Clear();
            ushort layer = (ushort)(table.GetLayer() + 1);
            string tabString = GetTabString(layer);
            FieldKeyTable[] array = table.GetFieldKeyTables();
            for (int index = 0; index < array.Length; index++) {
                FieldKeyTable keyValue = array[index];
                object value = table.GetFieldKeyTableValue(keyValue.key);
                if (keyValue.type == ValueType.Table) {
                    ITable valueTable = value as ITable;
                    if (!valueTable.IsNullTable())
                        builder.Append(valueTable.ToString());
                    continue;
                }
                SetBaseValueTableString(builder, keyValue, tabString, value);
            }
            return GetTableText(table, builder.ToString());
        }

        private static void SetBaseValueTableString(StringBuilder builder, FieldKeyTable keyValue, string tabString, object value) {
            builder.Append(tabString);
            builder.Append(keyValue.key);
            builder.Append(LuaFormat.SpaceSymbol);
            builder.Append(LuaFormat.EqualSymbol);
            builder.Append(LuaFormat.SpaceSymbol);
            switch (keyValue.type) {
                case ValueType.Int:
                    builder.Append((int)value);
                    break;
                case ValueType.Number:
                    builder.Append((float)value);
                    break;
                case ValueType.Reference:
                    builder.Append(value as string);
                    break;
                case ValueType.String:
                    builder.Append(LuaFormat.QuotationPair.start);
                    builder.Append(value as string);
                    builder.Append(LuaFormat.QuotationPair.end);
                    break;
            }
            builder.Append(LuaFormat.CommaSymbol);
            builder.Append(LuaFormat.LineSymbol);
        }

        public static string GetRepeatKeyTableText<T>(StringBuilder builder, IRepeatKeyTable<T> table) where T : ITable {
            if (table.IsNullTable())
                return string.Empty;
            builder.Clear();
            T[] tableList = table.GetTableList();
            for (int index = 0; index < tableList.Length; index++)
                builder.Append(tableList[index].ToString());
            return GetTableText(table, builder.ToString());
        }

        private static string GetTableText(ITable table, string tableContent) {
            string format = GetTableFormat(table);
            string tabString = GetTabString(table);
            string key = table.GetKey();
            string toString = string.Format(format,
                                            tabString,
                                            key,
                                            LuaFormat.CurlyBracesPair.start,
                                            tableContent,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
        }

        private static string GetTableFormat(ITable table) {
            switch (table.GetKeyType()) {
                case KeyType.Array:
                    return "{0}[{1}] = {2}\n{3}{0}{4},\n";
                case KeyType.String:
                    return "{0}[\"{1}\"] = {2}\n{3}{0}{4},\n";
                case KeyType.Reference:
                    return "{0}{1} = {2}\n{3}{0}{4},\n";
                default:
                    Debug.LogError("table key type error. table name " + table.GetTableName());
                    return null;
            }
        }

        public static string GetTabString(ITable table) {
            ushort layer = table.GetLayer();
            return GetTabString(layer);
        }

        public static string GetTabString(ushort layer) {
            string tabString = string.Empty;
            for (int index = 0; index < layer; index++)
                tabString = Tool.GetCacheString(tabString + LuaFormat.TabString);
            return tabString;
        }

        public static string GetArrayString<T>(StringBuilder builder, T[] list) where T : ITable {
            if (list == null || list.Length == 0)
                return string.Empty;
            ushort layer = list[0].GetLayer();
            string tabString = GetTabString(layer);
            builder.Clear();
            builder.Append(LuaFormat.LineSymbol);
            for (int index = 0; index < list.Length; index++) {
                T data = list[index];
                if (data.IsNullTable())
                    continue;
                builder.Append(data.ToString());
            }
            builder.Append(tabString);
            return Tool.GetCacheString(builder.ToString());
        }

        public static void SetTableKey(ref ITable table, string outerKey) {
            switch (table.GetKeyType()) {
                case KeyType.Array:
                    if (ushort.TryParse(outerKey, out ushort key))
                        table.SetKey(key);
                    return;
                case KeyType.String:
                case KeyType.Reference:
                    table.SetKey(outerKey);
                    return;
            }
        }
    }

    public interface ITable {

        string GetTableName();
        ushort GetLayer();
        KeyType GetKeyType();
        void SetKey(object key);
        string GetKey();
        bool IsNullTable();
        void Clear();
        string ToString();
    }

    public interface IRepeatKeyTable<T> : ITable where T : ITable {
        
        T[] GetTableList();
    }

    public interface IFieldKeyTable : ITable {
        
        void SetFieldKeyTableValue(string key, object type);
        object GetFieldKeyTableValue(string key);
        FieldKeyTable[] GetFieldKeyTables();
    }

    public enum KeyType {
        Array,
        String,
        Reference,
    }

    public enum ValueType {
        Int,
        Number,
        String,
        Reference,
        Table,
    }

    public struct FieldKeyTable {

        public string key;
        public ValueType type;

        public int KeyLength => key.Length;

        public FieldKeyTable(string key, ValueType type) {
            this.key = key;
            this.type = type;
        }
    }
}