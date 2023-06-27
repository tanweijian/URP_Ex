using System;
using System.Collections.Generic;

namespace BedRockRuntime.UI
{
    public static class ViewInstanceManager
    {
        private static Dictionary<Type, ViewInstance> viewInstances = new Dictionary<Type, ViewInstance>();

        static ViewInstanceManager()
        {
        }

        private static void _<V, VM, VC>() where V : ViewInstance, new() where VM : ViewModel, new() where VC : ViewController, new()
        {
            V view = new();
            VM vm = new();
            VC vc = new();
            ((IViewImpl)view).RegisterVMC(vm, vc);
        }
    }
}