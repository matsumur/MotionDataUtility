using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace MotionDataHandler.Misc {
    using Script;

    /// <summary>
    /// スクリプトの仮引数
    /// </summary>
    /// <typeparam name="TEnvironment">環境変数格納用クラス</typeparam>
    public abstract class ProcParam<TEnvironment> {
        /// <summary>
        /// スクリプト表示用の型名
        /// </summary>
        public abstract string TypeName { get; }
        /// <summary>
        /// 表示に用いられる変数名
        /// </summary>
        public readonly string ParamName;
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param title="paramName">変数名</param>
        public ProcParam(string paramName) {
            this.ParamName = paramName;
        }
        /// <summary>
        /// ダイアログ用のパネルを作成して返します。
        /// 変数の初期値が設定してある場合はそれを反映すべきです。
        /// </summary>
        /// <param title="environment">実行環境</param>
        /// <returns>パネル</returns>
        public abstract Panel CreatePanel(TEnvironment environment);
        /// <summary>
        /// 変数の内容が変更された時に呼び出されるイベント
        /// </summary>
        public event EventHandler ValueChanged;
        /// <summary>
        /// ValueChangedイベントを呼び出します。
        /// </summary>
        protected void DoValueChanged() {
            EventHandler tmp = this.ValueChanged;
            if(tmp != null) {
                tmp.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Script.ScriptVariableから値を読み込みます
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="MotionDataHandler.Script.FunctionException"></exception>
        public abstract bool FromScriptVariable(TEnvironment environment, ScriptVariable variable, ref string errorMessage);
        /// <summary>
        /// パラメータをScript.ScriptVariableに変換します
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public abstract ScriptVariable ToScriptVariable(TEnvironment environment);
    }


    public class BooleanProcParam<TEnvironment> : ProcParam<TEnvironment> {
        public override string TypeName { get { return "bool"; } }
        public bool Value;
        public BooleanProcParam(string paramName)
            : base(paramName) {
            Value = false;
        }

        public override Panel CreatePanel(TEnvironment environment) {
            Panel ret = new Panel();
            CheckBox check = new CheckBox();
            check.Text = ParamName;
            check.Location = new System.Drawing.Point(4, 4);
            check.Dock = DockStyle.Fill;
            check.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            check.Checked = Value;
            check.TextAlign = ContentAlignment.MiddleLeft;
            check.CheckedChanged += new EventHandler((s, e) => {
                Value = check.Checked;
                DoValueChanged();
            });
            ret.Controls.Add(check);
            ret.Height = 32;
            return ret;
        }
        public override bool FromScriptVariable(TEnvironment environment, ScriptVariable variable, ref string errorMessage) {
            switch(variable.Type) {
            case ScriptVariableType.Boolean:
                this.Value = variable.ToBoolean();
                return true;
            default:
                errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SpecifyBoolean;
                return false;
            }
        }
        public override ScriptVariable ToScriptVariable(TEnvironment environment) {
            return new BooleanVariable(this.Value);
        }
    }

    public class NumberProcParam<TEnvironment> : ProcParam<TEnvironment> {
        public override string TypeName { get { return "number"; } }
        public readonly decimal Minimum, Maximum;
        public readonly int DecimalPlaces;
        public decimal Value;
        public decimal Increment = 1;
        public NumberProcParam(string paramName, decimal minimum, decimal maximum, int decimalPlaces)
            : this(paramName, minimum, maximum, decimalPlaces, 1) {
        }
        public NumberProcParam(string paramName, decimal minimum, decimal maximum, int decimalPlaces, decimal increment)
            : base(paramName) {
            Minimum = minimum;
            if(maximum < minimum)
                maximum = minimum;
            Maximum = maximum;
            using(NumericUpDown numTest = new NumericUpDown()) {
                numTest.DecimalPlaces = 0;
                try {
                    numTest.DecimalPlaces = decimalPlaces;
                    DecimalPlaces = decimalPlaces;
                } catch(OverflowException) {
                    DecimalPlaces = 0;
                }
            }
            Value = 0;
            decimal minIncrement = 1M;
            for(int i = 0; i < this.DecimalPlaces; i++) {
                minIncrement /= 10M;
            }
            decimal maxIncrement = (maximum - minimum) / 100;
            this.Increment = Math.Min(Math.Max(increment, minIncrement), maxIncrement);
        }

        public override Panel CreatePanel(TEnvironment environment) {
            Panel ret = new Panel();
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            NumericUpDown num = new NumericUpDown();
            num.DecimalPlaces = DecimalPlaces;
            num.Minimum = Minimum;
            num.Maximum = Maximum;
            if(Value < Minimum)
                Value = Minimum;
            if(Value > Maximum)
                Value = Maximum;
            num.Value = Value;
            num.TextAlign = HorizontalAlignment.Right;
            num.ValueChanged += new EventHandler((s, e) => {
                this.Value = num.Value;
                DoValueChanged();
            });
            num.Validated += new EventHandler((s, e) => {
                this.Value = num.Value;
                DoValueChanged();
            });

            num.Increment = this.Increment;
            num.Dock = DockStyle.Fill;
            group.Controls.Add(num);
            ret.Controls.Add(group);
            ret.Height = 48;
            return ret;
        }

        public override bool FromScriptVariable(TEnvironment environment, ScriptVariable variable, ref string errorMessage) {
            switch(variable.Type) {
            case ScriptVariableType.Number:
                decimal value = variable.ToNumber();
                if(value < this.Minimum || value > this.Maximum) {
                    errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_ValueOutOfRange + ": " + value.ToString();
                    return false;
                }
                this.Value = value;
                return true;
            default:
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_SpecifyNumber;
                return false;
            }
        }
        public override ScriptVariable ToScriptVariable(TEnvironment environment) {
            return new NumberVariable(this.Value);
        }
    }

    public class StringProcParam<TEnvironment> : ProcParam<TEnvironment> {
        public override string TypeName { get { return "string"; } }
        public string Value;
        public StringProcParam(string paramName)
            : base(paramName) {
            Value = "";
        }

        public override Panel CreatePanel(TEnvironment environment) {
            Panel ret = new Panel();
            ret.Height = 40;
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            TextBox text = new TextBox();
            text.Text = Value;
            text.TextChanged += new EventHandler((s, e) => {
                Value = text.Text;
                DoValueChanged();
            });
            text.Validated += new EventHandler((s, e) => {
                Value = text.Text;
                DoValueChanged();
            });
            text.Dock = DockStyle.Fill;
            group.Controls.Add(text);
            ret.Controls.Add(group);
            return ret;
        }
        public override bool FromScriptVariable(TEnvironment environment, ScriptVariable variable, ref string errorMessage) {
            this.Value = variable.ToString();
            return true;
        }
        public override ScriptVariable ToScriptVariable(TEnvironment environment) {
            return new StringVariable(this.Value);
        }
    }


    public class SingleSelectProcParam<TEnvironment> : ProcParam<TEnvironment> {
        public override string TypeName {
            get {
                StringBuilder candidates = new StringBuilder();
                int index = 0;
                foreach(string text in _RadioTexts ?? new string[0]) {
                    if(candidates.Length != 0)
                        candidates.Append(", ");
                    candidates.AppendFormat("{0} => {1}", index, text);
                    index++;
                }
                return "enum { " + candidates.ToString() + " }";
            }
        }
        public int Value;
        protected IList<string> _RadioTexts;
        public SingleSelectProcParam(string paramName, IList<string> radioTexts)
            : base(paramName) {
            _RadioTexts = radioTexts.ToList();
        }

        public override Panel CreatePanel(TEnvironment environment) {
            Panel ret = new Panel();
            RadioButton r = new RadioButton();
            ret.Height = _RadioTexts.Count * r.Height + 24;
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            if(Value < 0 || Value >= _RadioTexts.Count)
                Value = 0;
            int count = 0;
            foreach(var text in _RadioTexts) {
                int value = count;
                RadioButton radio = new RadioButton();
                radio.Text = text;
                radio.CheckedChanged += new EventHandler((s, e) => {
                    Value = value;
                    DoValueChanged();
                });
                radio.Checked = count == Value;
                radio.Dock = DockStyle.Top;
                group.Controls.Add(radio);
                radio.BringToFront();
                count++;
            }
            ret.Controls.Add(group);
            return ret;
        }

        public override bool FromScriptVariable(TEnvironment environment, ScriptVariable variable, ref string errorMessage) {
            if(variable.Type != ScriptVariableType.Number) {
                errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SpecifyInteger;
                return false;
            }
            int index = variable.ToInteger();
            if(index < 0 || index >= _RadioTexts.Count) {
                errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_IndexOutOfRange + ": " + index.ToString();
                return false;
            }
            this.Value = index;
            return true;
        }
        public override ScriptVariable ToScriptVariable(TEnvironment environment) {
            return new NumberVariable(this.Value);
        }
    }
}
