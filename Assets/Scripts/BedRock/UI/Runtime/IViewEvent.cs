namespace BedRockRuntime.UI
{
    interface IViewEvent
    {
        void OnViewLoad();
        void OnShow();
        void OnClose();
        void OnViewRelease();
    }
}
