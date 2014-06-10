using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Motion {
    public class ReadOnlyMotionObjectInfo : IReadableMotionObjectInfo {
        private MotionObjectInfo _internalInfo;

        public ReadOnlyMotionObjectInfo(MotionObjectInfo info) {
            if(info == null)
                throw new ArgumentNullException("info", "'info' cannot be null");
            _internalInfo = info;
        }

        #region IMotionObjectInfoReadable メンバ

        public System.Drawing.Color Color {
            get { return _internalInfo.Color; }
        }

        public uint Id {
            get { return _internalInfo.Id; }
        }

        public bool IsIdSet {
            get { return _internalInfo.IsIdSet; }
        }

        public bool IsTypeOf(Type type) {
            return _internalInfo.IsTypeOf(type);
        }

        public string Name {
            get { return _internalInfo.Name; }
        }

        public Type ObjectType {
            get { return _internalInfo.ObjectType; }
        }

        public bool IsVisible {
            get { return _internalInfo.IsVisible; }
        }

        public bool IsSelected {
            get { return _internalInfo.IsSelected; }
        }

        //public MotionDataSet Parent {
        //    get { return _internalInfo.Parent; }
        //}

        #endregion
    }
}
