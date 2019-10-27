using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace SimpleTweening
{
    public abstract class SimpleTweenable : IComparable<SimpleTweenable>
    {
        private float _startTime;
        private float _localTime;
        private float _duration;
        private bool _isPaused;
        private float _pauseDuration;

        private float _rawPercentage;
        private float _previousPercentage;
        private float _percentage;

        private float _currentPauseStartTime;
        private SimpleTweenSequence _parent;

        // TODO: https://github.com/thomaslevesque/WeakEvent
        protected event Action<SimpleTweenable> EndEvent;
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

        public SimpleTweenable(SimpleTweenSequence parent = null)
        {
            Sequence = parent;

            // When the parent exists, use its time
            // Otherwise, this tween is attached to a script. Then, we can use the game time
            StartTime = (parent != null) ? parent.LocalTime : Time.GameTime;
        }

        /// <summary>
        /// The sequence this tween belongs to
        /// </summary>
        public SimpleTweenSequence Sequence { get => _parent; set { ParentChanged(value, _parent); _parent = value; } }

        /// <summary>
        /// On the parent's timeline
        /// </summary>
        public float StartTime { get => _startTime; set { _startTime = value; } }

        /// <summary>
        /// Time offset
        /// </summary>
        public float LocalTime { get => _localTime; protected set => _localTime = value; }

        /// <summary>
        /// Global Time
        /// </summary>
        public float Duration { get => _duration; set { _duration = value; } }

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
        public float RawPercentage { get => _rawPercentage; protected set => _rawPercentage = value; }

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

        public float EndTime => StartTime + PauseDuration + Duration;
        public bool IsReversedLoop => Options.Reversed ^ (Options.LoopType == LoopType.PingPong && RawPercentage % 2 > 1);

        /// <summary>
        /// Updates the tween
        /// </summary>
        /// <param name="time">Parent LocalTime</param>
        public void Update(float parentTime)
        {
            if (IsPaused) return;
            if (parentTime < StartTime) return;

            LocalTime = parentTime - StartTime - PauseDuration;
            RawPercentage = LocalTime / Duration;

            PreviousPercentage = Percentage;

            // TODO: Are you sure?
            bool isDone = EndTime < parentTime;
            if (isDone)
            {
                Percentage = IsReversedLoop ? 0 : 1;
            }
            /*else if (RawPercentage % 1 == 0)
            {
                Percentage = 0;
            }*/
            else
            {
                float percentage = RawPercentage % 1f;

                // Interpolates and saturates the value
                Percentage = Mathf.InterpolateAlphaBlend(percentage, Options.EasingType);
                Percentage = IsReversedLoop ? 1 - Percentage : Percentage;
            }
            // End of // TODO: Are you sure?

            OnUpdate();

            if (isDone)
            {
                OnDone();
            }
        }

        // TODO: Return self
        // So that you can write .Cancel() /*cancel the previous garbage*/ .MoveTo() /*do this instead*/

        /// <summary>
        /// Forcibly finishes an animation
        /// </summary>
        public virtual void Finish()
        {
            // I'm just going to do a finish update and let the update function handle this
            Update(EndTime);
        }

        public virtual void Cancel()
        {
            throw new NotImplementedException();
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

        protected abstract void OnUpdate();

        protected virtual void OnDone()
        {
            EndEvent?.Invoke(this);
        }

        protected virtual void OnChildRemoved(SimpleTweenable child) { }

        protected virtual void OnChildAdded(SimpleTweenable child) { }

        protected void ParentChanged(SimpleTweenSequence newParent, SimpleTweenSequence oldParent)
        {
            if (newParent != oldParent)
            {
                oldParent?.OnChildRemoved(this);
                newParent?.OnChildAdded(this);
            }
        }

        public int CompareTo(SimpleTweenable other)
        {
            return StartTime.CompareTo(other.StartTime);
        }





        /*
         
        private float _time;

        private float _pausedDuration = 0;
        private bool _isFinished = false;
        private int _loopCounter = 0;
        private bool _hasFirstUpdate = false;


        public float TotalDuration => Duration * LoopOptions.Repetitions;

        public float EndTime => StartTime + _pausedDuration + TotalDuration;

        public float PausedDuration => _pausedDuration;

        public bool IsFinished => _isFinished;



        public int LoopCounter
        {
            get => _loopCounter;
            set
            {
                SetPercentage(_percentage, value);
                _loopCounter = value;
            }
        }

        public bool IsReversedLoop => LoopOptions.LoopType == LoopType.PingPong && _loopCounter % 2 == 1;


        public float Percentage
        {
            // Warning: Do not use this property internally
            get => (this.IsReversedLoop) ? 1 - _percentage : _percentage;
            set
            {
                if (this.IsReversedLoop) value = 1 - value;
                SetPercentage(value, _loopCounter);
                _percentage = value;
            }
        }

        public SimpleTweenLoopOptions LoopOptions { get; } = new SimpleTweenLoopOptions();

        public float TotalTime
        {
            get => _time;
            protected set
            {
                _time = value;

                // Update Percentage
                float elapsedTime = _time - PausedDuration;
                _percentage = elapsedTime / Duration;
                float loopC = Mathf.Floor(_percentage);
                _loopCounter = (int)loopC;
                _percentage = _percentage - loopC;
            }
        }






        /// <summary>
        /// Updates the tween
        /// </summary>
        /// <param name="time">Parent Time minus Start Time</param>
        public void Update(float time)
        {
            if (_isFinished) return;
            if (_isPaused) return;
            if (time < 0) return;

            TotalTime = time;

            if (_loopCounter >= LoopOptions.Repetitions)
            {
                _percentage = 1;
                Finish();
            }
            else
            {
                if (!_hasFirstUpdate)
                {
                    _hasFirstUpdate = true;
                    OnFirstUpdate();
                }
                OnUpdate();
            }
        }

        protected abstract void OnUpdate();

        protected abstract void OnFirstUpdate();

        public virtual void Cancel()
        {
            if (_isFinished) return;
            _isFinished = true;
        }

        public virtual void Finish()
        {
            if (_isFinished) return;
            _isFinished = true;

            // Last execution, set the percentage to one of the 2 edge-cases
            if (_percentage > 0.5)
            {
                _percentage = 1;
            }
            else
            {
                _percentage = 0;
            }

            if (!_hasFirstUpdate)
            {
                _hasFirstUpdate = true;
                OnFirstUpdate();
            }

            OnUpdate();
        }

        public virtual void Repeat()
        {
            if (_isFinished) return;
            _percentage = 0;
            _loopCounter = 0;
            SetPercentage(0, 0);
        }

        private void PausedChanged(bool isPausedNew, bool isPausedOld)
        {
            if (isPausedNew != isPausedOld)
            {
                // Paused state has changed
                // It's ok to use FlaxEngine.Time.GameTime, because we're only using it to calculate a time **delta**
                if (isPaused)
                {
                    _currentPauseStartTime = FlaxEngine.Time.GameTime;
                }
                else
                {
                    _pausedDuration += FlaxEngine.Time.GameTime - _currentPauseStartTime;
                }
            }
        }

        /// <summary>
        /// Makes sure that the next percentage calculation will yield the desired percentage
        /// </summary>
        /// <param name="percentage">Desired percentage</param>
        private void SetPercentage(float percentage, int loopCount)
        {
            if (_isPaused)
            {
                _currentPauseStartTime = FlaxEngine.Time.GameTime;
            }

            /* Quick Maffs:
             * float elapsedTime = (FlaxEngine.Time.GameTime - StartTime) + PausedDuration;
             * Percentage = elapsedTime / Duration; <=== percentage
             *
             * Percentage = ((FlaxEngine.Time.GameTime - StartTime) + PausedDuration) / Duration;
             *
             * Percentage * Duration - (FlaxEngine.Time.GameTime - StartTime) === PausedDuration;
             *

            _pausedDuration = (percentage + loopCount) * Duration - (FlaxEngine.Time.GameTime - StartTime);
        }*/
    }
}