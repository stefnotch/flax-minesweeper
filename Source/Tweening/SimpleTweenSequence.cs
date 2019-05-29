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

        // TODO: Sort the list of actions by their start time
        // A LinkedList + a reference to the current action might be faster?
        protected readonly List<SimpleTweenable> _actions = new List<SimpleTweenable>();

        protected SimpleTweenSequence(SimpleTweenSequence parent = null) : base(parent)
        {

        }

        public override void Cancel()
        {
            // Cancel the children
            for (int i = _actions.Count - 1; i >= 0; i--)
            {
                _actions[i].Cancel();
            }
            _actions.Clear(); // TODO: Clear?

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
            _actions.Clear(); // TODO: Clear?

            // Finish self
            base.Finish();
        }

        protected override void ChildRemoved(SimpleTweenable child)
        {
            _actions.Remove(child);
        }

        protected override void ChildAdded(SimpleTweenable child)
        {
            _actions.Add(child);
        }

        // TODO: return this so that this method call can be chained?
        public void Add(SimpleTweenable simpleTweenable)
        {
            simpleTweenable.Sequence = this;
        }

        protected override void OnUpdate()
        {
            _actions.RemoveAll(a => a.Done);

            // Send the update event to all children
            // TODO: Optimisation: only the ones that are currently active
            foreach (var action in _actions)
            {
                action.Update(LocalTime);
            }
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

        public U Target => _target;

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

        public SimpleTweenAction<Actor> Wait(float duration, Action callback = null)
        {
            throw new NotImplementedException();
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

        #endregion Modify Options

    }
}