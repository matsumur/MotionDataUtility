using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 長い文を表示するメッセージボックス
    /// </summary>
    public partial class DialogOKMessage : Form {
        public DialogOKMessage(string message, string title) {
            InitializeComponent();
            textMsg.Text = message;
            Text = title;
        }
        public DialogOKMessage(string message) : this(message, "") { }
    }
}
