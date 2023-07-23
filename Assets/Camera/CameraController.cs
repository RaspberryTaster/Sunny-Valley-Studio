using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform followTransform;
	private Transform cameraTransform;
	public bool follow;
	private bool hasLockedOn;


	public float normalsSpeed = 0.5f;
	public float fastSpeed = 3;
	public float movementSpeed = 1;
	public float movementTime = 5;
	public float toFollowTime = 5;
	public float rotation_Amount = 1;
	public float edgesize = 20f;
	public float distanceTillLock = 0.2f;
	[Space]
	public Vector3 zoomAmount = new Vector3(0, -5, 5);
    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;
	public Vector3 maximumZoom = new Vector3(0, 200, 200);
	public Vector3 minimumZoom = new Vector3(0, 10, 10);
    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;


	private void Awake()
	{
		cameraTransform = GetComponentInChildren<Camera>().transform;
	}
	// Start is called before the first frame update
	void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
		if(follow)
		{
			if (hasLockedOn)
			{
				transform.position = followTransform.position;
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, followTransform.position, Time.deltaTime * toFollowTime);
				
				if(Vector3.Distance(transform.position, followTransform.position) <= distanceTillLock)
				{
					hasLockedOn = true;
				}

			}
		}
		else
		{
			Mouse_Input();
			Handle_Movement_Input();

			transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);

		}

		KeyBoard_Rotation();
		KeyBoard_Zoom();

		if(Input.GetKeyDown(KeyCode.Y))
		{
			follow = !follow;
			newPosition = transform.position;
			hasLockedOn = false;

		}

		transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
		cameraTransform.localPosition = newZoom;
	}
    void Handle_Movement_Input()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			movementSpeed = fastSpeed;
		}
		else
		{
			movementSpeed = normalsSpeed;
		}

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			Move_Up();
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			Move_Down();
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			Move_Right();
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			Move_Left();
		}
	}

	private void KeyBoard_Zoom()
	{
		if (Input.GetKey(KeyCode.R))
		{
			if (newZoom.y <= minimumZoom.y) return;
			newZoom += zoomAmount;
		}
		if (Input.GetKey(KeyCode.F))
		{
			if (newZoom.y >= maximumZoom.y) return;
			newZoom -= zoomAmount;
		}
	}

	private void KeyBoard_Rotation()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			newRotation *= Quaternion.Euler(Vector3.up * rotation_Amount);
		}
		if (Input.GetKey(KeyCode.E))
		{
			newRotation *= Quaternion.Euler(Vector3.up * -rotation_Amount);
		}
	}

	void Handle_Mouse_Input()
	{
		if (Input.mouseScrollDelta.y != 0)
		{
			newZoom += Input.mouseScrollDelta.y * zoomAmount;
		}
		if (Input.GetMouseButtonDown(0))
		{
			Plane plane = new Plane(Vector3.up, Vector3.zero);

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (plane.Raycast(ray, out float entry))
			{
				dragStartPosition = ray.GetPoint(entry);
				newPosition = transform.position + dragStartPosition - dragCurrentPosition;
			}


		}
	}

	private void Mouse_Rotate()
	{
		if (Input.GetMouseButtonDown(2))
		{
			rotateStartPosition = Input.mousePosition;
		}
		if (Input.GetMouseButton(2))
		{
			rotateCurrentPosition = Input.mousePosition;

			Vector3 difference = rotateStartPosition - rotateCurrentPosition;

			rotateStartPosition = rotateCurrentPosition;

			newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
		}
	}

	void Mouse_Input()
	{
		bool mouse_At_Right = Input.mousePosition.x > Screen.width - edgesize;
		if (mouse_At_Right)
		{
            Move_Right();
		}

		bool mouse_At_Left = Input.mousePosition.x < edgesize;
		if (mouse_At_Left)
		{
			Move_Left();
		}

		bool mouse_At_Top = Input.mousePosition.y > Screen.height - edgesize;
		if (mouse_At_Top)
		{
			Move_Up();
		}

		bool mouse_At_Bottom = Input.mousePosition.y < edgesize;
		if (mouse_At_Bottom)
		{
			Move_Down();
		}
	}

	private void Move_Up()
	{
		Vector3 value = transform.forward * movementSpeed;
		newPosition += value;
	}

	private void Move_Down()
	{
		Vector3 value = transform.forward * -movementSpeed;
		newPosition += value;
	}

	private void Move_Left()
	{
		Vector3 value = transform.right * -movementSpeed;
		newPosition += value;
	}

	private void Move_Right()
	{
		Vector3 value = transform.right * movementSpeed;
		newPosition += value;
	}

}
