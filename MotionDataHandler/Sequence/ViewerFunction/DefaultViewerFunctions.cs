using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Sequence.ViewerFunction {
    using Script;
    public class FunctionSetLabel : IViewerFunction {
        #region IViewerFunction メンバ

        public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
            string title = args[0].ToString();
            decimal beginTime = args[1].ToNumber();
            decimal endTime = args[2].ToNumber();
            string label = args[3].ToString();
            SequenceView viewer = controller.GetViewByTitle(title);
            if(viewer == null)
                return new BooleanVariable(false);
            viewer.Sequence.SetLabelAt(beginTime, endTime, label);
            return new BooleanVariable(true);
        }

        public string GetCommandName() { return "SetLabel"; }

        public string Usage { get { return "(sequence name, begin time, end time, label name)"; } }

        #endregion
    }


    public class FunctionSetLabelStart : IViewerFunction {
        #region IViewerFunction メンバ

        public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
            string title = args[0].ToString();
            decimal beginTime = args[1].ToNumber();
            string label = args[2].ToString();
            SequenceView viewer = controller.GetViewByTitle(title);
            if(viewer == null)
                return new BooleanVariable(false);
            viewer.Sequence.SetLabelAt(beginTime, label);
            return new BooleanVariable(true);
        }

        public string GetCommandName() { return "SetLabelStart"; }

        public string Usage { get { return "(sequence name, time, label name)"; } }

        #endregion
    }

    public class FunctionSetLabelingBorder : IViewerFunction {
        #region IViewerFunction メンバ

        public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
            string title = args[0].ToString();
            decimal lower = args[1].ToNumber();
            decimal upper = args[2].ToNumber();
            string label = args[3].ToString();
            SequenceView viewer = controller.GetViewByTitle(title);
            if(viewer == null)
                return new BooleanVariable(false);
            viewer.Sequence.Borders.SetBorderRange(lower, upper, label);
            return new BooleanVariable(true);
        }

        public string GetCommandName() { return "SetLabelingBorder"; }

        public string Usage { get { return "(sequence name, value lower bound, value upper bound, label name)"; } }

        #endregion
    }

    public class FunctionSetLabelingBorderStart : IViewerFunction {
        #region IViewerFunction メンバ

        public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
            string title = args[0].ToString();
            decimal lower = args[1].ToNumber();
            string label = args[2].ToString();
            SequenceView viewer = controller.GetViewByTitle(title);
            if(viewer == null)
                return new BooleanVariable(false);
            viewer.Sequence.Borders.SetBorder(lower, label);
            return new BooleanVariable(true);
        }

        public string GetCommandName() { return "SetLabelingBorderStart"; }

        public string Usage { get { return "(sequence name, value lower bound, label name)"; } }

        #endregion
    }

    public class FunctionGetTimeline : IViewerFunction {
        #region IViewerFunction メンバ

        public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
            string title = args[0].ToString();
            SequenceView view = controller.GetViewByTitle(title);
            if(view == null)
                return null;
            return new ListVariable(view.Sequence.Values.EnumerateTime().Select(v => new NumberVariable(v)));
        }

        public string GetCommandName() {
            return "GetTimeline";
        }

        public string Usage {
            get { return "(sequence name)"; }
        }

        #endregion
    }
    public class FunctionGetValues : IViewerFunction {

        #region IViewerFunction メンバ

        public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
            string title = args[0].ToString();
            SequenceView view = controller.GetViewByTitle(title);
            if(view == null)
                return null;
            if(args.Count < 2 || args[1].IsNull()) {
                List<ListVariable> ret = new List<ListVariable>();
                foreach(var pair in view.Sequence.Values.Enumerate()) {
                    ret.Add(new ListVariable(pair.Value.Select(v => v.HasValue ? new NumberVariable(v.Value) : null)));
                }
                return new ListVariable(ret);
            } else {
                IList<ScriptVariable> list = args[1].ToList();
                if(list != null) {
                    int[] indices = list.Select(p => Convert.ToInt32(p.ToNumber())).ToArray();
                    List<ListVariable> ret = new List<ListVariable>();
                    foreach(var pair in view.Sequence.Values.Enumerate()) {
                        List<NumberVariable> numbers = new List<NumberVariable>();
                        foreach(int index in indices) {
                            decimal? value = pair.Value[index];
                            numbers.Add(value.HasValue ? new NumberVariable(value.Value) : null);
                        }
                        ret.Add(new ListVariable(numbers));
                    }
                    return new ListVariable(ret);
                } else {
                    int index = Convert.ToInt32(args[1].ToNumber());
                    List<NumberVariable> ret = new List<NumberVariable>();
                    foreach(var pair in view.Sequence.Values.Enumerate()) {
                        decimal? value = pair.Value[index];
                        ret.Add(value.HasValue ? new NumberVariable(value.Value) : null);
                    }
                    return new ListVariable(ret);
                }
            }
        }

        public string GetCommandName() {
            return "GetValues";
        }

        public string Usage {
            get { return "(sequnece name, [column index])"; }
        }

        #endregion
    }
}
