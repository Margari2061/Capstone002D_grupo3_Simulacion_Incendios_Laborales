using UnityEngine;

namespace AideTool.Patterns
{
    public class AideBehaviour : MonoBehaviour, IAideCore
    {
        private GameObject m_gameObject;
        public GameObject GameObject
        {
            get
            {
                if (m_gameObject == null)
                    m_gameObject = gameObject;
                return m_gameObject;
            }
        }

        private Transform m_transform;
        public Transform Transform
        {
            get
            {
                if(m_transform == null)
                    m_transform = transform;
                return m_transform;
            }
        }

        private string m_name;
        public string Name
        {
            get
            {
                if(string.IsNullOrWhiteSpace(m_name))
                    m_name = Transform.name;
                return m_name;
            }
            set
            {
                m_name= value;
                Transform.name = value;
            }
        }

        private AideComponent<Camera> m_camera;
        public AideComponent<Camera> Camera
        {
            get
            {
                if(m_camera == null)
                    m_camera = new(UnityEngine.Camera.main);
                return m_camera;
            }
        }

        protected virtual void Awake() { }

        protected virtual void OnEnable() { }

        protected virtual void Start() { }

        protected virtual void Update() { }

        protected virtual void FixedUpdate() { }

        protected virtual void LateUpdate() { }

        protected virtual void OnAudioFilterRead(float[] data, int channels) { }

        protected virtual void OnCollisionEnter(Collision collision) { }

        protected virtual void OnCollisionStay(Collision collision) { }

        protected virtual void OnCollisionExit(Collision collision) { }

        protected virtual void OnTriggerEnter(Collider other) { }

        protected virtual void OnTriggerStay(Collider other) { }

        protected virtual void OnTriggerExit(Collider other) { }

        protected virtual void OnCollisionEnter2D(Collision2D collision) { }

        protected virtual void OnCollisionStay2D(Collision2D collision) { }

        protected virtual void OnCollisionExit2D(Collision2D collision) { }

        protected virtual void OnTriggerEnter2D(Collider2D collision) { }

        protected virtual void OnTriggerStay2D(Collider2D collision) { }

        protected virtual void OnTriggerExit2D(Collider2D collision) { }

        protected virtual void OnBecameInvisible() { }

        protected virtual void OnBecameVisible() { }

        protected virtual void OnDisable() { }

        protected virtual void OnDestroy() 
        {
            m_gameObject = null;
            m_transform = null;
            m_camera = null;
        }

        protected virtual void OnApplicationFocus(bool focus) { }

        protected virtual void OnApplicationPause(bool pause) { }

        protected virtual void OnApplicationQuit() { }

        protected virtual void OnValidate() { }

        protected virtual void OnDrawGizmos() { }

        protected virtual void OnDrawGizmosSelected() { }

        public T Instantiate<T>(Transform parent) where T : AideBehaviour
        {
            T instance = Instantiate((T)this, parent);
            return instance;
        }

        public T Instantiate<T>(Vector3 position, Quaternion rotation) where T : AideBehaviour
        {
            T instance = Instantiate((T)this, position, rotation);
            return instance;
        }

        public void Destroy(float delay = 0f)
        {
            Destroy(GameObject, delay);
        }

        public void Destroy(GameObject gameObject, float delay = 0f)
        {
            Object.Destroy(gameObject, delay);
        }
    }
}
