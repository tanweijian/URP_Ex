namespace BedRockRuntime.UI
{
    public class ViewInstance : IViewEvent, IViewImpl
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
        
        void IViewImpl.RegisterVMC(ViewModel m, ViewController c)
        {
            RegisterVMC(m, c);
        }

        protected ViewModel vm;
        protected ViewController vc;
        
        private void RegisterVMC(ViewModel m, ViewController c)
        {
            vm = m;
            vc = c;
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