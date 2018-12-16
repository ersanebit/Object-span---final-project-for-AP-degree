using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tester : MonoBehaviour
{

    public Text Us;

    // Use this for initialization
    void Start()
    {
        string user;
        user = PlayerPrefs.GetString("user");

        Us.text = user;

    }

    void Update()
    {
       
    }

}
