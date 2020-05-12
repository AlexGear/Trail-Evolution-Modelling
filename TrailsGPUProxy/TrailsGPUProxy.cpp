#include "TrailsGPUProxy.h"
#include "IsolatedAttractorsException.h"
#include "Attractor.h"
#include "AttractorsMap.h"
#include "EdgesData.h"
#include "TramplabilityMask.h"
#include "EdgesWeights.h"
#include "WavefrontJob.h"
#include "WavefrontJobsFactory.h"

namespace TrailEvolutionModelling {
	namespace GPUProxy {

		TrailsComputationsOutput^ TrailsGPUProxy::ComputeTrails(TrailsComputationsInput^ input) {
			TrailsComputationsOutput^ result = nullptr;

			ResourceManager resources;
			Graph^ graph = input->Graph;
			try {
				NotifyProgress(L"������������ ������ ����� ������� ����������");
				AttractorsMap attractors(graph, input->Attractors);
				
				NotifyProgress(L"���������� ����� ���������������");
				TramplabilityMask* tramplabilityMask = resources.New<TramplabilityMask>(graph);

				NotifyProgress(L"������������� ����� ���� ��� \"������������ ���������\"");
				EdgesWeights* edgesWeights = resources.New<EdgesWeights>(graph, resources, true);

				NotifyProgress(L"�������� ������������ ��������� ��������� �� GPU");
				std::vector<WavefrontJob*> wavefrontJobs =
					WavefrontJobsFactory::CreateJobs(graph->Width, graph->Height, resources, attractors);

				WaitForGPU();

				NotifyProgress(L"��������� �������� ���������");

				NotifyProgress(L"�������� ����������");
				EdgesDataHost<float>* trampledness = resources.New<EdgesDataHost<float>>(edgesWeights,
					input->Graph->Width, input->Graph->Height);

				resources.Free(edgesWeights);

				result = gcnew TrailsComputationsOutput();
				result->Graph = input->Graph;
				ApplyTrampledness(result->Graph, trampledness);
			}
			finally {
				resources.FreeAll();
			}

			return result;
		}

		void TrailsGPUProxy::NotifyProgress(const wchar_t* stage) {
			ProgressChanged(gcnew String(stage));
		}
	}
}