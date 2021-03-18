using UnityEngine;

// deprecated . . . 미사용
public class VillageCameraManager : MonoBehaviour, ICMInterface
{
    public class MainCameraInformation
    {
        public float cameraMoveSpeed;
        public float prevDistance;
        public Vector2 prevPosition;

        public Vector2 minPosition;
        public Vector2 maxPosition;
    }

    private MainCameraInformation info;

    private GameObject cameraObject;
    private Camera _camera;

    private void Awake()
    {
        this.info = new MainCameraInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitMainCameraManager();
    }

    private void Update()
    {
        MoveVillageCamera();
    }

    public void PrepareBaseObjects()
    {
        if (this.cameraObject == null)
        {
            this.cameraObject = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Folder"), "MainCamera", true);
        }

        if (this._camera == null)
        {
            this._camera = this.cameraObject.GetComponent<Camera>();
        }

        this.info.cameraMoveSpeed = 9.0f;
        this.info.prevDistance = 0.0f;
        this.info.prevPosition = Vector2.zero;

        this.info.minPosition = new Vector2(-2.5f, 0.0f);
        this.info.maxPosition = new Vector2(2.5f, 0.0f);
    }

    private void InitMainCameraManager()
    {

    }
        
    public void MoveVillageCamera()
    {
#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        if (Input.touchCount > 0)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
#else
        if (Input.GetMouseButton(0) == true)
        {
            Vector2 touchPosition = Input.mousePosition;
#endif
            if (this.info.prevPosition != Vector2.zero)
            {
                Vector2 direction = (touchPosition - this.info.prevPosition).normalized;
                Vector2 v = new Vector2(direction.x, direction.y);
                Vector2 position = v * this.info.cameraMoveSpeed * Time.deltaTime;

                position -= (Vector2)this.cameraObject.transform.position;
                position *= -1;

                CorrectCameraPosition(ref position, this.info.minPosition, this.info.maxPosition);

                this.cameraObject.transform.position = position;
            }

            this.info.prevPosition = touchPosition;
        }
#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        else if (Input.touchCount <= 0)
#else
        else if (Input.GetMouseButtonUp(0) == true)
#endif
        {
            this.info.prevDistance = 0.0f;
            this.info.prevPosition = Vector2.zero;
        }
    }

    private void OnDrag_MoveCamera()
    {
        Vector2 mousePosition;

        mousePosition = Input.mousePosition;

        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        CorrectCameraPosition(ref mousePosition, this.info.minPosition, this.info.maxPosition);

        this.cameraObject.transform.position = mousePosition;
    }

    private void CorrectCameraPosition(ref Vector2 vector, Vector2 minVector, Vector2 maxVector)
    {
        if (vector.x < minVector.x)
        {
            vector.x = minVector.x;
        }
        else if (vector.x > maxVector.x)
        {
            vector.x = maxVector.x;
        }

        if (vector.y < minVector.y)
        {
            vector.y = minVector.y;
        }
        else if (vector.y > maxVector.y)
        {
            vector.y = maxVector.y;
        }
    }
}
