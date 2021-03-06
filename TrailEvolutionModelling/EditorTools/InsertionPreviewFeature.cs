﻿using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using TrailEvolutionModelling.MapObjects;
using Polygon = TrailEvolutionModelling.MapObjects.Polygon;

namespace TrailEvolutionModelling.EditorTools
{
    class InsertionPreviewFeature : Feature
    {
        private readonly IList<Point> vertices;
        public Point Vertex { get; private set; }
        public int Index { get; private set; }

        public InsertionPreviewFeature(MapObject mapObject, Point vertex, int index)
        {
            if (mapObject == null)
            {
                throw new ArgumentNullException(nameof(mapObject));
            }
            vertices = mapObject.Vertices;
            Update(vertex, index);
        }

        public void Update(Point vertex, int index)
        {
            if (index < 0 || index >= vertices.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            Geometry = Vertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
            Index = index;
        }
    }
}
