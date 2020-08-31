using UnityEngine;

namespace SUOKI
{
    public abstract class Actor : MonoBehaviour
    {
        private GameObject _cachedgameObject;
        public GameObject cGameObj {
            get {
                if ( _cachedgameObject == null )
                    _cachedgameObject = gameObject;
                return _cachedgameObject;
            }
        }

        private Transform _cachedTransform;
        public Transform cTrf {
            get {
                if ( _cachedTransform == null )
                    _cachedTransform = transform;
                return _cachedTransform;
            }
        }

        public Transform root { get { return cTrf.root; } }
        public Transform parent {
            get { return cTrf.parent; }
            set { cTrf.parent = value; }
        }

        public string parentName { get { return parent == null ? string.Empty : parent.name; } }

        public Vector3 position {
            get { return cTrf.position; }
            set { cTrf.position = value; }
        }

        public Quaternion rotation {
            get { return cTrf.rotation; }
            set { cTrf.rotation = value; }
        }

        public Vector3 forward {
            get { return cTrf.forward; }
            set { cTrf.forward = value; }
        }

        public Vector3 localPosition {
            get { return cTrf.localPosition; }
            set { cTrf.localPosition = value; }
        }

        public Quaternion localRotation {
            get { return cTrf.localRotation; }
            set { cTrf.localRotation = value; }
        }

        public Vector3 localScale {
            get { return cTrf.localScale; }
            set { cTrf.localScale = value; }
        }

        public int layer {
            get { return gameObject.layer; }
            set { gameObject.layer = value; }
        }
    }


}
