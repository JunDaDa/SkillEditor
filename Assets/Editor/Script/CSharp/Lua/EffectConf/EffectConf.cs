using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using SkillEditor;

namespace Lua.EffectConf {

    public struct EffectData : IFieldValueTable, ILuaFile<EffectData> {
        
        public uint id;
        public string name;
        public PivotType pivotType;
        public string pivotNodeName;
        public ParentPivotType parentPivotType;
        public string resourceName;
        public bool isLoop;
        public bool isBreak;
        public EffectConfTransform offset;
        public EffectConfTransform scale;
        public EffectConfTransform rotation;
        public EffectConfShow showType;
        public EffectConfSound sound;
        public ushort playTime;
        public ushort trackRotate;
        public ushort canMirror;
        public string pivotWeapon;
        public ushort beginTime;
        public ushort lastTime;
        public ushort minHeight;
        public ushort scaleByEntity;

        #region ITable Function

        public string GetTableName() => "EffectData";
        public ushort GetLayer() => 1;
        public ReadType GetReadType() => ReadType.RepeatToFixed;
        public KeyType GetKeyType() => KeyType.Array;
        public void SetKey(object key) => id = (uint)(int)key;
        public string GetKey() => id.ToString();
        public bool IsNullTable() =>
            id == 0 || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pivotNodeName) ||
            string.IsNullOrEmpty(resourceName);
        public void Clear() {
            id = 0;
            name = pivotNodeName = resourceName = null;
            isLoop = isBreak = false;
            offset.Clear();
            scale.Clear();
            rotation.Clear();
        }

        private static StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 8));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion
    
        #region IFieldKeyTable Function

        private const string Key_ID = "ID";
        private const string Key_Name = "sName";
        private const string Key_PivotType = "iPivotType";
        private const string Key_PivotNodeName = "sPivot";
        private const string Key_ParentPivotType = "iParenPivotType";
        private const string Key_ResourceName = "sResName";
        private const string Key_Loop = "iLoop";
        private const string Key_Break = "iBreak";
        private const string Key_PlayTime = "iPlayTime";
        private const string Key_TrackRotate = "TrackRotate";
        private const string Key_CanMirror = "iCanMirror";
        private const string Key_PivotWeapon = "sPivotWeapon";
        private const string Key_BeginTime = "iBeginTime";
        private const string Key_LastTime = "iLastTime";
        private const string Key_MinHeight = "iMinHeight";
        private const string Key_ScaleByEntity = "iScaleByEntity";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_ID:
                    id = (uint)(int)value;
                    return;
                case Key_Name:
                    name = value as string;
                    return;
                case Key_PivotType:
                    int intTemp = (int)value;
                    if (Enum.IsDefined(typeof(PivotType), intTemp))
                        pivotType = (PivotType)intTemp;
                    return;
                case Key_PivotNodeName:
                    pivotNodeName = value as string;
                    return;
                case Key_ParentPivotType:
                    intTemp = (int)value;
                    if (Enum.IsDefined(typeof(ParentPivotType), intTemp))
                    parentPivotType = (ParentPivotType)intTemp;
                    return;
                case Key_ResourceName:
                    resourceName = value as string;
                    return;
                case Key_Loop:
                    isLoop = (int)value > 0;
                    return;
                case Key_Break:
                    isBreak = (int)value > 0;
                    return;
                case EffectConfShow.Key_ShowType:
                    showType = (EffectConfShow)value;
                    return;
                case EffectConfSound.Key_Sound:
                    sound = (EffectConfSound)value;
                    return;
                case Key_PlayTime:
                    playTime = (ushort)(int)value;
                    return;
                case Key_TrackRotate:
                    trackRotate = (ushort)(int)value;
                    return;
                case Key_CanMirror:
                    canMirror = (ushort)(int)value;
                    return;
                case Key_PivotWeapon:
                    pivotWeapon = value as string;
                    return;
                case Key_BeginTime:
                    beginTime = (ushort)(int)value;
                    return;
                case Key_LastTime:
                    lastTime = (ushort)(int)value;
                    return;
                case Key_MinHeight:
                    minHeight = (ushort)(int)value;
                    return;
                case Key_ScaleByEntity:
                    scaleByEntity = (ushort)(int)value;
                    return;
            }
            if (!Enum.TryParse(key.Substring(1), false, out TransformType transformType))
                return;
            switch (transformType) {
                case TransformType.Offset:
                    offset = (EffectConfTransform)value;
                    offset.type = TransformType.Offset;
                    return;
                case TransformType.Scale:
                    scale = (EffectConfTransform)value;
                    scale.type = TransformType.Scale;
                    return;
                case TransformType.Rotation:
                    rotation = (EffectConfTransform)value;
                    rotation.type = TransformType.Rotation;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_ID:
                    return id;
                case Key_Name:
                    return name;
                case Key_PivotType:
                    return pivotType;
                case Key_PivotNodeName:
                    return pivotNodeName;
                case Key_ParentPivotType:
                    return (short)parentPivotType;
                case Key_ResourceName:
                    return resourceName;
                case Key_Loop:
                    return isLoop ? 1 : 0;
                case Key_Break:
                    return isBreak ? 1 : 0;
                case EffectConfShow.Key_ShowType:
                    return showType;
                case EffectConfSound.Key_Sound:
                    return sound;
                case Key_PlayTime:
                    return playTime;
                case Key_TrackRotate:
                    return trackRotate;
                case Key_CanMirror:
                    return canMirror;
                case Key_PivotWeapon:
                    return pivotWeapon;
                case Key_BeginTime:
                    return beginTime;
                case Key_LastTime:
                    return lastTime;
                case Key_MinHeight:
                    return minHeight;
                case Key_ScaleByEntity:
                    return scaleByEntity;
            }
            string typeString = key.Substring(1);
            if (!Enum.TryParse(typeString, false, out TransformType type)) {
                Debug.LogError("EffectData::GetFieldValueTableValue error. key " + key);
                return null;
            }
            switch (type) {
                case TransformType.Offset:
                    return offset;
                case TransformType.Scale:
                    return scale;
                case TransformType.Rotation:
                    return rotation;
                default:
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            Array arrayType = Enum.GetValues(typeof(TransformType));
            ushort length = (ushort)(18 + arrayType.Length);
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_ID, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Name, ValueType.String);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_PivotType, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_ResourceName, ValueType.String);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_PivotNodeName, ValueType.String);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Loop, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Break, ValueType.Int);
            offset.type = TransformType.Offset;
            m_arraykeyValue[count++] = new FieldValueTableInfo(offset.GetKey(), ValueType.Table);
            scale.type = TransformType.Scale;
            m_arraykeyValue[count++] = new FieldValueTableInfo(scale.GetKey(), ValueType.Table);
            rotation.type = TransformType.Rotation;
            m_arraykeyValue[count++] = new FieldValueTableInfo(rotation.GetKey(), ValueType.Table);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_ParentPivotType, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(showType.GetKey(), ValueType.Table);
            m_arraykeyValue[count++] = new FieldValueTableInfo(sound.GetKey(), ValueType.Table);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_PlayTime, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_TrackRotate, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_CanMirror, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_PivotWeapon, ValueType.String);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_BeginTime, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_LastTime, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_MinHeight, ValueType.Int);
            m_arraykeyValue[count] = new FieldValueTableInfo(Key_ScaleByEntity, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion

        #region ILuaFile Function

        public string GetLuaFilePath() => Tool.CombinePath(Config.ProjectPath, "../Resources/lua/data/config/EffectConf.lua");
        public string GetLuaFileHeadStart() => "EffectConf = Class(\"EffectConf\", ConfigBase)";
        public List<EffectData> GetModel() => LuaEffectConfModel.EffectList;
        public string GetWriteFileString() => LuaEffectConfModel.GetWriteFileString();
        #endregion
    }

    public enum PivotType {

        Follow = 2,
        World = 3,
    }

    public enum ParentPivotType {

        Body = 0,
        Weapon = 1,
    }
}