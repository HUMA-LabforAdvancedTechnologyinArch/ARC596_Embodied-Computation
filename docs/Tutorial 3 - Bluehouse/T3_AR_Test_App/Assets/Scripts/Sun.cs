using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sun : MonoBehaviour
{
    public Slider azimuth_slider;
    public Slider altitude_slider;
    public GameObject houseParent;
    public GameObject sun;

    private float current_latitude;
    private float current_longitude;
    private GameObject house;
    private int shadowMode=0;
    public float radius = 5;

    public float New_Azimuth;
    public float New_Altitude;
    private Vector3 coordPosition;
    private Vector3 centerpoint;

    void Start()
    {
        //**Set the minimum, maximum and default value of sun rotation**

        //time adgustment >> Rotation in X axis
        azimuth_slider.minValue = -180;
        azimuth_slider.maxValue = 180;
        azimuth_slider.value = 50;
        
        //Set the minimum, maximum and default value of sun rotation
        //rotation adgustment >> Rotation in Z axis
        altitude_slider.minValue = 0;
        altitude_slider.maxValue = 180;
        altitude_slider.value = 30;

        //Adjust rotation using a slider
        azimuth_slider.onValueChanged.AddListener(AdjustAzimuth);
        altitude_slider.onValueChanged.AddListener(AdjustAltitude);

    }

    public void AdjustAltitude(float value)
     {
         New_Azimuth = value;
         AdjustTime(New_Azimuth, New_Altitude);
     }

     public void AdjustAzimuth(float value)
     {
         New_Altitude = value;
         AdjustTime(New_Azimuth, New_Altitude);
     }  

    void AdjustTime(float New_Azimuth, float New_Altitude) //Longitude
    {
        //sun.transform.SetParent(houseParent.transform.GetChild(0));
        //Check if the house is instantiated
        Debug.Log("script");
        if (houseParent.transform.childCount != 0)
        {
            house = houseParent.transform.GetChild(0).gameObject;
            centerpoint = house.transform.position;
        }

        //align the sun symbol
        if (house!=null)
        {
            coordPosition.x = radius*Mathf.Cos(New_Azimuth*Mathf.Deg2Rad)*Mathf.Cos(New_Altitude*Mathf.Deg2Rad);
            coordPosition.z = radius*Mathf.Cos(New_Azimuth*Mathf.Deg2Rad)*Mathf.Sin(New_Altitude*Mathf.Deg2Rad);
            coordPosition.y = radius*Mathf.Sin(New_Azimuth*Mathf.Deg2Rad);
     
            coordPosition += centerpoint;
            sun.transform.position = new Vector3(coordPosition.x, coordPosition.y, coordPosition.z);
            sun.transform.LookAt(house.transform);      
        }
    }

    public void VisibilityToggle()
    {
        //**Preview ON/Off the house 3dmodel**

        //Check if the house is instantiated
        if (houseParent.transform.childCount != 0)
            house = houseParent.transform.GetChild(0).gameObject;

        if(house != null)
        {
            //Get access to the model obj and adjust the MeshRenderer parameters
            GameObject obj = house.transform.GetChild(0).gameObject;
            Debug.Log(obj);

            if (shadowMode == 0)
            {
                obj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                shadowMode = 1;
            }
            else
            {
                obj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                shadowMode = 0;
            }
        }
            
        
            
    }
}
