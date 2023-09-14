using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    public float jumpAmount;
    Queue<Piranha> attachedPiranhas;
    
    // Start is called before the first frame update
    void Start()
    {
        attachedPiranhas = new Queue<Piranha>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // check for piranhas
            if (attachedPiranhas.Count > 0)
            {
                // detach from the first one
                Piranha first = attachedPiranhas.Dequeue();
                first.Cut();
                // jump
                rigidbody.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
            }
        }
    }

    public void AttachPiranha(Piranha piranha)
    {
        attachedPiranhas.Enqueue(piranha);
    }
}
