using System;

namespace CardMatch.Navigation
{
    public interface IView<T> : IView where T : IViewContext
    {
        void SetContext(T context);
    }

}
