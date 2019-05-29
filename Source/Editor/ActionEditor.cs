using System;
using System.Collections.Generic;
using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Editors;
using FlaxEngine;

namespace FlaxMinesweeper.Source.Editor
{
    [CustomEditor(typeof(Action))]
    public class ActionEditor : GenericEditor
    {
        public override void Initialize(LayoutElementsContainer layout)
        {
            base.Initialize(layout);

            var button = layout.Button("Run", Color.Green);
            button.Button.Clicked += () =>
            {
                if (!HasDifferentValues)
                {
                    var action = Values[0] as Action;
                    action?.Invoke();
                }
            };
        }
    }
}
