using Unity.Entities;

namespace Pathfinding
{

    public struct Node : IComponentData
    {
        public UnitType Type;
        public int WorldIndex; // { get; } debug purposees;
        public bool IsTaken; //{ get; private set; }
        public int FCost { get; private set; }
        public int GCost { get; private set; }
        public int HCost { get; private set; }
        public int X { get; }
        public int Y { get; }
        public bool IsWalkable { get; private set; }
        public int PreviousNodeIndex { get; private set; }

        public Node(int x, int y, int worldPositionIndex, bool isWalkable, int prevIndex, int gCost, int hCost, int fCost, UnitType type, bool isTaken = false)
        {
            X = x;
            Y = y;
            WorldIndex = worldPositionIndex;
            IsWalkable = isWalkable;
            PreviousNodeIndex = prevIndex;

            GCost = gCost;
            HCost = hCost;
            FCost = fCost;
            Type = type;
            IsTaken = isTaken;
        }

        public void SetCosts(int gCost, int hCost)
        {
            GCost = gCost;
            HCost = hCost;
            CalculateFCost();
        }

        public void SetGCost(int gCost) => GCost = gCost;

        public void CalculateFCost() => FCost = GCost + HCost;

        public void SetPreviousNodeIndex(int i) => PreviousNodeIndex = i;

        public void SetWalkable(bool isWalkable) => IsWalkable = isWalkable;

        public void TakeNode() { IsTaken = true; }
    }
}