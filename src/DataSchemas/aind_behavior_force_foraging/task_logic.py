from __future__ import annotations

from enum import Enum
from typing import Annotated, Dict, List, Literal, Optional, Union

import aind_behavior_services.task_logic.distributions as distributions
from aind_behavior_force_foraging import __version__
from aind_behavior_services.task_logic import AindBehaviorTaskLogicModel
from aind_data_schema.base import AindModel
from pydantic import BaseModel, Field, RootModel

MAX_LOAD_CELL_FORCE = 32768


def scalar_value(value: float) -> distributions.Scalar:
    """
    Helper function to create a scalar value distribution for a given value.

    Args:
        value (float): The value of the scalar distribution.

    Returns:
        distributions.Scalar: The scalar distribution type.
    """
    return distributions.Scalar(distribution_parameters=distributions.ScalarDistributionParameter(value=value))


def uniform_distribution_value(min: float, max: float) -> distributions.Uniform:
    """
    Helper function to create a uniform distribution for a given range.

    Args:
        min (float): The minimum value of the uniform distribution.
        max (float): The maximum value of the uniform distribution.

    Returns:
        distributions.Uniform: The uniform distribution type.
    """
    return distributions.UniformDistribution(
        distribution_parameters=distributions.UniformDistributionParameters(max=max, min=min)
    )


# Available actions
class HarvestActionBase(BaseModel):
    """Defines an abstract class for an harvest action"""

    label: str = Field(default="HarvestAction", description="Label of the action")
    probability: float = Field(default=1, description="Probability of reward")
    amount: float = Field(default=1, description="Amount of reward to be delivered")
    delay: float = Field(default=0, description="Delay between successful harvest and reward delivery")
    force_duration: float = Field(default=0.5, description="Duration that the force much stay above threshold")
    force_threshold: float = Field(
        default=5000, le=MAX_LOAD_CELL_FORCE, ge=-MAX_LOAD_CELL_FORCE, description="Force to be applied"
    )


class RightHarvestAction(HarvestActionBase):
    label: Literal["RightHarvestAction"] = "RightHarvestAction"


class LeftHarvestAction(HarvestActionBase):
    label: Literal["LeftHarvestAction"] = "LeftHarvestAction"


class HarvestAction(RootModel):
    root: Annotated[Union[LeftHarvestAction, RightHarvestAction], Field(discriminator="label")]


class QuiescencePeriod(BaseModel):
    """Defines a quiescence settings"""

    duration: float = Field(default=0, ge=0, description="Duration of the quiescence period")
    force_threshold: float = Field(
        default=0, le=MAX_LOAD_CELL_FORCE, ge=-MAX_LOAD_CELL_FORCE, description="Time out for the quiescence period"
    )
    dithering_duration: float = Field(
        default=0, ge=0, description="Dithering duration used to exclude sporadic fast force changes."
    )
    has_cue: bool = Field(default=False, description="Whether to use a cue to signal the start of the period.")


class InitiationPeriod(BaseModel):
    """Defines an initiation period"""

    duration: float = Field(default=0, ge=0, description="Duration of the initiation period")
    has_cue: bool = Field(default=True, description="Whether to use a cue to signal the start of the period.")
    abort_on_force: bool = Field(
        default=False, description="Whether to abort the trial if a choice is made during the initiation period."
    )
    abort_on_force_threshold: float = Field(
        default=0, le=MAX_LOAD_CELL_FORCE, ge=-MAX_LOAD_CELL_FORCE, description="Time out for the quiescence period"
    )


class ResponsePeriod(BaseModel):
    """Defines a response period"""

    duration: float = Field(
        default=0, ge=0, description="Duration of the response period. I.e. the time the animal has to make a choice."
    )
    has_cue: bool = Field(default=True, description="Whether to use a cue to signal the start of the period.")
    abort_on_force: bool = Field(
        default=False, description="Whether to abort the trial if a choice is made during the initiation period."
    )
    abort_on_force_threshold: float = Field(
        default=0, le=MAX_LOAD_CELL_FORCE, ge=-MAX_LOAD_CELL_FORCE, description="Time out for the quiescence period"
    )
    has_feedback: bool = Field(
        default=False, description="Whether to provide feedback to the animal after the response period."
    )


class RewardPeriod(BaseModel):
    """Defines a reward period"""

    delay: float = Field(default=0, ge=0, description="Delay to reward availability.")
    is_operant: bool = Field(default=True, description="Whether the reward delivery is contingent on licking.")
    has_cue: bool = Field(default=True, description="Whether to use a cue to signal the availability of the reward.")
    time_to_collect: Optional[float] = Field(
        default=None,
        ge=0,
        description="Time to collect the reward after it is available. If null, the reward will be available indefinitely.",
    )


class Trial(BaseModel):
    """Defines a trial"""

    inter_trial_interval: float = Field(default=0, ge=0, description="Time between trials")
    quiescence_period: Optional[QuiescencePeriod] = Field(default=None, description="Quiescence settings")
    initiation_period: InitiationPeriod = Field(
        default=InitiationPeriod(), validate_default=True, description="Initiation settings"
    )
    response_period: ResponsePeriod = Field(
        default=ResponsePeriod(), validate_default=True, description="Response settings"
    )
    reward_period: RewardPeriod = Field(default=RewardPeriod(), validate_default=True, description="Reward settings")
    left_action: HarvestAction = Field(
        default=LeftHarvestAction(), validate_default=True, description="Specification of the left action"
    )
    right_action: HarvestAction = Field(
        default=RightHarvestAction(), validate_default=True, description="Specification of the right action"
    )
    time_out: float = Field(default=0, ge=0, description="Time out for the trial")


class BlockStatisticsMode(str, Enum):
    """Defines the mode of the environment"""

    BLOCK = "Block"
    RANDOMWALK = "RandomWalk"
    BLOCKGENERATOR = "BlockGenerator"


class Block(BaseModel):
    mode: Literal[BlockStatisticsMode.BLOCK] = BlockStatisticsMode.BLOCK
    trials: List[Trial] = Field(default=[], description="List of trials in the block")
    shuffle: bool = Field(default=False, description="Whether to shuffle the trials in the block")
    repeat_count: Optional[int] = Field(
        default=0, description="Number of times to repeat the block. If null, the block will be repeated indefinitely"
    )


class BlockGenerator(BaseModel):
    mode: Literal[BlockStatisticsMode.BLOCKGENERATOR] = BlockStatisticsMode.BLOCKGENERATOR
    baited: bool = Field(default=False, description="Whether the trials are baited")
    block_size: distributions.Distribution = Field(
        default=uniform_distribution_value(min=50, max=60), validate_default=True, description="Size of the block"
    )
    end_block_criteria: Optional[EndBlockCriteria] = Field(default=None, description="Criteria to end the block")

    class _EndBlockCriteriaBase(BaseModel):
        criteria: str

    class RewardRateCriteria(_EndBlockCriteriaBase):
        criteria: Literal["RewardRate"] = "RewardRate"
        threshold: float = Field(..., description="Reward rate to end the block")

    class PerformanceCriteria(_EndBlockCriteriaBase):
        criteria: Literal["RewardRate"] = "RewardRate"
        threshold: float = Field(..., description="Reward rate to end the block")

    class TimeCriteria(_EndBlockCriteriaBase):
        criteria: Literal["Time"] = "Time"
        duration_threshold: float = Field(..., description="Duration to end the block", ge=0)

    class EndBlockCriteria(RootModel):
        root: Annotated[
            Union[BlockGenerator.RewardRateCriteria, BlockGenerator.PerformanceCriteria, BlockGenerator.TimeCriteria],
            Field(discriminator="criteria"),
        ]


class RandomWalk(BaseModel):
    mode: Literal[BlockStatisticsMode.RANDOMWALK] = BlockStatisticsMode.RANDOMWALK


class BlockStatistics(RootModel):
    root: Annotated[Union[Block, BlockGenerator, RandomWalk], Field(discriminator="mode")]


class Environment(BaseModel):
    block_statistics: List[BlockStatistics] = Field(..., description="Statistics of the environment")
    shuffle: bool = Field(default=False, description="Whether to shuffle the blocks")
    repeat_count: Optional[int] = Field(
        default=0,
        description="Number of times to repeat the environment. If null, the environment will be repeated indefinitely",
    )


# Updaters
class NumericalUpdaterOperation(str, Enum):
    NONE = "None"
    OFFSET = "Offset"
    GAIN = "Gain"
    SET = "Set"
    OFFSETPERCENTAGE = "OffsetPercentage"


class NumericalUpdaterParameters(AindModel):
    initial_value: float = Field(default=0.0, description="Initial value of the parameter")
    increment: float = Field(default=0.0, description="Value to increment the parameter by")
    decrement: float = Field(default=0.0, description="Value to decrement the parameter by")
    minimum: float = Field(default=0.0, description="Minimum value of the parameter")
    maximum: float = Field(default=0.0, description="Minimum value of the parameter")


class NumericalUpdater(AindModel):
    operation: NumericalUpdaterOperation = Field(
        default=NumericalUpdaterOperation.NONE, description="Operation to perform on the parameter"
    )
    parameters: NumericalUpdaterParameters = Field(
        NumericalUpdaterParameters(), description="Parameters of the updater"
    )


class AindForceForagingTaskLogic(AindBehaviorTaskLogicModel):
    describedBy: str = Field("")
    schema_version: Literal[__version__] = __version__
    environment: Environment = Field(..., description="Environment settings")
    updaters: Dict[str, NumericalUpdater] = Field(default_factory=dict, description="List of numerical updaters")


def schema() -> BaseModel:
    return AindForceForagingTaskLogic
