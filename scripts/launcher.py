from aind_behavior_services.launcher import LauncherCli
from aind_behavior_force_foraging.rig import AindForceForagingRig
from aind_behavior_force_foraging.session import AindForceForagingSession
from aind_behavior_force_foraging.task_logic import AindForceForagingTaskLogic

if __name__ == "__main__":
    launcher_cli = LauncherCli(
        rig_schema=AindForceForagingRig,
        session_schema=AindForceForagingSession,
        task_logic_schema=AindForceForagingTaskLogic,
        data_dir=r"C:/Data",
        remote_data_dir=None,
        config_library_dir=r"\\allen\aind\scratch\AindBehavior.db\AindForceForaging",
        workflow=r"./src/main.bonsai",
    )
    launcher_cli.run()
