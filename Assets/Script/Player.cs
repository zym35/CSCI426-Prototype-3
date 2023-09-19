using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public float jumpPowerStep, speed, chargeTimeStep;
    public List<Piranha> attachedPiranhas = new List<Piranha>();
    public TMP_Text scoreCounter;
    public GameObject popupScorePrefab;

    private Rigidbody2D _rb;
    private float _score;
    private float _highestPosition;
    private Vector3 _moveDir = Vector3.right;
    private float _jumpPower, _chargeTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _highestPosition = transform.position.y;
    }

    private void Update()
    {
        // checked if a new height was reached
        if (transform.position.y > _highestPosition)
        {
            AddScore(transform.position.y - _highestPosition, false);
            _highestPosition = transform.position.y;
        }
        
        HandleInput();
        Walk();
    }

    private void Walk()
    {
        transform.position += speed * Time.deltaTime * _moveDir;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Wall"))
        {
            if (_moveDir.x > 0 && other.transform.position.x > transform.position.x)
            {
                _moveDir.x = -1;
            }
            else if (_moveDir.x < 0 && other.transform.position.x < transform.position.x)
                _moveDir.x = 1;
        }

        if (other.collider.CompareTag("Danger"))
        {
            Debug.Log("Dead");
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _chargeTimer += Time.deltaTime;
            var amount = Mathf.Clamp(_chargeTimer / chargeTimeStep, 0, 3);
            _jumpPower = (int)amount * jumpPowerStep * (attachedPiranhas.Count > 0 ? .5f: 1);
            UIManager.Instance.Fill(amount);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            // check for piranhas
            if (attachedPiranhas.Count > 0)
            {
                // detach from the first one
                Piranha first = attachedPiranhas[0];
                first.Cut();
                attachedPiranhas.RemoveAt(0);

                AddScore(100, true);
            }

            _chargeTimer = 0;
            UIManager.Instance.Clear();
            Jump();
        }
    }

    private void Jump()
    {
        _rb.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }

    public void AttachPiranha(Piranha piranha)
    {
        attachedPiranhas.Add(piranha);
    }

    public void DetachPiranha(Piranha piranha)
    {
        attachedPiranhas.Remove(piranha);
    }

    private void AddScore(float points, bool showPopup)
    {
        _score += points;
        scoreCounter.text = ((int)_score).ToString();

        if (showPopup)
        {
            GameObject popupText = Instantiate(popupScorePrefab, transform);
            popupText.transform.GetChild(0).GetComponent<TextMeshPro>().SetText($"+{points}");
        }
    }
}
