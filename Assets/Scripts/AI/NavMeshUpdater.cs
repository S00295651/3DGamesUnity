using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshUpdater : MonoBehaviour
{
    public static NavMeshUpdater Instance { get; private set; }

    private NavMeshSurface surface;

    private void Awake()
    {
        Instance = this;
        surface = GetComponent<NavMeshSurface>();
    }

    public void Rebake()
    {
        surface.BuildNavMesh();
    }
}