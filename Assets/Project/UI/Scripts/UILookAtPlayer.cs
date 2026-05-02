using UnityEngine;

public class UILookAtPlayer : MonoBehaviour
{
    private Transform _camera;
    private void Awake()
    {
        _camera = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.rotation = _camera.rotation;
    }
}
