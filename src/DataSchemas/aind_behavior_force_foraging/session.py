from typing import Literal

from aind_behavior_services import __version__
from aind_behavior_services.session import AindBehaviorSessionModel
from pydantic import BaseModel, Field


def schema() -> BaseModel:
    return AindBehaviorSessionModel
