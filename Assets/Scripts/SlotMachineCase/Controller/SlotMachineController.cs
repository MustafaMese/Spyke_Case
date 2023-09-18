using SlotMachineCase.Command;
using SlotMachineCase.Initializer;
using SlotMachineCase.Manager;
using SlotMachineCase.Model;
using SlotMachineCase.View;

namespace SlotMachineCase.Controller
{
    public class SlotMachineController : IController
    {
        private SlotMachineModel _model;
        private SlotMachineView _view;
        private SlotMachineRoomController _roomController;
        
        public SlotMachineController(IModel model, IView view, SlotMachineRoomInitializer slotMachineRoomInitializer)
        {
            _model = (SlotMachineModel) model;
            _view = (SlotMachineView) view;
            
            _model.Initialize();
            _view.Initialize();

            Initialize();
            
            slotMachineRoomInitializer.Initialize();
            _roomController = slotMachineRoomInitializer.GetController();
        }
        
        public void Initialize()
        {
            GameManager.Instance.CommandManager.AddCommandListener<StartInputCommand>(Controller_SlotMachineStart);
            GameManager.Instance.CommandManager.AddCommandListener<SlotMachineCanStartAgainCommand>(Controller_SlotMachineRestart);
            
            _view.OnSpinningStart.AddListener(View_StartSpinning);
        }

        private void Controller_SlotMachineStart(StartInputCommand e)
        {
            if (!_model.IsSpinning.Value)
            {
                _model.IsSpinning.Value = true;
                
                GameManager.Instance.CommandManager.InvokeCommand(new StartSpinningCommand());
            }
        }

        private void Controller_SlotMachineRestart(SlotMachineCanStartAgainCommand e)
        {
            _model.IsSpinning.Value = false;
        }

        private void View_StartSpinning()
        {
            _roomController.OnStartMoving.Invoke();
        }
    }
}
