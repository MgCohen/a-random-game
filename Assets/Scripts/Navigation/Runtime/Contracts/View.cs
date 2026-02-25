using UnityEngine;

namespace CardMatch.Navigation
{
    public abstract class View : MonoBehaviour, IView
    {
        private ViewStatus status = ViewStatus.Closed;

        public ViewStatus Status => status;

        protected INavigation Navigation { get; private set; }

        public void SetNavigation(INavigation navigation)
        {
            Navigation = navigation;
        }

        public void Show()
        {
            status = ViewStatus.Open;
            gameObject.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            status = ViewStatus.Hidden;
            OnHide();
            gameObject.SetActive(false);
        }

        public void Close()
        {
            status = ViewStatus.Closed;
            OnClose();
            gameObject.SetActive(false);
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected void GoTo(IViewContext context)
        {
            if (Navigation == null)
            {
                return;
            }
            Navigation.Open(context);
        }
    }
}
