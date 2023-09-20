using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TorcheyeUtility;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float jumpPowerStep, speed, chargeTimeStep, groundDetect;
    public List<Piranha> attachedPiranhas = new List<Piranha>();
    public TMP_Text scoreCounter;
    public GameObject popupScorePrefab;
    public CanvasGroup deathScreen;
    public float chargeSlowMultiplier;

    private Rigidbody2D _rb;
    private float _score;
    private float _highestPosition;
    private Vector3 _moveDir = Vector3.right;
    private float _jumpPower, _chargeTimer;
    private bool _dead;
    private int _defaultLayerMask;
    private float _speedMultiplier = 1;
    
    public CameraZoom cameraShake;
    public GameObject jumpEffect;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _highestPosition = transform.position.y;
        _defaultLayerMask = LayerMask.GetMask("Default");
    }

    private void Update()
    {

        if (_dead)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
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
    }

    private void Walk()
    {
        transform.position += speed * Time.deltaTime * _speedMultiplier * _moveDir;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_rb.velocity.magnitude > 1)
            StartCoroutine(cameraShake.Shake(_rb.velocity.magnitude));
        
        if (other.collider.CompareTag("Wall"))
        {
            if (_moveDir.x > 0 && other.transform.position.x > transform.position.x)
            {
                _moveDir.x = -1;
            }
            else if (_moveDir.x < 0 && other.transform.position.x < transform.position.x)
                _moveDir.x = 1;
        }

        if (other.collider.CompareTag("Spike") || other.collider.CompareTag("Danger"))
        {
            AudioManager.Instance.PlaySoundEffect(AudioManager.SoundEffect.Spikes);
            Death();
        }

        if (other.collider.CompareTag("Flood"))
        {
            AudioManager.Instance.PlaySoundEffect(AudioManager.SoundEffect.Water);
            Death();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _chargeTimer += Time.deltaTime;
            var amount = Mathf.Clamp(_chargeTimer / chargeTimeStep, 0, 3);

            if (OnGround())
            {
                _speedMultiplier = Mathf.Pow(chargeSlowMultiplier, amount);
            }
            _jumpPower = (int)amount * jumpPowerStep * (attachedPiranhas.Count > 0 ? .5f: 1);
            
            UIManager.Instance.Fill(amount);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _chargeTimer = 0;
            _speedMultiplier = 1;
            UIManager.Instance.Clear();
            if (OnGround() || !OnGround() && attachedPiranhas.Count > 0)
            {
                Jump();
            }
            else
            {
                //TODO: audio warning cant jump
            }
            
            // check for piranhas
            if (attachedPiranhas.Count > 0)
            {
                // detach from the first one
                Piranha first = attachedPiranhas[0];
                first.Cut();
                attachedPiranhas.RemoveAt(0);

                AudioManager.Instance.PlaySoundEffect(AudioManager.SoundEffect.CutTongue);
                AddScore(20, true);
            }
        }
    }

    private bool OnGround()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, groundDetect, _defaultLayerMask);
    }

    private void Jump()
    {
        if (_jumpPower > 0)
        {
            _rb.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            AudioManager.Instance.PlaySoundEffect(AudioManager.SoundEffect.Jump, .5f);

            GameObject JumpEffectIns = Instantiate(jumpEffect, transform.position - new Vector3(0, 1, 0), Quaternion.identity);
            Destroy(JumpEffectIns, 1);
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

    private void Death()
    {
        _dead = true;
        deathScreen.alpha = 1;
    }
}
