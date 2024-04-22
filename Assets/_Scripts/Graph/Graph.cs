using System.Collections.Generic;
using UnityEngine;

public class EdgeData {
    public int Weight { get; private set; }
    public RoadSetup Src { get; private set; }
    public RoadSetup Dest { get; private set; }
    public RoadSetup IncommingFrom { get; private set; }
    public RoadConnector FromRoadConnector { get; private set; }
    public RoadConnector ToRoadConnector { get; private set; }
    public EdgeData(RoadSetup src, RoadSetup dest, RoadSetup incommingFrom, RoadConnector from, RoadConnector to, int weight = 1) {
        Src = src;
        Dest = dest;
        IncommingFrom = incommingFrom;
        FromRoadConnector = from;
        ToRoadConnector = to;
        this.Weight = weight;
    }

    public override string ToString() {
        return "(" + this.Src.name + "," + this.Dest.name + "," + this.FromRoadConnector.name + "," + this.ToRoadConnector.name + ")";
    }
}
public class Graph {
    private readonly List<RoadSetup> adjencyList;

    public List<RoadSetup> AdjencyList {
        get {
            return adjencyList;
        }
    }
    public Graph() {
        adjencyList = new List<RoadSetup>();
    }

    public bool AddNode(RoadSetup node) {
        if (adjencyList.Contains(node))
            return false;

        adjencyList.Add(node);
        return true;
    }

    public bool AddEdge(EdgeData edgeData) {
        if (!adjencyList.Contains(edgeData.Src))
            return false;
        return edgeData.Src.AddEdge(edgeData);
    }
    public bool CheckEdge(RoadSetup src, RoadSetup dest) {
        if (!adjencyList.Contains(src))
            return false;

        return src.CheckEdge(dest);
    }
    public EdgeData GetEdge(RoadSetup src, RoadSetup dest) {
        return src.GetEdge(dest);
    }

    internal void Clear() {
        adjencyList.Clear();
    }

    internal void PrintGraph() {
        foreach (RoadSetup node in adjencyList) {
            string temp = $"EdgeCount: {node.Edges.Count}, {node.name}";
            foreach (EdgeData edge in node.Edges) {
                temp += $" -> {edge.Dest.name}";
            }
            Debug.Log(temp);
        }
    }

    internal void PrintNodes() {
        foreach (RoadSetup node in adjencyList) {
            Debug.Log(node.transform.name);
        }
    }

    internal List<RoadSetup> FindShortestPath(RoadSetup src, RoadSetup dest) {
        return BFSAlgorithm(src, dest);
    }

    private struct QueueStruct {
        public RoadSetup fromNode;
        public RoadSetup toNode;
        public int weight;
    }


    // https://www.youtube.com/watch?v=T_m27bhVQQQ
    private List<RoadSetup> BFSAlgorithm(RoadSetup src, RoadSetup dest) {
        List<RoadSetup> nodes = new();
        // HashSet<Node> visitedNodes = new();
        Queue<QueueStruct> queue = new();
        Dictionary<RoadSetup, (RoadSetup, int)> pathAndCost = new();


        queue.Enqueue(new QueueStruct { fromNode = src, toNode = src, weight = 0 });

        while (queue.Count > 0) {
            QueueStruct queueStruct = queue.Dequeue();

            if (pathAndCost.ContainsKey(queueStruct.toNode)) {
                if (pathAndCost[queueStruct.toNode].Item2 < queueStruct.weight)
                    continue;
                else
                    pathAndCost[queueStruct.toNode] = (queueStruct.fromNode, queueStruct.weight);

            }
            else {
                pathAndCost.Add(queueStruct.toNode, (queueStruct.fromNode, queueStruct.weight));
            }

            List<EdgeData> edgeNodesOfNode = queueStruct.toNode.Edges;

            if (edgeNodesOfNode.Count == 0)
                continue;

            foreach (EdgeData nodeEdge in edgeNodesOfNode) {
                if (queueStruct.fromNode == queueStruct.toNode || queueStruct.fromNode == nodeEdge.IncommingFrom) {
                    queue.Enqueue(new QueueStruct {
                        fromNode = nodeEdge.Src,
                        toNode = nodeEdge.Dest,
                        weight = queueStruct.weight + nodeEdge.Weight
                    });
                }
            }
        }

        /// Printing shortest path
        if (!pathAndCost.ContainsKey(dest)) {
            Debug.Log($"No path found between {src.transform.name} and {dest.transform.name}.");
            return null;
        }

        RoadSetup tempNode = dest;
        // int weight = pathAndCost[tempNode].Item2;

        do {
            nodes.Add(tempNode);
            tempNode = pathAndCost[tempNode].Item1;
        } while (tempNode != src);

        nodes.Add(src);

        nodes.Reverse();

        // string temp = $"{nodes[0].Value}";
        // for (int i = 1; i < nodes.Count; i++) {
        //   temp += $" -> {nodes[i].Value}";
        // }
        // Debug.Log("Weight: " + weight);
        // Debug.Log("number of nodes: " + nodes.Count);
        // Debug.Log(temp);

        return nodes;
    }

}