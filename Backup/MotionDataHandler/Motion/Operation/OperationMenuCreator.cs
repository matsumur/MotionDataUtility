using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MotionDataHandler.Motion.Operation {
    using Misc;
    using Script;

    /// <summary>
    /// メニュー
    /// </summary>
    struct OperationMenuItem {
        public IMotionOperationBase Operation;
        public ToolStripMenuItem MenuItem;
        public OperationMenuItem(IMotionOperationBase operation, ToolStripMenuItem menuItem) {
            this.Operation = operation;
            this.MenuItem = menuItem;
        }
    }


    /// <summary>
    /// IMotionOperationBaseを実装するクラスからメニュー項目を作成するためのクラス
    /// </summary>
    public class OperationMenuCreator : IDisposable {
        /// <summary>
        /// 
        /// </summary>
        struct MenuAutoGenerator {
            public ToolStripMenuItem OutputMenu;
            public Func<IList<IMotionOperationBase>> OperationGenerator;
            public string DefaultName;
            public MenuAutoGenerator(string name, ToolStripMenuItem outputMenu, Func<IList<IMotionOperationBase>> operationGen) {
                this.DefaultName = name;
                this.OutputMenu = outputMenu;
                this.OperationGenerator = operationGen;
            }
        }

        readonly MotionDataSet _dataSet;
        readonly Sequence.SequenceViewerController _sequenceController;
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="sequenceController"></param>
        public OperationMenuCreator(MotionDataSet dataSet, Sequence.SequenceViewerController sequenceController) {
            if(dataSet == null)
                throw new ArgumentNullException("dataSet", "'dataSet' cannot be null");
            if(sequenceController == null)
                throw new ArgumentNullException("sequenceController", "'sequenceController' cannot be nul");

            _dataSet = dataSet;
            _sequenceController = sequenceController;

            InitializeMenus();
        }

        List<OperationMenuItem> _menuAllItems;

        ToolStripMenuItem _createMenuItem;
        ToolStripMenuItem _editMenuItem;
        ToolStripMenuItem _outputMenuItem;
        ToolStripMenuItem _generalMenuItem;
        /// <summary>
        /// 作成メニューのリストを子要素として持つメニューを返します
        /// </summary>
        /// <returns></returns>
        public ToolStripMenuItem GetCreateMenuItem() {
            return _createMenuItem;
        }
        /// <summary>
        /// 編集メニューのリストを子要素として持つメニューを返します
        /// </summary>
        /// <returns></returns>
        public ToolStripMenuItem GetEditMenuItem() {
            return _editMenuItem;
        }
        /// <summary>
        /// 出力メニューのリストを子要素として持つメニューを返します
        /// </summary>
        /// <returns></returns>
        public ToolStripMenuItem GetOutputMenuItem() {
            return _outputMenuItem;
        }
        /// <summary>
        /// 一般メニューのリストを子要素として持つメニューを返します
        /// </summary>
        /// <returns></returns>
        public ToolStripMenuItem GetGeneralMenuItem() {
            return _generalMenuItem;
        }
        /// <summary>
        /// 各メニューを構築します
        /// </summary>
        public void InitializeMenus() {
            _menuAllItems = new List<OperationMenuItem>();

            // 作成メニュー
            _createMenuItem = new ToolStripMenuItem("Create");
            _createMenuItem.DropDownOpening += onMenuOpening;
            // 作成されるオブジェクトごとにメニューを分けるので，作成されるオブジェクトを列挙
            HashSet<Type> motionObjectTypes = new HashSet<Type>();
            bool otherTypeExists = false;
            foreach(IMotionOperationCreateObject ope in _dataSet.GetOperationCreateObject()) {
                Type t = ope.CreatedType;
                if(t != null && t.IsSubclassOf(typeof(MotionObject))) {
                    motionObjectTypes.Add(t);
                } else {
                    otherTypeExists = true;
                }
            }
            // 作成されるオブジェクトごとのメニューを作成
            Dictionary<Type, ToolStripMenuItem> typeMenus = new Dictionary<Type, ToolStripMenuItem>();
            foreach(Type type in motionObjectTypes) {
                MotionObjectInfo testInfo = new MotionObjectInfo(type);
                MotionObject testObj = testInfo.GetEmptyObject();
                ToolStripMenuItem item = new ToolStripMenuItem(type.Name, testObj.GetIcon() ?? global::MotionDataHandler.Properties.Resources.question);
                typeMenus[type] = item;
            }
            ToolStripMenuItem otherTypeMenu = new ToolStripMenuItem("General");
            // 処理ごとにメニューを作成
            foreach(bool isEditWrapper in new bool[] { false, true }) {
                if(isEditWrapper) {
                    foreach(Type t in motionObjectTypes) {
                        typeMenus[t].DropDownItems.Add(new ToolStripSeparator());
                    }
                    if(otherTypeExists) {
                        otherTypeMenu.DropDownItems.Add(new ToolStripSeparator());
                    }
                }
                foreach(IMotionOperationCreateObject ope in _dataSet.GetOperationCreateObject()) {
                    if((ope is MotionOperationEditToCreateWrapper) != isEditWrapper) {
                        continue;
                    }
                    Type t = ope.CreatedType;
                    ToolStripMenuItem item = new ToolStripMenuItem(ope.GetTitle());
                    try {
                        Bitmap icon = ope.IconBitmap;
                        if(icon != null) {
                            item.Image = icon;
                        }
                    } catch(NotImplementedException) { }
                    IMotionOperationCreateObject opeForLabmda = ope;
                    item.Click += new EventHandler((sender, e) => {
                        DialogMotionOperation dialog = new DialogMotionOperation(ScriptConsole.Singleton, opeForLabmda);
                        dialog.ShowDialog();
                    });
                    _menuAllItems.Add(new OperationMenuItem(ope, item));
                    if(motionObjectTypes.Contains(t)) {
                        typeMenus[t].DropDownItems.Add(item);
                    } else {
                        otherTypeMenu.DropDownItems.Add(item);
                    }
                }
            }
            foreach(Type t in motionObjectTypes.OrderBy(t => t.Name)) {
                _createMenuItem.DropDownItems.Add(typeMenus[t]);
            }
            if(otherTypeExists) {
                _createMenuItem.DropDownItems.Add(otherTypeMenu);
            }
            if(_createMenuItem.DropDownItems.Count == 0)
                _createMenuItem = null;

            // その他
            _editMenuItem = new ToolStripMenuItem();
            _outputMenuItem = new ToolStripMenuItem();
            _generalMenuItem = new ToolStripMenuItem();
            List<MenuAutoGenerator> genList = new List<MenuAutoGenerator>();
            genList.Add(new MenuAutoGenerator("Edit", _editMenuItem, () => _dataSet.GetOperationEditObject().Select(ope => (IMotionOperationBase)ope).ToList()));
            genList.Add(new MenuAutoGenerator("Output", _outputMenuItem, () => _dataSet.GetOperationOutputSequence().Select(ope => (IMotionOperationBase)ope).ToList()));
            genList.Add(new MenuAutoGenerator("General", _generalMenuItem, () => _dataSet.GetOperationGeneral().Select(ope => (IMotionOperationBase)ope).ToList()));
            foreach(MenuAutoGenerator gen in genList) {
                gen.OutputMenu.Text = gen.DefaultName;
                gen.OutputMenu.DropDownOpening += onMenuOpening;
                foreach(IMotionOperationBase ope in gen.OperationGenerator()) {
                    ToolStripMenuItem item = new ToolStripMenuItem(ope.GetTitle());
                    try {
                        Bitmap icon = ope.IconBitmap;
                        if(icon != null) {
                            item.Image = icon;
                        }
                    } catch(NotImplementedException) { }
                    IMotionOperationBase opeForLabmda = ope;
                    item.Click += new EventHandler((sender, e) => {
                        DialogMotionOperation dialog = new DialogMotionOperation(ScriptConsole.Singleton, opeForLabmda);
                        dialog.ShowDialog();
                    });
                    _menuAllItems.Add(new OperationMenuItem(ope, item));
                    gen.OutputMenu.DropDownItems.Add(item);
                }
            }
            if(_editMenuItem.DropDownItems.Count == 0)
                _editMenuItem = null;
            if(_outputMenuItem.DropDownItems.Count == 0)
                _outputMenuItem = null;
            if(_generalMenuItem.DropDownItems.Count == 0)
                _generalMenuItem = null;


        }

        void onMenuOpening(object sender, EventArgs e) {
            updateMenuEnabled();
        }

        /// <summary>
        /// 処理が実行可能かを判定して
        /// </summary>
        private void updateMenuEnabled() {
            IList<MotionObjectInfo> infoList = _dataSet.GetSelectedObjectInfoList();
            foreach(OperationMenuItem item in _menuAllItems) {
                string errorMessage = "";
                IList<MotionObjectInfo> subInfoList = infoList.Where(info => item.Operation.FilterSelection(info)).ToList();
                if(item.MenuItem.Enabled = item.Operation.ValidateSelection(subInfoList, ref errorMessage)) {
                    item.MenuItem.ToolTipText = null;
                } else {
                    item.MenuItem.ToolTipText = errorMessage;
                }
            }
        }

        #region IDisposable メンバ

        public void Dispose() {

        }

        #endregion
    }
}
