using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Piranha : MonoBehaviour
{
    public Transform tongue;
    public LineRenderer tongueLineRenderer;
    public Color shootColor, pullColor, retreatColor;
    public float detectMin, detectMax;
    public float tongueSpeed;
    public float pullForce;
    public float maxTongueLength;
    public float onetimeBoostForce;
    public float destroyBehindDist = 20;

    private GameObject _player;
    private Rigidbody2D _playerRb;
    private bool _toDestroy;
    private int _defaultLayerMask;

    private enum Status
    {
        Idle,
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
        _defaultLayerMask = LayerMask.GetMask("Default");
    }

    public void Cut()
    {
        _status = Status.Retreat;
        _toDestroy = true;
    }

    private void Update()
    {
        tongueLineRenderer.SetPosition(0, transform.position);
        tongueLineRenderer.SetPosition(1, tongue.position);
        
        var t2p = _player.transform.position - transform.position;
        switch (_status)
        {
            case Status.Idle:
                // check player below
                if (-t2p.y < detectMax && -t2p.y > detectMin && !BelowPlayer(t2p) && !Blocked(t2p))
                {
                    tongue.SetParent(null);
                    _status = Status.Shoot;
                }
                
                if (t2p.y > destroyBehindDist)
                {
                    Destroy(gameObject);
                }
                break;
            case Status.Shoot:
                tongueLineRenderer.endColor = shootColor;
                tongueLineRenderer.startColor = shootColor;
                if (TongueIsOut(.01f) && BelowPlayer(t2p) || TongueIsTooLong())
                {
                    _status = Status.Retreat;
                    _player.GetComponent<Player>().DetachPiranha(this);
                }
                else
                {
                    MoveTongue(_player.transform.position);
                    if ((_player.transform.position - tongue.position).sqrMagnitude < 1)
                    {
                        // the tongue captures player
                        _player.GetComponent<Player>().AttachPiranha(this);
                        // boost force for a short period of time
                        _playerRb.AddForce(onetimeBoostForce * -t2p, ForceMode2D.Impulse);
                        // TODO: effect
                        _status = Status.Pull;
                    }
                }
                break;
            case Status.Pull:
                tongue.position = _player.transform.position;
                tongueLineRenderer.endColor = pullColor;
                tongueLineRenderer.startColor = pullColor;
                if (TongueIsOut(.01f) && BelowPlayer(t2p) || TongueIsTooLong())
                {
                    _status = Status.Retreat;
                    _player.GetComponent<Player>().DetachPiranha(this);
                }
                break;
            case Status.Retreat:
                MoveTongue(transform.position);
                tongueLineRenderer.endColor = retreatColor;
                tongueLineRenderer.startColor = retreatColor;
                if ((transform.position - tongue.position).sqrMagnitude < .01f)
                {
                    tongue.SetParent(transform);
                    if (_toDestroy)
                    {
                        // TODO: death effect
                        Destroy(gameObject);
                    }
                    else
                    {
                        _status = Status.Idle;
                    }
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
            _playerRb.AddForce(pullForce  * vec);
        }
    }

    private bool Blocked(Vector3 t2p)
    {
        return Physics2D.Raycast(transform.position, t2p.normalized, t2p.magnitude, _defaultLayerMask);
    }

    private bool TongueIsTooLong()
    {
        return Vector2.Distance(transform.position, tongue.position) > maxTongueLength;
    }
    
    private void MoveTongue(Vector3 dest)
    {
        var dir = (dest - tongue.position).normalized;
        tongue.position += tongueSpeed * Time.deltaTime * dir;
    }

    private bool BelowPlayer(Vector3 t2p)
    {
        return t2p.y > 0;
    }

    private bool TongueIsOut(float distSqr)
    {
        return (transform.position - tongue.position).sqrMagnitude > distSqr;
    }
}
