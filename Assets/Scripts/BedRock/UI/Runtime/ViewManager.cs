using System;
using System.Collections.Generic;
using UnityEngine;

namespace BedRockRuntime.UI
{
    public static class ViewManager
    {
        private static readonly Dictionary<Type, ViewInstance> t2v = new Dictionary<Type, ViewInstance>();

        static ViewManager()
        {
        }

        private static void _<V, VM, VC>() where V : ViewInstance, new() where VM : ViewModel, new() where VC : ViewController, new()
        {
            V view = new V();
            VM vm = new VM();
            VC vc = new VC();
            ((IViewImpl)view).RegisterVMC(vm, vc);
            if (!t2v.TryAdd(view.GetType(), view))
            {
                Debug.LogError($"Duplicate register view {view.GetType()}");
            }
        }

        public static V GetViewInstance<V>() where V : ViewInstance
        {
            Type type = typeof(V);
            if (t2v.TryGetValue(type, out ViewInstance view))
            {
                return (V)view;
            }
            return null;
        }
    }
}