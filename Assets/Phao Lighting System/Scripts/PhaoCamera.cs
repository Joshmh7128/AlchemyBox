using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PhaoCamera : MonoBehaviour
{
    [SerializeField] Material EffectMaterial = null; // the material which performs the processing
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera lightCamera; // the main camera and our light camera
    [SerializeField] GameObject lightCameraPrefabObj = null; // the light camera object
    Transform lightCameraTransform; // the light camera transform

    public static PhaoCamera instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mainCamera = GetComponent<Camera>(); // get our main camera
        if (lightCamera == null)
        lightCamera = Instantiate(lightCameraPrefabObj).GetComponent<Camera>();
        lightCameraTransform = lightCamera.transform;
    }

    // runs once ever tick
    void Update()
    {
        // adjust our camera sizes so that they match
        MatchCameras();
    }

    // run to adjust our camera sizes
    void MatchCameras()
    {
        FindSetCamera();
        lightCamera.orthographicSize = mainCamera.orthographicSize;
        lightCameraTransform.position = mainCamera.transform.position;

        // check to make sure the light camera is not a child object to avoid light fighting
        if (lightCamera.transform.parent != null || mainCamera.transform.parent != null) try
        {
            if (mainCamera.transform.parent != null)
            {
                mainCamera.transform.parent = null; 
            }

            if (lightCamera.transform.parent != null)
            {
                lightCamera.transform.parent = null;
            }
        }
        catch // if it does not have a parent
        {
            Debug.LogAssertion("Phao Light Camera has no parent");
        }
        finally
        {
            Debug.Log("Phao Camera Successfully Parentless (batman time)");
        }
    }

    // check to find our highest depth active camera
    void FindSetCamera()
    {
        // loop and retrieve depth
        foreach (Camera camera in Camera.allCameras)
        {
            if (camera.gameObject.activeInHierarchy && camera.gameObject.activeSelf && camera.gameObject.tag != "ExcludeCamera")
                if (camera.depth > mainCamera.depth || mainCamera.gameObject.activeInHierarchy == false)
                mainCamera = camera;
        }
    }

    public static void StartCutsceneCamera(Camera cam)
    {
        instance.mainCamera = cam;
    }

    public static void EndCutsceneCamera()
    {
        instance.mainCamera = instance.GetComponent<Camera>();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, EffectMaterial);
    }
}
