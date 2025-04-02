using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField ] private Transform _player;
    [SerializeField] private float _distanceFromPlayer = 5.0f;
    [SerializeField] private float _mouseSen = 2.0f;
    private float _minY = 5f;
    private float _maxY = 60f;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallOffset = 0.2f;

    private float _currentX = 0f;
    private float _currentY = 0f;
    private GravityManipulation _gravityManipulation;
    private Vector3 _upDirection;
    private Vector3 _rightDirection;
    private Vector3 _forwardDirection;

    void Start()
    {
        _gravityManipulation = FindObjectOfType<GravityManipulation>();
    }

    void Update()
    {
        if (_gravityManipulation == null)
            return;

        Vector3 gravityDirection = _gravityManipulation.GetGravity();
        _upDirection = -gravityDirection;

        _rightDirection = Vector3.Cross(Vector3.up, _upDirection); //right depending on gravity
        if (_rightDirection.magnitude < 0.01f)                     // vertical right
            _rightDirection = Vector3.Cross(Vector3.right, _upDirection);
        _rightDirection.Normalize();
        _forwardDirection = Vector3.Cross(_rightDirection, _upDirection);

        float mouseX = Input.GetAxis("Mouse X") * _mouseSen;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSen;

        _currentX += mouseX;
        _currentY -= mouseY;

        // clamp y
        float upDot = Vector3.Dot(Vector3.up, _upDirection);
        if (Mathf.Abs(upDot) < 0.9f) //when verical
        {
            _minY = -20f;
            _maxY = 60f;
        }
        else // when stright or upside down
        {
            _minY = 5f;
            _maxY = 60f;
        }
        _currentY = Mathf.Clamp(_currentY, _minY, _maxY);
    }

    void LateUpdate()
    {
        if (_player == null || _gravityManipulation == null)
            return;

        Vector3 gravityDirection = _gravityManipulation.GetGravity();
        _upDirection = -gravityDirection;

        _rightDirection = Vector3.Cross(Vector3.up, _upDirection);
        if (_rightDirection.magnitude < 0.01f)
            _rightDirection = Vector3.Cross(Vector3.right, _upDirection);
        _rightDirection.Normalize();
        _forwardDirection = Vector3.Cross(_rightDirection, _upDirection);

        // CameraUp = -GravityDirection
        Quaternion horizontalRotation = Quaternion.AngleAxis(_currentX, _upDirection);
        Quaternion verticalRotation = Quaternion.AngleAxis(_currentY, _rightDirection);
        Quaternion finalRotation = horizontalRotation * verticalRotation;

        Vector3 desiredPosition = _player.position + finalRotation * (-_forwardDirection * _distanceFromPlayer);

        // wall Check
        RaycastHit hit;
        if (Physics.Raycast(_player.position, (desiredPosition - _player.position).normalized, out hit, _distanceFromPlayer, _wallLayer))
        {
            transform.position = hit.point + hit.normal * _wallOffset;
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.LookAt(_player.position, _upDirection);
    }
}
