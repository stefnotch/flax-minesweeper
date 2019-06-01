using System;
using FlaxEngine;

namespace FlaxMinesweeper.Source.Tweening
{
    internal static class SimpleTweenFunctions
    {
        internal static Vector3 GetLocalPosition<U>(U target) where U : Actor
        {
            return target.LocalPosition;
        }

        internal static Quaternion GetLocalOrientation<U>(U target) where U : Actor
        {
            return target.LocalOrientation;
        }

        internal static Vector3 GetLocalScale<U>(U target) where U : Actor
        {
            return target.LocalScale;
        }

        internal static float GetZero<U>(U target) where U : Actor
        {
            return 0f;
        }

        internal static float GetOne<U>(U target) where U : Actor
        {
            return 1f;
        }

        internal static void TranslateLocal<U>(SimpleTweenAction<U, Vector3> tweenAction) where U : Actor
        {
            Vector3 current = tweenAction.Options.IsAdditive ? tweenAction.Target.LocalPosition : tweenAction.FromValue;

            float percentage = tweenAction.Percentage;
            float previousPercentage = tweenAction.PreviousPercentage;
            if (tweenAction.Options.IsAdditive && previousPercentage < 1 && percentage != previousPercentage)
            {
                /*
                 * I'm at 0.25 and the next percentage is 0.5
                 * However, the current position is a tweened one instead of the starting position
                 * And thus, I want 0.3333 instead of 0.5
                 * 
                 * 0      0.25    0.5     0.75     1
                 * |-------|-------|-------|-------|
                 *         0      0.33    0.66     1
                 * 
                 */
                // This will probably break down as soon as I use a non-linear easing type
                percentage = (percentage - previousPercentage) / (1 - previousPercentage);
            }

            tweenAction.Target.LocalPosition = Vector3.Lerp(current, tweenAction.ToValue, percentage);
        }

        internal static void RotateLocal<U>(SimpleTweenAction<U, Quaternion> tweenAction) where U : Actor
        {
            if (tweenAction.Options.LerpType == LerpType.Slerp)
            {
                tweenAction.Target.LocalOrientation = Quaternion.Slerp(tweenAction.FromValue, tweenAction.ToValue, tweenAction.Percentage);
            }
            else
            {
                tweenAction.Target.LocalOrientation = Quaternion.Lerp(tweenAction.FromValue, tweenAction.ToValue, tweenAction.Percentage);
            }
        }

        internal static void ScaleLocal<U>(SimpleTweenAction<U, Vector3> tweenAction) where U : Actor
        {
            tweenAction.Target.LocalScale = Vector3.Lerp(tweenAction.FromValue, tweenAction.ToValue, tweenAction.Percentage);
        }

        internal static void Nothing<U>(SimpleTweenAction<U> tweenAction) where U : Actor
        {

        }
    }
}