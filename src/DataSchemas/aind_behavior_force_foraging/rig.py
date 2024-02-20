# Import core types
from __future__ import annotations

# Import core types
from typing import Literal, Optional

import aind_behavior_services.rig as rig
import aind_behavior_services.calibration.load_cells as lcc
import aind_behavior_services.calibration.water_valve as wvc
import aind_behavior_services.calibration.olfactometer as oc

from aind_behavior_services.rig import AindBehaviorRigModel
from pydantic import BaseModel, Field, RootModel

__version__ = "0.1.0"


class RigCalibration(BaseModel):
    load_cells: lcc.LoadCellsCalibration = Field(..., description="Load cells calibration")
    water_valve: wvc.WaterValveCalibration = Field(default=..., description="Water valve calibration")
    olfactometer: Optional[oc.OlfactometerCalibration] = Field(default=None, description="Olfactometer calibration")


class AindForceForagingRig(AindBehaviorRigModel):
    describedBy: str = Field("")
    schema_version: Literal[__version__] = __version__
    auxiliary_camera0: Optional[rig.WebCamera] = Field(default=rig.WebCamera(), description="Auxiliary camera 0")
    auxiliary_camera1: Optional[rig.WebCamera] = Field(default=rig.WebCamera(), description="Auxiliary camera 1")
    harp_behavior: rig.HarpBehavior = Field(..., description="Harp behavior")
    harp_olfactometer: Optional[rig.HarpOlfactometer] = Field(..., description="Harp olfactometer")
    harp_lickometer: rig.HarpLickometer = Field(..., description="Harp lickometer")
    harp_load_cells: rig.HarpLoadCells = Field(..., description="Harp load cells")
    harp_clock_generator: rig.HarpClockGenerator = Field(..., description="Harp clock generator")
    harp_analog_input: Optional[rig.HarpAnalogInput] = Field(default=None, description="Harp analog input")
    face_camera: rig.SpinnakerCamera = Field(..., description="Face camera")
    top_body_camera: Optional[rig.SpinnakerCamera] = Field(default=None, description="Top body camera")
    side_body_camera: Optional[rig.SpinnakerCamera] = Field(default=None, description="Side body camera")
    screen: rig.Screen = Field(default=rig.Screen(), description="Screen settings")
    treadmill: rig.Treadmill = Field(default=rig.Treadmill(), description="Treadmill settings")
    calibration: Optional[RigCalibration] = Field(default=None, description="Load cells calibration")


def schema() -> BaseModel:
    return AindForceForagingRig
