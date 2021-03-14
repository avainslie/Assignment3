using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class antBehaviour : MonoBehaviour
{
    [SerializeField] float health;

    private Rigidbody rigidbody;


    private void Awake()
    {
        // https://answers.unity.com/questions/17131/execute-code-every-x-seconds-with-update.html 
        InvokeRepeating("lowerAntHealthFixedAmount", 5f, 5f);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        if(!isAntAlive())
        {
            CancelInvoke("lowerAntHealthFixedAmount");
        }

    }

    
    private void lowerAntHealthFixedAmount()
    {
        health -= 30f;
    }

    // Checks if ant is dead and should be removed
    private bool isAntAlive()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }


   
}
