using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxMinesweeper.Source.Tweening;

namespace FlaxMinesweeper.Source.Tweening
{
    public static class SimpleTween
    {
        private static SimpleTweenSequence<U> GetSequence<U>(U actor) where U : Actor
        {
            // TODO: Performance of this code?
            var tweenScript = actor.GetScript<SimpleTweenScript>();
            if (!tweenScript)
            {
                tweenScript = actor.AddScript<SimpleTweenScript>();
                if (!tweenScript) throw new FlaxException($"Creating a {nameof(SimpleTweenScript)} failed");
                tweenScript.Init<U>();
            }
            return tweenScript.TweenSequence as SimpleTweenSequence<U>;
        }

        public static SimpleTweenSequence<U> Tween<U>(this U actor) where U : Actor
        {
            // TODO: ^ Destroy that sequence when the kids are done. For that: Create a new class: ContainerSequence
            return GetSequence(actor).NewSequence();
        }

        public static SimpleTweenAction<Actor, Vector3> MoveTo(Actor actor, Vector3 to, float duration)
        {
            return Tween(actor).MoveTo(to, duration);
        }

        public static SimpleTweenAction<Actor, Quaternion> RotateTo(Actor actor, Quaternion to, float duration)
        {
            return Tween(actor).RotateTo(to, duration);
        }

        public static SimpleTweenAction<Actor, Vector3> ScaleTo(Actor actor, Vector3 to, float duration)
        {
            return Tween(actor).ScaleTo(to, duration);
        }

        public static SimpleTweenSequence<U> CreateSequence<U>() where U : Actor
        {
            return new SimpleTweenSequence<U>(null);
            throw new NotImplementedException("I'm attempting to pass null to the SimpleTweenSequence..");
        }

        public static SimpleTweenSequence<U> Add<U>(SimpleTweenable simpleTweenable, U actor) where U : Actor
        {
            // TODO: Optimize the case where the actor doesn't have a SimpleTweenScript??
            var seq = GetSequence(actor);
            seq.Add(simpleTweenable);
            return seq;
        }

        /* TODO: 
public static SimpleTweenSequence<U> Wait<U>()
{
    throw new NotImplementedException();
    var script = new SimpleTweenScript();
    return null;
}*/
    }

    public class SimpleTweenScript : Script
    {
        private SimpleTweenSequence _tweenSequence;
        private int _deadCounter = 0; // TODO: Use this as a timeout for removing this script

        public SimpleTweenSequence<U> Init<U>() where U : Actor
        {
            var sequence = new SimpleTweenSequence<U>(this.Actor as U);
            _tweenSequence = sequence;
            return sequence;
        }

        public SimpleTweenSequence TweenSequence => _tweenSequence;

        private void OnEnable()
        {
        }

        private void Update()
        {
            _tweenSequence.Update(Time.GameTime);
            /*if(_tweenSequence.IsFinished)
            {
            }*/
        }

        private void OnDestroy()
        {
            _tweenSequence.Cancel();
            //this.Actor.RemoveScript(this);
            //FlaxEngine.Object.Destroy(this);
        }
    }

}