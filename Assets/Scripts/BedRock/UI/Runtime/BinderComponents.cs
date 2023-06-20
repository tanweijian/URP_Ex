using BedRockRuntime.Base;

namespace UnityEngine.UI
{
    [DisallowMultipleComponent]
    public class BinderComponents : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<string, Object> m_ReferenceObjects;

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_ReferenceObjects ??= new SerializedDictionary<string, Object>();
        }
#endif

        public T GetComponent<T>(string key) where T : Object
        {
            if (m_ReferenceObjects.TryGetValue(key, out Object value))
            {
                return value as T;
            }

            return null;
        }
    }
}