using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


[RequireComponent(typeof(SplineContainer))]
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
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private RoadConnector[] incomming;
    [SerializeField] private RoadConnector[] outgoing;
    [SerializeField] private RoutesMap[] routesMap;

    private void Awake() {
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();
    }

    public RoadConnector[] GetFromConnectors() {
        return incomming;
    }

    public RoadConnector[] GetToConnectors() {
        return outgoing;
    }

    public EdgeData GetIncommingConnector(RoadSetup adjecentIncommingNode) {
        foreach (var incommingNode in incomming) {
            if (incommingNode.AdjecentRoadConnector.ParentRoadSetup == adjecentIncommingNode) {
                foreach (var edge in incommingNode.AdjecentRoadConnector.ParentRoadSetup.Edges) {
                    if (edge.Dest == this)
                        return edge;
                }
            }
            //if (edge.Dest == adjecentIncommingNode)
            //    return edge;
        }
        return null;
    }

    public EdgeData GetOutgoingConnector(RoadSetup adjecentOutgoingNode) {
        foreach (var edge in edges) {
            if (edge.Dest == adjecentOutgoingNode)
                return edge;
        }
        return null;
    }

    [Range(0f, 2f)]
    public float t;
    public Spline GetRouteFromConnectors(RoadConnector fromConnector, RoadConnector toConnector) {
        Spline result = null;

        if (fromConnector == null && toConnector == null)
            return result;

        if (fromConnector == null && toConnector != null) {
            foreach (var route in routesMap) {
                if (route.to == toConnector) {
                    int splineIndex = route.routes[0].splineIndex;
                    //float3 tempF = splineContainer.EvaluatePosition();
                    //result.Add(new Vector3(tempF.x, tempF.y, tempF.z));
                    return splineContainer.Splines[splineIndex];
                }
            }
        }

        if (fromConnector != null && toConnector == null) {

            return result;
        }



        return result;
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
