﻿using UnityEngine;

public class VRLookWalk : MonoBehaviour
{
	
	public Transform VrCamera;
    public Terrain TerrainBorder;

	public float ToggleAngleMin = 30.0f;
	public float ToggleAngleMax = 90.0f;
	public float Speed = 10.0f;


	private CharacterController _characterController;
    private float _rotationY = 0.0f;
    private float _rotationX = 0.0f;

    private const float MIN_Y = -45.0f;
    private const float MAX_Y = 45.0f;

    private const float SENS_X = 100.0f;
    private const float SENS_Y = 100.0f;

	void Start ()
	{
		_characterController = GetComponent<CharacterController> ();
	}

	private bool IsMoveForwardAngle (Vector3 angle)
	{
		return angle.x > ToggleAngleMin && angle.x < ToggleAngleMax;
	}


    private bool IsStepOutOfBorders(Vector3 step){
        var position = TerrainBorder.GetPosition();
        var size = TerrainBorder.terrainData.size;

        var nextX = VrCamera.position.x + step.x;
        var nextZ = VrCamera.position.z + step.z;

        var isXOut = (nextX < position.x || nextX > position.x + size.x);
        var isZOut = (nextZ < position.z || nextZ > position.z + size.z);

        return isXOut || isZOut;

    }
	private Vector3 NextForwardStep {
		get {
			Vector3 step = VrCamera.TransformDirection (Vector3.forward);
			step *= Speed * Time.deltaTime;

            if(IsStepOutOfBorders(step)){
                //Avoid player to fall into the void if hes out of scene border
                step = Vector3.zero;
            }

            if(step.y < 0){
                step.y = 0;
            }
			return step;
		}

	}

	void Update ()
	{
	    #if UNITY_ANDROID || UNITY_IPHONE
            MoveForwardByAngle();
		#else
		    RotateCamera();
            MoveForwardByAngle();
		#endif
		
	}

	void MoveForwardByAngle(){
		if (IsMoveForwardAngle (VrCamera.eulerAngles)) {
			_characterController.Move (NextForwardStep);
		}
	}

    void RotateCamera(){
        _rotationX += Input.GetAxis("Mouse X") * SENS_X * Time.deltaTime;
        _rotationY += Input.GetAxis("Mouse Y") * SENS_Y * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY, MIN_Y, MAX_Y);

        _characterController.transform.localEulerAngles = new Vector3(-_rotationY, _rotationX, 0);
	}
}

