using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace MotionDataHandler.Motion {
    public partial class MotionDataObjectSelectList : UserControl {
        MotionDataSet _dataSet;
        Predicate<MotionObjectInfo> _targetCondition;
        Predicate<MotionObjectInfo> _defaultSelectedCondition;
        IList<MotionObjectInfo> _targetInfoList;

        public MotionDataObjectSelectList() {
            InitializeComponent();
            SelectionMode = this.listSelect.SelectionMode;

        }

        public void AttachDataSet(MotionDataSet dataSet, IEnumerable<Type> targetTypes, bool? selectedOrNotSelected) {
            this.AttachDataSet(dataSet, new Predicate<MotionObjectInfo>(info => targetTypes.Any(type => info.IsTypeOf(type))), new Predicate<MotionObjectInfo>(info => selectedOrNotSelected.HasValue ? dataSet.IsSelecting(info) == selectedOrNotSelected.Value : false));
        }

        public void AttachDataSet(MotionDataSet dataSet, Predicate<MotionObjectInfo> targetCondition, Predicate<MotionObjectInfo> defaultSelectedCondition) {
            DetachDataSet();
            _dataSet = dataSet;
            _targetCondition = targetCondition;
            _defaultSelectedCondition = defaultSelectedCondition;
            _dataSet.ObjectSelectionChanged += OnDataSetSelectedChanged;
            _dataSet.ObjectInfoSetChanged += OnDataSetSelectedChanged;
            OnDataSetSelectedChanged(this, new EventArgs());
            if(listSelect.Items.Count > 0) {
                listSelect.SelectedIndex = 0;
            }
        }

        public void DetachDataSet() {
            if(_dataSet != null) {
                _dataSet.ObjectSelectionChanged -= OnDataSetSelectedChanged;
                _dataSet.ObjectInfoSetChanged -= motionDataObjectSelectList_disposed;
                _dataSet = null;
                OnDataSetSelectedChanged(this, new EventArgs());
            }
        }

        private void motionDataObjectSelectList_disposed(object sender, EventArgs e) {
            this.DetachDataSet();
        }


        bool _isSelectionSetting = false;
        private void OnDataSetSelectedChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.Invoke(new EventHandler(OnDataSetSelectedChanged), sender, e);
                return;
            }
            if(_dataSet == null) {
                listSelect.Items.Clear();
                return;
            }
            lock(_dataSet) {
                var targetItems = _dataSet.GetObjectInfoList(info => _targetCondition(info));
                var selectedItems = _dataSet.GetObjectInfoList(info => _targetCondition(info) && _defaultSelectedCondition(info));

                _targetInfoList = targetItems;
                _isSelectionSetting = true;
                try {
                    listSelect.Items.Clear();
                    foreach(MotionObjectInfo info in _targetInfoList) {
                        listSelect.Items.Add(info.Name);
                    }
                    listSelect.SelectedIndices.Clear();
                    for(int i = 0; i < targetItems.Count; i++) {
                        MotionObjectInfo info = targetItems[i];
                        if(selectedItems.Contains(info))
                            listSelect.SelectedIndices.Add(i);
                    }
                } finally { _isSelectionSetting = false; }
            }
        }
        private SelectionMode _selectionMode;
        public SelectionMode SelectionMode {
            get { return _selectionMode; }
            set { setSelectionMode(value); }
        }

        private void setSelectionMode(SelectionMode selectionMode) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<SelectionMode>(setSelectionMode), selectionMode);
                return;
            }
            _selectionMode = selectionMode;
            listSelect.SelectionMode = selectionMode;
        }

        public Collection<MotionObjectInfo> GetListSelectedInfoIndices() {
            Collection<MotionObjectInfo> ret = new Collection<MotionObjectInfo>();
            lock(_dataSet) {
                foreach(var index in listSelect.SelectedIndices) {
                    ret.Add(_targetInfoList[(int)index]);
                }
            }
            return ret;
        }
        public Collection<MotionObjectInfo> GetListSelectedInfoIndices(IEnumerable<Type> targetTypes) {
            Collection<MotionObjectInfo> ret = new Collection<MotionObjectInfo>();
            lock(_dataSet) {
                foreach(var index in listSelect.SelectedIndices) {
                    if(targetTypes.Any(type => _targetInfoList[(int)index].ObjectType == type || _targetInfoList[(int)index].ObjectType.IsSubclassOf(type))) {
                        ret.Add(_targetInfoList[(int)index]);
                    }
                }
            }
            return ret;
        }

        private void MotionDataObjectSelectList_Load(object sender, EventArgs e) {
            this.Disposed += motionDataObjectSelectList_disposed;
        }

        public event EventHandler SelectedIndexChanged;

        private void listSelect_SelectedIndexChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.Invoke(new EventHandler(listSelect_SelectedIndexChanged), sender, e);
                return;
            }
            if(!_isSelectionSetting) {
                EventHandler tmp = this.SelectedIndexChanged;
                if(tmp != null)
                    tmp.Invoke(sender, e);
            }
        }

    }
}
