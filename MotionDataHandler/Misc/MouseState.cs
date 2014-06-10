using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace MotionDataHandler.Misc {

    /// <summary>
    /// 現在押されているボタンの種類
    /// </summary>
    public enum RegulatedMouseButton {
        /// <summary>
        /// 押されているボタンはありません
        /// </summary>
        None,
        /// <summary>
        /// 修飾キーなしで左ボタンが押されています
        /// </summary>
        Left,
        /// <summary>
        /// Controlキー+左ボタン，もしくは真ん中ボタンが押されています
        /// </summary>
        CtrlLeft,
        /// <summary>
        /// Shiftキー+左ボタンが押されています
        /// </summary>
        ShiftLeft,
        /// <summary>
        /// Shiftキー+Controlキー+左ボタンが押されています
        /// </summary>
        ShiftCtrlLeft,
        /// <summary>
        /// Altキー+左ボタン，もしくはControlキー+右ボタンが押されています
        /// </summary>
        AltLeft,
        /// <summary>
        /// 右ボタンが押されています
        /// </summary>
        Right,
    }

    /// <summary>
    /// ボタンが押された後の位置に応じた状態
    /// </summary>
    public enum RegulatedMouseClickState {
        /// <summary>
        /// ボタンが押されたその場にいます
        /// </summary>
        Click,
        /// <summary>
        /// ボタンが押されてからマウスが一定距離移動しました
        /// </summary>
        Drag,
    }

    /// <summary>
    /// ボタン情報とクリック情報の組
    /// </summary>
    public struct RegulatedMouseInfo {
        /// <summary>
        /// 現在押されているボタンの種類
        /// </summary>
        public RegulatedMouseButton Button;
        /// <summary>
        /// ボタンが押された後の位置に応じた状態
        /// </summary>
        public RegulatedMouseClickState State;
    }


    /// <summary>
    /// マウス操作の標準化用クラス
    /// </summary>
    /// <remarks>
    /// 実際のボタンと論理的なボタンの対応(上から判定)
    /// Shift+Ctrl Left _______ ShiftCtrlLeft
    /// Shift+Ctrl Middle _/
    /// Ctrl Left _____________ CtrlLeft
    /// Ctrl Miffle _/
    /// Ctrl Right ____________ AltLeft
    /// Alt Left ___/
    /// Shift Left ____________ ShiftLeft
    /// Left __________________ Left
    /// Middle ________________ CtrlLeft
    /// Right _________________ Right
    /// 無効な論理ボタン時の変換(オプション)
    /// ShiftCtrlLeft _________ ShiftLeft
    /// ShiftCtrlLeft _________ CtrlLeft (ShiftLeftが無効な場合)
    /// ShiftLeft _____________ Left
    /// CtrlLeft ______________ Left
    /// AltLeft _______________ Left
    /// </remarks>
    public class RegulatedMouseControl {
        /// <summary>
        /// 一定の規則でボタンと修飾キーの状態をButtonTypeに変換します 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static RegulatedMouseButton StandardizeButton(MouseEventArgs e) {
            // Shift+Ctrl時の左・中クリック
            if((Control.ModifierKeys & Keys.Shift) != 0 && (Control.ModifierKeys & Keys.Control) != 0) {
                switch(e.Button) {
                case MouseButtons.Left:
                case MouseButtons.Middle:
                    return RegulatedMouseButton.ShiftCtrlLeft;
                }
            }
            // Ctrl時のクリック
            if((Control.ModifierKeys & Keys.Control) != 0) {
                switch(e.Button) {
                case MouseButtons.Left:
                case MouseButtons.Middle:
                    return RegulatedMouseButton.CtrlLeft;
                case MouseButtons.Right:
                    return RegulatedMouseButton.AltLeft;
                }
            }
            // Alt時のクリック
            if((Control.ModifierKeys & Keys.Alt) != 0) {
                switch(e.Button) {
                case MouseButtons.Left:
                    return RegulatedMouseButton.AltLeft;
                }
            }
            // Shift時のクリック
            if((Control.ModifierKeys & Keys.Shift) != 0) {
                switch(e.Button) {
                case MouseButtons.Left:
                    return RegulatedMouseButton.ShiftLeft;
                }
            }
            // それ以外はそのまま
            switch(e.Button) {
            case MouseButtons.Left:
                return RegulatedMouseButton.Left;
            case MouseButtons.Middle:
                return RegulatedMouseButton.CtrlLeft;
            case MouseButtons.Right:
                return RegulatedMouseButton.Right;
            }
            return RegulatedMouseButton.None;
        }
        /// <summary>
        /// 有効なボタンの種類
        /// </summary>
        private readonly HashSet<RegulatedMouseButton> _validButtonTypes = new HashSet<RegulatedMouseButton>();
        /// <summary>
        /// ドラッグだとみなされる距離
        /// </summary>
        private int _distanceAsDrag = 0;
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="distanceAsDrag">ボタン押下後ドラッグとみなされる移動距離</param>
        /// <param name="validButtonTypes">有効なボタンの種類．ButtonType.NoneとButtonType.Leftは自動的に含まれます</param>
        public RegulatedMouseControl(int distanceAsDrag, params RegulatedMouseButton[] validButtonTypes) {
            _distanceAsDrag = distanceAsDrag;
            _validButtonTypes.Add(RegulatedMouseButton.None);
            _validButtonTypes.Add(RegulatedMouseButton.Left);
            foreach(RegulatedMouseButton type in validButtonTypes) {
                _validButtonTypes.Add(type);
            }
        }

        /// <summary>
        /// 有効でないボタンタイプが指定されたときに有効なボタンに変換します．
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        private RegulatedMouseButton convertValidButtonType(RegulatedMouseButton bt) {
            if(_validButtonTypes.Contains(bt))
                return bt;
            switch(bt) {
            case RegulatedMouseButton.AltLeft:
            case RegulatedMouseButton.CtrlLeft:
            case RegulatedMouseButton.ShiftLeft:
                return convertValidButtonType(RegulatedMouseButton.Left);
            case RegulatedMouseButton.ShiftCtrlLeft:
                if(!_validButtonTypes.Contains(RegulatedMouseButton.ShiftLeft))
                    return convertValidButtonType(RegulatedMouseButton.CtrlLeft);
                return convertValidButtonType(RegulatedMouseButton.ShiftLeft);
            }
            return RegulatedMouseButton.None;
        }

        private Point _location, _downLocation;
        /// <summary>
        /// マウスの座標を取得します
        /// </summary>
        public Point Location { get { return _location; } }
        /// <summary>
        /// マウスのボタンが押された時の座標を取得します．
        /// </summary>
        public Point DownLocation { get { return _downLocation; } }

        MouseButtons _pressed = MouseButtons.None;
        bool _multiPressed = false;
        RegulatedMouseClickState _clickType = RegulatedMouseClickState.Click;
        RegulatedMouseButton _buttonType = RegulatedMouseButton.None;
        /// <summary>
        /// 現在の押されているボタンの種類
        /// </summary>
        public RegulatedMouseButton Button { get { return _buttonType; } }
        /// <summary>
        /// ボタンが押された後の位置に応じた状態
        /// </summary>
        public RegulatedMouseClickState Click { get { return _clickType; } }
        private Point _moveDelta;
        /// <summary>
        /// 今回のMouseMoveで移動した量
        /// </summary>
        public Point MoveDelta { get { return _moveDelta; } }
        /// <summary>
        /// 最後にボタンが押された位置から現在位置までの相対座標
        /// </summary>
        public Point DragOffset { get { return new Point(_location.X - _downLocation.X, _location.Y - _downLocation.Y); } }
        /// <summary>
        /// ボタンが押された時の処理を行います．
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public RegulatedMouseButton MouseDown(MouseEventArgs e) {
            MouseButtons pressed = _pressed;
            _pressed |= e.Button;
            if(pressed == MouseButtons.None) {
                _buttonType = convertValidButtonType(StandardizeButton(e));
                _clickType = RegulatedMouseClickState.Click;
                _multiPressed = false;
                _downLocation = e.Location;
            } else {
                _multiPressed = true;
                _buttonType = RegulatedMouseButton.None;
            }
            _location = e.Location;
            return _buttonType;
        }
        /// <summary>
        /// ボタンが離された時の処理を行います．
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public RegulatedMouseInfo MouseUp(MouseEventArgs e) {
            _location = e.Location;
            _pressed &= ~e.Button;
            if(_multiPressed) {
                return new RegulatedMouseInfo { Button = RegulatedMouseButton.None, State = RegulatedMouseClickState.Click };
            }
            RegulatedMouseInfo ret = new RegulatedMouseInfo { Button = _buttonType, State = _clickType };
            _buttonType = RegulatedMouseButton.None;
            _clickType = RegulatedMouseClickState.Click;
            return ret;
        }
        /// <summary>
        /// マウスが移動したときの処理を行います．
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public RegulatedMouseInfo MouseMove(MouseEventArgs e) {
            _moveDelta = new Point(e.X - _location.X, e.Y - _location.Y);
            _location = e.Location;
            if(_buttonType != RegulatedMouseButton.None) {
                if(Math.Abs(this.DragOffset.X) + Math.Abs(this.DragOffset.Y) >= _distanceAsDrag) {
                    _clickType = RegulatedMouseClickState.Drag;
                }
            }
            return new RegulatedMouseInfo { Button = _buttonType, State = _clickType };
        }
    }
}
