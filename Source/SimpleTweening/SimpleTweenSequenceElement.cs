using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace SimpleTweening
{
    public abstract class SimpleTweenSequenceElement : IComparable<SimpleTweenSequenceElement>
    {
        private SimpleTweenSequence _sequence;
        private int _loopCount = 1;
        private float _startTime;

        protected SimpleTweenSequenceElement(SimpleTweenSequence sequence)
        {
            Sequence = sequence;
        }

        /// <summary>
        /// The sequence this tween belongs to
        /// </summary>
        public SimpleTweenSequence Sequence { get => _sequence; set { SequenceChanged(value, _sequence); _sequence = value; } }

        /// <summary>
        /// How often this tween should repeat
        /// </summary>
        /// <remarks>Zero means that this tween is done</remarks>
        public int LoopCount { get => _loopCount; set { _loopCount = value; } }

        /// <summary>
        /// On the parent's timeline
        /// </summary>
        public float StartTime { get => _startTime; set { _startTime = value; } }

        /// <summary>
        /// Global Time
        /// </summary>
        public abstract float TotalDuration { get; }

        /// <summary>
        /// The parent's time
        /// </summary>
        public float ParentTime => Sequence != null ? Sequence.LocalTime : Time.GameTime;


        /// <summary>
        /// Updates the tween
        /// </summary>
        /// <param name="time">Parent's local time</param>
        public abstract void Update(float parentTime);

        protected virtual void OnChildRemoved(SimpleTweenSequenceElement child) { }

        protected virtual void OnChildAdded(SimpleTweenSequenceElement child) { }

        protected void SequenceChanged(SimpleTweenSequence newSequence, SimpleTweenSequence oldSequence)
        {
            if (newSequence != oldSequence)
            {
                oldSequence?.OnChildRemoved(this);
                newSequence?.OnChildAdded(this);
            }

            StartTime = ParentTime;
        }

        public int CompareTo(SimpleTweenSequenceElement other)
        {
            return StartTime.CompareTo(other.StartTime);
        }
    }
}
