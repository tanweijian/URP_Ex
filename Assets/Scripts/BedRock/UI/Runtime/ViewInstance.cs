namespace BedRockRuntime.UI
{
    public class ViewInstance : IViewEvent
    {
        void IViewEvent.OnLoadView()
        {
            OnLoadView();
        }

        void IViewEvent.OnShow()
        {
            OnShow();
        }

        void IViewEvent.OnResume()
        {
            OnResume();
        }

        void IViewEvent.OnClose()
        {
            OnClose();
        }

        void IViewEvent.OnFreeView()
        {
            OnFreeView();
        }

        protected ViewModel vm;
        protected ViewController vc;

        protected void Register<VM, VC>() where VM : ViewModel, new() where VC : ViewController, new()
        {
            vm = new VM();
            vc = new VC();
        }
        
        protected virtual void OnLoadView()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnResume()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnFreeView()
        {
        }
    }
}