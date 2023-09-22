using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class ArcadeCarController : MonoBehaviour
{
    //hpublic GameObject[] models;

    [Header("Audio")]
    public AudioSource engineSFX;
    public float enginePitchMin = 1;
    public float enginePitchMax = 2;
    public AudioSource clickSFX;

    [Header("Final Lap")]
    [SerializeField] float extraFOV = 5;
    [SerializeField] float extraCA = .4f;
    [SerializeField] bool finalLap = false;

    [Header("Other")]
    public PostProcessProfile chromaticAberProfile;
    public CheckpointSystem checkpointSystem;
    public Rigidbody sphereRB;
    public Rigidbody carRB;
    public TMP_Text speedText;
    public TMP_Text tooFastText;
    public GameObject rearView;
    public Camera cam;
    public GameObject[] wheelsToRotate;
    public GameObject[] frontWheels;
    public TrailRenderer[] skidTrails;
    public GameObject[] headlights;
    AudioSource audioSource;

    private float speed;
    public float slowRate;
    public float fwdMax;
    public float fwdAccel;
    public float revMax;
    public float revAccel;
    public float turnSpeed;
    public float driftSpeed;
    public float minDriftSpeed;
    public LayerMask groundLayer;
    public LayerMask roadLayer;

    private bool resetWait = false;
    private float moveInput = 0;
    private float turnInput = 0;
    private bool canGo;
    private bool finish = false;
    private bool isCarGrounded;
    private bool drifting;
    private bool offRoad;
    private bool rearViewToggle;
    private bool headlightsToggle;
    bool reversing = false;

    private float normalDrag;
    public float offRoadDrag;
    public float modifiedDrag;

    public float alignToGroundTime;

    public float rotationSpeed;

    [SerializeField] GameObject confettiPrefab;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        canGo = false;

        // detatch shere from car
        sphereRB.transform.parent = null;
        carRB.transform.parent = null;
        carRB.GetComponent<ArcadeCollisionEffect>().controller = this;

        rearViewToggle = rearView.activeSelf;
        headlightsToggle = headlights[0].activeSelf;
        audioSource = GetComponent<AudioSource>();
        normalDrag = sphereRB.drag;
        tooFastText.enabled = false;
    }

    void Update()
    {
        // set cars position to the sphere
        transform.position = sphereRB.transform.position;

        // reset wait
        if (resetWait)
        {
            carRB.velocity = Vector3.zero;
            carRB.angularVelocity = Vector3.zero;
            speed = 0;
            sphereRB.transform.position = checkpointSystem.GetCurrentCheckpointPos();
            transform.rotation = checkpointSystem.GetCurrentCheckpointRotation();
        }

        // calculate turning rotation
        float newRot = turnInput * turnSpeed * Time.deltaTime * moveInput;
        if (moveInput == 0 && speed != 0) newRot = turnInput * turnSpeed * Time.deltaTime;
        if (drifting) newRot *= driftSpeed / turnSpeed;


        if (isCarGrounded)
            transform.Rotate(0, newRot, 0, Space.World);

        if (((turnInput != 0 && IsFast()) || sphereRB.velocity.magnitude >= 42) && isCarGrounded)
        {
            foreach (var trail in skidTrails)
                trail.emitting = true;
        }
        else
        {
            foreach (var trail in skidTrails)
                trail.emitting = false;
        }

        // raycast to the ground and get normal to align car with it
        RaycastHit groundHit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out groundHit, 1f, groundLayer);
        RaycastHit roadHit;
        offRoad = !Physics.Raycast(transform.position, -transform.up, out roadHit, 1f, roadLayer);

        // rotate car to align with ground
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, groundHit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);

        // calculate movement
        if (isCarGrounded)
        {
            if (moveInput > 0)
            {
                reversing = false;
                float accel = speed >= fwdMax * 0.5 ? fwdAccel / 5 : fwdAccel;
                if (speed >= fwdMax * 0.8) accel = fwdAccel / 12;
                speed = Mathf.Lerp(speed, fwdMax * moveInput, accel);
            }
            else if (moveInput < 0)
            {
                reversing = true;
                if (speed >= 15)
                    speed = Mathf.Lerp(speed, 0, revAccel * 200);
                else
                    speed = -Mathf.Lerp(Mathf.Abs(speed), revMax, revAccel);
            }
            else
            {
                speed = Mathf.Lerp(speed, 0, slowRate);
                if (speed < 12) speed = 0;
            }
        }

        // rotate wheels x
        foreach (var wheel in wheelsToRotate)
        {
            wheel.transform.Rotate(Time.deltaTime * moveInput * rotationSpeed, 0, 0, Space.Self);
        }
        // rotate wheels y
        float rot = drifting ? 50 : 30;
        foreach (var wheel in frontWheels)
        {
            wheel.transform.localEulerAngles = new Vector3(0, turnInput * rot, 0);
        }

        // calculate drag
        if (isCarGrounded && offRoad) sphereRB.drag = offRoadDrag;
        else if (isCarGrounded) sphereRB.drag = normalDrag;
        else sphereRB.drag = modifiedDrag;

        // speedometer
        speedText.text = (int)(sphereRB.velocity.magnitude * 3) + " km/h";

        float rbSpeed = sphereRB.velocity.magnitude;

        // FOV
        // 50 m/s is 75 degrees, 0 m/s is 60 degrees, y = mx + b, y = 0.3x + 60
        float desiredFOV = (0.3f * rbSpeed) + (finalLap ? 60 + extraFOV : 60);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFOV, .5f);

        // Chromatic Aberration
        float desiredAberration = Mathf.Lerp
            (chromaticAberProfile.GetSetting<ChromaticAberration>().intensity, (rbSpeed / 120) + (finalLap ? extraCA : 0), .5f);
        chromaticAberProfile.GetSetting<ChromaticAberration>().intensity.Override(desiredAberration);

        // SFX
        float desiredEnginePitch = (rbSpeed * enginePitchMax) / 50;

        if (rbSpeed < enginePitchMin) desiredEnginePitch = enginePitchMin;
        else if (desiredEnginePitch < enginePitchMin) desiredEnginePitch = enginePitchMin;
        engineSFX.pitch = desiredEnginePitch;
    }

    private void FixedUpdate()
    {
        if (isCarGrounded)
        {
            if (moveInput > 0)
                sphereRB.AddForce(transform.forward * speed, ForceMode.Acceleration); // add movement
            else if (moveInput < 0)
                sphereRB.AddForce(transform.forward * speed, ForceMode.Acceleration); // add movement
            else if (reversing)
                sphereRB.AddForce(transform.forward * speed, ForceMode.Acceleration);
            else
                sphereRB.AddForce(transform.forward * speed, ForceMode.Acceleration);
        }
        else sphereRB.AddForce(transform.up * -60f); // add gravity

        carRB.MoveRotation(transform.rotation);
    }

    public void FinalLap() { finalLap = true; }

    public void Honk(InputAction.CallbackContext context)
    {
        if (context.performed && !finish || Time.timeScale == 0)
        {
            audioSource.Play();
        }
    }

    public void Accelerate(InputAction.CallbackContext context)
    {
        if (!canGo) return;
        moveInput = context.ReadValue<float>();
    }

    public void Turn(InputAction.CallbackContext context)
    {
        if (finish) return;
        turnInput = context.ReadValue<float>();
    }

    public void Drift(InputAction.CallbackContext context)
    {
        if (context.canceled || !canGo) drifting = false;
        else if (context.performed && sphereRB.velocity.magnitude >= minDriftSpeed) drifting = true;
    }

    public void RearView(InputAction.CallbackContext context)
    {
        if (finish || Time.timeScale == 0) return;
        rearViewToggle = !rearViewToggle;
        if (context.performed) rearView.SetActive(rearViewToggle);
    }

    public void ToggleHeadlights(InputAction.CallbackContext context)
    {
        if (!context.performed || finish || Time.timeScale == 0) return;

        headlightsToggle = !headlightsToggle;
        foreach (GameObject o in headlights)
        {
            o.SetActive(headlightsToggle);
        }
        clickSFX.Play();
    }

    public void RestartCheckpoint(InputAction.CallbackContext context)
    {
        if (!context.performed || checkpointSystem.FirstCheckpoint() || !canGo || Time.timeScale == 0) return;

        if (IsFast())
        {
            StartCoroutine(TooFastText());
            return;
        }

        StartCoroutine(ResetWait());
    }

    private IEnumerator ResetWait()
    {
        resetWait = true;
        yield return new WaitForSeconds(1f);
        resetWait = false;
    }

    IEnumerator TooFastText()
    {
        tooFastText.enabled = true;
        yield return new WaitForSeconds(3f);
        tooFastText.enabled = false;
    }
    
    public void SetSpeed(float s) { speed = s; }

    public bool IsFast()
    {
        if (sphereRB.velocity.magnitude > 15) return true;
        else return false;
    }

    public bool IsSomewhatFast()
    {
        if (sphereRB.velocity.magnitude > 2) return true;
        else return false;
    }

    public void SetOffRoad(bool b) { offRoad = b; }

    public void StartGame()
    {
        canGo = true;
    }

    public void EndGame(bool early)
    {
        canGo = false;
        finish = true;
        moveInput = 0;
        turnInput = 0;

        if (early) return;

        // confetti
        GameObject obj = Instantiate(confettiPrefab, new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), Quaternion.identity);
        obj.transform.SetParent(transform);
    }
}
