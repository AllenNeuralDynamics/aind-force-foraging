& python "src\DataSchemas\pyd_export_models.py" "src\DataSchemas\"
& bonsai.sgen --schema "src\DataSchemas\rig.json" --namespace ForceForagingDataSchema.Rig --root ForceForagingRig --output "src\Extensions\ForceForagingRig.cs" --serializer NewtonsoftJson YamlDotNet
& bonsai.sgen --schema "src\DataSchemas\session.json" --namespace ForceForagingDataSchema.Session --root ForceForagingSession --output "src\Extensions\ForceForagingSession.cs" --serializer NewtonsoftJson YamlDotNet
& bonsai.sgen --schema "src\DataSchemas\tasklogic.json" --namespace ForceForagingDataSchema.TaskLogic --root ForceForagingTaskLogic --output "src\Extensions\ForceForagingTaskLogic.cs" --serializer NewtonsoftJson YamlDotNet
