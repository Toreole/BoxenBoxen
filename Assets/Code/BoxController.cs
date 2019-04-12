using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour
{
    [Header("Input Axes")]
    [SerializeField]
    protected string horizontal;
    [SerializeField]
    protected string vertical;
    [SerializeField]
    protected string punchButton;
    [SerializeField]
    protected string blockButton;

    [Header("Movement")]
    [SerializeField]
    protected float moveSpeed;
    [SerializeField, Tooltip("The timeframe within a double tap needs to occur for a dash to happen")]
    protected float dashTimeframe = 0.15f;
    [SerializeField, Tooltip("The distance of the dash")]
    protected float dashDistance = 1f;
    [SerializeField, Tooltip("the time between dashes")]
    protected float dashCooldown = 2f;

    [Header("Combat Stats")]
    [SerializeField, Tooltip("The current health and the start amount")]
    protected float health  = 50f;
    [SerializeField, Tooltip("the maximum amount of health")]
    protected float maxHealth = 50f;
    [SerializeField, Tooltip("The amount of damage that gets inflicted")]
    protected float attackDamage = 10f;
    [SerializeField, Tooltip("The amount of attacks per second that can hit")]
    protected float attackSpeed = 1.5f;
    [SerializeField]
    protected float knockBackStrength = 1f;
    [SerializeField]
    protected float blockDamageMultiplier = 0.3f;
    [SerializeField]
    protected float blockSpeedMultiplier = 0.3f;

    [Header("Sound")]
    [SerializeField]
    protected AudioClip takeDamage;
    [SerializeField]
    protected AudioClip movement;

    protected AudioSource damageAudioSource;
    protected AudioSource movementAudioSource;

    [Header("UI")]
    [SerializeField]
    protected Slider healthSlider;

    //Runtime only
    protected bool isActive = false;
    protected Transform orientation;
    protected Rigidbody body;
    protected bool canDash = true;
    protected bool isDashing = false;
    protected float xDash = 0f;
    protected float zDash = 0f;
    protected bool isDead = false;
    protected bool isMoving = false;

    protected FistController[] fists;
    protected int fistIndex = 0;
    protected bool canPunch = true;

    //input
    protected float xInput;
    protected float zInput;
    protected bool punch;
    protected bool block;

    //some properties for reasons
    public Rigidbody Body => body;
    public Vector3 Position => transform.position;
    public bool IsDead => isDead;

    /// <summary>
    /// just some basic setup
    /// </summary>
    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        body = GetComponent<Rigidbody>();
        fists = GetComponentsInChildren<FistController>();
        foreach (var fist in fists)
            fist.Claim(this);

        damageAudioSource = gameObject.AddComponent<AudioSource>();
        damageAudioSource.loop = false;
        damageAudioSource.playOnAwake = false;
        damageAudioSource.clip = takeDamage;

        movementAudioSource = gameObject.AddComponent<AudioSource>();
        movementAudioSource.loop = true;
        movementAudioSource.playOnAwake = false;
        movementAudioSource.clip = movement;
    }

    /// <summary>
    /// Get Input first.
    /// </summary>
    private void Update()
    {
        if (!isActive || isDashing)
            return;
        GetInput();
        UpdateMovementAudio();
        if(Mathf.Abs(xDash) > 1.5f)
        {
            StartCoroutine(DoDash(orientation.right * xDash));
        }
        else if(Mathf.Abs(zDash) > 1.5f)
        {
            StartCoroutine(DoDash(orientation.forward * zDash));
        }
        if (punch)
            Punch();
    }

    void UpdateMovementAudio()
    {
        var velocity = body.velocity.magnitude;
        if(!isMoving && velocity > 0.05f)
        {
            isMoving = true;
            movementAudioSource.Play();
        }
        else if(isMoving && velocity < 0.05f)
        {
            isMoving = false;
            movementAudioSource.Pause();
        }
    }

    void GetInput()
    {
        xInput = Input.GetAxis(horizontal);
        zInput = Input.GetAxis(vertical);
        block = Input.GetButton(blockButton);
        if (Input.GetButtonDown(blockButton))
        {
            foreach (var fist in fists)
                fist.transform.Rotate(new Vector3(0f, -15f, 0f));
        }
        else if (Input.GetButtonUp(blockButton))
        {
            foreach (var fist in fists)
                fist.transform.Rotate(new Vector3(0f, 15f, 0f));
        }
        punch = Input.GetButtonDown(punchButton) && canPunch && !block;

        if (canDash)
        {
            if (Input.GetButtonDown(horizontal))
            {
                xDash += Input.GetAxisRaw(horizontal);
                StopCoroutine("ResetXDashInput");
                StartCoroutine(ResetXDashInput());
            }
            if (Input.GetButtonDown(vertical))
            {
                zDash += Input.GetAxisRaw(vertical);
                StopCoroutine("ResetZDashInput");
                StartCoroutine(ResetZDashInput());
            }
        }
    }

    IEnumerator ResetXDashInput()
    {
        yield return new WaitForSeconds(dashTimeframe);
        xDash = 0f;
    }
    IEnumerator ResetZDashInput()
    {
        yield return new WaitForSeconds(dashTimeframe);
        zDash = 0f;
    }

    //Actually dash
    IEnumerator DoDash(Vector3 direction)
    {
        direction.Normalize();
        canDash = false;
        isDashing = true;
        var lastSpeed = body.velocity;
        var speed = direction / dashTimeframe * dashDistance;
        //maintain speed
        for (float t = 0f; t < dashTimeframe; t += Time.deltaTime)
        {
            body.velocity = speed;
            yield return null;
        }
        body.velocity = lastSpeed;
        isDashing = false;
        //reset cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    /// <summary>
    /// all physics here please
    /// </summary>
    //TODO: fix movement
    void FixedUpdate()
    {
        if (!isActive || isDashing)
            return;
        var movement = (orientation.forward * zInput + orientation.right * xInput).normalized * moveSpeed * Time.fixedDeltaTime;
        //movement.y += body.velocity.y;
        if (block)
            movement *= blockSpeedMultiplier;
        body.velocity += movement;
        if(body.velocity.magnitude > 0.05f && !block)
        {
            body.MoveRotation(Quaternion.Lerp(body.rotation, Quaternion.LookRotation(body.velocity.normalized, Vector3.up), 0.5f));
        }
    }

    //initialize punch
    protected void Punch()
    {
        fists[fistIndex].Punch();
        canPunch = false;
        fistIndex = (fistIndex + 1) % fists.Length;
        StartCoroutine(ResetPunch());
    }

    IEnumerator ResetPunch()
    {
        yield return new WaitForSeconds(1f / attackSpeed);
        canPunch = true;
    }

    //check if you should punch that box
    public bool ValidatePunch(BoxController other)
    {
        if (other == this)
            return false;
        other.KnockBack((other.Position - this.Position).normalized * knockBackStrength);
        other.Damage(attackDamage);
        return true;
    }

    //take damage if needed
    protected void Damage(float dmg)
    {
        if (isDashing || !isActive)
            return;
        if (block)
            dmg *= blockDamageMultiplier;
        health -= dmg;
        damageAudioSource.Play();
        healthSlider.value = Mathf.Clamp(health, 0f, maxHealth);
        if(health <= 0f)
        {
            Die();
        }
    }

    protected void Die()
    {
        isActive = false;
        isDead = true;
    }

    public void KnockBack(Vector3 knock)
    {
        if (isDashing || !isActive)
            return;
        Body.AddForce(knock, ForceMode.Impulse);
    }

    //helpers
    public void SetOrientationPlane(Transform plane)
    {
        orientation = plane;
    }

    public void Activate()
    {
        isActive = true;
    }
}