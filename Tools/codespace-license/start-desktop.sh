#!/usr/bin/env bash
# start-desktop.sh — 启动 noVNC 桌面(端口 6080)并自动拉起 Unity Hub
# 可重复执行（先清理旧实例），也可作为 devcontainer 的 postStartCommand

DISP=":1"
HUB_DIR="$HOME/hub"

if ! command -v websockify >/dev/null 2>&1; then
  echo "!! 还没安装，请先执行: bash Tools/codespace-license/setup-desktop.sh"
  exit 1
fi

echo "=== 清理旧实例 ==="
pkill -f "Xvfb $DISP" 2>/dev/null
pkill -f "x11vnc -display $DISP" 2>/dev/null
pkill -f "websockify .*6080" 2>/dev/null
pkill -f "squashfs-root/UnityHub" 2>/dev/null
sleep 1

echo "=== 启动 Xvfb ($DISP) ==="
Xvfb $DISP -screen 0 1600x900x24 -nolisten tcp > "$HOME/xvfb.log" 2>&1 &
sleep 2
export DISPLAY=$DISP

echo "=== 启动 Xfce ==="
dbus-launch --exit-with-session startxfce4 > "$HOME/xfce.log" 2>&1 &
sleep 4

echo "=== 启动 x11vnc ==="
x11vnc -display $DISP -forever -shared -nopw -rfbport 5900 -bg -o "$HOME/x11vnc.log"
sleep 1

echo "=== 启动 noVNC (6080) ==="
websockify --web=/usr/share/novnc/ 6080 127.0.0.1:5900 > "$HOME/websockify.log" 2>&1 &
sleep 2

echo "=== 拉起 Unity Hub ==="
if [ -x "$HUB_DIR/squashfs-root/UnityHub" ]; then
  "$HUB_DIR/squashfs-root/UnityHub" --no-sandbox --disable-gpu > "$HOME/unityhub.log" 2>&1 &
else
  echo "  !! 未找到 UnityHub，请先跑 setup-desktop.sh"
fi

echo ""
echo "=== noVNC 就绪 ==="
echo "在 Codespaces 的【端口 / Ports】面板找到 6080，打开浏览器访问："
echo "  https://<codespace名>-6080.app.github.dev/vnc.html?autoconnect=1&resize=scale"
echo "若 vnc.html 打不开，改试 /vnc_lite.html"
