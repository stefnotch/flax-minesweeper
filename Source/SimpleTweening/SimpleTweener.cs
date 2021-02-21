using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace SimpleTweening
{
    public class SimpleTweener : SimpleTweenSequenceElement
    {
        private float _localTime;
        private float _duration;
        private bool _isPaused;
        private float _pauseDuration;
        private float _currentPauseStartTime;

        private float _previousPercentage;
        private float _percentage;

        /*Tweening: Callbacks/events for
       - Starting?
       - Updating?
       - Pause?
       - End!


       OnComplete(TweenCallback callback)
       OnKill(TweenCallback callback)
       OnPlay(TweenCallback callback)
       OnPause(TweenCallback callback)
       OnRewind(TweenCallback callback)
       OnStart(TweenCallback callback)
       OnStepComplete(TweenCallback callback)
       OnUpdate(TweenCallback callback)
       OnWaypointChange(TweenCallback<int> callback)*/

        public SimpleTweener(SimpleTweenSequence sequence = null) : base(sequence)
        {
        }

        /// <summary>
        /// Time offset
        /// </summary>
        public float LocalTime { get => _localTime; protected set => _localTime = value; }

        /// <summary>
        /// Global Time
        /// </summary>
        public float Duration { get => _duration; set { _duration = value; } }

        /// <summary>
        /// Global Time
        /// </summary>
        public override float TotalDuration => Duration * LoopCount + PauseDuration;

        /// <summary>
        /// On the parent's timeline
        /// </summary>
        public float EndTime => StartTime + PauseDuration + Duration;

        /// <summary>
        /// If this tween is paused
        /// </summary>
        public bool IsPaused { get => _isPaused; set { PausedChanged(value, _isPaused); _isPaused = value; } }

        /// <summary>
        /// How long has this tween been paused for
        /// </summary>
        public float PauseDuration { get => _pauseDuration; protected set { _pauseDuration = value; } }

        /// <summary>
        /// The percentage before applying the tween function
        /// </summary>
        public float RawPercentage { get => LocalTime / Duration; /*TODO: Setter*/ }

        /// <summary>
        /// The previous percentage
        /// </summary>
        public float PreviousPercentage { get => _previousPercentage; protected set => _previousPercentage = value; }

        /// <summary>
        /// Percentage from 0 to 1, both inclusive
        /// </summary>
        public float Percentage { get => _percentage; protected set => _percentage = value; }

        /// <summary>
        /// Tweening options
        /// </summary>
        public SimpleTweenOptions Options { get; set; } = new SimpleTweenOptions();

        public override void Update(float parentTime)
        {
            if (parentTime < StartTime) return;
            if (LoopCount <= 0) return;
            if (IsPaused) return;

            LocalTime = parentTime - StartTime - PauseDuration;
            PreviousPercentage = Percentage;

            bool isDone = EndTime < parentTime;
            if (isDone)
            {
                Percentage = Options.Reversed ? 0 : 1;
            }
            else
            {
                // Interpolates and saturates the value
                Percentage = Mathf.InterpolateAlphaBlend(RawPercentage, Options.EasingType);
                Percentage = Options.Reversed ? 1 - Percentage : Percentage;
            }

            OnUpdate();

            if (isDone)
            {
                OnDone();
            }
        }

        /// <summary>
        /// Restarts an animation
        /// </summary>
        public SimpleTweener Restart()
        {
            if (LoopCount <= 0) LoopCount = 1;
            StartTime = ParentTime;
            IsPaused = false;
            PauseDuration = 0;
            PreviousPercentage = 0;
            return this;
        }

        /// <summary>
        /// Forcibly finishes an animation
        /// </summary>
        public SimpleTweener Finish()
        {
            // I'm just going to do a finish update and let the update function handle this
            Update(EndTime);
            return this;
        }

        /// <summary>
        /// Cancels an animation
        /// </summary>
        public SimpleTweener Cancel()
        {
            // Just remove ourselves from the sequence
            Sequence = null;
            return this;
        }

        protected virtual void OnUpdate()
        {
        }

        protected void OnDone()
        {
            LoopCount--;
            if (LoopCount > 0)
            {
                if (Options.LoopType == LoopType.PingPong)
                {
                    Options.Reversed = !Options.Reversed;
                }
                float endTime = EndTime;
                Restart();
                StartTime = endTime;
            }
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
    }

    public class SimpleTweener<T> : SimpleTweener
    {
        public delegate void TweenFunctionDelegate(float percentage, SimpleTweener<T> tweener);

        public SimpleTweener(SimpleTweenSequence sequence,
                             TweenFunctionDelegate tweenFunction,
                             float duration,
                             T fromValue = default(T),
                             T toValue = default(T)) : base(sequence)
        {
            TweenFunction = tweenFunction;
            Duration = duration;
            FromValue = fromValue;
            ToValue = toValue;
        }

        public SimpleTweener(TweenFunctionDelegate tweenFunction,
                             T fromValue = default(T),
                             T toValue = default(T)) : this(null, tweenFunction, fromValue, toValue)
        {
        }

        public TweenFunctionDelegate TweenFunction { get; set; }

        public T FromValue { get; set; }

        public T ToValue { get; set; }

        protected override void OnUpdate()
        {
            TweenFunction(Percentage, this);
        }
    }
}