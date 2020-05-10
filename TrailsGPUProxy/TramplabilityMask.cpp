#include "TramplabilityMask.h"

namespace TrailEvolutionModelling {
	namespace GPUProxy {

		TramplabilityMask::TramplabilityMask(Graph^ graph)
			: EdgesDataHost<uint8_t>(graph->Width, graph->Height) 
		{
			ZipWithGraphEdges(graph, [](uint8_t& tramplable, Edge^ edge) {
				tramplable = (edge != nullptr && edge->IsTramplable);
			});
		}
	}
}