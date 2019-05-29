﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace FlaxMinesweeper.Source.Tweening
{
    public abstract class SimpleTweenAction<U> : SimpleTweenable where U : Actor
    {
        protected SimpleTweenOptions _options = new SimpleTweenOptions();

        public SimpleTweenAction(SimpleTweenSequence parent = null) : base(parent)
        {
        }

        // TODO: Is the new keyword optimal?
        // TODO: Type cast or private property?
        new public SimpleTweenSequence<U> Sequence
        {
            get => (SimpleTweenSequence<U>)base.Sequence;
            set => base.Sequence = value;
        }

        #region Actions

        public SimpleTweenAction<U, Vector3> MoveTo(Vector3 to, float duration, float? startDelay = null)
        {
            return Sequence.MoveTo(to, duration, startDelay);
        }

        public SimpleTweenAction<U, Quaternion> RotateTo(Quaternion to, float duration, float? startDelay = null)
        {
            return Sequence.RotateTo(to, duration, startDelay);
        }

        public SimpleTweenAction<U, Vector3> ScaleTo(Vector3 to, float duration, float? startDelay = null)
        {
            return Sequence.ScaleTo(to, duration, startDelay);
        }

        public SimpleTweenAction<Actor> Wait(float timeInSeconds, Action callback = null)
        {
            return this.Sequence.Wait(timeInSeconds, callback);
        }

        #endregion Actions
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="U">Affected object</typeparam>
    /// <typeparam name="T">Affected type</typeparam>
    public class SimpleTweenAction<U, T> : SimpleTweenAction<U> where U : Actor
    {
        /**
         * Alternative Approach:
         * # Multi-Stage Approach
         * In the constructor, some important data (e.g. a direction vector) is saved
         * Stage 1: Generate (float percentage)
         * Stage 2: Reset (object) 
         * Stage 3: Apply (object, generated value)
         */

        private bool _firstUpdate = false;
        private bool _fromValueSet = false;
        private bool _toValueSet = false;
        private T _fromValue;
        private T _toValue;
        private Action<SimpleTweenAction<U, T>> _tweenFunction;
        private readonly Func<U, T> _defaultFromValue;
        private readonly Func<U, T> _defaultToValue;

        public SimpleTweenAction(SimpleTweenSequence<U> sequence, Action<SimpleTweenAction<U, T>> tweenFunction, Func<U, T> defaultFromValue = null, Func<U, T> defaultToValue = null) : base(sequence)
        {
            _tweenFunction = tweenFunction;
            _defaultFromValue = defaultFromValue;
            _defaultToValue = defaultToValue;
        }

        public T FromValue => _fromValue;
        public T ToValue => _toValue;

        public U Target => Sequence.Target;
        // SetPaused

        // TODO: Update
        // TODO: OnStart
        // TODO: OnEnd

        /*
         * OnComplete(TweenCallback callback)
OnKill(TweenCallback callback)
OnPlay(TweenCallback callback)
OnPause(TweenCallback callback)
OnRewind(TweenCallback callback)
OnStart(TweenCallback callback)
OnStepComplete(TweenCallback callback)
OnUpdate(TweenCallback callback)
OnWaypointChange(TweenCallback<int> callback)
*/

        public SimpleTweenAction<U, T> OnFinished(Action<SimpleTweenAction<U, T>> onFinished)
        {
            throw new NotImplementedException();
            return this;
        }

        #region Modify Options

        /// <summary>
        /// Sets the scale
        /// </summary>
        /// <param name="repetitionCount"></param>
        /// <returns></returns>
        public SimpleTweenAction<U, T> SetRepetitions(int repetitionCount)
        {
            this.Scale = repetitionCount;
            return this;
        }

        public SimpleTweenAction<U, T> SetLoopType(LoopType loopType)
        {
            this.Options.LoopType = loopType;
            return this;
        }

        public SimpleTweenAction<U, T> SetFrom(T from)
        {
            _fromValueSet = true;
            _fromValue = from;
            return this;
        }

        public SimpleTweenAction<U, T> SetTo(T to)
        {
            _toValueSet = true;
            _toValue = to;
            return this;
        }

        public SimpleTweenAction<U, T> SetReversed(bool reversed = true)
        {
            _options.Reversed = reversed;
            return this;
        }

        public SimpleTweenAction<U, T> SetStartDelay(float startDelay)
        {
            this.StartTime = (Sequence?.LocalTime ?? ParentTime) + startDelay;
            return this;
        }

        public SimpleTweenAction<U, T> SetStartTime(float startTime)
        {
            this.StartTime = startTime;
            return this;
        }

        #endregion Modify Options

        protected void OnFirstUpdate()
        {
            if (!_fromValueSet && _defaultFromValue != null)
            {
                _fromValueSet = true;
                _fromValue = _defaultFromValue(Target);
            }
            if (!_toValueSet && _defaultToValue != null)
            {
                _toValueSet = true;
                _toValue = _defaultToValue(Target);
            }
        }

        protected override void OnUpdate()
        {
            if (!_firstUpdate)
            {
                _firstUpdate = true;
                OnFirstUpdate();
            }
            _tweenFunction(this);
        }
    }
}