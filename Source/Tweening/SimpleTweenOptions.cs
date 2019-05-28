using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace FlaxMinesweeper.Source.Tweening
{
    public enum LerpType
    {
        Lerp,
        Slerp
    }

    public enum LoopType
    {
        Loop,
        PingPong
    }

    public class SimpleTweenLoopOptions
    {
        public int Repetitions = 1;
    }

    public class SimpleTweenOptions
    {
        public LoopType LoopType = LoopType.Loop; // Used
        public bool IsLocal = true;
        public bool IsPhysics = false;
        public bool Reversed = false;
        public LerpType LerpType = LerpType.Lerp; // Used
        public AlphaBlendMode EasingType = AlphaBlendMode.Linear; // Used

        /*
    // TODO: Or refactor to events?
    // I don't have to worry about disposing them:

    public Action TweenStarted;//done

    public Action<float> TweenUpdate;//done

    public Action TweenEnded;*/
    }
}