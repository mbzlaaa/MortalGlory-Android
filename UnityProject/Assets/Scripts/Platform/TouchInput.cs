// ============================================================
//  TouchInput.cs —— 鼠标/键盘 → 触屏 输入适配层
// ------------------------------------------------------------
//  原游戏是 PC 的鼠标点击/拖拽操作。安卓上需要：
//    - 鼠标左键按下  ->  单指触摸按下
//    - 鼠标拖拽      ->  单指滑动拖拽
//    - 鼠标悬停提示  ->  长按提示（可选）
//
//  Unity 的 Input 系统本身会把「单指触摸」自动映射为
//  GetMouseButtonDown(0)，所以很多地方无需改动。
//  但拖拽（Drag）在触屏上需要额外处理，本类提供统一入口。
// ============================================================

using UnityEngine;
using UnityEngine.EventSystems;

namespace MortalGlory.Platform
{
    /// <summary>
    /// 统一输入查询：PC 用鼠标，安卓用触摸，对外暴露统一接口。
    /// </summary>
    public static class TouchInput
    {
        /// <summary>是否有「指针按下」（鼠标左键 或 第一根手指）</summary>
        public static bool PointerDown =>
            Input.GetMouseButtonDown(0) || (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began);

        /// <summary>是否有「指针持续按住」</summary>
        public static bool PointerHeld =>
            Input.GetMouseButton(0) || (Input.touchCount > 0 &&
            (Input.GetTouch(0).phase == TouchPhase.Moved ||
             Input.GetTouch(0).phase == TouchPhase.Stationary));

        /// <summary>是否「指针抬起」</summary>
        public static bool PointerUp =>
            Input.GetMouseButtonUp(0) || (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Ended);

        /// <summary>当前指针屏幕坐标</summary>
        public static Vector2 PointerPosition => Input.mousePosition;

        /// <summary>是否处于触摸设备（用于切换 UI 提示文案等）</summary>
        public static bool IsTouchDevice =>
            Application.platform == RuntimePlatform.Android ||
            Input.touchSupported;
    }

    /// <summary>
    /// 供拖拽类（DragHandler*）继承的触屏拖拽基类骨架。
    /// 后续把原 DragHandlerCharacter / DragHandlerItem 改为继承此基类。
    /// </summary>
    public abstract class TouchDragHandler : MonoBehaviour,
        IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData e) => OnDragBegin(e);
        public void OnDrag(PointerEventData e) => OnDragMove(e);
        public void OnPointerUp(PointerEventData e) => OnDragEnd(e);

        protected abstract void OnDragBegin(PointerEventData e);
        protected abstract void OnDragMove(PointerEventData e);
        protected abstract void OnDragEnd(PointerEventData e);
    }
}
