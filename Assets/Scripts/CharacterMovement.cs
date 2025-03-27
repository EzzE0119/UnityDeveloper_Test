using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody _playerRB;
    private Camera _mainCamera;

    //movement and rotation
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    private Vector3 _moveDirection;

    //gravity
    private float _gravity = 4f;
    private Vector3 _gravityDirection;
    private GravityManipulation _gravityManipulation;

    //ground check
    public bool _grounded = true;
    public float _groundedOffset = -0.14f;
    public float _groundedRadius = 0.28f;
    public LayerMask _groundLayers;

    //jump and fall
    [SerializeField] float _jumpForce;
    private float _fallTimer = 0f;

    //animation
    private bool _hasAnimator;
    private Animator _animator;
    private int _animIDIdle;
    private int _animIDFallingIdle;
    private int _animIDRunning;


    [SerializeField] private GameObject _gameOverScreen;
    GameTimeManager _gameTimeManager;

    void Start()
    {
        _gameTimeManager = FindObjectOfType<GameTimeManager>();
        _playerRB = GetComponent<Rigidbody>();
        _playerRB.useGravity = false;
        _playerRB.freezeRotation = true;
        _mainCamera = Camera.main;
        _gravityManipulation = GetComponent<GravityManipulation>();
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        _animIDIdle = Animator.StringToHash("Idle");
        _animIDFallingIdle = Animator.StringToHash("FallingIdle");
        _animIDRunning = Animator.StringToHash("Running");
    }

    void Update()
    {
        _gravityDirection = _gravityManipulation.GetGravity();
        _hasAnimator = TryGetComponent(out _animator);
        ProcessInput();
        ApplyGravity();
        Jump();
        GroundedCheck();
        HandleFallingTimer();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void ProcessInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        forward = Vector3.ProjectOnPlane(forward, -_gravityDirection).normalized;
        right = Vector3.ProjectOnPlane(right, -_gravityDirection).normalized;

        _moveDirection = (forward * vertical + right * horizontal).normalized;

        if (_moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, -_gravityDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        if (_moveDirection.magnitude > 0.1f)
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDRunning, true);
                _animator.SetBool(_animIDIdle, false);
            }
        }
        else
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDRunning, false);
                _animator.SetBool(_animIDIdle, true);
            }

        }
    }

    void ApplyGravity()
    {
        if (!_grounded)
        {
            _playerRB.AddForce(_gravityDirection * _gravity, ForceMode.Acceleration);
        }
    }

    private void Jump()
    {
        if (_grounded)
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDFallingIdle, false);
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _playerRB.AddForce(_jumpForce * -_gravityDirection);
            }
        }
        else
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDFallingIdle, true);
                _animator.SetBool(_animIDIdle, false);
                _animator.SetBool(_animIDRunning, false);
            }
        }
    }

    private void GroundedCheck()
    {
        // checksphere with ground layer to detect player is grounded or not
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset,
            transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void HandleFallingTimer()
    {
        if (!_grounded)
        {
            _fallTimer += Time.deltaTime;
            if (_fallTimer >= 5f)
            {
                _gameOverScreen.SetActive(true);
                _gameTimeManager.pauseTimer(true);
            }
        }
        else
        {
            _fallTimer = 0f;
        }
    }

    void MoveCharacter()
    {
        _playerRB.velocity = _moveDirection * _moveSpeed + _gravityDirection * _gravity;
        
    }

    //draw the sphere that is used to check grounded or not
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere( new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);
    }
}
