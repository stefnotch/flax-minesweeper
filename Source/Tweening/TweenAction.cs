using System;
using System.Threading.Tasks;
using FlaxEngine;

namespace FlaxMinesweeper.Source.Tweening
{
	/*

	public class TweenActionScript : Script
	{
		[NoSerialize]
		public TweenAction TweenAction;

		private void OnEnable()
		{
			TweenAction.End.ContinueWith((_) => Dispose());
		}

		private bool _disposed = false;

		private void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				Enabled = false;
				//this.Actor.RemoveScript(this);
				//Destroy(this);
				//this.Actor.DeleteScript(this.OrderInParent);
			}
		}

		private void Update()
		{
			TweenAction.Update();
		}

		private void FixedUpdate()
		{
			// TODO: Tween physics
		}

		private void OnDisable()
		{
			TweenAction.Cancel();
			Dispose();

			this.Actor.RemoveScript(this);
			FlaxEngine.Object.Destroy(this);
		}

		private void OnDestroy()
		{
			Debug.Log("OnDestroy");
		}
	}

	// Major:
	// TODO: What the hell should happen if I have multiple MoveTo s running at the same time?

	public abstract class TweenAction
	{
		private readonly TaskCompletionSource<float> _taskCompletionSource = new TaskCompletionSource<float>();
		// TODO: You might not want to make everything totally public (e.g. Add a bunch of get only properties & stuff..)

		private bool _isRunning = false;
		private bool _isCompleted = false;
		private float _pauseStartTime = 0;
		private float _pauseDuration = 0;
		public float Percentage;
		public float Duration;
		public readonly float StartTime;
		public readonly SimpleTweenOld.SimpleTweenOptions TweenOptions;

		/// <summary>
		/// Completes when the TweenAction is completely done and will be disposed of
		/// </summary>
		public Task<float> End => _taskCompletionSource.Task;

		public bool IsRunning => _isRunning;

		public bool IsCompleted => _isCompleted;

		//public Action TweenStarted;//done

		//public Action<float> TweenUpdate;//done

		//public Action TweenEnded;

		protected TweenAction(float durationInSeconds) : this(durationInSeconds, new SimpleTweenOld.SimpleTweenOptions())
		{
		}

		protected TweenAction(float durationInSeconds, SimpleTweenOld.SimpleTweenOptions tweenOptions)
		{
			Duration = durationInSeconds;
			StartTime = Time.GameTime;
			TweenOptions = tweenOptions;
			Start();
		}

		public void Start()
		{
			if (_isCompleted) throw new InvalidOperationException("Tween Action has already been completed.");
			_isRunning = true;
			RaiseTweenStarted();
		}

		public void Cancel()
		{
			if (_isCompleted) return;
			_isRunning = false;
			_isCompleted = true;

			RaiseTweenEnded();
			_taskCompletionSource.TrySetCanceled();
		}

		public void Rewind() // And a restart function?
		{
			if (_isCompleted) return; // TODO: Or throw an exception?

			// TODO: Uh oh, taskcompletionsource?
			throw new NotImplementedException();
		}

		public void Finish()
		{
			if (_isCompleted) return;
			_isRunning = false;
			_isCompleted = true;
			// Last execution, set the percentage to one of the 2 edge-cases
			if (Percentage > 0.5)
			{
				Percentage = 1;
			}
			else
			{
				Percentage = 0;
			}

			UpdateTweenFunction();
			RaiseTweenUpdate(Percentage);

			RaiseTweenEnded();
			_taskCompletionSource.TrySetResult(Percentage);
		}

		public void Pause()
		{
			if (!_isRunning) return;

			_pauseStartTime = Time.GameTime;
			_isRunning = false;
		}

		public void Resume()
		{
			if (_isRunning) return;

			_pauseDuration = Time.GameTime - _pauseStartTime;
			_isRunning = true;
		}

		public void Update()
		{
			if (_isCompleted) return;
			if (!_isRunning) return;

			float elapsedTime = (Time.GameTime - StartTime) - _pauseDuration;
			Percentage = elapsedTime / Duration;
			if (Percentage >= 1)
			{
				Percentage = 1;
				Finish();
			}
			else
			{
				UpdateTweenFunction();
				RaiseTweenUpdate(Percentage);
			}
		}

		private void RaiseTweenStarted()
		{
			TweenOptions.TweenStarted?.Invoke();
		}

		private void RaiseTweenUpdate(float percentage)
		{
			TweenOptions.TweenUpdate?.Invoke(percentage);
		}

		private void RaiseTweenEnded()
		{
			TweenOptions.TweenEnded?.Invoke();
		}

		protected abstract void UpdateTweenFunction();
	}

	public class TweenAction<U, T> : TweenAction
	{
		public T From;
		public T To;
		public readonly U TargetObject;
		public readonly Action<TweenAction<U, T>> TweenFunction;

		public TweenAction(U targetObject, Action<TweenAction<U, T>> tweenFunction, float durationInSeconds) : base(durationInSeconds)
		{
			TargetObject = targetObject;
			TweenFunction = tweenFunction;
		}

		public TweenAction(U targetObject, Action<TweenAction<U, T>> tweenFunction, float durationInSeconds, SimpleTweenOld.SimpleTweenOptions tweenOptions) : base(durationInSeconds, tweenOptions)
		{
			TargetObject = targetObject;
			TweenFunction = tweenFunction;
		}

		protected override void UpdateTweenFunction()
		{
			TweenFunction(this);
		}
	}*/
}