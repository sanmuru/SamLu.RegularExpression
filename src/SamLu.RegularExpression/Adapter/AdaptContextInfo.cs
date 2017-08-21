using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.Adapter
{
    public class AdaptContextInfo<TSource, TTarget>
    {
        public AdaptDelegate<TSource, TTarget> SourceAdaptor { get; protected set; }

        public AdaptDelegate<TTarget, TSource> TargetAdaptor { get; protected set; }

        public AdaptOption AdaptOption { get; protected set; }

        public virtual bool TryAdaptSource(TSource source, out TTarget target) => this.TryAdaptSource(source, out target, out Exception e);

        protected internal virtual bool TryAdaptSource(TSource source, out TTarget target, out Exception e)
        {
            target = default(TTarget);
            e = null;

            if (this.SourceAdaptor == null)
            {
                e = new ArgumentNullException(nameof(this.SourceAdaptor));
                return false;
            }
            else
            {
                try { target = this.SourceAdaptor(source); }
                catch (Exception _e)
                {
                    e = _e;
                    return false;
                }

                return true;
            }
        }

        public virtual bool TryAdaptTarget(TTarget target, out TSource source) => this.TryAdaptTarget(target, out source, out Exception e);

        protected internal virtual bool TryAdaptTarget(TTarget target, out TSource source, out Exception e)
        {
            source = default(TSource);
            e = null;

            if (this.TargetAdaptor == null)
            {
                e = new ArgumentNullException(nameof(this.TargetAdaptor));
                return false;
            }
            else
            {
                try { source = this.TargetAdaptor(target); }
                catch (Exception _e)
                {
                    e = _e;
                    return false;
                }

                return true;
            }
        }

        public bool OnlyAdaptConstSourceWhenInitialization =>
            (this.AdaptOption & AdaptOption.OnlyAdaptConstSourceWhenInitialization) != 0;

        public bool AlwaysAdaptSource => !this.OnlyAdaptConstSourceWhenInitialization;

        public AdaptContextInfo(AdaptDelegate<TSource, TTarget> sourceAdaptor, AdaptDelegate<TTarget, TSource> targetAdaptor)
        {
            this.SourceAdaptor = sourceAdaptor ?? (source => default(TTarget));
            this.TargetAdaptor = targetAdaptor ?? (target => default(TSource));
        }
    }
}
