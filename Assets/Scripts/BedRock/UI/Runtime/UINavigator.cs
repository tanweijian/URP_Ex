using System.Collections.Generic;

namespace BedRockRuntime.UI
{
    public static class UINavigator
    {
        private static Stack<ViewInstance> instances;

        public static V Open<V>() where V : ViewInstance
        {
            ViewInstance view = ViewManager.GetViewInstance<V>();
            if (!view.IsOpen)
            {
            }
            if (view != null)
            {
                return (V)view;    
            }
            return null;
        }
        
        public static void Open(ViewInstance view)
        {
        }
        
        public static void Close(ViewInstance view)
        {
        }
    }
}
