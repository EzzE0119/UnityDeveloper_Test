using UnityEngine;

public class GravityManipulation : MonoBehaviour
{
    [SerializeField] private GameObject _projection;
    [SerializeField] private Transform _projectionPoint;

    private Vector3 _gravityDirection = Vector3.down;

    private Transform _characterTransform;

    private void Start()
    {
        _characterTransform = transform;
        
    }

    private void Update()
    {
        //instead of player seting hologram depending on camera
        Vector3 chracterForward = GetClosestWorldAxis(Camera.main.transform.forward);
        Vector3 characterRight = GetClosestWorldAxis(Camera.main.transform.right);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            SetProjection(chracterForward);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SetGravity(-chracterForward);
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            SetProjection(characterRight);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SetGravity(-characterRight);
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            SetProjection(-chracterForward);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SetGravity(chracterForward);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            SetProjection(-characterRight);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SetGravity(characterRight);
            }
        }
        else
        {
            _projection.SetActive(false);
        }
    }

    private void SetProjection(Vector3 direction)
    {
        _projection.SetActive(true);
        _projection.transform.position = _projectionPoint.position;
        _projection.transform.up = direction;
    }

    private void SetGravity(Vector3 newGravityDirection)
    {
        _gravityDirection = newGravityDirection.normalized;
        transform.up = -newGravityDirection;
    }

    public Vector3 GetGravity()
    {
        return _gravityDirection;
    }

    //snap a vector to the closest world axis
    private Vector3 GetClosestWorldAxis(Vector3 a_direction)
    {
        a_direction = a_direction.normalized;
        Vector3[] worldAxes = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down };

        Vector3 closestAxis = worldAxes[0];
        float maxDot = Vector3.Dot(a_direction, worldAxes[0]);

        for (int i = 1; i < worldAxes.Length; i++)
        {
            float dot = Vector3.Dot(a_direction, worldAxes[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                closestAxis = worldAxes[i];
            }
        }

        return -closestAxis;
    }
}
