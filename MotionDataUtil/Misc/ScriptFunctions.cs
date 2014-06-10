using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MotionDataHandler;
using MotionDataHandler.Script;

namespace MotionDataUtil.Misc {
    public class FunctionOpenSequenceViewer : IScriptFunction {

        #region IScriptFunction メンバ

        public string Name { get { return "OpenSequenceViewer"; } }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            if(console.ParentControl.InvokeRequired) {
                return (ScriptVariable)console.ParentControl.Invoke(new Func<IList<ScriptVariable>, ScriptConsole, ScriptVariable>(Call), args, console);
            }
            SequenceViewerForm form = SequenceViewerForm.Singleton;
            form.Show();
            return null;
        }

        public string Usage { get { return "()"; } }

        #endregion
    }
}
