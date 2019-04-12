using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistController : MonoBehaviour
{
    [SerializeField]
    protected float punchDistance = 0.7f;
    [SerializeField]
    protected float punchTimeforward = 0.2f;
    [SerializeField]
    protected float punchTimeBack = 0.1f;
    [SerializeField]
    protected AnimationCurve offsetCurveForward;
    [SerializeField]
    protected AnimationCurve offsetCurveBackwards;

    protected BoxController owner;
    protected bool hasOwner = false;
    protected Transform fist;
    protected Vector3 startPos;
    protected bool isPunching = false;
    protected bool canHit = true;

    private void Start()
    {
        fist = transform.parent;
        startPos = fist.localPosition;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, fist.forward, Color.blue);
    }

    public void Claim(BoxController controller)
    {
        if (hasOwner)
            return;
        owner = controller;
        hasOwner = true;
    }

    //basically a wrapper
    public void Punch()
    {
        canHit = true;
        StartCoroutine(ThrowPunch());
    }

    //not perfect but good enough
    protected IEnumerator ThrowPunch()
    {
        isPunching = true;
        var startPos = transform.localPosition;
        var originalPosition = transform.position;
        for (float t = 0f; t < punchTimeforward; t += Time.deltaTime)
        {
            //normalize progress
            var progress = offsetCurveForward.Evaluate(t / punchTimeforward);
            transform.position = originalPosition + (progress * punchDistance * fist.forward);
            yield return null;
        }
        isPunching = false;
        originalPosition = transform.position;
        for(float t = punchTimeBack; t > 0f; t -= Time.deltaTime)
        {
            //normalize progress
            var progress = offsetCurveBackwards.Evaluate(t / punchTimeBack);
            transform.position = originalPosition - progress * punchDistance * fist.forward;
            yield return null;
        }
        transform.localPosition = startPos;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isPunching || !canHit)
            return;
        var box = other.GetComponent<BoxController>();
        if(box)
        {
            if(owner.ValidatePunch(box))
                canHit = false;
        }
    }
}
