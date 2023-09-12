using System.Collections.Generic;
using SlotMachineCase.Command;
using UnityEngine;

namespace SlotMachineCase.Manager
{
    public class CommandManager
    {
        public delegate void InvokeCommandDelegate<TCommand>(TCommand e) where TCommand : ICommand;
        private delegate void InvokeCommandDelegate(ICommand e);
        private Dictionary<System.Type, InvokeCommandDelegate> _invokeCommandDelegates = new Dictionary<System.Type, InvokeCommandDelegate>();
        
        public void InvokeCommand(ICommand command)
        {
            InvokeCommandDelegate del;
            if (_invokeCommandDelegates.TryGetValue(command.GetType(), out del))
            {
                del.Invoke(command);
            }
        }
        
        public void AddCommandListener<TCommand>(InvokeCommandDelegate<TCommand> invokeDelegate) where TCommand : ICommand
        {
            AddCommandListenerImpl(invokeDelegate);
        }
        
        private void AddCommandListenerImpl<TCommand>(InvokeCommandDelegate<TCommand> del) where TCommand : ICommand
        {
            InvokeCommandDelegate internalDelegate = (e) => del((TCommand)e);

            InvokeCommandDelegate tempDel;
            
            if (_invokeCommandDelegates.TryGetValue(typeof(TCommand), out tempDel))
            {
                _invokeCommandDelegates[typeof(TCommand)] = tempDel += internalDelegate;
            }
            else
            {
                _invokeCommandDelegates[typeof(TCommand)] = internalDelegate;
            }
        }
    }
}
