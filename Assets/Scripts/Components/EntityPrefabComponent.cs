using Unity.Entities;

[GenerateAuthoringComponent]
public struct EntityPrefabComponent : IComponentData
{
    public Entity entityPrefab;
}
