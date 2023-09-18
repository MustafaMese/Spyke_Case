using System.Collections.Generic;

namespace SlotMachineCase.Model
{
    public class SlotMachineRoomModel : IModel
    {
        private Observable<bool> _isMoving;

        public Observable<bool> IsMoving => _isMoving;

        private readonly Dictionary<int, int> _outcomeIndexes = new();

        public SlotMachineRoomModel()
        {
            
        }
        
        public void Initialize()
        {
            _outcomeIndexes.Add(0, 0);
            _outcomeIndexes.Add(1, 1);
            _outcomeIndexes.Add(2, 2);
            _outcomeIndexes.Add(3, 3);
            _outcomeIndexes.Add(4, 4);

            _isMoving = new Observable<bool>();
        }

        public int GetIndex(int key) => _outcomeIndexes[key];
    }
}
