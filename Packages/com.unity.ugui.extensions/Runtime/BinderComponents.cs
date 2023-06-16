using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class BinderComponents : MonoBehaviour
    {
        [SerializeField] private List<string> m_ReferenceKeys;
        [SerializeField] private List<Object> m_ReferenceObjects;

        private void OnValidate()
        {
            m_ReferenceKeys ??= new List<string>();
            m_ReferenceObjects ??= new List<Object>();
        }

        public T GetComponent<T>(string key) where T : Object
        {
            int index = m_ReferenceKeys.BinarySearch(key);
            if (index < 0)
            {
                return null;
            }
            return m_ReferenceObjects[index] as T;
        }
    }
}