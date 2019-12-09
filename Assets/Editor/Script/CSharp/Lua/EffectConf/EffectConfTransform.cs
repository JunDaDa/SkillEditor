using UnityEngine;
using System;
using System.Text;

namespace Lua.EffectConf {

    public struct EffectConfTransform : IFieldValueTable {

        public TransformType type;
        public short x;
        public short y;
        public short z;

        public EffectConfTransform(TransformType type, short x, short y, short z) {
            this.type = type;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        private static Vector3 m_vector3;

        public Vector3 Vector {
            set {
                x = (short)value.x;
                y = (short)value.y;
                z = (short)value.z;
            }
            get {
                m_vector3.x = x;
                m_vector3.y = y;
                m_vector3.z = z;
                return m_vector3;
            }
        }

        #region ITable Function

        public string GetTableName() => "EffectConfTransform";
        public ushort GetLayer() => 2;
        public ReadType GetReadType() => ReadType.Fixed;
        public KeyType GetKeyType() => KeyType.FixedField;
        public void SetKey(object key) { }
        public string GetKey() => 't' + type.ToString();
        public bool IsNullTable() => Vector == Vector3.zero;
        public void Clear() => Vector = Vector3.zero;

        private static readonly StringBuilder m_staticBuilder = new StringBuilder((ushort)Math.Pow(2, 5));
        public override string ToString() => LuaTable.GetFieldKeyTableText(m_staticBuilder, this);
        #endregion

        #region IFieldKeyTable Function
        
        private const string Key_X = "[1]";
        private const string Key_Y = "[2]";
        private const string Key_Z = "[3]";

        public void SetFieldValueTableValue(string key, object value) {
            switch (key) {
                case Key_X:
                    x = (short)(int)value;
                    return;
                case Key_Y:
                    y = (short)(int)value;
                    return;
                case Key_Z:
                    z = (short)(int)value;
                    return;
            }
        }

        public object GetFieldValueTableValue(string key) {
            switch (key) {
                case Key_X:
                    return x;
                case Key_Y:
                    return y;
                case Key_Z:
                    return z;
                default:
                    Debug.LogError("EffectRotationData::GetFieldValueTableValue key is not exit. key " + key);
                    return null;
            }
        }

        private static FieldValueTableInfo[] m_arraykeyValue;
        public FieldValueTableInfo[] GetFieldValueTableInfo() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            const ushort length = 3;
            ushort count = 0;
            m_arraykeyValue = new FieldValueTableInfo[length];
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_X, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Y, ValueType.Int);
            m_arraykeyValue[count++] = new FieldValueTableInfo(Key_Z, ValueType.Int);
            return m_arraykeyValue;
        }
        #endregion
    }

    public enum TransformType {

        Offset,
        Scale,
        Rotation,
    }
}