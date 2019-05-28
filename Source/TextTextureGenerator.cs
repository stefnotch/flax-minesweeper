using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaxEngine;
using FlaxEngine.Rendering;

namespace FlaxMinesweeper.Source
{
	public class TextTextureGenerator
	{
		private CustomRenderTask _task;
		private string _text;

		private RenderTarget _output;

		private Texture _texture;
		private TaskCompletionSource<Texture> _promise = new TaskCompletionSource<Texture>();

		public async Task<Texture> FromText(string text, Vector2 size)
		{
			_text = text;

			_output = RenderTarget.New();
			_output.Init(PixelFormat.R8G8B8A8_UNorm, size);
			if (_task == null) _task = RenderTask.Create<CustomRenderTask>();
			_task.OnRender += DrawText;


			return await _promise.Task;
		}

		private void DrawText(GPUContext context)
		{
			//Render2D.DrawText()
			//context.Draw(_texture);
			//Render2D.Draw
			//Render2D.CallDrawing()
			//_promise.SetResult(null);

			_task?.Dispose();
			FlaxEngine.Object.Destroy(ref _task);
			FlaxEngine.Object.Destroy(ref _output);
		}

		/*private class TextureIDrawable : IDrawable
		{
			public void Draw()
			{
				Render2D.DrawBezier(...);
			}
		}*/
	}
}