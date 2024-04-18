using System.Collections.Generic;
using UnityEngine;


public class RoadSetup : MonoBehaviour {

    //public enum Labels {
    //    A, B, C, D, E, F, G, H, I, P, Q, R, S, T, U, V, W, X
    //}



    [System.Serializable]
    public struct RoutesPriorities {
        public int splineIndex;
        public int priority;
    }

    [System.Serializable]
    public struct RoutesMap {
        public RoadConnector from;
        public RoadConnector to;
        public RoutesPriorities[] routes;
    }

    //[System.Serializable]
    //public struct ConnectorType {
    //    public Labels from;
    //    //public Transform fromTransform;
    //    //public RoadSetup incomingFromNode;
    //    public RoadConnector connector;
    //}


    [SerializeField] private RoadTypeScriptableObject roadType;
    [SerializeField] private RoadConnector[] incomming;
    [SerializeField] private RoadConnector[] outgoing;
    [SerializeField] private RoutesMap[] routesMap;

    public RoadConnector[] GetFromConnectors() {
        return incomming;
    }

    public RoadConnector[] GetToConnectors() {
        return outgoing;
    }

    #region Graph related     
    private readonly List<EdgeData> edges = new();
    public List<EdgeData> Edges {
        get {
            return edges;
        }
    }

    public bool AddEdge(EdgeData edgeData) {
        edges.Add(edgeData);
        return true;
    }

    public bool CheckEdge(RoadSetup dest) {
        if (GetEdge(dest) == null)
            return false;
        return true;
    }

    public EdgeData GetEdge(RoadSetup dest) {
        return edges.Find(ne => ne.Dest == dest);
    }

    public void Clear() {
        edges.Clear();
    }

    #endregion



    //[SerializeField] private Transform 


}
