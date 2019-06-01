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

    public class SimpleTweenOptions
    {
        public LoopType LoopType = LoopType.Loop; // Used
        public bool IsLocal = true;
        public bool IsPhysics = false;
        public bool IsAdditive = false;
        public bool Reversed = false; // Used
        public LerpType LerpType = LerpType.Lerp; // Used
        public AlphaBlendMode EasingType = AlphaBlendMode.Linear; // Used
    }
}