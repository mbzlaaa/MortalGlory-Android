#!/usr/bin/env bash
# push-ulf.sh — 把 Unity Hub 生成的 .ulf 直接写成仓库的 Actions secret
# 用法（在 Codespace 终端）: bash Tools/codespace-license/push-ulf.sh
# 可选环境变量： MG_REPO 仓库(默认 mbzlaaa/MortalGlory-Android)
#               MG_PAT  Codespace 内置令牌权限不足时，用 PAT 兜底

REPO="${MG_REPO:-mbzlaaa/MortalGlory-Android}"

echo "=== 查找 .ulf ==="
ULF=""
for p in "$HOME/.local/share/unity3d/Unity/Unity_lic.ulf" \
         "$HOME/.config/unity3d/Unity/Unity_lic.ulf" \
         "/usr/share/unity3d/config/Unity_lic.ulf"; do
  if [ -s "$p" ]; then ULF="$p"; break; fi
done
if [ -z "$ULF" ]; then
  ULF=$(find "$HOME" -name '*.ulf' -size +1k 2>/dev/null | head -1)
fi
if [ -z "$ULF" ]; then
  echo "!! 没找到 .ulf。请先在 Unity Hub 里完成："
  echo "   Preferences > Licenses > Add > Get a free personal license"
  exit 1
fi
echo "找到: $ULF ($(wc -c < "$ULF") 字节)"

echo "=== 检查 gh 登录 ==="
if ! gh auth status >/dev/null 2>&1; then
  if [ -n "$MG_PAT" ]; then
    export GH_TOKEN="$MG_PAT"
  else
    echo "!! gh 未登录，且未提供 MG_PAT"
    exit 1
  fi
fi

echo "=== 采集 Unity 账号 ==="
if [ -z "$MG_EMAIL" ]; then read -rp "UNITY_EMAIL: " MG_EMAIL; fi
if [ -z "$MG_PASSWORD" ]; then read -rsp "UNITY_PASSWORD: " MG_PASSWORD; echo; fi

echo "=== 写入 secrets 到 $REPO ==="
gh secret set UNITY_LICENSE  --repo "$REPO" < "$ULF" && echo "  UNITY_LICENSE  OK"
gh secret set UNITY_EMAIL    --repo "$REPO" -b "$MG_EMAIL" && echo "  UNITY_EMAIL    OK"
gh secret set UNITY_PASSWORD --repo "$REPO" -b "$MG_PASSWORD" && echo "  UNITY_PASSWORD OK"

echo "=== 当前 secrets ==="
gh secret list --repo "$REPO"
