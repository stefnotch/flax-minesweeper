using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace SimpleTweening
{
    public static class SimpleTweenableExtensions
    {
        // Options setters as extension methods so that they can return the correct type
        #region Tween Options
        public static T SetAdditive<T>(this T tweenAble, bool isAdditive) where T : SimpleTweener
        {
            tweenAble.Options.IsAdditive = isAdditive;
            return tweenAble;
        }

        public static T SetLoopType<T>(this T tweenAble, LoopType loopType) where T : SimpleTweener
        {
            tweenAble.Options.LoopType = loopType;
            return tweenAble;
        }

        public static T SetReversed<T>(this T tweenAble, bool reversed = true) where T : SimpleTweener
        {
            tweenAble.Options.Reversed = reversed;
            return tweenAble;
        }

        public static T SetEasing<T>(this T tweenAble, AlphaBlendMode easingType) where T : SimpleTweener
        {
            tweenAble.Options.EasingType = easingType;
            return tweenAble;
        }

        public static T SetLerpType<T>(this T tweenAble, LerpType lerpType) where T : SimpleTweener
        {
            tweenAble.Options.LerpType = lerpType;
            return tweenAble;
        }

        public static T SetOptions<T>(this T tweenAble, SimpleTweenOptions options) where T : SimpleTweener
        {
            tweenAble.Options = options;
            return tweenAble;
        }

        public static T SetOptions<T>(this T tweenAble, Action<SimpleTweenOptions> optionsModifier) where T : SimpleTweener
        {
            optionsModifier?.Invoke(tweenAble.Options);
            return tweenAble;
        }
        #endregion Tween Options

        #region Tween Sequence Element
        public static T SetStartDelay<T>(this T tweenAble, float startDelay) where T : SimpleTweenSequenceElement
        {
            float startTime = (tweenAble.Sequence != null) ? tweenAble.Sequence.LocalTime : Time.GameTime;

            tweenAble.StartTime = startTime + startDelay;
            return tweenAble;
        }

        public static T SetStartTime<T>(this T tweenAble, float startTime) where T : SimpleTweenSequenceElement
        {
            tweenAble.StartTime = startTime;
            return tweenAble;
        }

        public static T SetRepetitions<T>(this T tweenAble, int repetitionCount) where T : SimpleTweenSequenceElement
        {
            tweenAble.LoopCount = repetitionCount;
            return tweenAble;
        }
        #endregion Tween Sequence Element

        // TODO: Duration
        // TODO: TweenFunction, FromValue, ToValue
    }
}
