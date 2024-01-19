using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;
using UnityEngine.EventSystems;

public class Instantiator : MonoBehaviour
{

    /// <summary>
    /// -----------------------------------------------
    /// The Instantiator Script generates all the objects according to the user input.
    ///
    /// -----------------------------------------------
    /// Mode 0 = default = Add one object with each tap -- single tap 
    /// 
    /// Mode 1 =**To be Added in Day 2 and 3
    /// Mode 2 =**To be Added in Day 2 and 3
    /// Mode 3 =**To be Added in Day 2 and 3
    /// </summary>

    //1. Variables
    public GameObject selected_prefab; //we declare it as public to have a default prefab

    public Transform arCameraTransform; //we use the position to make the objects look at the camera
    public GameObject houseParent; //house prefab parent 

    private int mode = 0; //placing a default value as 0 , we will later add more "modes"
    private GameObject ARObject_new;  // this will be the Instantiated Object name

    //raycast related variables here
    private ARRaycastManager rayManager;


    // Start is called before the first frame update
    void Start()
    {
        //find the Raycast manager in the script in order to use it to acquire data
        rayManager = FindObjectOfType<ARRaycastManager>();
    }


    // Update is called once per frame
    void Update()
    {
        _InstantiateOnTouch();
    }


    private void _InstantiateOnTouch()
    {

        if (Input.touchCount != 0)
        {
            _PlaceInstant(houseParent); //houseParent = the GameObject where the House will be Instantiated

        }
    }
    private void _PlaceInstant(GameObject parentGameObject)
    {
        Touch touch;
        touch = Input.GetTouch(0);
        if (Input.touchCount == 1 && touch.phase != TouchPhase.Began)

        {
            Debug.Log("Single Touch");


            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayManager.Raycast(touch.position, hits);
            if (hits.Count > 0)
            {

                if ((hits[0].hitType & TrackableType.Planes) != 0) //if our touch hits a scanned plane, it instantiates an object
                {
                    Debug.Log("HIT TYPE = " + hits[0].hitType);

                    if (mode == 0) //default mode
                    {
                        Debug.Log("mode 0");
                        Destroy(ARObject_new); //destroys the previous object in every frame
                    }

                    // You can instantiate a 3D object here if you haven't set Raycast Prefab in the scene.
                    ARObject_new = Instantiate(selected_prefab, hits[0].pose.position, hits[0].pose.rotation);
                    ARObject_new.transform.SetParent(parentGameObject.transform);  //Place the GameObject in the correct GameObject folder

                    //transform object to Look at our phone camera
                    Vector3 cameraPosition = arCameraTransform.position;
                    cameraPosition.y = hits[0].pose.position.y;
                    ARObject_new.transform.LookAt(cameraPosition, ARObject_new.transform.up);

                    //create AR Anchor for each instantiated object
                    if (ARObject_new.GetComponent<ARAnchor>() == null)
                    {
                        Debug.Log("Anchor created");
                        ARObject_new.AddComponent<ARAnchor>();
                    }
                }
            }
        }
    }
    private void _PinchtoZoom(GameObject objectToZoom)

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

            // add rotation according to pinch position
            objectToZoom.transform.Rotate(Vector3.up * 40f * Time.deltaTime * touchZero.deltaPosition.x, Space.Self);
        }
    }


}
