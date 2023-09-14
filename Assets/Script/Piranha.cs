using System;
using System.Collections.Generic;
using UnityEngine;

public class Piranha : MonoBehaviour
{
    public Transform tongue;
    public LineRenderer tongueLineRenderer;
    public float detectMin, detectMax;
    public float tongueSpeed;

    private GameObject _player;
    private float _alertTimer = 0.5f;
    
    private enum Status
    {
        Idle,
        Alert,
        Shoot,
        Pull
    }

    private Status _status;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        tongue.position = transform.position;
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
                    if (MoveTongue(transform.position))
                    {
                        tongue.SetParent(transform);
                        _status = Status.Idle;
                    }
                }
                else if (MoveTongue(_player.transform.position))
                {
                    // TODO: effect
                    _status = Status.Pull;
                }
                break;
            case Status.Pull:
                _player.transform.position = tongue.position;
                if (MoveTongue(transform.position))
                {
                    // TODO: player death
                    tongue.SetParent(transform);
                    _status = Status.Idle;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
