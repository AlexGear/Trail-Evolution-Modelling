#pragma once

#define SIMULATION_STEP_SECONDS 10 * 60

#define MIN_TRAMPLABLE_WEIGHT 1.1f
#define TRAMPLABLE_WEIGHT_FOR_INDECENT 1.1f
#define MAX_LAWN_WEIGHT 2.7f
#define DELTA_MIN_MAX_LAWN_WEIGHT (MAX_LAWN_WEIGHT - MIN_TRAMPLABLE_WEIGHT)

#define FIRST_PHASE_PATH_THICKNESS 5.0f
#define SECOND_PHASE_PATH_THICKNESS 1.5f

#define INDECENT_PEDESTRIANS_SHARE 0.075f
#define DECENT_PEDESTRIANS_SHARE (1 - INDECENT_PEDESTRIANS_SHARE)
#define HUMAN_STEPS_PER_METER 1.25f
#define TRAMPLING_EFFECT_PER_HUMAN_STEP (0.000526f * DELTA_MIN_MAX_LAWN_WEIGHT)
#define LAWN_REGENERATION_PER_SECOND (0.0033 * DELTA_MIN_MAX_LAWN_WEIGHT / 3600)
#define LAWN_REGENERATION_PER_SIMULATION_STEP (LAWN_REGENERATION_PER_SECOND * SIMULATION_STEP_SECONDS)