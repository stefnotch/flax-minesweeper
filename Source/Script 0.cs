using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxMinesweeper.Source;
using FlaxMinesweeper.Source.Tweening;

namespace FlaxMinesweeper
{
    public class Script0 : Script
    {
        [NoSerialize]
        public Dictionary<string, Action> Actions = new Dictionary<string, Action>();

        private void Start()
        {
            // 0
            Actions.Add("Reset", () =>
            {
                SimpleTween
                    .Tween(this.Actor)
                    .Sequence
                    .Cancel();

                SimpleTween
                    .Tween(this.Actor)
                    .MoveTo(new Vector3(0, 0, 0), 0);
            });
            // 1
            Actions.Add("Simple Move and repeat", () =>
            {
                this.Actor.Tween()
                    .MoveTo(new Vector3(0, -100, 0), 2)
                    .SetRepetitions(2);
            });
            // 2
            Actions.Add("Simple Move and repeat (API)", () =>
            {
                SimpleTween
                    .Tween(this.Actor)
                    .MoveTo(new Vector3(0, -100, 0), 2)
                    .SetRepetitions(2);
            });
            // 3
            Actions.Add("Simple Move (API)", () =>
            {
                SimpleTween
                    .MoveTo(this.Actor, new Vector3(0, -100, 0), 2);
            });
            // 4
            Actions.Add("Chain Move and reverse", () =>
            {
                SimpleTween
                    .MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                    .SetFrom(new Vector3(0, 0, 0))
                    .SetReversed()
                    //.Chain()
                    .MoveTo(new Vector3(0, -100, 0), 2);
            });
            // 5
            Actions.Add("Simple Move and cancel", () =>
            {
                var x = SimpleTween
                    .MoveTo(this.Actor, new Vector3(0, -100, 0), 2);

                // This has to create a new sequence. Discussion's over. 
                // Otherwise, this would happen AFTER the MoveTo. Which is weird.
                SimpleTween
                    .Tween(this.Actor)
                    .Wait(1, (_) => x.Cancel());
            });
            // 6
            Actions.Add("Start time with startDelay", () =>
            {
                var y = SimpleTween
                    .MoveTo(this.Actor, new Vector3(0, -100, 0), duration: 2) // Move for 2 seconds
                    .ScaleTo(new Vector3(2, 2, 1), duration: 1, startDelay: 0) // While scaling the object up
                    .ScaleTo(new Vector3(1, 1, 1), duration: 1); // Now scale the object down. This happens after **the previous tween**
            });

            // 7
            Actions.Add("Chain Move and instant cancel", () =>
            {
                var y = SimpleTween
                    .MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                    .SetFrom(new Vector3(0, 0, 0))
                    .SetReversed()
                    //.Chain()  
                    .MoveTo(new Vector3(-100, -100, 0), 2);

                y.Sequence.Cancel();
            });
            // 8
            Actions.Add("Tween and instant finish", () =>
            {
                SimpleTween
                    .Tween(this.Actor)
                    .Finish();
            });
            // 9
            Actions.Add("NOT IMPLEMENTED Set Percentage", () =>
            {
                var x1 = SimpleTween
                    .MoveTo(this.Actor, new Vector3(0, -100, 0), 2);

                //x1.Percentage = 0.5f;
            });
            // 10
            Actions.Add("Move, wait and move", () =>
            {
                SimpleTween
                    .MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                    .Wait(2) // Acts like .Sequence()? (At least the return value does?)
                    .MoveTo(new Vector3(-100, -50, 0), 2); // Uh...
            });
            // 11
            Actions.Add("NOT IMPLEMENTED 2", () =>
            {
                //SimpleTween
                //	.MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                //	.OnFinished((x2) => x2.Repeat());
            });
            // 12
            Actions.Add("Multiple Moves and repeat", () =>
            {
                SimpleTween
                    .MoveTo(this.Actor, new Vector3(0, -100, 0), 2)
                    .MoveTo(new Vector3(100, -100, 0), 2)
                    .MoveTo(new Vector3(100, 0, 0), 2)
                    .Sequence
                    .SetRepetitions(3); // TODO: This doesn't work...
            });
            // TODO: We've reached this point vvv
            // 13
            Actions.Add("NOT IMPLEMENTED Create sequence and add to actor", () =>
            {
                var seq = SimpleTween
                    .CreateSequence<Actor>()
                    .MoveTo(new Vector3(0, -100, 0), 2)
                    .MoveTo(new Vector3(0, -100, 0), 2)
                    .MoveTo(new Vector3(0, -100, 0), 2)
                    .Sequence
                    .SetRepetitions(3);

                SimpleTween
                    .Add(seq, this.Actor);
            });
            // 14
            Actions.Add("Single Animation Additive Mode", () =>
            {
                SimpleTween
                    .Tween(Actor)
                    .MoveTo(new Vector3(0, -100, 0), 2)
                    .SetAdditive(true);
            });
            // 15
            /*Actions.Add("Multile Animations with Additive Mode", () =>
            {
                SimpleTween
                    .Tween(Actor)
                    .MoveTo(new Vector3(0, -100, 0), 2)
                    .SetAdditive(true);
            });
            // 16
            Actions.Add("Multile Animations including Additive Mode", () =>
            {
                SimpleTween
                    .Tween(Actor)
                    .MoveTo(new Vector3(0, -100, 0), 2)
                    .SetAdditive(true);
            });*/

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