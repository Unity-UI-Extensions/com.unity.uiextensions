/// <summary>
/// Credit - ryanslikesocool 
/// Sourced from - https://github.com/ryanslikesocool/Unity-Card-UI
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{

[RequireComponent(typeof(Rigidbody))]
public class CardPopup2D : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 1f;
    [SerializeField]
    private float centeringSpeed = 4f;
    [SerializeField]
    private bool singleScene = false;

    private Rigidbody rbody;
    private bool isFalling;
    private Vector3 cardFallRotation;
    private bool fallToZero;
    private float startZPos;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rbody.useGravity = false;
        startZPos = transform.position.z;
    }

    void Update()
    {
        if (isFalling)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cardFallRotation), Time.deltaTime * rotationSpeed);
        }

        ///This conditional makes the popup fall nicely into place.		
        if (fallToZero)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, startZPos), Time.deltaTime * centeringSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * centeringSpeed);
            if (Vector3.Distance(transform.position, new Vector3(0, 0, startZPos)) < 0.0025f)
            {
                transform.position = new Vector3(0, 0, startZPos);
                fallToZero = false;
            }
        }

        ///This is totally unnecessary.
        if (transform.position.y < -4)
        {
            isFalling = false;
            rbody.useGravity = false;
            rbody.velocity = Vector3.zero;
            transform.position = new Vector3(0, 8, startZPos);
            if (singleScene)
            {
                CardEnter();
            }
        }
    }

    public void CardEnter()
    {
        fallToZero = true;
    }

    ///A negative fallRotation will result in the card turning clockwise, while a positive fallRotation makes the card turn counterclockwise.
    public void CardFallAway(float fallRotation)
    {
        rbody.useGravity = true;
        isFalling = true;
        cardFallRotation = new Vector3(0, 0, fallRotation);
    }
}
}