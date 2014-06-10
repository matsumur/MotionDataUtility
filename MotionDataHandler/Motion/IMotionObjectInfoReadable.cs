using System;
using System.Drawing;
namespace MotionDataHandler.Motion {
    public interface IReadableMotionObjectInfo {
        Color Color { get; }
        uint Id { get; }
        bool IsIdSet { get; }
        bool IsTypeOf(Type type);
        string Name { get; }
        Type ObjectType { get; }
        bool IsVisible { get; }
        bool IsSelected { get; }
        //MotionDataSet Parent { get; }
    }
}
