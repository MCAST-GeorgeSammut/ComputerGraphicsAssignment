using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubeCamera : MonoBehaviour
{
    public float mMinPitch = -30.0f;
    public float mMaxPitch = 30.0f;
    public float mRotationSpeed = 5.0f;
    private float angleX = 0.0f;
    public Vector3 mPositionOffset = new Vector3(0.0f, 2.0f, -2.5f);
    public Vector3 mAngleOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [Tooltip("The damping factor to smooth the changes in position and rotation of the camera.")]
    public float mDamping = 1.0f;
    public GameObject mPlayer;
    void Follow_IndependentRotation()
    {
        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        // We apply the initial rotation to the camera.
        Quaternion initialRotation = Quaternion.Euler(mAngleOffset);

        Vector3 eu = transform.rotation.eulerAngles;

        angleX -= my * mRotationSpeed;

        // We clamp the angle along the X axis to be between the min and max pitch.
        angleX = Mathf.Clamp(angleX, mMinPitch, mMaxPitch);

        eu.y += mx * mRotationSpeed;
        Quaternion newRot = Quaternion.Euler(angleX, eu.y, 0.0f) * initialRotation;

        transform.rotation = newRot;

        Vector3 forward = transform.rotation * Vector3.forward;
        Vector3 right = transform.rotation * Vector3.right;
        Vector3 up = transform.rotation * Vector3.up;

        Vector3 targetPos = mPlayer.transform.position;
        Vector3 desiredPosition = targetPos
            + forward * mPositionOffset.z
            + right * mPositionOffset.x
            + up * mPositionOffset.y;

        Vector3 position = Vector3.Lerp(transform.position,
            desiredPosition,
            Time.deltaTime * mDamping);
        transform.position = position;
    }

     void LateUpdate()
    {
        Follow_IndependentRotation();
    }
}
