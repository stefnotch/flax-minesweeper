using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FlaxMinesweeper.Source
{
	public class TextTexture : Script
	{
		public static Texture Text;

		public void OnEnable()
		{
			XY();
		}

		private async void XY()
		{
			Text = await new TextTextureGenerator().FromText("123456789", new Vector2(256));
		}

		public void OnDisable()
		{
			Destroy(ref Text);
		}
	}
}