using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animanimals: MonoBehaviour
{

    public float movementSpeed = 0.02f;
    //public Button Anim_Button;
    Animator anim;
    Rigidbody rb;
    private bool move;
    private Button btn;
    
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //Button btn = Anim_Button.GetComponent<Button>();
        btn = GameObject.Find("/AR_Canvas/Menu/Animation_Button").GetComponent<Button>(); //.GetComponent<Button>();
        btn.onClick.AddListener(ControllPlayer);
        
    }

    void Update()
    {
        Debug.Log(move);
        if (move)
        {
            Vector3 movement = transform.forward;
            transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
        }
    }
    public void ControllPlayer()
    {

        Debug.Log("walk");
        anim.SetInteger("Walk", 1);
        if (move)
        {
            move = false;
        }
        else{
        move = true;
        }

        Debug.Log("move");

        
     }
}