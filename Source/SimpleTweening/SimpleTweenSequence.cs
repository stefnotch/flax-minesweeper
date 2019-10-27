using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace SimpleTweening
{
    /// <summary>
    /// A sequence of tweens
    /// </summary>
    public abstract class SimpleTweenSequence : SimpleTweenable
    {
        // TODO: If DestroyOnFinish = true, switch to a wrap-around list, where old actions get overwritten
        protected int Index = 0;
        protected readonly List<SimpleTweenable> _actions = new List<SimpleTweenable>(); // Or an interval tree?

        protected SimpleTweenSequence(SimpleTweenSequence parent = null) : base(parent)
        {
            EndEvent += (_) =>
            {
                if (DestroyOnFinish)
                {
                    _actions.Clear();
                }
            };
        }

        public bool DestroyOnFinish { get; set; } = true;

        public override void Cancel()
        {
            // Cancel the children
            for (int i = _actions.Count - 1; i >= 0; i--)
            {
                _actions[i].Cancel();
            }

            if (DestroyOnFinish)
            {
                _actions.Clear();
                // No need to clear the event, https://stackoverflow.com/a/17400033/3492994
            }

            // Cancel self
            base.Cancel();
        }

        public override void Finish()
        {
            // Finish the children
            foreach (var action in _actions)
            {
                action.Finish();
            }

            if (DestroyOnFinish)
            {
                _actions.Clear();
            }

            // Finish self
            base.Finish();
        }

        protected override void OnChildRemoved(SimpleTweenable child)
        {
            // TODO: Optimize this
            _actions.Remove(child);
        }

        protected override void OnChildAdded(SimpleTweenable child)
        {
            // TODO: Optimize this
            _actions.Add(child);
            //_actions.Sort(); 
        }

        public T Add<T>(T simpleTweenable) where T : SimpleTweenable
        {
            simpleTweenable.Sequence = this;
            return simpleTweenable;
        }

        protected override void OnUpdate()
        {
            // Send the update event to all children
            // TODO: Optimisation: only the ones that are currently active
            foreach (var action in _actions)
            {
                action.Update(LocalTime);
            }

            // Every animation deserves to be played once (so that it's in the end state)
        }
    }

    /// <summary>
    /// A sequence of tweens
    /// </summary>
    /// <typeparam name="U">The affected actor</typeparam>
    public class SimpleTweenSequence<U> : SimpleTweenSequence where U : Actor
    {
        // TODO: Target Actor?
        // TODO: Get this from the parent as well?
        protected U _target;

        public SimpleTweenSequence(U target, SimpleTweenSequence parent = null) : base(parent)
        {
            _target = target;
        }

        public U Target { get => _target; protected set => _target = value; }

        /// <summary>
        /// Creates a new child-sequence
        /// </summary>
        /// <returns>The new sequence</returns>
        public SimpleTweenSequence<U> NewSequence()
        {
            return new SimpleTweenSequence<U>(Target, this);
            // TODO: This HAS to be behave identical to
            //return Add(new SimpleTweenSequence<U>(Target, null));
        }

        #region Actions

        public SimpleTweenAction<U, Vector3> MoveTo(Vector3 to, float duration, float? startDelay = null)
        {
            return AddTweenAction(to, duration, startDelay, SimpleTweenFunctions.TranslateLocal<U>, SimpleTweenFunctions.GetLocalPosition, SimpleTweenFunctions.GetLocalPosition);
        }

        public SimpleTweenAction<U, Quaternion> RotateTo(Quaternion to, float duration, float? startDelay = null)
        {
            return AddTweenAction(to, duration, startDelay, SimpleTweenFunctions.RotateLocal<U>, SimpleTweenFunctions.GetLocalOrientation, SimpleTweenFunctions.GetLocalOrientation);
        }

        public SimpleTweenAction<U, Vector3> ScaleTo(Vector3 to, float duration, float? startDelay = null)
        {
            return AddTweenAction(to, duration, startDelay, SimpleTweenFunctions.ScaleLocal<U>, SimpleTweenFunctions.GetLocalScale, SimpleTweenFunctions.GetLocalScale);
        }

        public SimpleTweenAction<U> Wait(float duration, Action<SimpleTweenAction<U, float>> callback = null)
        {
            return Wait(duration, null, callback);
        }

        public SimpleTweenAction<U> Wait(float duration, float? startDelay, Action<SimpleTweenAction<U, float>> callback = null)
        {
            // If we want the wait thingy to start at 0 or something.
            var tweenAction = AddTweenAction(1f, duration, startDelay, SimpleTweenFunctions.Nothing<U>, SimpleTweenFunctions.GetZero, SimpleTweenFunctions.GetOne);
            if (callback != null) tweenAction.OnEnd(callback);
            return tweenAction;
        }

        #endregion Actions

        private SimpleTweenAction<U, T> AddTweenAction<T>(T to, float duration, float? startDelay, Action<SimpleTweenAction<U, T>> tweenFunction, Func<U, T> defaultFromValue = null, Func<U, T> defaultToValue = null)
        {
            float? previousEndTime = _actions.DefaultIfEmpty(null).LastOrDefault()?.EndTime;

            // Since we pass the parent, it automatically gets added to our _actions
            var tweenAction = new SimpleTweenAction<U, T>(this, tweenFunction, defaultFromValue, defaultToValue);

            tweenAction.SetTo(to);
            tweenAction.Duration = duration;
            if (startDelay.HasValue)
            {
                tweenAction.SetStartDelay(startDelay.Value);
            }
            else
            {
                // Take the previous tween's end time or take the current time
                // TODO: This is wrong at the beginning (when LocalTime hasn't been initialized yet)

                if (previousEndTime.HasValue)
                {
                    tweenAction.SetStartTime(previousEndTime.Value);
                }
            }
            return tweenAction;
        }

        /// <summary>
        /// If a child that is a sequence gets added, update the target <see cref="Actor"/>
        /// </summary>
        /// <param name="child">The added child</param>
        protected override void OnChildAdded(SimpleTweenable child)
        {
            base.OnChildAdded(child);
            if (child is SimpleTweenSequence<U> sequence)
            {
                sequence.Target = Target;
            }
        }
    }
}