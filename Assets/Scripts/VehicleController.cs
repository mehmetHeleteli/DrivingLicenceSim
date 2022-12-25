using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
    public bool brake;
}

public class VehicleController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float maxBrakeTorque;
    float brake, throttle, steering;
    public float drivemode = 1;

    public VehicleMovement controls;
    public InputActionAsset inputActions;
    InputActionMap carActionsMap;
    InputAction brakeInputAction;
    InputAction throttleInputAction;
    InputAction steeringInputAction;
    InputAction driveModeInputAction;
    InputAction indicatorInputAction;


    [Header("Steering Wheel Visualizer")]
    public Transform steeringWheel;
    public float steeringWheelMaxRotation = 30f;
    private Vector3 steeringWheelDefaultRotation;
    public GameObject rightIndicator;
    public GameObject leftIndicator;
    [Header("Physics")]
    public float centerOfMass = -0.9f;
    private Rigidbody rb;

    public void Start()
    {

        controls = new VehicleMovement();
        rightIndicator.SetActive(false);
        leftIndicator.SetActive(false);

    }
    public void Awake()
    {
        carActionsMap = inputActions.FindActionMap("Car");

        brakeInputAction = carActionsMap.FindAction("Brake");
        throttleInputAction = carActionsMap.FindAction("Throttle");
        steeringInputAction = carActionsMap.FindAction("Steering");
        driveModeInputAction = carActionsMap.FindAction("DriveMode");
        indicatorInputAction = carActionsMap.FindAction("Indicator");

        brakeInputAction.performed += GetBrakeInput;
        brakeInputAction.canceled += GetBrakeInput;

        throttleInputAction.performed += GetThrottleInput;
        throttleInputAction.canceled += GetThrottleInput;

        steeringInputAction.performed += GetSteeringInput;
        steeringInputAction.canceled += GetSteeringInput;

        indicatorInputAction.performed += GetIndicatorInput;
        indicatorInputAction.canceled += GetIndicatorInput;

        driveModeInputAction.started += GetTransmissionInput;

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, centerOfMass, centerOfMass);

        steeringWheelDefaultRotation = steeringWheel.localRotation.eulerAngles;
    }

    public void Update()
    {
        Driving();
        SteeringWheelVisulaizer();
        //Debug.Log("Horizotnal: " + Input.GetAxis("Horizontal"));
        //Debug.Log("New Input: " + steeringInputAction.ReadValue<float>());
    }

    void GetBrakeInput(InputAction.CallbackContext context)
    {
        brake = context.ReadValue<float>() * maxBrakeTorque;
    }
    void GetThrottleInput(InputAction.CallbackContext context)
    {
        throttle = context.ReadValue<float>() * maxMotorTorque * drivemode;
    }
    void GetSteeringInput(InputAction.CallbackContext context)
    {
        steering = context.ReadValue<float>() * maxSteeringAngle;
    }
    void GetTransmissionInput(InputAction.CallbackContext context)
    {
        drivemode = -drivemode;
        //Debug.Log("Shifted");
    }
    void GetIndicatorInput(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == 1)
        {
            StartCoroutine(Indicator(rightIndicator));
        }

        else if (context.ReadValue<float>() == -1)
        {
            StartCoroutine(Indicator(leftIndicator));
        }
    }
    private void OnEnable()
    {
        brakeInputAction.Enable();
        throttleInputAction.Enable();
        steeringInputAction.Enable();
        driveModeInputAction.Enable();
        indicatorInputAction.Enable();
    }
    private void OnDisable()
    {
        brakeInputAction.Disable();
        throttleInputAction.Disable();
        steeringInputAction.Disable();
        driveModeInputAction.Disable();
        indicatorInputAction.Disable();
    }
    public void SteeringWheelVisulaizer()
    {
        steeringWheel.transform.localRotation = Quaternion.Euler(steeringWheelDefaultRotation.x, steeringWheelDefaultRotation.y
            , steeringWheelDefaultRotation.z + steeringInputAction.ReadValue<float>() * steeringWheelMaxRotation);
    }
    private IEnumerator Indicator(GameObject indicator)
    {
        for (int counter = 0; counter < 3; counter++)
        {
            indicator.SetActive(true);
            yield return new WaitForSeconds(.4f);
            indicator.SetActive(false);
            yield return new WaitForSeconds(.4f);
        }
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        //visualWheel.transform.rotation = rotation;
    }

    public void Driving()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = throttle;
                axleInfo.rightWheel.motorTorque = throttle;
            }
            if (axleInfo.brake)
            {
                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("Hit");
        }
    }
}
