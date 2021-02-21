using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlaxEngine;

namespace SimpleTweening
{
    public static class SimpleTween
    {
        private static SimpleTweenSequence GetSequence(Actor actor)
        {
            var tweenScript = actor.GetScript<SimpleTweenScript>();
            if (!tweenScript)
            {
                tweenScript = actor.AddScript<SimpleTweenScript>();
                if (!tweenScript) throw new FlaxException($"Creating a {nameof(SimpleTweenScript)} failed");
            }
            return tweenScript.TweenSequence;
        }

        public static SimpleTweenSequence Tween(Actor actor)
        {
            return GetSequence(actor).NewSequence();
        }

        public static SimpleTweener<Vector3> MoveTo(Actor actor, Vector3 to, float duration, float? startDelay = null)
        {
            return Tween(actor).MoveTo(to, duration, startDelay);
        }

        public static SimpleTweenAction<Actor, Quaternion> RotateTo(Actor actor, Quaternion to, float duration)
        {
            return Tween(actor).RotateTo(to, duration);
        }

        public static SimpleTweenAction<Actor, Vector3> ScaleTo(Actor actor, Vector3 to, float duration)
        {
            return Tween(actor).ScaleTo(to, duration);
        }

        public static SimpleTweenSequence CreateSequence()
        {
            return new SimpleTweenSequence(null);
        }

        public static T Add<T>(T simpleTweenable, Actor actor) where T : SimpleTweenSequenceElement
        {
            var seq = Tween(actor);
            return seq.Add(simpleTweenable);
        }

        /* TODO: 
public static SimpleTweenSequence<U> Wait<U>()
{
    throw new NotImplementedException();
    var script = new SimpleTweenScript();
    return null;
}*/

        /* TODO:
         public static ??? In(Action<..> toExecute)
{
    throw new NotImplementedException();
    var script = new SimpleTweenScript();
    return null;
}*/
    }

    public class SimpleTweenScript : Script
    {
        private int _removalTimeout = 0; // TODO: Use this as a timeout for removing this script

        public SimpleTweenSequence TweenSequence { get; private set; } = new SimpleTweenSequence(null);

        public override void OnEnable()
        {
        }

        public override void OnUpdate()
        {
            TweenSequence.Update(Time.GameTime);
            /*if(_tweenSequence.IsFinished)
            {
            }*/
        }

        public override void OnDestroy()
        {
            TweenSequence.Cancel();
            TweenSequence = null;
            //this.Actor.RemoveScript(this);
            //FlaxEngine.Object.Destroy(this);
        }
    }

}