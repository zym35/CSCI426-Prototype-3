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

    private GameObject _player;
    private float _alertTimer = 0.5f;
    private Rigidbody2D _playerRb;
    private bool _toDestroy;

    public GameObject popupAlertPrefab;

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
        _toDestroy = true;
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
                    GameObject popupText = Instantiate(popupAlertPrefab, transform);
                    popupText.transform.GetChild(0).GetComponent<TextMeshPro>().SetText("!");
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
                tongueLineRenderer.endColor = shootColor;
                if (TongueIsOut(.1f) && BelowPlayer() || TongueIsTooLong())
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
                        _playerRb.AddForce(onetimeBoostForce * (transform.position - _player.transform.position), ForceMode2D.Impulse);
                        // TODO: effect
                        _status = Status.Pull;
                    }
                }
                break;
            case Status.Pull:
                tongue.position = _player.transform.position;
                tongueLineRenderer.endColor = pullColor;
                if (TongueIsOut(.1f) && BelowPlayer() || TongueIsTooLong())
                {
                    _status = Status.Retreat;
                    _player.GetComponent<Player>().DetachPiranha(this);
                }
                break;
            case Status.Retreat:
                MoveTongue(transform.position);
                tongueLineRenderer.endColor = retreatColor;
                if ((transform.position - tongue.position).sqrMagnitude < .1f)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_status == Status.Pull && other.CompareTag("Player"))
        {
            // TODO: player death
            tongue.SetParent(transform);
            tongue.localPosition = Vector3.zero;
            _status = Status.Retreat;
            _player.GetComponent<Player>().DetachPiranha(this);
        }
    }

    private bool TongueIsTooLong()
    {
        return Vector2.Distance(transform.position, tongue.position) > maxTongueLength;
    }
    
    private void MoveTongue(Vector3 dest)
    {
        tongue.position = Vector3.Lerp(tongue.position, dest, tongueSpeed * Time.deltaTime);
    }

    private bool BelowPlayer()
    {
        return transform.position.y < _player.transform.position.y;
    }

    private bool TongueIsOut(float distSqr)
    {
        return (transform.position - tongue.position).sqrMagnitude > distSqr;
    }
}
