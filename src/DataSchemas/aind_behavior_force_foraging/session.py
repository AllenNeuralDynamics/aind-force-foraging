from typing import Literal

from aind_behavior_services import __version__
from aind_behavior_services.session import AindBehaviorSessionModel
from pydantic import BaseModel, Field


class AindForceForagingSession(AindBehaviorSessionModel):
    describedBy: str = Field("https://raw.githubusercontent.com/AllenNeuralDynamics/Aind.Behavior.ForceForaging/main/src/DataSchemas/aind_force_foraging_session.json")
    schema_version: Literal[__version__] = __version__


def schema() -> BaseModel:
    return AindForceForagingSession
