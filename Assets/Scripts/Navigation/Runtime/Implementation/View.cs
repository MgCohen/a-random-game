using System;
using UnityEngine;

namespace CardMatch.Navigation
{
    public abstract class View : MonoBehaviour, IView
    {
        private ViewStatus status = ViewStatus.Closed;
        public ViewStatus Status => status;

        public abstract Type ContextType { get; }
        public abstract void SetContext(IViewContext context);

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
    }
}
