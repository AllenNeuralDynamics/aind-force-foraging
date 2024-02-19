from __future__ import annotations

from enum import Enum
from typing import Annotated, Dict, List, Literal, Optional, Union

import aind_behavior_services.task_logic.distributions as distributions
from aind_behavior_services.task_logic import AindBehaviorTaskLogicModel
from aind_behavior_force_foraging import __version__
from aind_data_schema.base import AindModel
from pydantic import BaseModel, Field, RootModel


def scalar_value(value: float) -> distributions.Scalar:
    """
    Helper function to create a scalar value distribution for a given value.

    Args:
        value (float): The value of the scalar distribution.

    Returns:
        distributions.Scalar: The scalar distribution type.
    """
    return distributions.Scalar(distribution_parameters=distributions.ScalarDistributionParameter(value=value))


class UpdateTrigger(str, Enum):
    TIME = 'Time'
    TRIAL = 'Trial'
    REWARD = 'Reward'


class _UpdateFunction(AindModel):
    function_type: str
    update_on: UpdateTrigger = Field(default=UpdateTrigger.TRIAL)
class PowerFunction(_UpdateFunction):
    function_type: Literal["PowerFunction"] = "PowerFunction"
    mininum: float = Field(default=0, description="Minimum value of the function")
    maximum: float = Field(default=1, description="Maximum value of the function")
    a: float = Field(default=1, description="Coefficient a of the function: value = a * pow(b, c * x) + d")
    b: float = Field(
        default=2.718281828459045, description="Coefficient b of the function: value = a * pow(b, c * x) + d"
    )
    c: float = Field(default=-1, description="Coefficient c of the function: value = a * pow(b, c * x) + d")
    d: float = Field(default=0, description="Coefficient d of the function: value = a * pow(b, c * x) + d")


class LinearFunction(_UpdateFunction):
    function_type: Literal["LinearFunction"] = "LinearFunction"
    mininum: float = Field(default=0, description="Minimum value of the function")
    maximum: float = Field(default=9999, description="Maximum value of the function")
    a: float = Field(default=1, description="Coefficient a of the function: value = a * x + b")
    b: float = Field(default=0, description="Coefficient b of the function: value = a * x + b")


class ConstantFunction(_UpdateFunction):
    function_type: Literal["ConstantFunction"] = "ConstantFunction"
    value: float = Field(default=1, description="Value of the function")

class LookUpTable(_UpdateFunction):
    function_type: Literal["LookUpTable"] = "LookUpTable"
    values: List[float] = Field(..., min_length=1)
    is_repeat: bool = Field(default=False)
    is_shuffle: bool = Field(default=False)

class UpdateFunction(RootModel):
    root: Annotated[Union[ConstantFunction, LinearFunction, PowerFunction, LookUpTable], Field(discriminator="function_type")]



class HarvestAction(BaseModel):
    probability: UpdateFunction = Field(default=ConstantFunction(value=1), description="Probability of reward", validate_default=True)
    amount: UpdateFunction = Field(default=ConstantFunction(value=1), description="Amount of reward to be delivered", validate_default=True)
    delay: UpdateFunction = Field(default=ConstantFunction(value=0), description="Delay between sucessful harvest and reward delivery", validate_default=True)
    press_duration: UpdateFunction = Field(default=ConstantFunction(value=1), description="Duration that the force much stay above threshold", validate_default=True)
    press_force_threshold: UpdateFunction = Field(default=ConstantFunction(value=10_000), description="Force to be applied", validate_default=True)


class Block(BaseModel):
    left_action: HarvestAction = Field(..., validate_default=True, description="Specification of the left action")
    right_action: HarvestAction = Field(..., validate_default=True, description="Specification of the right action")
    block_size: distributions.Distribution = scalar_value(20)




class EnvironmentStatistics(BaseModel):
    pass


class AindForceForagingTaskLogic(AindBehaviorTaskLogicModel):
    describedBy: str = Field("")
    schema_version: Literal[__version__] = __version__
    block: List[Block] = Field(default=[], description="Block settings")

def schema() -> BaseModel:
    return AindForceForagingTaskLogic
