from __future__ import annotations

from enum import Enum
from typing import Annotated, Dict, List, Literal, Optional, Union

import aind_behavior_services.task_logic.distributions as distributions
from aind_behavior_services.task_logic import AindBehaviorTaskLogicModel
from aind_behavior_force_foraging import __version__
from aind_data_schema.base import AindModel
from pydantic import BaseModel, Field, RootModel, ConfigDict


def scalar_value(value: float) -> distributions.Scalar:
    """
    Helper function to create a scalar value distribution for a given value.

    Args:
        value (float): The value of the scalar distribution.

    Returns:
        distributions.Scalar: The scalar distribution type.
    """
    return distributions.Scalar(distribution_parameters=distributions.ScalarDistributionParameter(value=value))


## Available actions
class HarvestActionBase(BaseModel):
    '''Defines an abstract class for an harvest action'''
    label: str = Field(default='HarvestAction', description="Label of the action")
    probability: float = Field(default=1, description="Probability of reward")
    amount: float = Field(default=1, description="Amount of reward to be delivered")
    delay: float = Field(default=0, description="Delay between sucessful harvest and reward delivery")
    press_duration: float = Field(default=5, description="Duration that the force much stay above threshold")
    press_force_threshold: float = Field(default=5000, description="Force to be applied")


class RightHarvestAction(HarvestActionBase):
    label: Literal['RightHarvestAction'] = 'RightHarvestAction'


class LeftHarvestAction(HarvestActionBase):
    label: Literal['LeftHarvestAction'] = 'LeftHarvestAction'


class HarvestAction(RootModel):
    root: Annotated[Union[LeftHarvestAction, RightHarvestAction], Field(discriminator='label')]


class Trial(BaseModel):
    '''Defines a trial'''
    left_action: HarvestAction = Field(default=LeftHarvestAction(), validate_default=True, description="Specification of the left action")
    right_action: HarvestAction = Field(default=RightHarvestAction(), validate_default=True, description="Specification of the right action")
    inter_trial_interval: float = Field(default=0, ge=0, description="Time between trials")
    time_out: float = Field(default=0, ge=0, description="Time out for the trial")


class Block(BaseModel):
    trials: List[Trial] = Field(default=[], description="List of trials in the block")
    shuffle: bool = Field(default=False, description="Whether to shuffle the trials in the block")
    repeat_count: Optional[int] = Field(default=0, description="Number of times to repeat the block. If null, the block will be repeated indefinitely")


class EnvironmentStatisticsMode(str, Enum):
    '''Defines the mode of the environment'''
    BLOCK = "Block"
    RANDOMWALK = "RandomWalk"


class BlockedStatistics(BaseModel):
    mode: Literal[EnvironmentStatisticsMode.BLOCK] = EnvironmentStatisticsMode.BLOCK
    blocks: List[Block] = Field(default=[], description="List of blocks in the environment")
    shuffle: bool = Field(default=False, description="Whether to shuffle the blocks in the environment")
    repeat_count: Optional[int] = Field(default=0, description="Number of times to repeat the environment. If null, the environment will be repeated indefinitely")


class RandomWalkStatistics(BaseModel):
    mode: Literal[EnvironmentStatisticsMode.RANDOMWALK] = EnvironmentStatisticsMode.RANDOMWALK


class EnvironmentStatistics(RootModel):
    root: Annotated[Union[BlockedStatistics, RandomWalkStatistics], Field(discriminator='mode')]


class Environment(BaseModel):
    statistics: EnvironmentStatistics = Field(..., description="Statistics of the environment")


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
