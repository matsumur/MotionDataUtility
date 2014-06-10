using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Script.Parse {
    /// <summary>
    /// 変数アクセス処理の汎用インターフェース
    /// </summary>
    public interface IVariableAccessor {
        ScriptVariable Get(ScriptExecutionEnvironment environment);
        void Set(ScriptExecutionEnvironment environment, ScriptVariable value);
    }
    /// <summary>
    /// 識別子によって変数にアクセスする場合の処理用クラス
    /// </summary>
    public class IdentifierAccessor : IVariableAccessor {
        private string _name;
        /// <summary>
        /// 識別子を取得します
        /// </summary>
        public string Name { get { return _name; } }
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="name">識別子</param>
        public IdentifierAccessor(string name) {
            _name = name;
        }

        #region IVariableAccessor メンバ

        public ScriptVariable Get(ScriptExecutionEnvironment environment) {
            return environment.Variables.LookUp(_name);
        }

        public void Set(ScriptExecutionEnvironment environment, ScriptVariable value) {
            environment.Variables.Set(_name, value);
        }

        #endregion
    }
    /// <summary>
    /// インデクサを経由して変数にアクセスする場合の処理用のクラス
    /// </summary>
    public class IndexedVariableAccessor : IVariableAccessor {
        private ScriptVariable _variable;
        private ScriptVariable _index;
        public ScriptVariable Variable { get { return _variable; } }
        public ScriptVariable Indexer { get { return _index; } }
        public IndexedVariableAccessor(ScriptVariable variable, ScriptVariable index) {
            _variable = variable;
            _index = index;
        }

        #region IVariableAccessor メンバ

        public ScriptVariable Get(ScriptExecutionEnvironment environment) {
            if(_variable == null) {
                environment.Console.Warn("Cannot use indexer for null variable");
                return null;
            }
            if(_index == null) {
                environment.Console.Warn("Cannot use null as indexer");
                return null;
            }
            ListVariable list = _variable as ListVariable;
            StringVariable key = _index as StringVariable;
            if(list != null && key != null) {
                return list[key.Value];
            }

            int indexInt = _index.ToInteger();
            ScriptVariable ret;
            if(!_variable.GetIndexedValue(indexInt, out ret)) {
                environment.Console.Warn(string.Format("Invalid index : {0}", _index.ToString()));
                ret = null;
            }
            return ret;
        }

        public void Set(ScriptExecutionEnvironment environment, ScriptVariable value) {
            if(_variable == null) {
                environment.Console.Warn("Cannot use index for null variable");
                return;
            }
            if(_index == null) {
                environment.Console.Warn("Cannot use null as index");
                return;
            }
            ListVariable list = _variable as ListVariable;
            StringVariable key = _index as StringVariable;
            if(list != null && key != null) {
                list[key.Value] = value;
                return;
            }

            int indexInt = _index.ToInteger();
            if(!_variable.SetIndexedValue(indexInt, value)) {
                environment.Console.Warn(string.Format("Invalid index : {0}", _index.ToString()));
            }
        }

        #endregion
    }

    /// <summary>
    /// 無効な変数アクセス時の処理用クラス
    /// </summary>
    public class NullAccessor : IVariableAccessor {
        public NullAccessor() { }

        #region IVariableAccessor メンバ

        public ScriptVariable Get(ScriptExecutionEnvironment environment) {
            return null;
        }

        public void Set(ScriptExecutionEnvironment environment, ScriptVariable value) {
        }

        #endregion
    }
}
