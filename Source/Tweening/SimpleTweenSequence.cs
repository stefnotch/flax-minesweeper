using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace FlaxMinesweeper.Source.Tweening
{
    /// <summary>
    /// A sequence of tweens
    /// </summary>
    public abstract class SimpleTweenSequence : SimpleTweenable
    {
        /* Minor Redesign:
        * ContainerSequence: (Doesn't bother calculating the percentage?)
        * - Duration: max(child.EndTime)
        * - Scale: If it's more than 1, the LocalTime **that** gets passed in the Update() function wraps around
        * - All Sequences are ContainerSequences. Because the other one doesn't make sense.
        * 
        */

        // TODO: If DestroyOnFinish = true, switch to a wrap-around list, where old actions get overwritten
        protected int Index = 0;
        protected readonly List<SimpleTweenable> _actions = new List<SimpleTweenable>(); // Or an interval tree?

        protected SimpleTweenSequence(SimpleTweenSequence parent = null) : base(parent)
        {

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
                //this.EndEvent = null; // TODO: Also remove the event
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
                //this.EndEvent = null; // TODO: Also remove the event
            }

            // Finish self
            base.Finish();
        }

        protected override void ChildRemoved(SimpleTweenable child)
        {
            // TODO: Optimize this
            _actions.Remove(child);
        }

        protected override void ChildAdded(SimpleTweenable child)
        {
            // TODO: Optimize this
            _actions.Add(child);
            //_actions.Sort(); // TODO: Notify the parent (this) when the StartTime changes
            if (child.EndTime > Duration)
            {
                Duration = child.EndTime;
            }
        }

        public TweenAble Add<TweenAble>(TweenAble simpleTweenable) where TweenAble : SimpleTweenable
        {
            // TODO: Check if the child actually fits into this sequence.
            simpleTweenable.Sequence = this;
            return simpleTweenable;
        }

        protected override void OnUpdate()
        {
            // Send the update event to all children
            // TODO: Optimisation: only the ones that are currently active
            float maxEndTime = -1f;
            foreach (var action in _actions)
            {
                if (Scale > 1)
                {
                    // Wrap around local time
                    action.Update(LocalTime % Duration);
                }
                else
                {
                    action.Update(LocalTime);
                }

                float endTime = action.EndTime;
                if (endTime > maxEndTime)
                {
                    maxEndTime = endTime;
                }
            }

            Duration = maxEndTime; // TODO: Or notify the parent whenever a Startime/(pause?)/Duration changes

            if (Done && DestroyOnFinish)
            {
                _actions.Clear();
            }

            // Every animation deserves to be played once (so that it's in the end state)
            //_actions.RemoveAll(a => a.Done);
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
        protected readonly U _target;

        public SimpleTweenSequence(U target, SimpleTweenSequence parent = null) : base(parent)
        {
            _target = target;
        }

        public U Target => _target;

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
            var tweenAction = AddTweenAction(1f, duration, null, SimpleTweenFunctions.Nothing<U>, SimpleTweenFunctions.GetZero, SimpleTweenFunctions.GetOne);
            if (callback != null) tweenAction.OnEnd(callback);
            return tweenAction;
        }

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

        #endregion Actions

        /* public SimpleTweenSequence<U> Add(SimpleTweenable simpleTweenable)
         {
         // TODO: What was this?
             // Case where it's a sequence
             // Case where it's just an action

             // For both cases: Case where the actor doesn't have a sequence yet VS case where the actor already has a sequence
             throw new NotImplementedException();
         }*/


        #region Modify Options
        // Note: They aren't in SimpleTweenable so that I can specify the correct return type

        public SimpleTweenSequence<U> SetRepetitions(int repetitionCount)
        {
            this.Scale = repetitionCount;
            return this;
        }

        public SimpleTweenSequence<U> SetLoopType(LoopType loopType)
        {
            this.Options.LoopType = loopType;
            return this;
        }

        public SimpleTweenSequence<U> SetReversed(bool reversed = true)
        {
            this.Options.Reversed = reversed;
            return this;
        }

        public SimpleTweenSequence<U> SetStartDelay(float startDelay)
        {
            this.StartTime = (Sequence?.LocalTime ?? ParentTime) + startDelay;
            return this;
        }

        public SimpleTweenSequence<U> SetStartTime(float startTime)
        {
            this.StartTime = startTime;
            return this;
        }

        #endregion Modify Options

    }
}