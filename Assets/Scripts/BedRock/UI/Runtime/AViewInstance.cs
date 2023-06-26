namespace BedRockRuntime.UI
{
    public abstract class AViewInstance<VM, VC> : IViewEvent where VM : AViewModel where VC : AViewController
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

        private VM vm;
        private VC vc;

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