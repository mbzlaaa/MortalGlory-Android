#!/usr/bin/env bash
# setup-desktop.sh — 在 GitHub Codespace 内搭建「noVNC 桌面 + Unity Hub」
# 目的：用手机浏览器操作 Unity Hub 图形界面，生成 Unity Personal 许可证(.ulf)
# 注意：本项目硬性约束——不使用 set -e / set -o errexit

HUB_URL="https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage"
HUB_URL_CN="https://public-cdn.cloud.unitychina.cn/hub/prod/UnityHub.AppImage"
HUB_DIR="$HOME/hub"
LOG="$HOME/setup-desktop.log"

main() {
  echo "======== [1/6] apt update + 启用 universe ========"
  sudo apt-get update -y
  sudo DEBIAN_FRONTEND=noninteractive apt-get install -y software-properties-common
  sudo add-apt-repository -y universe
  sudo apt-get update -y

  echo "======== [2/6] 安装 Xvfb / x11vnc / noVNC / Xfce / 浏览器 ========"
  sudo DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
    xvfb x11vnc novnc websockify \
    xfce4 xfce4-terminal \
    epiphany-browser \
    dbus-x11 xdg-utils gnome-keyring libsecret-1-0 \
    fonts-noto-cjk fonts-dejavu-core \
    curl ca-certificates

  echo "======== [3/6] 下载并解包 Unity Hub ========"
  mkdir -p "$HUB_DIR"
  cd "$HUB_DIR" || return 1
  if [ ! -s UnityHub.AppImage ]; then
    curl -fL --retry 3 -o UnityHub.AppImage "$HUB_URL" || curl -fL --retry 3 -o UnityHub.AppImage "$HUB_URL_CN"
  fi
  chmod +x UnityHub.AppImage
  ./UnityHub.AppImage --appimage-extract >/dev/null 2>&1
  if [ -x "$HUB_DIR/squashfs-root/UnityHub" ]; then
    echo "  -> UnityHub 解包成功"
  else
    echo "  !! UnityHub 解包失败，请把本日志发回"
  fi

  echo "======== [4/6] 注册 unityhub:// 协议 + 默认浏览器 ========"
  mkdir -p "$HOME/.local/share/applications"
  cat > "$HOME/.local/share/applications/unityhub.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=Unity Hub
Exec=$HUB_DIR/squashfs-root/UnityHub %u
Terminal=false
MimeType=x-scheme-handler/unityhub;
EOF
  update-desktop-database "$HOME/.local/share/applications" 2>/dev/null
  xdg-mime default unityhub.desktop x-scheme-handler/unityhub 2>/dev/null
  BROWSER_DESKTOP=$(ls /usr/share/applications 2>/dev/null | grep -i epiphany | head -1)
  [ -n "$BROWSER_DESKTOP" ] && xdg-settings set default-web-browser "$BROWSER_DESKTOP" 2>/dev/null
  echo "  -> 默认浏览器 desktop 文件: ${BROWSER_DESKTOP:-未找到}"

  echo "======== [5/6] 校验 ========"
  for c in Xvfb x11vnc websockify startxfce4 epiphany; do
    printf "  %-12s %s\n" "$c" "$(command -v $c || echo '缺失！')"
  done

  echo "======== [6/6] 完成 ========"
  echo "  下一步：bash Tools/codespace-license/start-desktop.sh"
}

main 2>&1 | tee "$LOG"
