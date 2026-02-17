using UnityEngine;
using System.Collections;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// MouseLook rotates the transform based on the mouse delta.
// To make an FPS style character:
// - Create a capsule.
// - Add the MouseLook script to the capsule.
//   -> Set the mouse look to use MouseX. (You want to only turn character but not tilt it)
// - Add FPSInput script to the capsule
//   -> A CharacterController component will be automatically added.
//
// - Create a camera. Make the camera a child of the capsule. Position in the head and reset the rotation.
// - Add a MouseLook script to the camera.
//   -> Set the mouse look to use MouseY. (You want the camera to tilt up and down like a head. The character already turns.)

[AddComponentMenu("Control Script/Mouse Look")]
public class MouseLook : MonoBehaviour {
	public enum RotationAxes {
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2
	}
	public RotationAxes axes = RotationAxes.MouseXAndY;
	
	public float sensitivityHor = 9.0f;
	public float sensitivityVert = 9.0f;
	
	public float minimumVert = -45.0f;
	public float maximumVert = 45.0f;

	private float _rotationX = 0;
	
	void Start() {
		// Make the rigid body not change rotation
		Rigidbody body = GetComponent<Rigidbody>();
		if (body != null)
			body.freezeRotation = true;
	}

	void Update() {
#if ENABLE_INPUT_SYSTEM
        InputAction lookAction = InputSystem.actions.FindAction("Look");
#endif

        if (axes == RotationAxes.MouseX) {
#if ENABLE_INPUT_SYSTEM
            transform.Rotate(0, lookAction.ReadValue<Vector2>().x * sensitivityHor * 5*Time.fixedDeltaTime, 0);
#else
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
#endif
        }
		else if (axes == RotationAxes.MouseY) {
#if ENABLE_INPUT_SYSTEM
			_rotationX -= lookAction.ReadValue<Vector2>().y * sensitivityVert * 5*Time.fixedDeltaTime;
#else
			_rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
#endif
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
			
			transform.localEulerAngles = new Vector3(_rotationX, transform.localEulerAngles.y, 0);
		}
		else {
#if ENABLE_INPUT_SYSTEM
			float rotationY = transform.localEulerAngles.y + lookAction.ReadValue<Vector2>().x * sensitivityHor * 5*Time.fixedDeltaTime;
            _rotationX -= lookAction.ReadValue<Vector2>().y * sensitivityVert * 5*Time.fixedDeltaTime;
#else
            float rotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityHor;
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
#endif
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

			transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
		}
	}
}