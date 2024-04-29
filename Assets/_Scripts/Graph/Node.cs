using Simulator.Road;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;

namespace Simulator.Graph {
    public interface INode : IEquatable<INode> {
        string Name { get; }

        List<Edge<INode>> Edges { get; }
        List<Edge<INode>> InternalEdges { get; }

        bool AddEdge(Edge<INode> edgeData);
        bool CheckEdge(INode dest);

        void Clear();
    }


    public class Edge<T> where T : INode {
        public int Weight { get; private set; }
        //public T Src { get; private set; }
        public T Dest { get; private set; }

        //public Edge(T src, T dest, int weight = 1) {
        //    Src = src;
        //    Dest = dest;
        //    Weight = weight;
        //}

        public Edge(T dest, int weight = 1) {
            Dest = dest;
            Weight = weight;
        }

    }


    public class Node : INode {
        public string Name { get; private set; }
        //public RoadSetup roadSetup;
        public float3 position;
        public RoadSetup roadSetup;
        public List<Edge<INode>> Edges { get; private set; }
        public List<Edge<INode>> InternalEdges { get; private set; }

        //List<Edge<RoadSetup>> INode<RoadSetup>.Edges => throw new System.NotImplementedException();

        //List<Edge<RoadSetup>> INode<RoadSetup>.InternalEdges => throw new System.NotImplementedException();


        public Node(string name, float3 position, RoadSetup roadSetup) {
            Name = name;
            Edges = new();
            InternalEdges = new();
            this.position = position;
            this.roadSetup = roadSetup;
        }

        //public virtual bool AddEdge(Edge<INode> edgeData) {
        //    Edges.Add(edgeData);
        //    return true;
        //}

        //public virtual bool CheckEdge(Node dest) {
        //    if (Edges.Find(ne => ne.Dest == dest) == null)
        //        return false;
        //    return true;
        //}

        //public virtual void Clear() {
        //    Edges.Clear(); InternalEdges.Clear();
        //}

        public override string ToString() {
            StringBuilder sb = new();
            sb.Append("(");
            sb.Append(Name);
            sb.Append(" -> ");
            foreach (var edge in Edges) {
                sb.Append(edge.Dest.Name);
                sb.Append(", ");
            }
            //return "(" + this.Name + "," + Dest.Name + ")";
            return sb.ToString();
        }

        public bool Equals(INode obj) {
            if (obj is null)
                return false;
            else
                return this.GetHashCode() == obj.GetHashCode();
        }

        public bool AddEdge(Edge<INode> edgeData) {
            Edges.Add(edgeData);
            return true;
        }

        public bool CheckEdge(INode dest) {
            if (Edges.Find(ne => ne.Dest == dest) == null)
                return false;
            return true;
        }


        public void Clear() {
            Edges.Clear(); InternalEdges.Clear();
        }

    }


}

