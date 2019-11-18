using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua {

    internal static class LuaTable {

        public static string GetFieldKeyTableText(StringBuilder builder, IFieldValueTable table) {
            if (table.IsNullTable())
                return string.Empty;
            builder.Clear();
            ushort layer = (ushort)(table.GetLayer() + 1);
            string tabString = GetTabString(layer);
            FieldValueTableInfo[] array = table.GetFieldValueTableInfo();
            for (int index = 0; index < array.Length; index++) {
                FieldValueTableInfo keyValue = array[index];
                object value = table.GetFieldValueTableValue(keyValue.key);
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

        private static void SetBaseValueTableString(StringBuilder builder, FieldValueTableInfo keyValue, string tabString, object value) {
            builder.Append(tabString);
            builder.Append(keyValue.key);
            builder.Append(LuaFormat.SpaceSymbol);
            builder.Append(LuaFormat.EqualSymbol);
            builder.Append(LuaFormat.SpaceSymbol);
            builder.Append(value);
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
                case KeyType.FixedField:
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

        public static Type GetFieldValueTableValueType(IFieldValueTable table, string key) {
            object value = table.GetFieldValueTableValue(key);
            if (value == null)
                return null;
            return value.GetType();
        }
    }

    public interface ITable {

        string GetTableName();
        ushort GetLayer();
        ReadType GetReadType();
        KeyType GetKeyType();
        void SetKey(object key);
        string GetKey();
        bool IsNullTable();
        void Clear();
        string ToString();
    }

    public interface ILuaFile<T> where T : ITable {
        
        string GetLuaFilePath();
        string GetLuaFileHeadStart();
        List<T> GetModel();
    }

    public interface IRepeatKeyTable<T> : ITable where T : ITable {
        
        Type GetTableListType();
        List<T> GetStaticCacheList();
        object SetTableList();
        T[] GetTableList();
    }

    public interface IFieldValueTable : ITable {
        
        void SetFieldValueTableValue(string key, object value);
        object GetFieldValueTableValue(string key);
        FieldValueTableInfo[] GetFieldValueTableInfo();
    }

    public enum ReadType {
        /// <summary>
        /// Only read array or hash data
        /// </summary>
        Repeat,
        /// <summary>
        /// Repeat table to fixed table
        /// </summary>
        RepeatToFixed,
        /// <summary>
        /// Only read fixed key and value
        /// </summary>
        Fixed,
        /// <summary>
        /// Fixed table to repeat table
        /// </summary>
        FixedToRepeat,
    }

    public enum KeyType {
        Array,
        String,
        Reference,
        FixedField,
    }

    public enum ValueType {
        Int,
        Number,
        String,
        Reference,
        Table,
    }

    public struct FieldValueTableInfo {

        public string key;
        public ValueType type;

        public int KeyLength => key.Length;

        public FieldValueTableInfo(string key, ValueType type) {
            this.key = key;
            this.type = type;
        }
    }
}