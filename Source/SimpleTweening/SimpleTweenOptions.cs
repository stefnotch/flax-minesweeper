using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;

namespace SimpleTweening
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

    public class SimpleTweenOptions
    {
        public LoopType LoopType = LoopType.Loop; // Done
        public bool Reversed = false;

        public bool IsLocal = true;
        public bool IsPhysics = false;
        public bool IsAdditive = false;
        public LerpType LerpType = LerpType.Lerp;
        public AlphaBlendMode EasingType = AlphaBlendMode.Linear; // Done
    }
}