using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Script.DefaultFunctions {
    public class PrintFunction : IScriptFunction {
        #region IScriptSubRoutine メンバ

        public string Name {
            get { return "Print"; }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            StringWriter writer = new StringWriter();
            foreach(var arg in args) {
                if(arg.IsNull()) {
                    writer.Write("null");
                } else {
                    writer.Write(arg.ToString());
                }
            }
            string str = writer.ToString();
            console.Print(str);
            return new NumberVariable(str.Length);
        }

        public string Usage { get { return "(any, ...)"; } }

        #endregion
    }

    public class PrintLnFunction : IScriptFunction {
        #region IScriptSubRoutine メンバ

        public string Name {
            get { return "PrintLn"; }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            StringWriter writer = new StringWriter();
            foreach(var arg in args) {
                if(arg.IsNull()) {
                    writer.Write("null");
                } else {
                    writer.Write(arg.ToString());
                }
            }
            writer.WriteLine();
            string str = writer.ToString();
            console.Print(str);
            return new NumberVariable(str.Length);
        }

        public string Usage { get { return "(any, ...)"; } }

        #endregion
    }

    public class CountLengthFunction : IScriptFunction {
        #region IScriptSubRoutine メンバ

        public string Name { get { return "Count"; } }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            if(args.Count == 0)
                return null;
            if(args.Count == 1) {
                return lengthOf(args[0]);
            }
            return new ListVariable(args.Select(arg => lengthOf(arg)));
        }

        private ScriptVariable lengthOf(ScriptVariable arg) {
            if(arg.IsNull())
                return null;
            switch(arg.Type) {
            case ScriptVariableType.String:
                return new NumberVariable(arg.ToString().Length);
            case ScriptVariableType.List:
                return new NumberVariable(arg.ToList().Count);
            }
            return null;
        }

        public string Usage {
            get { return "(list or string, ...)"; }
        }

        #endregion
    }

    public class UsageFunction : IScriptFunction {

        #region IScriptFunction メンバ

        public string Name { get { return "Usage"; } }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            if(args.Count < 1) {
                args = new ScriptVariable[] { new StringVariable("") }.ToList();
            }
            List<ScriptVariable> msgs = new List<ScriptVariable>();
            bool isList = args[0] is ListVariable;
            IList<ScriptVariable> firstArg = args[0].IsNull() ? new[] { new StringVariable("") } : args[0].ToList();
            foreach(var arg in firstArg) {
                string name = arg.IsNull() ? "null" : arg.ToString();
                try {
                    msgs.Add(new StringVariable(name + " " + ScriptConsole.Singleton.GetUsage(name)));
                } catch(ArgumentException) {
                    List<string> names = ScriptConsole.Singleton.GetFunctionNames(name);
                    StringBuilder ret = new StringBuilder();

                    ret.AppendLine("subroutines starting with \"" + name + "\"");
                    foreach(var n in names.Take(512)) {
                        ret.AppendLine("  " + n);
                    }
                    if(names.Count > 512) {
                        ret.Append("... ");
                    }
                    ret.AppendLine("(" + names.Count + " subroutines)");

                    msgs.Add(new StringVariable(ret.ToString()));
                }
            }
            if(args.Count >= 2 && !args[1].IsNull() && args[1].ToBoolean()) {
                if(isList) {
                    return new ListVariable(msgs);
                } else {
                    return msgs[0];
                }
            }
            return ScriptConsole.Singleton.Invoke(new PrintFunction(), msgs);
        }

        public string Usage { get { return "(function name)"; } }

        #endregion
    }

    public class SleepFunction : IScriptFunction {
        #region IScriptFunction メンバ

        public string Name {
            get { return "Sleep"; }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            if(args.Count == 0 || args[0] == null || args[0].ToNumber() < 0) {
                System.Windows.Forms.MessageBox.Show("Sleeing...", typeof(SleepFunction).Name, System.Windows.Forms.MessageBoxButtons.OK);
            } else {
                System.Threading.Thread.Sleep((int)(args[0].ToNumber() * 1000M));
            }
            return null;
        }

        public string Usage {
            get { return "(wait_second)"; }
        }

        #endregion
    }

    public class ToNumberFunction : IScriptFunction {
        #region IScriptFunction メンバ

        public string Name {
            get { return "ToNumber"; }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            ScriptVariable arg = null;
            if(args.Count >= 1)
                arg = args[0];
            if(arg == null)
                return null;
            return new NumberVariable(arg.ToNumber());
        }

        public string Usage {
            get { return "(arg)"; }
        }

        #endregion
    }

    public class ToStringFunction : IScriptFunction {
        #region IScriptFunction メンバ

        public string Name {
            get { return "ToString"; }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            ScriptVariable arg = null;
            if(args.Count >= 1)
                arg = args[0];
            if(arg == null)
                return null;
            return new StringVariable(arg.ToString());
        }

        public string Usage {
            get { return "(arg)"; }
        }

        #endregion
    }

    public class ToBooleanFunction : IScriptFunction {
        #region IScriptFunction メンバ

        public string Name {
            get { return "ToBoolean"; }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            ScriptVariable arg = null;
            if(args.Count >= 1)
                arg = args[0];
            if(arg == null)
                return null;
            return new BooleanVariable(arg.ToBoolean());
        }

        public string Usage {
            get { return "(arg)"; }
        }

        #endregion
    }

    public class ToListFunction : IScriptFunction {
        #region IScriptFunction メンバ

        public string Name {
            get { return "ToList"; }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            ScriptVariable arg = null;
            if(args.Count >= 1)
                arg = args[0];
            if(arg == null)
                return null;
            return new ListVariable(arg.ToList());
        }

        public string Usage {
            get { return "(arg)"; }
        }

        #endregion
    }

    public class ReferenceEqualsFunction : IScriptFunction {
        #region IScriptFunction メンバ

        public string Name {
            get { return "ReferenceEquals"; }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            if(args.Count != 2) {
                console.Warn("2 arguments required");
                return null;
            }
            return new BooleanVariable(Object.ReferenceEquals(args[0], args[1]));
        }

        public string Usage {
            get { return "(argument1, argument2)"; }
        }

        #endregion
    }
}
