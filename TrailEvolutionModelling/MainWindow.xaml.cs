﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.UI.Wpf;
using Mapsui.Utilities;
using TrailEvolutionModelling.EditorTools;
using TrailEvolutionModelling.GPUProxy;
using TrailEvolutionModelling.MapObjects;
using TrailEvolutionModelling.Util;
using Point = Mapsui.Geometries.Point;
using Polygon = TrailEvolutionModelling.MapObjects.Polygon;

namespace TrailEvolutionModelling
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WritableLayer polygonLayer;

        private PolygonTool polygonTool;
        private MapObjectEditing mapObjectEditing;
        private Tool[] allTools;

        public MainWindow()
        {
            InitializeComponent();

            mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                EndAllTools();
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            polygonLayer = new PolygonLayer();
            InitializeMapControl();
            //polygonLayer.AddRange(polygonStorage.Polygons);

            InitTools();

            ZoomToPoint(new Point(9231625, 7402608));
        }

        private void InitTools()
        {
            polygonTool = new PolygonTool(mapControl, polygonLayer);
            mapObjectEditing = new MapObjectEditing(mapControl, polygonLayer);
            allTools = new Tool[] { polygonTool, mapObjectEditing };
        }

        private void ZoomToPoint(Point center)
        {
            var extent = new Point(1000, 1000);
            mapControl.ZoomToBox(center - extent, center + extent);
        }

        private void InitializeMapControl()
        {
            mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            mapControl.Map.Layers.Add(polygonLayer);
            //mapControl.Map.Layers.Add(polygonLayer);

            mapControl.MouseLeftButtonDown += OnMapLeftClick;
            mapControl.MouseRightButtonDown += OnMapRightClick;
        }

        private ContextMenu CreatePolygonContextMenu(Polygon polygon)
        {
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(CreateModifyItem());
            contextMenu.Items.Add(CreateRemoveItem());
            return contextMenu;

            ///////// BUTTONS CREATION LOCAL FUNCS
            MenuItem CreateModifyItem()
            {
                var item = new MenuItem
                {
                    Header = "Редактировать",
                    Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/EditPolygon.png")) }
                };
                item.Click += (s, e) =>
                {
                    UnhighlightAllPolygons();
                    EndAllTools();
                    mapObjectEditing.TargetObject = polygon;
                    mapObjectEditing.Begin();
                };
                return item;
            }

            MenuItem CreateRemoveItem()
            {
                var item = new MenuItem
                {
                    Header = "Удалить полигон",
                    Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Delete.png")) }
                };
                item.Click += (s, e) =>
                {
                    polygonLayer.TryRemove(polygon);
                    polygonLayer.Refresh();
                };
                return item;
            }
            ///////// END BUTTONS CREATION LOCAL FUNCS
        }

        private void OnMapRightClick(object sender, MouseButtonEventArgs e)
        {
            UnhighlightAllPolygons();

            bool anyToolWasAcitve = EndAllTools();
            if (anyToolWasAcitve)
            {
                return;
            }

            var clickScreenPos = e.GetPosition(mapControl).ToMapsui();
            var clickWorldPos = mapControl.Viewport.ScreenToWorld(clickScreenPos);
            IEnumerable<Polygon> polygons = GetPolygonsAt(clickWorldPos);

            int count = polygons.Count();
            if (count == 0)
            {
                return;
            }
            if (count == 1)
            {
                OnPolygonRightClick(polygons.First());
            }
            else
            {
                var contextMenu = new ContextMenu();
                contextMenu.Items.Add(new Label
                {
                    Content = "Выберите объект:",
                    IsEnabled = false
                });
                foreach (var polygon in polygons)
                {
                    var item = new MenuItem
                    {
                        //Header = polygon.ObjectKindName
                    };
                    item.GotFocus += (s, ee) => { polygon.Highlighter.IsHighlighted = true; polygonLayer.Refresh(); };
                    item.LostFocus += (s, ee) => { polygon.Highlighter.IsHighlighted = false; polygonLayer.Refresh(); };
                    item.Click += (s, ee) => OnPolygonRightClick(polygon);
                    contextMenu.Items.Add(item);
                }
                contextMenu.IsOpen = true;
            }
        }

        private IEnumerable<Polygon> GetPolygonsAt(Point point)
        {
            var boundingBox = new BoundingBox(point, point);
            var polygons = polygonLayer.GetFeaturesInView(boundingBox, resolution: 1f).OfType<Polygon>();
            return polygons.Where(p => p.Geometry.Distance(point) <= 0);
        }

        private bool EndAllTools()
        {
            bool any = false;
            foreach (var tool in allTools)
            {
                any |= tool.End();
            }
            return any;
        }

        private void OnMapLeftClick(object sender, MouseButtonEventArgs e)
        {
            UnhighlightAllPolygons();
        }

        private void OnPolygonRightClick(Polygon polygon)
        {
            polygon.Highlighter.IsHighlighted = true;
            polygonLayer.Refresh();

            var contextMenu = CreatePolygonContextMenu(polygon);
            contextMenu.IsOpen = true;
        }

        private void UnhighlightAllPolygons()
        {
            foreach (var polygon in polygonLayer.GetFeatures().OfType<Polygon>())
            {
                polygon.Highlighter.IsHighlighted = false;
            }
            polygonLayer.Refresh();
        }

        private void OnPolygonDrawClick(object sender, RoutedEventArgs e)
        {
            EndAllTools();

            string areaTypeName = (string)((Button)sender).Tag;
            polygonTool.AreaType = AreaTypes.GetByName(areaTypeName);
            polygonTool.Begin();
        }

        private void OnLineDrawClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
