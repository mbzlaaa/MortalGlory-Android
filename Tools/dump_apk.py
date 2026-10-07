#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""构建后自动解剖 APK 的 assets/bin/Data 布局（Mortal Glory 移植取证用）。

用途：CI 构建完成后直接打印 APK 的关键布局证据，便于云端一眼判断
构建产物是否自洽（是否出现 .resS 外置、大文件是否被切分、压缩方式等）。
不依赖第三方库，只用标准库。
"""
import zipfile
import glob
import os
import sys
from collections import Counter


def main():
    apks = sorted(glob.glob('build/Android/*.apk'))
    if not apks:
        # 兼容自定义输出目录
        apks = sorted(glob.glob('**/*.apk', recursive=True))
    if not apks:
        print('NO_APK')
        return 0

    for apk in apks:
        print('==== APK', apk, os.path.getsize(apk))
        try:
            z = zipfile.ZipFile(apk)
        except Exception as e:
            print('OPEN_FAIL', e)
            continue

        names = z.namelist()
        prefix = 'assets/bin/Data/'
        data = [i for i in z.infolist() if i.filename.startswith(prefix)]

        comp = Counter(i.compress_type for i in data)
        print('DATA_ENTRIES', len(data), 'COMP(0=STORED,8=DEFLATE)', dict(comp))

        # 顶层（Data/ 下不再含子目录）文件
        top = [i for i in data if '/' not in i.filename[len(prefix):]]
        print('TOP_LEVEL', len(top))
        for i in sorted(top, key=lambda x: x.filename):
            print('  %-45s comp=%d size=%d' % (
                i.filename.split('/')[-1], i.compress_type, i.file_size))

        # 分片统计
        sc = Counter()
        for n in names:
            if n.startswith(prefix) and '.split' in n:
                sc[n.split('.split')[0].split('/')[-1]] += 1
        print('SPLITS', dict(sc))

        # 关键文件是否存在（裸名或分片形式均算存在）
        def has(p):
            return any(n == prefix + p or n.startswith(prefix + p + '.split')
                       for n in names)

        keys = [
            'sharedassets0.assets', 'sharedassets0.assets.resS',
            'sharedassets0.resource', 'sharedassets1.assets.resS',
            'sharedassets2.assets.resS', 'globalgamemanagers',
            'globalgamemanagers.assets.resS', 'resources.assets',
            'resources.assets.resS', 'level0', 'unity default resources',
        ]
        for k in keys:
            print('HAS %-32s %s' % (k, has(k)))
        print('RES_S_COUNT', sum(1 for n in names if n.endswith('.resS')))
    return 0


if __name__ == '__main__':
    sys.exit(main())
