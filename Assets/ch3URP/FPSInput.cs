using UnityEngine;
using System.Collections;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// basic WASD-style movement control
// commented out line demonstrates that transform.Translate instead of charController.Move doesn't have collision detection

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour {
	public float speed = 6.0f;
	public float gravity = -9.8f;

	private CharacterController _charController;
	
	void Start() {
		_charController = GetComponent<CharacterController>();
	}
	
	void Update() {
        //transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime);
#if ENABLE_INPUT_SYSTEM
        InputAction moveAction = InputSystem.actions.FindAction("Move");
        float deltaX = Mathf.Clamp(moveAction.ReadValue<Vector2>().x, -1, 1) * speed;
		float deltaZ = Mathf.Clamp(moveAction.ReadValue<Vector2>().y, -1, 1) * speed;
#else
        float deltaX = Input.GetAxis("Horizontal") * speed;
		float deltaZ = Input.GetAxis("Vertical") * speed;
#endif

        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
		movement = Vector3.ClampMagnitude(movement, speed);

		movement.y = gravity;

		movement *= Time.deltaTime;
		movement = transform.TransformDirection(movement);
		_charController.Move(movement);
	}
}
