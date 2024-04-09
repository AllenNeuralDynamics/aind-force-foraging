from aind_behavior_force_foraging.rig import AindForceForagingRig
from aind_behavior_force_foraging.session import AindBehaviorSessionModel
from aind_behavior_force_foraging.task_logic import AindForceForagingTaskLogic
from aind_behavior_services.launcher import LauncherCli

if __name__ == "__main__":
    launcher_cli = LauncherCli(
        rig_schema=AindForceForagingRig,
        session_schema=AindBehaviorSessionModel,
        task_logic_schema=AindForceForagingTaskLogic,
        data_dir=r"C:/Data",
        remote_data_dir=None,
        config_library_dir=r"\\allen\aind\scratch\AindBehavior.db\AindForceForaging",
        workflow=r"./src/main.bonsai",
    )
    launcher_cli.run()
