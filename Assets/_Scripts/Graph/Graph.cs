using System.Collections.Generic;
using UnityEngine;

namespace Simulator.Graph {
    public abstract class GraphBase<T>
        where T : INode {

        protected readonly List<T> adjencyList;
        public List<T> AdjencyList {
            get {
                return adjencyList;
            }
        }
        public GraphBase() {
            adjencyList = new List<T>();
        }

        public virtual bool AddNode(T node) {
            if (adjencyList.Contains(node))
                return false;

            adjencyList.Add(node);
            return true;
        }

        public void Clear() {
            foreach (var item in adjencyList) {
                item.Clear();
            }

            adjencyList.Clear();
        }

        public void PrintGraph() {
            foreach (T node in adjencyList) {
                string temp = $"EdgeCount: {node.Edges.Count}, {node.Name}";
                foreach (Edge<INode> edge in node.Edges) {
                    temp += $" -> {edge.Dest.Name}";
                }
                Debug.Log(temp);
            }
        }

        internal void PrintNodes() {
            foreach (T node in adjencyList) {
                Debug.Log(node.Name);
            }
        }
        public virtual bool AddEdge(T src, Edge<INode> edgeData) {
            if (!adjencyList.Contains(src))
                return false;
            return src.AddEdge(edgeData);
        }

        public bool CheckEdge(T src, T dest) {
            if (!adjencyList.Contains(src))
                return false;

            return src.CheckEdge(dest);
        }

        //public Edge<INode> GetEdge(T src, T dest) {
        //    return src.GetEdge(dest);
        //}


    }

    public class Graph<T> : GraphBase<T>
        where T : INode {

        internal List<T> FindShortestPath(T src, T dest) {
            return BFSAlgorithm(src, dest);
        }

        private struct QueueStruct {
            public T fromNode;
            public T toNode;
            public int weight;
        }


        // https://www.youtube.com/watch?v=T_m27bhVQQQ
        private List<T> BFSAlgorithm(T src, T dest) {
            // HashSet<Node> visitedNodes = new();
            Queue<QueueStruct> queue = new();


            /// Key: To node , Value.Item1: FromNode, Item2: Cost
            Dictionary<T, (T, int)> pathAndCost = new();


            queue.Enqueue(new QueueStruct { fromNode = src, toNode = src, weight = 0 });

            while (queue.Count > 0) {
                QueueStruct queueStruct = queue.Dequeue();
                //Debug.Log($"from:{queueStruct.fromNode.Name}, to:{queueStruct.toNode.Name}, weight:{queueStruct.weight}");

                if (pathAndCost.ContainsKey(queueStruct.toNode)) {
                    if (pathAndCost[queueStruct.toNode].Item2 < queueStruct.weight)
                        continue;
                    else
                        pathAndCost[queueStruct.toNode] = (queueStruct.fromNode, queueStruct.weight);

                }
                else {
                    pathAndCost.Add(queueStruct.toNode, (queueStruct.fromNode, queueStruct.weight));
                }

                var edgeNodesOfNode = queueStruct.toNode.Edges;

                if (edgeNodesOfNode.Count == 0)
                    continue;

                foreach (var nodeEdge in edgeNodesOfNode) {
                    //if (queueStruct.fromNode == queueStruct.toNode || queueStruct.fromNode == nodeEdge.IncommingFrom) {
                    if (queueStruct.fromNode.Equals(nodeEdge.Dest))
                        continue;

                    queue.Enqueue(new QueueStruct {
                        fromNode = queueStruct.toNode,
                        toNode = (T)nodeEdge.Dest,
                        weight = queueStruct.weight + nodeEdge.Weight
                    });
                }

            }

            /// Printing shortest path
            if (!pathAndCost.ContainsKey(dest)) {
                return null;
            }

            T tempNode = dest;
            // int weight = pathAndCost[tempNode].Item2;

            //Debug.Log(src.Name);
            //Debug.Log(tempNode.Name);

            List<T> nodes = new();
            do {
                nodes.Add(tempNode);
                tempNode = pathAndCost[tempNode].Item1;
            } while (!tempNode.Equals(src));

            //foreach (var item in pathAndCost) {
            //    Debug.Log($"toNode:{item.Key.Name}, fromNode:{item.Value.Item1.Name}, cost:{item.Value.Item2}");
            //}
            //Debug.Log(nodes.Count);


            nodes.Add(src);

            nodes.Reverse();

            // string temp = $"{nodes[0].Value}";
            // for (int i = 1; i < nodes.Count; i++) {
            //   temp += $" -> {nodes[i].Value}";
            // }
            // Debug.Log("Weight: " + weight);
            // Debug.Log("number of nodes: " + nodes.Count);
            // Debug.Log(temp);

            if (0 == nodes.Count)
                return null;

            return nodes;
        }

    }
}