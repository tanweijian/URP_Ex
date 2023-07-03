using System;
using UnityEngine;
using System.Collections.Generic;

namespace BedRockRuntime.Base
{
    [Serializable]
    public class SerializedDictionary<Tkey, TValue> : Dictionary<Tkey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<Tkey> m_Keys = new List<Tkey>();
        [SerializeField] private List<TValue> m_Values = new List<TValue>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();

            foreach (var kvp in this)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < m_Keys.Count; i++)
            {
                Add(m_Keys[i], m_Values[i]);
            }
            m_Keys.Clear();
            m_Values.Clear();
        }
    }
}