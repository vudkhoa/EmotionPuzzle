using UnityEngine;
using CustomUtils;

public class CameraFollower : SingletonMono<CameraFollower>
{
    public Transform target;
    public Camera mainCamera;

    [SerializeField] private float smooth;

    private Vector3 distance;
    public bool canFollow = true;

    

    private void Start()
    {
    }

    void Update()
    {
        if (!canFollow)
        {
            return;
        }
        
        if (target != null)
        {
            Follow();
        }
    }

    void Follow()
    {
        Vector3 currentPosition = transform.position;

        Vector3 newPosiotion = target.position;
        newPosiotion.z = -10f;

        this.transform.position = Vector3.Lerp(currentPosition, newPosiotion, smooth * Time.deltaTime);
    }
}
