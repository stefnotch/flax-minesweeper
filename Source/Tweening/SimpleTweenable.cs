using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace FlaxMinesweeper.Source.Tweening
{
    public abstract class SimpleTweenable
    {
        private float _startTime;
        private float _scale = 1f;
        private float _duration;
        private float _parentTime;
        private float _pauseTime;
        //private float _unscaledLocalTime; // Computed on the fly
        //private bool _hasFirstUpdate // TODO: If I need it
        private float _localTime;
        private float _rawPercentage;
        private float _percentage;

        private bool _isPaused;
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
            if (parent == null)
            {
                // e.g. parent == Script
                this.StartTime = Time.GameTime;
            }
            else
            {
                // parent == sequence
                this.StartTime = parent.LocalTime;
            }
        }

        public float StartTime { get => _startTime; set => _startTime = value; }
        public float Scale { get => _scale; set => _scale = value; }
        public float Duration { get => _duration; set => _duration = value; }
        public float ParentTime { get => _parentTime; protected set => _parentTime = value; }
        public float UnscaledLocalTime { get => _localTime / Scale; set => _localTime = value * Scale; }
        public float LocalTime { get => _localTime; protected set => _localTime = value; }
        public float RawPercentage { get => _rawPercentage; protected set => _rawPercentage = value; }
        /// <summary>
        /// From 0 to 1, both inclusive
        /// </summary>
        public float Percentage { get => _percentage; protected set => _percentage = value; }
        public bool IsPaused { get => _isPaused; set { PausedChanged(value, _isPaused); _isPaused = value; } }
        public float PauseTime { get => _pauseTime; protected set => _pauseTime = value; }

        public SimpleTweenOptions Options { get; set; } = new SimpleTweenOptions();


        public float EndTime => StartTime + PauseTime + Duration;
        public bool IsReversedLoop => Options.Reversed ^ (Options.LoopType == LoopType.PingPong && RawPercentage % 2 > 1);
        public bool Done => ParentTime >= EndTime;
        public SimpleTweenSequence Sequence { get => _parent; set { ParentChanged(value, _parent); _parent = value; } }

        /// <summary>
        /// Updates the tween
        /// </summary>
        /// <param name="time">Parent LocalTime</param>
        public void Update(float parentTime)
        {
            if (IsPaused) return;
            if (parentTime < StartTime) return;
            /* TODO: parentTime < StartTime: or set the Percentage to 0? Or set the Percentage to a negative value?;*/

            ParentTime = parentTime;
            LocalTime = (ParentTime - StartTime - PauseTime) * Scale;
            RawPercentage = LocalTime / Duration;

            bool isDone = Done;
            if (isDone)
            {
                Percentage = IsReversedLoop ? 0 : 1;
            }
            else if (RawPercentage % 1 == 0)
            {
                Percentage = 0;
            }
            else
            {
                // Interpolates and saturates the value
                Percentage = Mathf.InterpolateAlphaBlend(RawPercentage % 1, Options.EasingType);
                Percentage = IsReversedLoop ? 1 - Percentage : Percentage;
            }

            OnUpdate();

            if (isDone)
            {
                EndEvent?.Invoke(this);
            }
        }

        // TODO: Return self
        // So that you can write .Cancel() /*cancel the previous garbage*/ .MoveTo() /*do this instead*/

        /// <summary>
        /// Forcibly finishes an animation
        /// </summary>
        public virtual void Finish()
        {
            // TODO: What if the animation hasn't started yet (if (parentTime < StartTime) return;)

            // TODO: Should I mess around with the StartTime or the Duration?

            // TODO: Notify parent that this animation has been finished?

            // TODO: Or should I wait until the next Update() call?

            // I'm just going to force a finish update and then cancel the animation
            Update(EndTime);
            Cancel();
        }

        public virtual void Cancel()
        {
            // Notify parent that this animation has been cancelled
            Sequence?.ChildRemoved(this);
        }

        protected virtual void FinishChild(SimpleTweenable child)
        {

        }

        protected virtual void CancelChild(SimpleTweenable child)
        {

        }

        protected virtual void ChildAdded(SimpleTweenable child)
        {

        }

        protected virtual void ChildRemoved(SimpleTweenable child)
        {

        }

        private void PausedChanged(bool isPausedNew, bool isPausedOld)
        {
            if (isPausedNew != isPausedOld)
            {
                // Paused state has changed
                if (isPausedNew)
                {
                    _currentPauseStartTime = ParentTime;
                }
                else
                {
                    PauseTime += ParentTime - _currentPauseStartTime;
                }
            }
        }

        protected void ParentChanged(SimpleTweenSequence newParent, SimpleTweenSequence oldParent)
        {
            if (newParent != oldParent)
            {
                ParentTime = newParent?.LocalTime ?? Time.GameTime;
                oldParent?.ChildRemoved(this);
                newParent?.ChildAdded(this);
            }
        }

        protected abstract void OnUpdate();





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