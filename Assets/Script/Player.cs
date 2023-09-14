using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    public float jumpAmount;
    public float maxSpeed;

    List<Piranha> attachedPiranhas;

    public TMP_Text scoreCounter;
    private int score = 0;
    private int highestPosition;
    public GameObject popupScorePrefab;

    // Start is called before the first frame update
    void Start()
    {
        attachedPiranhas = new List<Piranha>();
        highestPosition = (int)transform.position.y;
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
                Piranha first = attachedPiranhas[0];
                first.Cut();
                attachedPiranhas.RemoveAt(0);
                // jump
                rigidbody.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
                // show score
                AddScore(100);
                GameObject popupText = Instantiate(popupScorePrefab, transform);
                popupText.transform.GetChild(0).GetComponent<TextMeshPro>().SetText("+100");
            }
        }

        // checked if a new height was reached
        if ((int)transform.position.y > highestPosition)
        {
            AddScore((int)transform.position.y - highestPosition);
            highestPosition = (int)transform.position.y;
        }
    }

    public void AttachPiranha(Piranha piranha)
    {
        attachedPiranhas.Add(piranha);
    }

    public void DetachPiranha(Piranha piranha)
    {
        attachedPiranhas.Remove(piranha);
    }

    private void AddScore(int points)
    {
        score += points;
        scoreCounter.text = score.ToString();
    }
}
