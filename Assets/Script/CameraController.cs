using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float panSpeed = 10f;
    //valeur min et max
    [SerializeField] float panBorderThickness = 10f;
    //use map size to set limit
    public Vector2 panLimit;
    [SerializeField] float targetPosY;

    //require to be more flexible in negative if angle < 90
    [SerializeField] float scrollSpeed = 20f;
    [SerializeField] float minY = 3f;
    [SerializeField] float maxY = 15f;

    //ajuste l'angle entre 60 et 90 degré quand on dézoom
    [SerializeField] Vector2 rotationLimit;

    bool camIsLock;

    //add a follow/unfollow object function
    public GameObject toFollow;
    
    private void Start()
    {
        panLimit.x = (GameManager.Instance.size.x / 2) - 3;
        panLimit.y = (GameManager.Instance.size.y / 2) - 5;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        if (!camIsLock)
        {
            if (Input.GetKey(KeyCode.Z) || Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                pos.z += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
            {
                pos.z -= panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.Q) || Input.mousePosition.x <= panBorderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                pos.x += panSpeed * Time.deltaTime;
            }
        }

        //check if hovering map not a menu
        float scroll = 0.0f;
        if (!camIsLock)
            scroll = Input.GetAxis("MouseScrollWheel");

        targetPosY = targetPosY - scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.y = Mathf.Lerp(pos.y, targetPosY, 0.2f);
        //pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y - 5, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        targetPosY = Mathf.Clamp(targetPosY, minY, maxY);

        transform.position = pos;

        float targetRotX = Mathf.Lerp(rotationLimit.x, rotationLimit.y, (pos.y - minY) / maxY);

        //rot.x = Mathf.Lerp(rot.x, targetRotX, 0.3f);
        rot.x = Mathf.Lerp(rotationLimit.x, rotationLimit.y, (pos.y - minY) / maxY);

        transform.rotation = rot;

        panSpeed = Mathf.Lerp(8, 20, (pos.y - minY) / maxY);
    }

    public void LockCam(bool nLockState)
    {
        camIsLock = nLockState;
    }

    public void LockCamOnPos(Vector2 nPos)
    {
        Vector3 pos = transform.position;

        pos.x = nPos.x;
        pos.z = nPos.y;

        transform.position = pos;
    }

    public void LockCamHeight(float nPosY)
    {
        targetPosY = nPosY;
    }
}
