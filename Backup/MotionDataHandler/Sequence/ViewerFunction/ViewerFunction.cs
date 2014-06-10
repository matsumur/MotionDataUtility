using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence.ViewerFunction {
    using Script;
    public interface IViewerFunction {
        ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller);
        string GetCommandName();
        string Usage { get; }
    }

    public class ViewerFunctionScriptFunction : IScriptFunction {
        public readonly IViewerFunction Operation;

        public ViewerFunctionScriptFunction(IViewerFunction operation) {
            if(operation == null)
                throw new ArgumentNullException("'operation' cannot be null", "operation");
            this.Operation = operation;
        }

        #region IScriptFunction メンバ

        public string Name {
            get { return string.Format("Sequence_{0}", this.Operation.GetCommandName()); }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            if(args == null)
                throw new ArgumentNullException("args", "args cannot be null");

            return this.Operation.Call(args, console.SequenceController);
        }
        public string Usage { get { return this.Operation.Usage; } }
        #endregion

        public static void Invoke(Control parentControl, SequenceViewerController controller, IViewerFunction operation, IList<ScriptVariable> args) {
            ScriptConsole.Singleton.Invoke(new ViewerFunctionScriptFunction(operation), args);
        }
        public static void Invoke(Control parentControl, SequenceViewerController controller, IViewerFunction operation, params ScriptVariable[] args) {
            Invoke(parentControl, controller, operation, (IList<ScriptVariable>)args);
        }
    }

}
