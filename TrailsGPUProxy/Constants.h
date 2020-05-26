#pragma once

constexpr float SIMULATION_STEP_SECONDS = 60 * 60;

constexpr float MIN_TRAMPLABLE_WEIGHT = 1.1f;
constexpr float TRAMPLABLE_WEIGHT_REDUCTION_FACTOR_FOR_INDECENT = 2.0f;
constexpr float MAX_LAWN_WEIGHT = 2.7f;
constexpr float DELTA_MIN_MAX_LAWN_WEIGHT = (MAX_LAWN_WEIGHT - MIN_TRAMPLABLE_WEIGHT);

constexpr float FIRST_PHASE_PATH_THICKNESS = 5.0f;
constexpr float SECOND_PHASE_PATH_THICKNESS = 1.5f;

constexpr float INDECENT_PEDESTRIANS_SHARE = 0.027f;
constexpr float DECENT_PEDESTRIANS_SHARE = (1 - INDECENT_PEDESTRIANS_SHARE);
constexpr float HUMAN_STEPS_PER_METER = 1.25f;
constexpr float TRAMPLING_EFFECT_PER_HUMAN_STEP = (0.000526f * DELTA_MIN_MAX_LAWN_WEIGHT);
constexpr float LAWN_REGENERATION_PER_SECOND = (0.0033f / 3600 * DELTA_MIN_MAX_LAWN_WEIGHT);