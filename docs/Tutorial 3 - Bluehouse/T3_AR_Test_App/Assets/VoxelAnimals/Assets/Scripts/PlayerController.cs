using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController: MonoBehaviour
{

    public float movementSpeed = 3;
    public Button Play_Button;
    Animator anim;
    Rigidbody rb;
    private bool move;
    
    
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Button btn = Play_Button.GetComponent<Button>();
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
        move = true;
        Debug.Log("move");

        // WaitForSeconds(5);
        
        // anim.SetInteger("Walk", 0);
        
     }
}