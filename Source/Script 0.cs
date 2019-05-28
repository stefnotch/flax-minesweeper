using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxMinesweeper.Source;
using FlaxMinesweeper.Source.Tweening;

namespace FlaxMinesweeper
{
    public class Script0 : Script
    {
        private void Start()
        {
            // Types: Get/Creates a SimpleTweenScript
            // this.Actor.Tween() and SimpleTween(this.Actor)

            // Types: Creates a new SimpleTweenAction { script = SimpleTweenScript, settings = SimpleTweenSettings }
            // MoveTo, ScaleTo, (Wait?)

            // Modifies the current action.settings (Prefix with Set? Like SetReverse, SetRepeat)
            // Reverse, Repeat, Wait (modifies the delay AFTER the end? Or should we think up a better name so that ti can modify the delay **before** the start)

            // TODO: .Sequence()?
            // 0
            SimpleTween
                .Tween(this.Actor)
                .MoveTo(new Vector3(0, -100, 0), 2);
            return;

            // 1
            this.Actor.Tween()
                .MoveTo(new Vector3(0, -100, 0), 2)
                .SetRepetitions(2);

            // 2
            SimpleTween
                .Tween(this.Actor)
                .MoveTo(new Vector3(0, -100, 0), 2)
                .SetRepetitions(2);

            // 3
            SimpleTween
                .MoveTo(this.Actor, new Vector3(0, -100, 0), 2);

            // 4
            SimpleTween
                .MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                .SetFrom(new Vector3(0, 0, 0))
                .SetReversed()
                //.Chain()
                .MoveTo(new Vector3(0, -100, 0), 2);

            // 5
            var x = SimpleTween
                .MoveTo(this.Actor, new Vector3(0, -100, 0), 2);

            SimpleTween
                .Tween(this.Actor)
                .Wait(3, () => x.Cancel());

            // 6

            // 7
            var y = SimpleTween
                .MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                .SetFrom(new Vector3(0, 0, 0))
                .SetReversed()
                //.Chain()
                .MoveTo(new Vector3(0, -100, 0), 2);

            y.Cancel();

            // 8
            SimpleTween
                .Tween(this.Actor)
                .Finish();

            // 9
            var x1 = SimpleTween
                .MoveTo(this.Actor, new Vector3(0, -100, 0), 2);

            //x1.Percentage = 0.5f;

            // 10
            SimpleTween
                .MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                .Wait(2) // Acts like .Sequence()? (At least the return value does?)
                .MoveTo(new Vector3(0, -100, 0), 2); // Uh...

            // 11
            //SimpleTween
            //	.MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
            //	.OnFinished((x2) => x2.Repeat());

            // 12
            SimpleTween
                .MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                .MoveTo(new Vector3(0, -100, 0), 2)
                .MoveTo(new Vector3(0, -100, 0), 2)
                .Sequence
                .SetRepetitions(3);

            // 13
            var seq = SimpleTween
                .CreateSequence<Actor>()
                .MoveTo(new Vector3(0, -100, 0), 2)
                .MoveTo(new Vector3(0, -100, 0), 2)
                .MoveTo(new Vector3(0, -100, 0), 2)
                .Sequence
                .SetRepetitions(3);

            SimpleTween
                .Add(seq, this.Actor);

            Debug.Log("BeforeAdd");
            Actor.AddScript<TestEnable>();
            Debug.Log("AfterAdd");

            /*Actor
                .Tween()*/
            /*SimpleTween
                .RotateTo(Actor, Quaternion.RotationY(Mathf.Pi), 3.6f);
            SimpleTween
                .In(5, () =>
                {
                    SimpleTween
                        .RotateTo(Actor, Quaternion.RotationY(Mathf.PiOverTwo), 3f);

                    SimpleTween
                        .MoveTo(Actor, new Vector3(0, -100, 0), 2);
                    SimpleTween
                        .ScaleTo(Actor, new Vector3(2), 2);
                });
                */
            // Here you can add code that needs to be called when script is created
            //var ta = TweenAction<Vector3>.Create(Vector3.Zero, Vector3.One, 1, (a, b, c) => a);

            //this.Actor.AddScript(ta);

            //var test = New<GenericScript<Vector3>>();
            //this.Actor.AddScript(test);

            //var otherTest = GenericScript<Vector3>.Make();
            //this.Actor.AddScript(otherTest);

            //var otherTest = GenericScript<Vector3>.Make<Vector3>();
            //this.Actor.AddScript(otherTest);
        }
    }

    internal class TestEnable : Script
    {
        private void OnEnable()
        {
            Debug.Log("enabled!");
        }

        private void Start()
        {
            Debug.Log("start");
        }

        private void OnDisable()
        {
            Debug.Log("disabled");
        }
    }
}