using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTweening;

namespace FlaxMinesweeper.Source.SimpleTweening
{
    public class SimpleTweenAction : SimpleTweenSequenceElement
    {

        public SimpleTweenAction(SimpleTweenSequence sequence, Action<SimpleTweenAction> action) : base(sequence)
        {
            Action = action;
        }

        public SimpleTweenAction(Action<SimpleTweenAction> action) : this(null, action)
        {
        }

        public Action<SimpleTweenAction> Action { get; set; }

        public override float TotalDuration => 0;

        public override void Update(float parentTime)
        {
            if (parentTime < StartTime) return;
            if (LoopCount <= 0) return;

            Action(this);

            OnDone();
        }


        /// <summary>
        /// Restarts an action
        /// </summary>
        public SimpleTweenAction Restart()
        {
            if (LoopCount <= 0) LoopCount = 1;
            StartTime = ParentTime;
            return this;
        }

        /// <summary>
        /// Finishes an action
        /// </summary>
        public SimpleTweenAction Finish()
        {
            // I'm just going to do a finish update and let the update function handle this
            Update(StartTime);
            return this;
        }

        /// <summary>
        /// Cancels an action
        /// </summary>
        public SimpleTweenAction Cancel()
        {
            // Just remove ourselves from the sequence
            Sequence = null;
            return this;
        }

        protected void OnDone()
        {
            LoopCount--;
            if (LoopCount > 0)
            {
                float endTime = StartTime;
                Restart();
                StartTime = endTime;
            }
        }
    }
}
