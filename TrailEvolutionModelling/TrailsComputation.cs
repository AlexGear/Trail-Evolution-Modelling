﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrailEvolutionModelling.GPUProxy;
using TrailEvolutionModelling.GraphBuilding;
using TrailEvolutionModelling.GraphTypes;
using TrailEvolutionModelling.MapObjects;

namespace TrailEvolutionModelling
{
    class TrailsComputation
    {
        public event EventHandler ProgressChanged;
        public string CurrentStage { get; private set; }

        private World world;

        public TrailsComputation(World world)
        {
            this.world = world;
        }

        public TrailsComputationsOutput Run()
        {
            ReportProgress("Построение графа");
            Graph graph = BuildGraph();
            Attractor[] attractors = GraphBuilder.CreateAttractors(graph, world.AttractorObjects);

            var computationsInput = new TrailsComputationsInput
            {
                Graph = graph,
                Attractors = attractors
            };
            var proxy = new TrailsGPUProxy();
            proxy.ProgressChanged += ReportProgress;
            TrailsComputationsOutput output = proxy.ComputeTrails(computationsInput);
            return output;
        }

        private void ReportProgress(string message)
        {
            CurrentStage = message;
            ProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        private Graph BuildGraph()
        {
            return GraphBuilder.Build(new GraphBuilderInput
            {
                World = world,
                DesiredStep = 2.4f
            });
        }
    }
}
