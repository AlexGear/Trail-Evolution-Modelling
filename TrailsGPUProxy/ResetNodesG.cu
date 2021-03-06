#include "ResetNodesG.h"
#include <cmath>
#include "MathUtils.h"

#define BLOCK_SIZE_X 16
#define BLOCK_SIZE_Y 16

namespace TrailEvolutionModelling {
	namespace GPUProxy {

		__global__ void ResetNodesGKernel(NodesDataHaloedDevice<ComputeNode> nodes, 
			int extendedW, int extendedH, int goalIndex) 
		{
			int i = blockIdx.x * blockDim.x + threadIdx.x;
			int j = blockIdx.y * blockDim.y + threadIdx.y;
			if(i < extendedW && j < extendedH) {
				int index = i + j * extendedW;

				nodes.data[index].g = (index == goalIndex ? 0 : INFINITY);
			}
		}

		cudaError_t ResetNodesG(NodesDataHaloedDevice<ComputeNode> nodes, 
			int graphW, int graphH, int goalIndex, cudaStream_t stream) 
		{
			int extendedW = graphW + 2;
			int extendedH = graphH + 2;
			dim3 threadsDim(BLOCK_SIZE_X, BLOCK_SIZE_Y);
			dim3 blocksDim(divceil(extendedW, BLOCK_SIZE_X), divceil(extendedH, BLOCK_SIZE_Y));

			ResetNodesGKernel<<<blocksDim, threadsDim, 0, stream>>>(nodes, extendedW, extendedH, goalIndex);

			return cudaGetLastError();
		}

	}
}