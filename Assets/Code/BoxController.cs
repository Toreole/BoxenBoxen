using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour
{
    [SerializeField]
    protected string horizontal;
    [SerializeField]
    protected bool invertHorizontal = false;
    [SerializeField]
    protected string vertical;
    [SerializeField]
    protected bool invertVertical = false;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float health  = 50f;
    [SerializeField]
    protected float maxHealth = 50f;
    [SerializeField]
    protected Slider healthSlider;

    protected Rigidbody body;

    protected float xInput;
    protected float yInput;

    /// <summary>
    /// just some basic setup
    /// </summary>
    private void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        body = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Get Input first.
    /// </summary>
    private void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        xInput = Input.GetAxis(horizontal) * (invertHorizontal? -1f : 1f);
        yInput = Input.GetAxis(vertical)   * (invertVertical  ? -1f : 1f);
    }

    /// <summary>
    /// all physics here please
    /// </summary>
    void FixedUpdate()
    {
        var movement = (transform.forward * xInput + transform.right * yInput).normalized * speed;
        movement.y += body.velocity.y;
        body.AddForce(movement);
    }
}