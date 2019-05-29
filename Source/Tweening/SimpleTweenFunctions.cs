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
            tweenAction.Target.LocalPosition = Vector3.Lerp(tweenAction.FromValue, tweenAction.ToValue, tweenAction.Percentage);
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