using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineCameraControllerTopDownEffects : MonoBehaviour
{
    public new Camera camera;
    public Transform basePivot;
    public Transform farPivot;
    public float scrollSpeed = 10f;
    public float rotationSpeed = 10f;
    public float rotationAmount = 2f;
    [Range(10f, 40f)]
    public float maximumAngle = 20f;

    private float closeFar = 0.5f;
    private float closeFarLerp = 0.5f;
    private Vector3 mouseAxisToVector;
    private float x;
    private float y;
    private Quaternion rotation;
    private bool rotationPossible = false;

    void Start()
    {
        rotation = gameObject.transform.localRotation;
        mouseAxisToVector = new Vector3(0f, 0f, 0f);
    }

    // Update method without camera movement logic
    void Update()
    {
        // Removed camera zoom logic
        // Removed camera rotation logic

        // Example of remaining functionality or placeholder for future logic
        // (Add any additional features or logic you want to implement here)
    }
}
