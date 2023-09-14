using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    public float jumpAmount;
    public float maxSpeed;

    Queue<Piranha> attachedPiranhas;

    public TMP_Text scoreCounter;
    private int score = 0;
    private float highestPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        attachedPiranhas = new Queue<Piranha>();
        highestPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidbody.velocity.magnitude > maxSpeed)
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;

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
                AddScore(100);
            }
        }

    }

    public void AttachPiranha(Piranha piranha)
    {
        attachedPiranhas.Enqueue(piranha);
    }

    public void DetachPiranha()
    {
        attachedPiranhas.Dequeue();
    }

    private void AddScore(int points)
    {
        score += points;
        scoreCounter.text = score.ToString();
    }
}
