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
    public GameObject singleObjectParent; //house prefab parent 
    public GameObject multipleObjectParent; //all other objects prefab parent
    public int mode = 0; //place one house is mode 0, place multiple houses is mode 1
    private GameObject lastUsedPrefab; // Add this to track the last used prefab

    //raycast related variables here
    private ARRaycastManager rayManager;
    private ARSession arSession;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();
        arSession = FindObjectOfType<ARSession>();
    }

    void Update()
    {
        if (Input.touchCount > 0 && !IsPointerOverUIObject(Input.GetTouch(0).position))
        {
            HandleTouch(Input.GetTouch(0));
        }
    }

    private void HandleTouch(Touch touch)
    {

            // Handle finger movements based on TouchPhase
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    InstantiateOnTouch(touch);
                    break; //break: If this case is true, it will not check the other ones. More computational efficiency, 

                case TouchPhase.Moved:
                    if (instantiatedObject != null)
                    {
                        if (Input.touchCount == 1)
                        {
                            Rotate(instantiatedObject, touch);
                        }
                        
                        else if (Input.touchCount == 2)
                        {
                            PinchtoZoom(instantiatedObject);
                        }
                    }

                    break;

                case TouchPhase.Ended:
                    Debug.Log("Touch Phase Ended.");
                    break;
            }
    }


    private void InstantiateOnTouch(Touch touch)
    {
       
            Debug.Log("Single Touch");

            // Check if the raycast hit any trackables.
            if (rayManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first hit means the closest.
                var hitPose = hits[0].pose;

                //mode 0: single placement of objects, like the 3D printed house hologram
                //mode 1: multiple placement of objects, like multiple trees or characters
                bool shouldInstantiateNewObject = mode == 1 || (mode == 0 && instantiatedObject == null);
                bool prefabChanged = lastUsedPrefab != selectedPrefab && mode == 0;

                if (shouldInstantiateNewObject || prefabChanged)
                {
                    if (prefabChanged && instantiatedObject != null)
                    {
                        Destroy(instantiatedObject); // Optionally destroy the old object if a new prefab is selected
                        Debug.Log("Prefab changed, instantiating new prefab.");
                    }

                    instantiatedObject = Instantiate(selectedPrefab, hitPose.position, hitPose.rotation, GetParentTransform());
                    lastUsedPrefab = selectedPrefab;
                }
                else
                {
                    // Move the existing instantiated object
                    instantiatedObject.transform.position = hitPose.position;
                    instantiatedObject.transform.rotation = hitPose.rotation;
                }
                AdjustRotationToCamera(instantiatedObject);
        }
        
    }


    private Transform GetParentTransform()
    {
        return mode == 0 ? singleObjectParent.transform : multipleObjectParent.transform;
    }

    private void AdjustRotationToCamera(GameObject obj)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 direction = new Vector3(cameraPosition.x, obj.transform.position.y, cameraPosition.z) - obj.transform.position;
        obj.transform.rotation = Quaternion.LookRotation(direction);
    }

    private void Rotate(GameObject objectToRotate, Touch touch)
    {
        float rotationSpeed = 0.1f; // Adjust rotation speed as needed
        objectToRotate.transform.Rotate(Vector3.up, touch.deltaPosition.x * rotationSpeed, Space.World);
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


    //   UI Functions
    public void SetMode_A()
    {
        mode = 0; // for single placement of objects, like the 3D printed house hologram
    }
    public void SetMode_B()
    {
        mode = 1; // for multiple placement of objects, like multiple trees or characters
    }
    

    public static bool IsPointerOverUIObject(Vector2 touchPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = touchPosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    
    public void ResetApp()
    {
        //destroy all created objects
        if (singleObjectParent.transform.childCount > 0 || multipleObjectParent.transform.childCount > 0)
        {
            foreach (Transform child in singleObjectParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in multipleObjectParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
       
        //reset AR session : resets all trackable objects and planes. 
        arSession = FindObjectOfType<ARSession>();
        arSession.Reset();
    }

}