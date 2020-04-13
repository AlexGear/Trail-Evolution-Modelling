﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrailEvolutionModelling.Drawing;
using UnityEditor;
using UnityEngine;

namespace TrailEvolutionModelling
{
    [ExecuteInEditMode]
    public class PathFinderInvoker : LinesProvider, ILinesChangedNotifier
    {
        [SerializeField] GraphHolder graphHolder = null;
        [SerializeField] Transform start = null;
        [SerializeField] Transform end = null;
        [SerializeField] Color pathColor = Color.red;
        [SerializeField] int ind = 0;

        private Color oldPathColor;
        private Node[] path;

        public event Action<ILinesChangedNotifier> LinesChanged;

        [ContextMenu("Invoke A*")]
        public void InvokeAStar()
        {
            FindPath(PathFindingAlgorithm.AStar);
        }

        public int count;

        void OnDrawGizmos()
        {
            try
            {
                Handles.color = Color.white;
                foreach (var p in PathFinder.gg[ind])
                {
                    Handles.DrawSolidDisc(p, Vector3.forward, 0.2f);
                }
                count = PathFinder.gg[ind].Count;
            }
            catch { }
        }

        public void FindPath(PathFindingAlgorithm algorithm)
        {
            if (graphHolder == null || start == null || end == null)
            {
                return;
            }

            Graph graph = graphHolder.Graph;
            if (graph == null)
            {
                throw new InvalidOperationException("GraphHolder contains no Graph");
            }

            Node startNode = FindClosestNode(graph, start.position);
            Node endNode = FindClosestNode(graph, end.position);

            this.path = PathFinder.FindPath(graph, startNode, endNode, algorithm);
            if (this.path == null)
            {
                Debug.LogWarning("Path not found");
            }

            LinesChanged?.Invoke(this);
        }

        public override IEnumerable<ColoredLine> GetLines()
        {
            if (path == null || path.Length < 2)
                yield break;

            Node prev = path[0];
            foreach (var node in path.Skip(1))
            {
                Vector3 start = prev.Position;
                Vector3 end = node.Position;
                prev = node;
                yield return new ColoredLine(start, end, pathColor);
            }
        }

        private Node FindClosestNode(Graph graph, Vector2 position)
        {
            Node closest = null;
            float minSqrDist = float.PositiveInfinity;

            for (int i = 0; i < graph.Nodes.Length; i++)
                for (int j = 0; j < graph.Nodes[0].Length; j++)
                {
                    Node node = graph.Nodes[i][j];
                    if (node == null)
                        continue;

                    float sqrDist = (node.Position - position).sqrMagnitude;
                    if (sqrDist < minSqrDist)
                    {
                        minSqrDist = sqrDist;
                        closest = node;
                    }
                }

            return closest;
        }

        private void Update()
        {
            if (oldPathColor != pathColor)
            {
                oldPathColor = pathColor;

                LinesChanged?.Invoke(this);
            }
        }
    }
}