using System;

namespace CardMatch.Navigation
{
    public abstract class View<T> : View, IView<T> where T : IViewContext
    {
        public override Type ContextType => typeof(T);
        public T Context { get; private set; }

        public sealed override void SetContext(IViewContext context)
        {
            if (context is not T typedContext)
            {
                throw new ArgumentException($"Expected context of type {typeof(T).Name}, but got {context?.GetType().Name ?? "null"}.");
            }
            SetContext(typedContext);
        }

        public void SetContext(T context)
        {
            Context = context;
        }
    }
}
