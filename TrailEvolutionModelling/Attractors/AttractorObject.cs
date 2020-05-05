﻿using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;

namespace TrailEvolutionModelling.Attractors
{
    public enum AttractorType { Universal, Source, Drain }
    
    public class AttractorObject : Feature, IXmlSerializable
    {
        public static readonly float DefaultWorkingRadius = 1000;

        public float WorkingRadius { get; set; }
        public Point Position
        {
            get => Geometry as Point ?? new Point();
            set => Geometry = value;
        }

        public bool IsLarge
        {
            get => isLarge;
            set
            {
                isLarge = value;
                UpdateStyle();
            }
        }

        public AttractorType Type
        {
            get => type;
            set
            {
                type = value;
                UpdateStyle();
            }
        }

        private bool isLarge;
        private AttractorType type;


        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Position = (Point)Mapsui.Geometries.Geometry.GeomFromText(reader.GetAttribute("Position"));
            WorkingRadius = float.Parse(reader.GetAttribute("WorkingRadius"));
            IsLarge = bool.Parse(reader.GetAttribute("IsLarge"));
            Type = (AttractorType)Enum.Parse(typeof(AttractorType), reader.GetAttribute("Type"));

            reader.ReadStartElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Position", Position.AsText());
            writer.WriteAttributeString("WorkingRadius", WorkingRadius.ToString());
            writer.WriteAttributeString("IsLarge", IsLarge.ToString());
            writer.WriteAttributeString("Type", Type.ToString());
        }

        private void UpdateStyle()
        {
            Styles.Clear();
            Styles.Add(new VectorStyle
            {
                Fill = new Brush(GetColor()),
                Outline = new Pen(Dimmed(GetColor(), 0.5f), GetPenWidth())
            });

            Color GetColor()
            {
                switch (type)
                {
                    case AttractorType.Universal: return Color.FromArgb(255, 230, 230, 30);
                    case AttractorType.Source: return Color.FromArgb(255, 230, 30, 30);
                    case AttractorType.Drain: return Color.FromArgb(255, 30, 30, 230);
                    default: throw new NotSupportedException("Unknown AttractorType");
                }
            }

            Color Dimmed(Color color, float factor)
            {
                return Color.FromArgb(255, (int)(color.R * factor),
                                           (int)(color.G * factor),
                                           (int)(color.B * factor));
            }

            double GetPenWidth() => IsLarge ? 8 : 1.5;
        }
    }
}