using UnityEngine;


public class RoadSetup : MonoBehaviour {

    public enum Labels {
        A, B, C, D, E, F, G, H, I, P, Q, R, S, T, U, V, W, X
    }



    [System.Serializable]
    public struct RoutesPriorities {
        public int splineIndex;
        public int priority;
    }

    [System.Serializable]
    public struct RoutesMap {
        public Labels from;
        public Labels to;
        public RoutesPriorities[] routes;
    }

    [System.Serializable]
    public struct IncomingFrom {
        public Labels from;
        public Transform fromTransform;
        public Transform incomingFrom;
    }

    [System.Serializable]
    public struct OutgoingTo {
        public Labels to;
        public Transform toTransform;
        public Transform outgoingTo;
    }

    [SerializeField] private RoadTypeScriptableObject roadType;
    [SerializeField] private IncomingFrom[] from;
    [SerializeField] private OutgoingTo[] to;
    [SerializeField] private RoutesMap[] routesMap;



    //[SerializeField] private Transform 


}
