
using UnityEngine;

namespace SlotMachineCase.Model
{
    public class SlotMachineModel : IModel
    {
        private Observable<bool> _isSpinning;

        public Observable<bool> IsSpinning => _isSpinning;

        public SlotMachineModel()
        {
            
        }

        public void Initialize()
        {
            _isSpinning = new Observable<bool>();
        }
    }
}
