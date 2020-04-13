﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Priority_Queue;
using UnityEngine;

namespace TrailEvolutionModelling
{
    public sealed class Node : FastPriorityQueueNode
    {
        public Vector2 Position { get; set; }
        public List<Edge> IncidentEdges { get; }

        public bool IsClosed;
        public float G1;
        public float F1;
        public float G2;
        public float F2;
        public int H1;
        public int H2;
        public Node CameFrom1;
        public Node CameFrom2;

        public Node(Vector2 position)
        {
            Position = position;
            IncidentEdges = new List<Edge>();

            CleanupAfterPathSearch();
        }

        public void AddIncidentEdge(Edge edge)
        {
            if (!IncidentEdges.Contains(edge))
            {
                IncidentEdges.Add(edge);
            }
        }

        public bool RemoveIncidentEdge(Edge edge)
        {
            if (IncidentEdges.Remove(edge))
            {
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float G(bool forward) => ref (forward ? ref G1 : ref G2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref float F(bool forward) => ref (forward ? ref F1 : ref F2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref int H(bool forward) => ref (forward ? ref H1 : ref H2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Node CameFrom(bool forward) => ref (forward ? ref CameFrom1 : ref CameFrom2);



        public void CleanupAfterPathSearch()
        {
            IsClosed = false;
            G1 = float.PositiveInfinity;
            G2 = float.PositiveInfinity;
            F1 = float.PositiveInfinity;
            F2 = float.PositiveInfinity;
            H1 = -1;
            H2 = -1;
            CameFrom1 = null;
            CameFrom2 = null;
        }
    }

    public sealed class Edge : IEquatable<Edge>
    {
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public float Weight { get; set; }
        public float Trampledness { get; set; }
        public bool IsTramplable { get; set; }

        public Edge(Node node1, Node node2, float weight, bool isTramplable)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
            Trampledness = 0;
            IsTramplable = isTramplable;
        }

        public Node GetOppositeNode(Node node)
        {
            if (node == Node1)
                return Node2;
            if (node == Node2)
                return Node1;

            throw new ArgumentException("Non-incident node");
        }

        public bool Equals(Edge other)
        {
            return (this.Node1 == other.Node1 && this.Node2 == other.Node2 ||
                    this.Node2 == other.Node1 && this.Node1 == other.Node2);
        }

        public override bool Equals(object obj) => obj is Edge other && this.Equals(other);

        public override int GetHashCode() => Node1.GetHashCode() + Node2.GetHashCode();
    }

    public sealed class Graph
    {
        public Node[][] Nodes { get; set; } = new Node[0][];
        public HashSet<Edge> Edges { get; } = new HashSet<Edge>();

        //public Node AddNode(Vector2 position)
        //{
        //    var node = new Node(position);
        //    Nodes.Add(node);
        //    return node;
        //}

        //public void RemoveNode(Node node)
        //{
        //    Nodes.Remove(node);
        //    Edges.RemoveWhere(edge => node.IncidentEdges.Contains(edge));
        //}

        public Edge AddEdge(Node node1, Node node2, float weight, bool isTramplable)
        {
            var edge = new Edge(node1, node2, weight, isTramplable);
            if (Edges.Add(edge))
            {
                node1.AddIncidentEdge(edge);
                node2.AddIncidentEdge(edge);
                return edge;
            }
            return null;
        }

        public void RemoveEdge(Edge edge)
        {
            Edges.Remove(edge);
            edge.Node1.RemoveIncidentEdge(edge);
            edge.Node2.RemoveIncidentEdge(edge);
        }
    }
}