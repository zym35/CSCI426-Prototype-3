using System;
using System.Collections.Generic;
using UnityEngine;

public class Piranha : MonoBehaviour
{
    public Transform tongue;
    public LineRenderer tongueLineRenderer;
    public float detectMin, detectMax;
    public float tongueSpeed;
    public float pullForce;

    private GameObject _player;
    private float _alertTimer = 0.5f;
    private Rigidbody2D _playerRb;
    
    private enum Status
    {
        Idle,
        Alert,
        Shoot,
        Pull,
        Retreat
    }

    private Status _status;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        tongue.position = transform.position;
        _playerRb = _player.GetComponent<Rigidbody2D>();
    }

    public void Cut()
    {
        _status = Status.Retreat;
    }

    private void Update()
    {
        tongueLineRenderer.SetPosition(0, transform.position);
        tongueLineRenderer.SetPosition(1, tongue.position);
        
        switch (_status)
        {
            case Status.Idle:
                // check player below
                if (transform.position.y - _player.transform.position.y < detectMax 
                    && transform.position.y - _player.transform.position.y > detectMin && !BelowPlayer())
                {
                    // TODO: spawn alert
                    _status = Status.Alert;
                }
                break;
            case Status.Alert:
                _alertTimer -= Time.deltaTime;
                if (_alertTimer < 0)
                {
                    // TODO: Destroy alert
                    tongue.SetParent(null);
                    _status = Status.Shoot;
                }
                break;
            case Status.Shoot:
                if (TongueIsOut() && BelowPlayer())
                {
                    _status = Status.Retreat;
                }
                else if (MoveTongue(_player.transform.position))
                {
                    // the tongue captures player
                    // TODO: call player controller
                    // TODO: effect
                    _status = Status.Pull;
                }
                break;
            case Status.Pull:
                tongue.position = _player.transform.position;
                if (TongueIsOut() && BelowPlayer())
                {
                    _status = Status.Retreat;
                }
                break;
            case Status.Retreat:
                if (MoveTongue(transform.position))
                {
                    tongue.SetParent(transform);
                    _status = Status.Idle;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        if (_status == Status.Pull)
        {
            // add force to player
            Vector2 vec = transform.position - _player.transform.position;
            float multiplier = 1f / vec.magnitude;
            _playerRb.AddForce(pullForce * multiplier * vec.normalized);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_status == Status.Pull && other.CompareTag("Player"))
        {
            // TODO: player death
            tongue.SetParent(transform);
            tongue.localPosition = Vector3.zero;
            _status = Status.Idle;
        }
    }

    // return if at dest
    private bool MoveTongue(Vector2 dest)
    {
        tongue.position = Vector2.Lerp(tongue.position, dest, tongueSpeed * Time.deltaTime);
        return Vector2.Distance(dest, tongue.position) < .01f;
    }

    private bool BelowPlayer()
    {
        return transform.position.y < _player.transform.position.y;
    }

    private bool TongueIsOut()
    {
        return Vector2.Distance(transform.position, tongue.position) > .01f;
    }
}
