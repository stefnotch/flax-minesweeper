using System;
using System.Collections.Generic;
using FlaxEngine;

namespace NinaBirthday
{
	public class CakeCamera : Script
	{
		public Camera Camera { get; set; }

		public float Min = -88;
		public float Max = 88;
		public Vector2 Speed = new Vector2(6);
		public float Damping = 0.01f;
		private const float Smoothing = 0.5f;

		private float _distance;
		private Vector2 _angle;

		private Vector2 _velocity;

		private void Start()
		{
			if (Camera == null) Camera = Camera.MainCamera;
			// Here you can add code that needs to be called when script is created
			_distance = Camera.LocalPosition.Length;
			_angle = new Vector2(Camera.LocalEulerAngles);
			_angle.X = Mathf.Clamp(_angle.X, Min, Max);
		}

		private void Update()
		{
			if (Input.GetMouseButton(MouseButton.Right))
			{
				_velocity += new Vector2(Input.MousePositionDelta.Y, Input.MousePositionDelta.X);
			}
		}

		private void FixedUpdate()
		{
			_velocity = _velocity * (Damping / Time.DeltaTime);

			// Here you can add code that needs to be called every frame
			_angle += _velocity * Speed * Time.DeltaTime;
			_angle.X = Mathf.Clamp(_angle.X, Min, Max);

			Quaternion cameraOrientation = Quaternion.Euler(_angle.X, _angle.Y, 0);
			Camera.LocalOrientation = Quaternion.Slerp(Camera.LocalOrientation, cameraOrientation, Smoothing);

			Vector3 cameraPosition = (Vector3.Backward * _distance) * cameraOrientation;
			Camera.LocalPosition = Vector3.Lerp(Camera.LocalPosition, cameraPosition, Smoothing);
		}
	}
}