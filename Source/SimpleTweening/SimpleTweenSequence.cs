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
    public class SimpleTweenSequence : SimpleTweenSequenceElement
    {
        private float _localTime;
        private bool _isPaused;
        private float _pauseDuration;
        private float _currentPauseStartTime;

        // Or an interval tree?
        protected readonly List<SimpleTweenSequenceElement> _actions = new List<SimpleTweenSequenceElement>();

        public SimpleTweenSequence(SimpleTweenSequence sequence = null) : base(sequence)
        {
        }

        /// <summary>
        /// Time offset
        /// </summary>
        public float LocalTime { get => _localTime; protected set => _localTime = value; }

        /// <summary>
        /// Global Time
        /// </summary>
        public override float TotalDuration => StartTime - EndTime;

        /// <summary>
        /// On the parent's timeline
        /// </summary>
        public float EndTime => _actions.Max(action => action.StartTime + action.TotalDuration);

        /// <summary>
        /// If this tween is paused
        /// </summary>
        public bool IsPaused { get => _isPaused; set { PausedChanged(value, _isPaused); _isPaused = value; } }

        /// <summary>
        /// How long has this tween been paused for
        /// </summary>
        public float PauseDuration { get => _pauseDuration; protected set { _pauseDuration = value; } }

        /// <summary>
        /// Remove the sequence when it's finished
        /// </summary>
        public bool DestroyOnFinish { get; set; } = true;

        public override void Update(float parentTime)
        {
            if (parentTime < StartTime) return;
            if (LoopCount <= 0) return;
            if (IsPaused) return;

            LocalTime = parentTime - StartTime - PauseDuration;

            bool isDone = EndTime < parentTime;

            // Send the update event to all children
            // TODO: Optimisation: only the ones that are currently active
            foreach (var action in _actions)
            {
                // Every tween should be updated at least once so that it's in the end state
                action.Update(LocalTime);
            }

            if (isDone)
            {
                OnDone();
            }
        }

        /// <summary>
        /// Restarts a sequence
        /// </summary>
        public SimpleTweenSequence Restart()
        {
            if (LoopCount <= 0) LoopCount = 1;
            StartTime = ParentTime;
            IsPaused = false;
            PauseDuration = 0;
            return this;
        }

        /// <summary>
        /// Finishes a sequence
        /// </summary>
        public SimpleTweenSequence Finish()
        {
            // I'm just going to do a finish update and let the update function handle this
            Update(EndTime);
            return this;
        }

        /// <summary>
        /// Cancels a sequence
        /// </summary>
        public SimpleTweenSequence Cancel()
        {
            // Just remove ourselves from the sequence
            Sequence = null;
            throw new NotImplementedException();
            // TODO: This won't work for the root sequence
            // TODO: Should I also dispose of all the children? 
            // TODO: An alternative option is to have a script that inherits from SimpleTweenSequence and does something smart.
            // TODO: Yet another option is to finally implement all the events.
            return this;
        }

        protected void OnDone()
        {
            LoopCount--;
            if (LoopCount > 0)
            {
                float endTime = EndTime;
                Restart();
                StartTime = endTime;
            }
            else
            {
                if (DestroyOnFinish)
                {
                    for (int i = _actions.Count - 1; i >= 0; i--)
                    {
                        _actions[i].Sequence = null;
                    }
                    _actions.Clear();
                }
            }
        }

        protected override void OnChildRemoved(SimpleTweenSequenceElement child)
        {
            // TODO: Optimize this
            _actions.Remove(child);
        }

        protected override void OnChildAdded(SimpleTweenSequenceElement child)
        {
            // TODO: Optimize this
            _actions.Add(child);
            //_actions.Sort(); 
        }

        private void PausedChanged(bool isPausedNew, bool isPausedOld)
        {
            if (isPausedNew != isPausedOld)
            {
                if (isPausedNew)
                {
                    // TODO: Is this the best approach? Or should I use LocalTime? Or the parent time?
                    _currentPauseStartTime = Time.GameTime;
                }
                else
                {
                    PauseDuration += Time.GameTime - _currentPauseStartTime;
                }
            }
        }

        public T Add<T>(T simpleTweenable) where T : SimpleTweenSequenceElement
        {
            simpleTweenable.Sequence = this;
            return simpleTweenable;
        }

        /// <summary>
        /// Creates a new child-sequence
        /// </summary>
        /// <returns>The new sequence</returns>
        public SimpleTweenSequence NewSequence()
        {
            return new SimpleTweenSequence(this);
        }

        /*public SimpleTweenAction<T> AddTweenAction<T>(T to, float duration, float? startDelay, Func<SimpleTweenAction<T>, T> tweenFunction, Action<T> setter, Func<T> getter, Func<T> defaultFromValue = null, Func<T> defaultToValue = null)
        {
            float? previousEndTime = _actions.DefaultIfEmpty(null).LastOrDefault()?.EndTime;

            // Since we pass the parent, it automatically gets added to our _actions
            var tweenAction = new SimpleTweenAction<T>(this, tweenFunction, defaultFromValue, defaultToValue);

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
        }*/
    }

    /// <summary>
    /// A sequence of tweens
    /// </summary>
    /// <typeparam name="U">The affected actor</typeparam>
    /*public class SimpleTweenSequence<U> : SimpleTweenSequence where U : Actor
    {

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
    }*/
}