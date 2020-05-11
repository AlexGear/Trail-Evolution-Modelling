#include "TrailsGPUProxy.h"
#include "IsolatedAttractorsException.h"
#include "Attractor.h"
#include "AttractorsMap.h"
#include "EdgesData.h"
#include "TramplabilityMask.h"
#include "EdgesWeights.h"

namespace TrailEvolutionModelling {
	namespace GPUProxy {

		TrailsComputationsOutput^ TrailsGPUProxy::ComputeTrails(TrailsComputationsInput^ input) {
			TrailsComputationsOutput^ result = nullptr;

			ResourceManager resources;
			try {
				NotifyProgress(L"������������ ������ ����� ������� ����������");
				AttractorsMap attractors(input->Graph, input->Attractors);
				
				NotifyProgress(L"���������� ����� ���������������");
				TramplabilityMask tramplabilityMask = resources.New<TramplabilityMask>(input->Graph);

				NotifyProgress(L"������������� ����� ���� ��� \"������������ ���������\"");
				EdgesWeights indecentEdgesWeights = resources.New<EdgesWeights>(input->Graph, resources, true);



				NotifyProgress(L"��������� �������� ���������");

				NotifyProgress(L"�������� ����������");
				EdgesDataHost<float> trampledness = resources.New<EdgesDataHost<float>>(indecentEdgesWeights, 
					input->Graph->Width, input->Graph->Height);

				resources.Free(indecentEdgesWeights);

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