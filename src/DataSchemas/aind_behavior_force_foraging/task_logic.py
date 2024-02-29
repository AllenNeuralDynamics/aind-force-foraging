from __future__ import annotations

from enum import Enum
from typing import Annotated, Dict, List, Literal, Optional, Union
from functools import partial

import aind_behavior_services.task_logic.distributions as distributions
from aind_behavior_force_foraging import __version__
from aind_behavior_services.task_logic import AindBehaviorTaskLogicModel
from aind_data_schema.base import BaseModel
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


def normal_distribution_value(mean: float, std: float) -> distributions.Normal:
    """
    Helper function to create a normal distribution for a given range.

    Args:
        mean (float): The mean value of the normal distribution.
        std (float): The standard deviation of the normal distribution.

    Returns:
        distributions.Normal: The normal distribution type.
    """
    return distributions.NormalDistribution(
        distribution_parameters=distributions.NormalDistributionParameters(mean=mean, std=std)
    )


class HarvestActionLabel(str, Enum):
    """Defines the harvest actions"""

    LEFT = "Left"
    RIGHT = "Right"
    NONE = "None"


# Updaters
class NumericalUpdaterOperation(str, Enum):
    NONE = "None"
    ADD = "Offset"
    MULTIPLY = "Gain"
    SET = "Set"


class NumericalUpdaterParameters(BaseModel):
    value: distributions.Distribution = Field(
        default=scalar_value(0),
        validate_default=True,
        description="The value of the update. This value will be multiplied by the optional input event value.",
    )
    minimum: float = Field(default=0, description="Minimum value of the parameter")
    maximum: float = Field(default=0, description="Maximum value of the parameter")


class NumericalUpdater(BaseModel):
    operation: NumericalUpdaterOperation = Field(
        default=NumericalUpdaterOperation.NONE, description="Operation to perform on the parameter"
    )
    parameters: NumericalUpdaterParameters = Field(
        NumericalUpdaterParameters(), description="Parameters of the updater"
    )


class UpdateTargetParameter(str, Enum):
    """Defines the target parameters"""

    FORCETHRESHOLD = "ForceThreshold"
    PROBABILITY = "Probability"
    AMOUNT = "Amount"
    FORCEDURATION = "ForceDuration"
    DELAY = "Delay"


class UpdateTargetParameterBy(str, Enum):
    """Defines the independent variable used for the update"""

    TIME = "Time"
    REWARD = "Reward"
    TRIAL = "Trial"


class ActionUpdater(BaseModel):
    target_parameter: UpdateTargetParameter = Field(
        default=UpdateTargetParameter.PROBABILITY, description="Target parameter"
    )
    updated_by: UpdateTargetParameterBy = Field(
        default=UpdateTargetParameterBy.TIME, description="Independent variable"
    )
    updater: NumericalUpdater = Field(NumericalUpdater(), description="Updater")


class HarvestAction(BaseModel):
    """Defines an abstract class for an harvest action"""

    action: HarvestActionLabel = Field(default=HarvestActionLabel.NONE, description="Label of the action")
    probability: float = Field(default=1, description="Probability of reward")
    amount: float = Field(default=1, description="Amount of reward to be delivered")
    delay: float = Field(default=0, description="Delay between successful harvest and reward delivery")
    force_duration: float = Field(default=0.5, description="Duration that the force much stay above threshold")
    force_threshold: float = Field(
        default=5000, le=MAX_LOAD_CELL_FORCE, ge=-MAX_LOAD_CELL_FORCE, description="Force to be applied"
    )
    is_operant: bool = Field(default=True, description="Whether the reward delivery is contingent on licking.")
    time_to_collect: Optional[float] = Field(
        default=None,
        ge=0,
        description="Time to collect the reward after it is available. If null, the reward will be available indefinitely.",
    )
    action_updaters: List[ActionUpdater] = Field(
        [], description="List of action updaters. All updaters are called at the start of a new trial."
    )


class QuiescencePeriod(BaseModel):
    """Defines a quiescence settings"""

    duration: float = Field(default=0, ge=0, description="Duration of the quiescence period")
    force_threshold: float = Field(
        default=0, le=MAX_LOAD_CELL_FORCE, ge=-MAX_LOAD_CELL_FORCE, description="Time out for the quiescence period"
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
    has_feedback: bool = Field(
        default=False, description="Whether to provide feedback to the animal after the response period."
    )


RightHarvestAction = partial(HarvestAction, action=HarvestActionLabel.RIGHT)
LeftHarvestAction = partial(HarvestAction, action=HarvestActionLabel.LEFT)


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
    left_harvest: Optional[HarvestAction] = Field(
        default=LeftHarvestAction(), validate_default=True, description="Specification of the left action"
    )
    right_harvest: Optional[HarvestAction] = Field(
        default=RightHarvestAction(), validate_default=True, description="Specification of the right action"
    )


class BlockStatisticsMode(str, Enum):
    """Defines the mode of the environment"""

    BLOCK = "Block"
    BROWNIAN = "BownianRandomWalk"
    BLOCKGENERATOR = "BlockGenerator"


class Block(BaseModel):
    mode: Literal[BlockStatisticsMode.BLOCK] = BlockStatisticsMode.BLOCK
    is_baited: bool = Field(default=False, description="Whether the trials are baited")
    trials: List[Trial] = Field(default=[], description="List of trials in the block")
    shuffle: bool = Field(default=False, description="Whether to shuffle the trials in the block")
    repeat_count: Optional[int] = Field(
        default=0, description="Number of times to repeat the block. If null, the block will be repeated indefinitely"
    )


class BlockGenerator(BaseModel):
    mode: Literal[BlockStatisticsMode.BLOCKGENERATOR] = BlockStatisticsMode.BLOCKGENERATOR
    is_baited: bool = Field(default=False, description="Whether the trials are baited")
    block_size: distributions.Distribution = Field(
        default=uniform_distribution_value(min=50, max=60), validate_default=True, description="Size of the block"
    )
    trial_statistics: Trial = Field(..., description="Statistics of the trials in the block")


class BrownianRandomWalk(BaseModel):
    mode: Literal[BlockStatisticsMode.BROWNIAN] = BlockStatisticsMode.BROWNIAN
    is_baited: bool = Field(default=False, description="Whether the trials are baited")
    block_size: distributions.Distribution = Field(
        default=uniform_distribution_value(min=50, max=60), validate_default=True, description="Size of the block"
    )
    trial_statistics: Trial = Field(..., description="Statistics of the trials in the block")


class BlockStatistics(RootModel):
    root: Annotated[Union[Block, BlockGenerator, BrownianRandomWalk], Field(discriminator="mode")]


class Environment(BaseModel):
    block_statistics: List[BlockStatistics] = Field(..., description="Statistics of the environment")
    shuffle: bool = Field(default=False, description="Whether to shuffle the blocks")
    repeat_count: Optional[int] = Field(
        default=0,
        description="Number of times to repeat the environment. If null, the environment will be repeated indefinitely",
    )


class PressMode(str, Enum):
    """Defines the press mode"""

    DOUBLE = "Double"
    SINGLE_AVERAGE = "SingleAverage"
    SINGLE_MAX = "SingleMax"
    SINGLE_MIN = "SingleMin"
    SINGLE_LEFT = "SingleLeft"
    SINGLE_RIGHT = "SingleRight"


class ForceOperationControl(BaseModel):
    press_mode: PressMode = Field(
        default=PressMode.DOUBLE, description="Defines the press mode. Default is to use both sensors individually"
    )
    left_index: int = Field(default=0, description="Index of the left sensor")
    right_index: int = Field(default=1, description="Index of the right sensor")


class OperationControl(BaseModel):
    force: ForceOperationControl = Field(
        ForceOperationControl(), validate_default=True, description="Operation control for force sensor"
    )


class AindForceForagingTaskLogic(AindBehaviorTaskLogicModel):
    describedBy: str = Field("")
    schema_version: Literal[__version__] = __version__
    environment: Environment = Field(..., description="Environment settings")
    updaters: Dict[str, NumericalUpdater] = Field(default_factory=dict, description="List of numerical updaters")
    operation_control: OperationControl = Field(
        default=OperationControl(), validate_default=True, description="Operation control"
    )


def schema() -> BaseModel:
    return AindForceForagingTaskLogic
