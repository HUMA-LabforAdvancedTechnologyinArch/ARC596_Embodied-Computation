using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menuscript : MonoBehaviour
{

    //private variables
    public GameObject Shadow_Button;
    public GameObject Reset_Button;
    public GameObject Animation_Button;

    // Start is called before the first frame update
    void Start()
    {
        //For each button, define OnClick Action and prefab
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(Menu_Toggle);

    }

     //Toggle ON and OFF the dropdown submenu options
    void Menu_Toggle()
    {
        //deactivate the buttons if they are on
        if (Shadow_Button.activeSelf == true)
        {
            Shadow_Button.SetActive(false);
            Reset_Button.SetActive(false);
            Animation_Button.SetActive(false);
        }
        else 
        {
            Shadow_Button.SetActive(true);
            Reset_Button.SetActive(true);
            Animation_Button.SetActive(true);
        }
    }
}
