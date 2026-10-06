# 方案 B：用 GitHub Codespaces 获取 Unity 授权（.ulf）操作向导

> 目的：在云端 x86_64 Linux（Codespaces）里跑图形桌面，用**手机浏览器**操作 Unity Hub，
> 登录 Unity 账号并生成 Personal 授权文件 `.ulf`，再自动写入仓库 Secrets。
> 全程不需要电脑、不需要本地跑 Gradle、不需要复制粘贴授权内容。

---

## 0. 前置条件

- 已开启 VPN（访问 github.com / api.github.com）。
- 本地两个提交已推送到 `origin/main`（`eb337bf`、`835df9d`）。
- GitHub 账号有 Codespaces 额度（免费额度通常够，本流程仅需 1~2 小时）。
- 一个用于登录 Unity 的账号（推荐用邮箱注册的 Unity ID）。

---

## 1. 推送本地提交（手机 Termux / 终端）

```bash
cd /root/port-repo
git push origin main
```

推送成功的关键在 VPN。若卡在 `Resolving deltas` 或超时，重试即可。

---

## 2. 在 GitHub 上创建 Codespace

1. 手机浏览器打开 `https://github.com/mbzlaaa/MortalGlory-Android`
2. 点绿色 **Code** 按钮 → **Codespaces** 标签页 → **Create codespace on main**
3. Codespaces 会自动读取 `.devcontainer/devcontainer.json`：
   - 底盘：`mcr.microsoft.com/devcontainers/base:ubuntu-22.04`
   - 自动安装 GitHub CLI
   - `postCreateCommand` 自动跑 `setup-desktop.sh`（装桌面 + 拉 Unity Hub，首次约 3~8 分钟）
4. 等底部终端出现 **“✅ 环境搭建完成”** 字样。

> ⚠️ 若 postCreate 未自动执行，手动在终端里跑：
> ```bash
> bash Tools/codespace-license/setup-desktop.sh
> ```

---

## 3. 启动桌面

Codespace 终端里执行：

```bash
bash Tools/codespace-license/start-desktop.sh
```

看到 `noVNC 已启动 (6080)` 与 `Unity Hub 已拉起` 即可。

---

## 4. 手机浏览器打开桌面

1. Codespace 底部切到 **Ports（端口）** 面板，找到 **6080** 端口。
2. 把该端口可见性设为 **Public**（或保持默认，点地球图标可复制链接）。
3. 手机浏览器访问：

```
https://<codespace名>-6080.app.github.dev/vnc.html?autoconnect=1&resize=scale
```

4. 应当看到 Xfce 桌面，且 Unity Hub 已打开。

> 小屏操作技巧：noVNC 左侧有个折叠把手，可调缩放；建议横屏使用。

---

## 5. 在 Hub 里生成授权

1. Hub 右上角头像 → **Sign in**，用 Unity ID 登录。
2. 若弹验证码/邮箱验证：Hub 内会自动用 Epiphany 浏览器打开 OAuth 页，在 noVNC 里勾选/输入即可。
3. 登录后：**Preferences（齿轮）→ Licenses → Add → Get a free personal license → Agree**。
4. 成功后授权文件会写到：
   `~/.local/share/unity3d/Unity/Unity_lic.ulf`

验证（Codespace 终端）：

```bash
ls -l ~/.local/share/unity3d/Unity/Unity_lic.ulf
```

---

## 6. 把授权写进仓库 Secrets

Codespace 终端里执行（会自动找到 .ulf）：

```bash
bash Tools/codespace-license/push-ulf.sh
```

- 若 Codespace 内置令牌权限不足，会提示改成：
  ```bash
  MG_PAT=<你的PAT> bash Tools/codespace-license/push-ulf.sh
  ```
- 按提示输入 `UNITY_EMAIL` / `UNITY_PASSWORD`（与 Unity ID 一致）。
- 成功后应看到三行 OK：`UNITY_LICENSE / UNITY_EMAIL / UNITY_PASSWORD`。

> Secrets 写入目标仓库：`mbzlaaa/MortalGlory-Android`。

---

## 7. 收尾

1. 关闭 Codespace（释放额度）：GitHub → Codespaces 列表 → Stop / Delete。
2. 重触发 CI 验证：
   ```bash
   # 本地（需 VPN）
   cd /root/port-repo && git commit --allow-empty -m "ci: retrigger after license" && git push
   ```
3. 看 Actions 日志，确认 “Build with Unity” 不再报 `Missing Unity License File`。

---

## 附：故障排查

| 现象 | 对策 |
|---|---|
| Hub 打不开（白屏/闪退） | 已带 `--no-sandbox --disable-gpu`；再看 `~/unityhub.log` |
| noVNC 白屏 | 检查 `start-desktop.sh` 是否全部启动；看 `~/xvfb.log`、`~/x11vnc.log`、`~/websockify.log` |
| 6080 打不开 | Ports 面板把 6080 设为 Public；VPN 保持开启 |
| Hub 登录卡住 | 在 noVNC 里手动点 Hub 内嵌浏览器完成 OAuth |
| push-ulf 报 401/403 | 换用 `MG_PAT`（需 repo + secrets 写权限） |
| 找不到 .ulf | `find ~ -name 'Unity_lic.ulf' 2>/dev/null` |

---

_生成于方案 B 落地阶段；配套脚本见 `Tools/codespace-license/`。_
