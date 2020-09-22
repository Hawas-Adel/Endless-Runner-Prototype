using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
public class CarControls : MonoBehaviour
{
	[SerializeField] [Header("Wheel Refrences")] private List<WheelCollider> SteeringWheels = default;
	[SerializeField] private List<WheelCollider> MotorDrivingWheels = default;

	[SerializeField] [Min(0)] [Header("Driving and Steering")] private float MaxMotorTorque = 400; // maximum torque the motor can apply to wheel
	[SerializeField] [Min(0)] private float MaxSteeringAngle = 30; // maximum steer angle the wheel can have
	private float InputMotorTorque = 0;
	private float InputSteeringAngle = 0;

	[SerializeField] [Min(0)] [Header("Jumping")] private float JumpForce = 10;
	private Rigidbody Rigidbody;
	private bool Jumpthisframe = false;

	private void Start() => Rigidbody = GetComponent<Rigidbody>();

	private void FixedUpdate()
	{
		//Jumping
		// if any wheel collider in MotorDrivingWheel is not grounded then car is not grounded
		bool IsGrounded = !MotorDrivingWheels.ConvertAll(W => W.isGrounded).Contains(false);
		if (Jumpthisframe)
		{
			if (IsGrounded)
			{
				Rigidbody.AddForce(0, JumpForce, 0);
			}
			Jumpthisframe = false;
		}
	}

	public void OnPlayerInputMovment(InputAction.CallbackContext CTX)
	{
		if (CTX.performed || CTX.canceled)
		{
			Vector2 InputValue = CTX.ReadValue<Vector2>();
			InputSteeringAngle = InputValue.x * MaxSteeringAngle;
			SteeringWheels.ForEach(W => W.steerAngle = InputSteeringAngle);

			InputMotorTorque = InputValue.y * MaxMotorTorque;
			MotorDrivingWheels.ForEach(W => W.motorTorque = InputMotorTorque);
		}
	}
	public void OnPlayerInputjump(InputAction.CallbackContext CTX)
	{
		if (CTX.performed)
		{
			Jumpthisframe = true;
		}
	}
}
