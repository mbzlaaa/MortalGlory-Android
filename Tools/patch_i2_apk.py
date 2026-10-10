#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
构建后修复：APK 内 I2 多语言资产（MonoBehaviour）的中文/俄文/日文等
非 ASCII 字符串在 Unity 导入/序列化时被"按字符数当字节数"截断，
导致切到简中后 UI 文本残缺（纯 ASCII 的英/西/葡不受影响）。

本脚本在 CI 构建完成后运行：
  1) 从工程源 YAML(UnityProject/Assets/Resources/I2Languages.asset) 读出完整词条值；
  2) 在 build/Android/*.apk 里定位 I2 资产 entry（assets/bin/Data/<GUID>）；
  3) 按 UTF-8 真实字节长度重写 I2 数据区（保留尾部 mLanguages 语言列表）；
  4) 去掉旧 META-INF，输出 *-patched-unsigned.apk（由后续步骤 zipalign + apksigner 重签）。

不依赖第三方库，只用标准库。控制台输出 PATCHED=<path> 便于工作流引用。
"""
import os
import re
import sys
import glob
import struct
import zipfile

REPO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
YAML = os.path.join(REPO, 'UnityProject', 'Assets', 'Resources', 'I2Languages.asset')
META = YAML + '.meta'
FIRST_KEY = b'BUFF/BUFF_Agile'


# ---------------------------------------------------------------- YAML
def parse_yaml_terms(path):
    """从 I2Languages.asset(YAML) 解析 词条名 -> [语言完整值]（按出现顺序）。

    YAML 结构（缩进固定）：
        mTerms:
        - Term: BUFF/BUFF_Agile
          TermType: 0
          Languages:
          - 敏捷
          - Ketterä
          ...
          Flags: ...
          Languages_Touch: []
    注意：空语言值形如 "      - "，必须占位（否则会与槽位错位）。
    """
    with open(path, 'r', encoding='utf-8', errors='replace') as f:
        txt = f.read().split('\n')
    terms = []
    i = 0
    n = len(txt)
    while i < n:
        m = re.match(r'\s*- Term: (.*)$', txt[i])
        if m:
            name = m.group(1)
            j = i + 1
            while j < n and 'Languages:' not in txt[j]:
                j += 1
            j += 1
            vals = []
            while j < n and re.match(r'\s*- ', txt[j]):
                vals.append(txt[j].split('- ', 1)[1].strip())
                j += 1
            terms.append((name, vals))
            i = j
            continue
        i += 1
    return terms


def read_guid(meta_path, yaml_path):
    try:
        with open(meta_path, 'r', encoding='utf-8') as f:
            for line in f:
                line = line.strip()
                if line.startswith('guid:'):
                    return line.split(':', 1)[1].strip()
    except OSError:
        pass
    return None


# ---------------------------------------------------------------- I2 数据区
def rd_str(d, p):
    n = struct.unpack_from('<i', d, p)[0]
    p += 4
    s = d[p:p + n].decode('utf-8', 'replace')
    p += n
    p += (-n) % 4
    return s, p


def wr_str(s):
    b = s.encode('utf-8')
    return struct.pack('<i', len(b)) + b + b'\x00' * ((-len(b)) % 4)


def parse_full(d):
    i = d.find(FIRST_KEY)
    if i < 0:
        return None
    count_off = i - 8
    cnt = struct.unpack_from('<i', d, count_off)[0]
    p = count_off + 4
    terms = []
    for _ in range(cnt):
        term, p = rd_str(d, p)
        tt = struct.unpack_from('<i', d, p)[0]; p += 4
        lc = struct.unpack_from('<i', d, p)[0]; p += 4
        langs = []
        for _k in range(lc):
            s, p = rd_str(d, p)
            langs.append(s)
        fl = struct.unpack_from('<i', d, p)[0]; p += 4
        flags = d[p:p + fl]; p += fl; p += (-fl) % 4
        tc = struct.unpack_from('<i', d, p)[0]; p += 4
        touch = []
        for _k in range(tc):
            s, p = rd_str(d, p)
            touch.append(s)
        terms.append((term, tt, langs, flags, touch))
    return count_off, cnt, terms, p


def encode_term(term, tt, langs, flags, touch):
    out = wr_str(term) + struct.pack('<i', tt) + struct.pack('<i', len(langs))
    for x in langs:
        out += wr_str(x)
    out += struct.pack('<i', len(flags)) + flags + b'\x00' * ((-len(flags)) % 4)
    out += struct.pack('<i', len(touch))
    for x in touch:
        out += wr_str(x)
    return out


def repair_blob(d, langmap, tag=''):
    """把 blob 内的词条值替换成 YAML 完整值（按真实字节长度写回）。"""
    parsed = parse_full(d)
    if parsed is None:
        print('%s  I2_ENTRY_NOT_FOUND' % tag)
        return None
    count_off, cnt, terms, end = parsed
    head = d[:count_off]
    count = d[count_off:count_off + 4]
    out_terms = []
    fixed = 0
    for (term, tt, langs, flags, touch) in terms:
        src = langmap.get(term)
        new = list(langs)
        if src:
            for k in range(min(len(new), len(src))):
                if src[k] and new[k] != src[k]:
                    fixed += 1
                if src[k]:
                    new[k] = src[k]
        out_terms.append(encode_term(term, tt, new, flags, touch))
    newdata = bytearray(head + count + b''.join(out_terms) + d[end:])  # 保留尾部 mLanguages
    newdata[4:8] = struct.pack('>I', len(newdata))                     # header fileSize (BE)
    old_datasize = len(d) - 4096
    pat = struct.pack('<I', old_datasize)
    idx = bytes(newdata[:4096]).find(pat)
    if idx >= 0:
        newdata[idx:idx + 4] = struct.pack('<I', len(newdata) - 4096)  # 对象 byteSize (LE)
    print('%s  terms=%d fixed=%d old=%d new=%d' % (tag, cnt, fixed, len(d), len(newdata)))
    return bytes(newdata)


# ---------------------------------------------------------------- APK
def patch_apk(apk_in, apk_out, guid, langmap):
    target = 'assets/bin/Data/' + guid if guid else None
    zin = zipfile.ZipFile(apk_in)
    names = zin.namelist()
    if target is None or target not in names:
        # 退路：在 assets/bin/Data/ 下按 GUID 形态找
        for n in names:
            if n.startswith('assets/bin/Data/') and len(os.path.basename(n)) == 32:
                target = n
                break
    if target is None or target not in names:
        print('  NO_I2_ENTRY in', apk_in)
        zin.close()
        return False
    blob = zin.read(target)
    fixed = repair_blob(blob, langmap, tag='  ' + os.path.basename(apk_in))
    if fixed is None:
        zin.close()
        return False
    if os.path.exists(apk_out):
        os.remove(apk_out)
    zout = zipfile.ZipFile(apk_out, 'w', allowZip64=True)
    for i in zin.infolist():
        nm = i.filename
        if nm.startswith('META-INF/'):
            continue
        zi = zipfile.ZipInfo(nm, date_time=i.date_time)
        zi.external_attr = i.external_attr
        zi.internal_attr = i.internal_attr
        zi.create_system = i.create_system
        zi.compress_type = zipfile.ZIP_STORED if i.compress_type == 0 else zipfile.ZIP_DEFLATED
        data = fixed if nm == target else zin.read(nm)
        zout.writestr(zi, data)
    zout.close()
    zin.close()
    print('  WROTE', apk_out, os.path.getsize(apk_out))
    return True


def main():
    apks = sys.argv[1:]
    if not apks:
        apks = sorted(glob.glob('build/Android/*.apk'))
    apks = [a for a in apks if not a.endswith('-patched-unsigned.apk')]
    if not apks:
        print('NO_APK')
        print('PATCHED=')
        return 0
    if not os.path.exists(YAML):
        print('NO_YAML', YAML)
        print('PATCHED=')
        return 0
    terms = parse_yaml_terms(YAML)
    langmap = dict(terms)
    print('yaml terms', len(terms), 'sample', terms[0][0] if terms else None)
    guid = read_guid(META, YAML)
    print('i2 guid', guid)
    any_ok = []
    for apk in apks:
        out = os.path.join(os.path.dirname(apk),
                           os.path.basename(apk)[:-4] + '-patched-unsigned.apk')
        if patch_apk(apk, out, guid, langmap):
            any_ok.append(out)
    print('PATCHED=' + (','.join(any_ok) if any_ok else ''))
    return 0


if __name__ == '__main__':
    sys.exit(main())
