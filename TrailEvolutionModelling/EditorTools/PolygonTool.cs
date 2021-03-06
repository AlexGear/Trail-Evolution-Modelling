﻿using Mapsui;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrailEvolutionModelling.MapObjects;
using TrailEvolutionModelling.Util;

namespace TrailEvolutionModelling.EditorTools
{
    class PolygonTool : MapObjectTool<Polygon>
    {
        public PolygonTool(MapControl mapControl, WritableLayer targetLayer)
            : base(mapControl, targetLayer)
        {
        }

        protected override bool IsFinalResultAcceptable(Polygon finalResult)
        {
            return finalResult.IsVerticesNumberValid;
        }

        protected override Polygon CreateNewMapObject()
        {
            return new Polygon();
        }
    }
}
