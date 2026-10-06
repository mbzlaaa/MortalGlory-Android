using UnityEngine;

// 兼容辅助类。
// 反编译产物里出现了形如 `Color32.op_Implicit(x)` / `Vector2.op_Implicit(x)` /
// `Object.op_Implicit(x)` 的写法：C# 不允许显式调用 operator/accessor（CS0571）。
// 这里用一组同名重载把「隐式转换」还原成普通方法调用，调用点只需把
//   Xxx.op_Implicit(...)  ->  MGCompat.Op.Imp(...)
// 重载按参数类型区分，不会产生歧义。
namespace MGCompat
{
    public static class Op
    {
        // Color32 -> Color
        public static Color Imp(Color32 c)
        {
            return c;
        }

        // Color -> Color32
        public static Color32 Imp(Color c)
        {
            return c;
        }

        // Vector3 -> Vector2
        public static Vector2 Imp(Vector3 v)
        {
            return v;
        }

        // UnityEngine.Object -> bool（等价于 Unity 的 implicit operator bool）
        public static bool Imp(Object o)
        {
            return o != null;
        }
    }
}
