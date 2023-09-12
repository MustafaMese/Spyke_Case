using System.Collections.Generic;

namespace SlotMachineCase.Model
{
    public class SlotMachineRoomModel : IModel
    {
        private Observable<bool> _isMoving;

        public Observable<bool> IsMoving => _isMoving;

        private readonly Dictionary<string, int> _outcomeIndexes = new();

        public SlotMachineRoomModel()
        {
            
        }
        
        public void Initialize()
        {
            _outcomeIndexes.Add("Jackpot", 0);
            _outcomeIndexes.Add("Wild", 1);
            _outcomeIndexes.Add("Seven", 2);
            _outcomeIndexes.Add("Bonus", 3);
            _outcomeIndexes.Add("A", 4);

            _isMoving = new Observable<bool>();
        }

        public int GetIndex(string key) => _outcomeIndexes[key];
    }
}
