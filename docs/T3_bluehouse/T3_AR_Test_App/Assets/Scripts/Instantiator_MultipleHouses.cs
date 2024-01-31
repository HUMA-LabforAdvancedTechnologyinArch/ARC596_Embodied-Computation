using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;
using UnityEngine.EventSystems;

public class Instantiator_MultipleHouses: MonoBehaviour
{
    public GameObject selectedPrefab;
    private GameObject instantiatedObject;
    public GameObject houseParent; //house prefab parent 
    public Transform arCameraTransform;
    public int mode = 0; //place one house is mode 0, place multiple houses is mode 1

        //raycast related variables here
    private ARRaycastManager rayManager;
    private ARSession arSession;
    public Camera arCamera;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        HandleMode();
    }

    private void HandleMode()
    {

        Debug.Log($"we are in mode {mode}");

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger movements based on TouchPhase
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Input.touchCount == 1)
                    {
                        InstantiateOnTouch(houseParent);
                    }
                    break; //break: If this case is true, it will not check the other ones. More computational efficiency, 

                case TouchPhase.Moved:

                    if (Input.touchCount == 1)
                    {
                        Rotate(instantiatedObject);
                    }
                    
                    if (Input.touchCount == 2)
                    {
                        PinchtoZoom(instantiatedObject);
                    }
                    break;

                case TouchPhase.Ended:
                    Debug.Log("Touch Phase Ended.");
                    break;
            }
        }
    }

    private void InstantiateOnTouch(GameObject houseParent)
    {
        Touch touch = Input.GetTouch(0);
        
            Debug.Log("Single Touch");

            // Check if the raycast hit any trackables.
            if (rayManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first hit means the closest.
                var hitPose = hits[0].pose;

                if (mode == 0)
                    // Check if there is already spawned object. If there is none, instantiated the prefab.
                    if (instantiatedObject == null)
                    {
                        instantiatedObject = Instantiate(selectedPrefab, hitPose.position, hitPose.rotation);
                    }
                    else
                    {
                        // Change the spawned object position and rotation to the touch position.
                        instantiatedObject.transform.position = hitPose.position;
                        instantiatedObject.transform.rotation = hitPose.rotation;
                    }

                else if (mode == 1)
                {
                    instantiatedObject = Instantiate(selectedPrefab, hitPose.position, hitPose.rotation);
                    instantiatedObject.transform.SetParent(houseParent.transform);
                }

                // To make the spawned object always look at the camera. Delete if not needed.
                Vector3 lookPos = Camera.main.transform.position - instantiatedObject.transform.position;
                lookPos.y = 0;
                instantiatedObject.transform.rotation = Quaternion.LookRotation(lookPos);
            
        }
    }


    private void PinchtoZoom(GameObject objectToZoom)

    //scale using pinch involves 2 touches
    // we count both the touches, store them and measure the distance between pinch
    // and scale depending on the pinch distance
    {
        if (Input.touchCount == 2)
        {
            Debug.Log("Double Touch");
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float pinchAmount = deltaMagnitudeDiff * -0.02f * Time.deltaTime;

            Debug.Log("Scaling Initialized");
            Debug.Log(objectToZoom);
            //scale according to pinch amount
            objectToZoom.transform.localScale += new Vector3(pinchAmount, pinchAmount, pinchAmount);
        }
    }

    // we will write the following script together 
    private void Rotate(GameObject objectToRotate)
    {
        Touch touch = Input.GetTouch(0);
        Debug.Log("Rotate touch");
        objectToRotate.transform.Rotate(Vector3.up * 40f * Time.deltaTime * touch.deltaPosition.x, Space.World);
        Debug.Log("Delta Touch is " + touch.deltaPosition);

    }

    //   UI Functions
    public void SetMode_A()
    {
        mode = 0; // for single placement of objects, like the 3D printed house hologram
    }
    public void SetMode_B()
    {
        mode = 1; // for multiple placement of objects, like multiple trees or characters
    }
    
    public class PointerOverUI
    {
        public static bool IsPointerOverUIObject(Vector2 touchPosition)
        {
            //checking if we are touching a button
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = touchPosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            return raycastResults.Count > 0;
        }
    }
    private bool PhysicRayCastBlockedByUi(Vector2 touchPosition)
    {
        //creating a Boolean value if we are touching a button
        if (PointerOverUI.IsPointerOverUIObject(touchPosition))
        {
            return false;
        }

        return true;
    }

    public void ResetApp()
    {
        //destroy all created objects
        if (houseParent.transform.childCount > 0)
        {
            foreach (Transform child in houseParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
       
        //reset AR session : resets all trackable objects and planes. 
        arSession = FindObjectOfType<ARSession>();
        arSession.Reset();
    }

}