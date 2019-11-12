﻿using UnityEngine;
using System.Text;
using SkillEditor;

namespace Lua {

    internal static class LuaTable {

        public static string GetFieldKeyTableText(StringBuilder builder, IFieldKeyTable table) {
            if (table.IsNullTable() || table.GetKeyType() == KeyType.Field)
                return string.Empty;
            builder.Clear();
            for (int index = 0; index < tableList.Length; index++)
                builder.Append(tableList[index].ToString());
            string stateListString = builder.ToString();
            string tabString = LuaTable.GetTabString(table.GetLayer());
            string format = null;
            switch (table.GetKeyType()) {
                case KeyType.Array:
                    format = "{0}[{1}] = {2}\n{3}{0}{4},\n";
                    break;
                case KeyType.String:
                    format = "{0}[\"{1}\"] = {2}\n{3}{0}{4},\n";
                    break;
                case KeyType.Reference:
                    format = "{0}{1} = {2}\n{3}{0}{4},\n";
                    break;
            }
            string toString = string.Format(format, tabString,
                                            key,
                                            LuaFormat.CurlyBracesPair.start,
                                            stateListString,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
            return string.Empty;
        }

        public static string GetNotFieldKeyTableText(StringBuilder builder, IRepeatKeyTable table) {
            if (table.IsNullTable())
                return string.Empty;
            builder.Clear();
            ITable[] tableList = table.GetTableList();
            for (int index = 0; index < tableList.Length; index++)
                builder.Append(tableList[index].ToString());
            string stateListString = builder.ToString();
            string tabString = LuaTable.GetTabString(table.GetLayer());
            string format = null;
            switch (table.GetKeyType()) {
                case KeyType.Array:
                    format = "{0}[{1}] = {2}\n{3}{0}{4},\n";
                    break;
                case KeyType.String:
                    format = "{0}[\"{1}\"] = {2}\n{3}{0}{4},\n";
                    break;
                case KeyType.Reference:
                    format = "{0}{1} = {2}\n{3}{0}{4},\n";
                    break;
            }
            string toString = string.Format(format, tabString,
                                            key,
                                            LuaFormat.CurlyBracesPair.start,
                                            stateListString,
                                            LuaFormat.CurlyBracesPair.end);
            return Tool.GetCacheString(toString);
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

        public static void SetFieldKeyTableOuterKey(ref IFieldKeyTable table, string outerKey) {
            switch (table.GetOuterField()) {
                case KeyType.Array:
                    if (ushort.TryParse(outerKey, out ushort key))
                        table.SetOuterField(key);
                    return;
                case KeyType.String:
                case KeyType.Reference:
                case KeyType.Field:
                    table.SetOuterField(outerKey);
                    return;
            }
        }
    }

    public interface ITable {

        ushort GetLayer();
        KeyType GetKeyType();
        string GetKey();
        bool IsNullTable();
        void Clear();
        string ToString();
    }

    public interface IRepeatKeyTable : ITable {
        
        T[] GetTableList<T>() where T : ITable;
    }

    public interface IFieldKeyTable : ITable {
        
        void SetOuterField(object outerKey);
        KeyType GetOuterField();
        void SetFieldKeyTable(string key, object type);
        FieldKeyTable[] GetFieldKeyTables();
    }

    /// Config key type
    /// Array String Reference key is read repeatly, ValueType is Table fixedly 
    public enum KeyType {
        Array,
        String,
        Reference,
        Field,
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