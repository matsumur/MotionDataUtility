using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence {
    public partial class DialogLabelColorSet : Form {
        readonly SequenceViewerController _controller;

        struct FloatColor {
            public float R, G, B;

            public bool IsValidColor() {
                if(float.IsInfinity(this.R) || float.IsNaN(this.R))
                    return false;
                if(float.IsInfinity(this.G) || float.IsNaN(this.G))
                    return false;
                if(float.IsInfinity(this.B) || float.IsNaN(this.B))
                    return false;
                return true;
            }
            public FloatColor(Color color) {
                this.R = (float)color.R / 255;
                this.G = (float)color.G / 255;
                this.B = (float)color.B / 255;
            }
            public FloatColor(float r, float g, float b) {
                this.R = r;
                this.G = g;
                this.B = b;
            }
            public FloatColor LimitRange() {
                return this.LimitRange(0, 1);
            }
            public FloatColor LimitRange(float min, float max) {
                if(min > max)
                    throw new ArgumentOutOfRangeException("max", "'max' cannot be less than 'min'");
                return new FloatColor(Math.Min(Math.Max(this.R, min), max), Math.Min(Math.Max(this.G, min), max), Math.Min(Math.Max(this.B, min), max));
            }
            public Color ToColor() {
                FloatColor tmp = this.LimitRange();
                return Color.FromArgb((int)Math.Round(tmp.R * 255), (int)Math.Round(tmp.G * 255), (int)Math.Round(tmp.B * 255));
            }
            public FloatColor Add(FloatColor color) {
                return new FloatColor(this.R + color.R, this.G + color.G, this.B + color.B);
            }
            public FloatColor Divide(float d) {
                return new FloatColor(this.R / d, this.G / d, this.B / d);
            }
            public FloatColor Multiply(float a) {
                return new FloatColor(this.R * a, this.G * a, this.B * a);
            }
        }

        public DialogLabelColorSet(SequenceViewerController controller) {
            if(controller == null)
                throw new ArgumentNullException("controller", "'controller' cannot be null");
            InitializeComponent();
            _controller = controller;
        }

        private void DialogLabelColorSet_Load(object sender, EventArgs e) {
            setSequenceList();
        }

        Dictionary<string, IDictionary<string, Color>> _colorMap = new Dictionary<string, IDictionary<string, Color>>();

        private void setColorMap(IDictionary<string, Color> map) {
            foreach(ListViewItem item in listViewSequence.Items) {
                if(!item.Checked)
                    continue;
                if(!_colorMap.ContainsKey(item.Text))
                    _colorMap[item.Text] = new Dictionary<string, Color>();
                foreach(var pair in map) {
                    if(pair.Key == "")
                        continue;
                    _colorMap[item.Text][pair.Key] = pair.Value;
                }
            }
        }
        private void resetColorMap(IList<string> labels) {
            foreach(ListViewItem item in listViewSequence.Items) {
                if(!item.Checked)
                    continue;
                IDictionary<string, Color> map;
                if(_colorMap.TryGetValue(item.Text, out map)) {
                    foreach(string label in labels) {
                        map.Remove(label);
                    }
                }
            }
        }
        private void commitColorMap() {
            foreach(var seqPair in _colorMap) {
                SequenceData data = _controller.GetSequenceByTitle(seqPair.Key);
                if(data == null)
                    continue;
                foreach(var labelPair in seqPair.Value) {
                    data.Borders.SetColor(labelPair.Key, labelPair.Value);
                }
            }
            _colorMap.Clear();
        }

        private void setSequenceList() {
            listViewSequence.Items.Clear();
            SequenceView focusedViewer = _controller.GetFocusedView();
            SequenceData focusedData = null;
            if(focusedViewer != null) {
                focusedData = focusedViewer.Sequence;
            }
            foreach(var data in _controller.GetSequenceList()) {
                if((data.Type & SequenceType.Label) != 0) {
                    ListViewItem item = new ListViewItem(data.Title);
                    item.Checked = focusedData == data;
                    listViewSequence.Items.Add(item);
                }
            }
        }

        Dictionary<string, IList<Color>> getLabelColors() {
            Dictionary<string, IList<Color>> ret = new Dictionary<string, IList<Color>>();
            foreach(ListViewItem item in listViewSequence.Items) {
                if(!item.Checked)
                    continue;
                SequenceData data = _controller.GetSequenceByTitle(item.Text);
                if(data == null)
                    continue;
                Dictionary<string, Color> palette = data.Borders.GetColorPalette();
                if(!palette.ContainsKey("")) {
                    palette[""] = Color.White;
                }
                IDictionary<string, Color> map;
                if(!_colorMap.TryGetValue(item.Text, out map)) {
                    map = new Dictionary<string, Color>();
                }
                foreach(var pair in palette) {
                    if(!ret.ContainsKey(pair.Key))
                        ret[pair.Key] = new List<Color>();
                    Color color;
                    if(!map.TryGetValue(pair.Key, out color)) {
                        color = pair.Value;
                    }
                    ret[pair.Key].Add(color);
                }
            }
            return ret;
        }

        Dictionary<string, IList<Color>> getSelectedLabelColors() {
            Dictionary<string, IList<Color>> labelColors = getLabelColors();
            Dictionary<string, IList<Color>> ret = new Dictionary<string, IList<Color>>();
            foreach(ListViewItem item in listViewLabel.Items) {
                if(item.Checked) {
                    IList<Color> colors;
                    if(labelColors.TryGetValue(item.Text, out colors)) {
                        ret[item.Text] = colors.ToList();
                    }
                }
            }
            return ret;
        }

        private void setLabelList() {
            Dictionary<string, bool> checkedLabels = new Dictionary<string, bool>();
            foreach(ListViewItem item in listViewLabel.Items) {
                checkedLabels[item.Text] = item.Checked;
            }
            listViewLabel.Items.Clear();
            Dictionary<string, IList<Color>> labelColors = getLabelColors();
            List<string> labels = new List<string>(labelColors.Keys);
            labels.Sort();
            foreach(string label in labels) {
                bool labelChecked;
                if(!checkedLabels.TryGetValue(label, out labelChecked)) {
                    labelChecked = true;
                }
                ListViewItem item = new ListViewItem(label);
                item.UseItemStyleForSubItems = false;
                item.Checked = labelChecked;
                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem();
                if(labelColors[label].All(l => l == labelColors[label].First())) {
                    Color color = labelColors[label].First();
                    subItem.Text = ColorTranslator.ToHtml(color);
                    subItem.BackColor = color;
                    subItem.ForeColor = Misc.ColorEx.GetComplementaryColor(color);
                } else {
                    subItem.Text = "Indefinite";
                }
                item.SubItems.Add(subItem);
                listViewLabel.Items.Add(item);
            }
        }

        private void buttonUnify_Click(object sender, EventArgs e) {
            unifyColors();
            setLabelList();
        }

        private void unifyColors() {
            Dictionary<string, IList<Color>> labelColors = getSelectedLabelColors();
            Dictionary<string, Color> newColors = new Dictionary<string, Color>();
            foreach(var pair in labelColors) {
                if(!pair.Value.All(l => l == pair.Value.First())) {
                    FloatColor[] fColors = pair.Value.Select(c => new FloatColor(c)).ToArray();
                    FloatColor ret = new FloatColor();
                    foreach(FloatColor fColor in fColors) {
                        ret = ret.Add(fColor);
                    }
                    ret = ret.Divide(fColors.Length);
                    newColors.Add(pair.Key, ret.ToColor());
                }
            }
            setColorMap(newColors);
        }

        private void listViewSequence_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
        }

        private void listViewLabel_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
        }

        private void setButtonEnabled() {
            var labelColors = getSelectedLabelColors();
            buttonRestore.Enabled = labelColors.Count > 0;
            buttonDistributeRGB.Enabled = labelColors.Count > 0;
            buttonUnify.Enabled = labelColors.Count > 0;
            buttonDefault.Enabled = labelColors.Count > 0;
            buttonChangeColor.Enabled = labelColors.Count > 0;
        }

        private void listViewSequence_ItemChecked(object sender, ItemCheckedEventArgs e) {
            setLabelList();
            setButtonEnabled();
        }

        private void listViewLabel_ItemChecked(object sender, ItemCheckedEventArgs e) {
            setButtonEnabled();
        }

        private void buttonDistributeRGB_Click(object sender, EventArgs e) {
            distributeColor(new Func<FloatColor, FloatColor, FloatColor>((x, y) => new FloatColor(x.R - y.R, x.G - y.G, x.B - y.B)));
            setLabelList();
        }

        private void distributeColor(Func<FloatColor, FloatColor, FloatColor> subtract) {
            unifyColors();
            var labelColors = getSelectedLabelColors();
            Dictionary<string, Color> firstColors = labelColors.ToDictionary(p => p.Key, p => p.Value.First());
            Dictionary<string, FloatColor> floatColors = firstColors.ToDictionary(p => p.Key, p => new FloatColor(p.Value));
            for(int i = 0; i < 5; i++) {
                float coeff = 8f * (float)Math.Pow(0.5f, i / 250);
                Dictionary<string, FloatColor> prevColors = new Dictionary<string, FloatColor>(floatColors);
                foreach(var pair in prevColors) {
                    FloatColor displacement = new FloatColor();
                    foreach(var pair2 in prevColors) {
                        if(pair.Key == pair2.Key)
                            continue;
                        FloatColor diff = subtract(pair.Value, pair2.Value);
                        float distance = (float)Math.Sqrt(diff.R * diff.R + diff.G * diff.G + diff.B * diff.B);
                        FloatColor dir = diff.Divide(distance);
                        if(dir.IsValidColor()) {
                            FloatColor addend = dir.Multiply(0.0625f * 0.0625f / (float)Math.Sqrt(floatColors.Count) / ((distance + 0.0625f) * (distance + 0.0625f)));
                            displacement = displacement.Add(addend);
                        }
                    }
                    floatColors[pair.Key] = floatColors[pair.Key].Add(displacement.Multiply(coeff)).LimitRange();
                }
            }
            Dictionary<string, Color> resultColor = floatColors.ToDictionary(p => p.Key, p => p.Value.ToColor());
            setColorMap(resultColor);
        }

        private void buttonRestore_Click(object sender, EventArgs e) {
            var labelColors = getSelectedLabelColors();
            resetColorMap(labelColors.Select(p => p.Key).ToList());
            setLabelList();
        }

        private void DialogLabelColorSet_FormClosing(object sender, FormClosingEventArgs e) {
            commitColorMap();
        }

        private void buttonDistributeRB_Click(object sender, EventArgs e) {
            distributeColor(new Func<FloatColor, FloatColor, FloatColor>((x, y) => new FloatColor(x.R - y.R, 0, x.B - y.B)));
            setLabelList();
        }

        private void buttonDefault_Click(object sender, EventArgs e) {
            var labelColors = getSelectedLabelColors();
            Dictionary<string, Color> map = new Dictionary<string, Color>();
            foreach(var pair in labelColors) {
                map[pair.Key] = LabelingBorders.GetDefaultColorFor(pair.Key);
            }
            setColorMap(map);
            setLabelList();
        }

        private void buttonChangeColor_Click(object sender, EventArgs e) {
            var labelColors = getSelectedLabelColors();
            if(labelColors.Count > 0 && labelColors.First().Value.Count > 0) {
                colorDialog.Color = labelColors.First().Value.First();
                if(colorDialog.ShowDialog() == DialogResult.OK) {
                    Dictionary<string, Color> map = new Dictionary<string, Color>();
                    foreach(var pair in labelColors) {
                        map[pair.Key] = colorDialog.Color;
                    }
                    setColorMap(map);
                    setLabelList();
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
