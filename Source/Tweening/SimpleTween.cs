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
            var newSequence = GetSequence(actor).NewSequence();
            newSequence.Duration = float.PositiveInfinity; // Unlike the parent sequence, this one is susceptible to being removed
            // TODO: ^ Destroy that sequence when the kids are done. For that: Create a new class: ContainerSequence
            return newSequence;
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

        /*
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

    /*
    public static class SimpleTweenOld
    {
        public static void In(float timeInSeconds, Action action)
        {
            if (timeInSeconds > 0)
            {
                Task.Delay(TimeSpan.FromSeconds(timeInSeconds)).ContinueWith((_) => action());
            }
            else
            {
                action();
            }
        }

        // Returns a TweenAction
        public static TweenAction<Vector3, Actor> ScaleTo(this Actor actor, Vector3 to, float duration)
        {
            // TODO: Rigidbody handling

            return AddTweenAction(actor, new TweenAction<Vector3, Actor>(actor, ComputeScaleTo, duration)
            {
                From = actor.LocalScale,
                To = to
            });
        }

        private static void ComputeScaleTo(TweenAction<Vector3, Actor> tweenAction)
        {
            tweenAction.TargetObject.LocalScale = Vector3.Lerp(tweenAction.From, tweenAction.To, tweenAction.Percentage);
        }

        public static TweenAction<Quaternion, Actor> RotateTo(this Actor actor, Quaternion to, float duration)
        {
            // TODO: Rigidbody handling
            return AddTweenAction(actor, new TweenAction<Quaternion, Actor>(actor, ComputeRotateTo, duration)
            {
                From = actor.LocalOrientation,
                To = to
            });
        }

        private static void ComputeRotateTo(TweenAction<Quaternion, Actor> tweenAction)
        {
            tweenAction.TargetObject.LocalOrientation = Quaternion.Lerp(tweenAction.From, tweenAction.To, tweenAction.Percentage);
        }

        public static TweenAction<Vector3, Actor> MoveTo(this Actor actor, Vector3 to, float duration)
        {
            // TODO: Rigidbody handling
            return AddTweenAction(actor, new TweenAction<Vector3, Actor>(actor, ComputeMoveTo, duration)
            {
                From = actor.LocalPosition,
                To = to
            });
        }

        public static void ValueTo<T>(this Actor actor, T from, T to, float duration)
        {
            var tweenScript = FlaxEngine.Object.New<TweenActionScript>();
            tweenScript.TweenAction = new TweenAction<T, Actor>(actor, ComputeValueTo<T>, duration)
            {
                From = from,
                To = to
            };

            actor.AddScript(tweenScript);
        }

        private static void ComputeValueTo<T>(TweenAction<T, Actor> obj)
        {
            throw new NotImplementedException();
        }

        private static void ComputeMoveTo(TweenAction<Vector3, Actor> tweenAction)
        {
            tweenAction.TargetObject.LocalPosition = Vector3.Lerp(tweenAction.From, tweenAction.To, tweenAction.Percentage);
        }

        private static TweenAction<T, U> AddTweenAction<T, U>(Actor actor, TweenAction<T, U> tweenAction)
        {
            var tweenScript = FlaxEngine.Object.New<TweenActionScript>();
            tweenScript.TweenAction = tweenAction;
            actor.AddScript(tweenScript);

            return tweenAction;
        }
    }*/
}