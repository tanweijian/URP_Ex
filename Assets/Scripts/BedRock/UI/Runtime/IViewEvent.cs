namespace BedRockRuntime.UI
{
    interface IViewEvent
    {
        void OnLoadView();
        void OnShow();
        void OnResume();
        void OnClose();
        void OnFreeView();
    }
}
