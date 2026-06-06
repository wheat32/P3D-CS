#!/usr/bin/env python3
"""
Convert VB.NET Pokemon3D Ability subclass files to C# (.NET 10).
Each ability file is tiny — mostly just MyBase.New(id, name, desc).

Usage:
    python3 convert_ability.py Adaptability.vb > Adaptability.cs
    python3 convert_ability.py --dir /vb/Abilities /cs/Abilities
"""

import re
import sys
import os
import argparse

# Re-use conversion utilities from convert_move.py by importing directly
sys.path.insert(0, os.path.dirname(__file__))
from convert_move import (
    read_vb_file, convert_body_line, convert_expr, parse_method_sig,
    parse_param_list, _select_stack, map_type, _post_process
)

import convert_move as cm


def convert_ability_file(vb_text: str, source_path: str = '') -> str:
    lines = vb_text.splitlines()
    if lines and lines[0].startswith('﻿'):
        lines[0] = lines[0][1:]

    # Extract class name
    class_name = ''
    for line in lines:
        s = line.strip()
        m = re.match(r"Public\s+Class\s+\[?(\w+)\]?", s, re.IGNORECASE)
        if m and class_name == '':
            class_name = m.group(1)
    if not class_name:
        return f'// ERROR: could not parse class name from {source_path}\n'

    out: list[str] = []
    out.append('namespace P3D.Abilities;')
    out.append('')
    out.append(f'public class {class_name} : Ability')
    out.append('{')

    IN_NONE, IN_CTOR, IN_METHOD = 0, 1, 2
    state = IN_NONE
    method_header = ''
    method_body: list[str] = []
    ctor_base_args: str | None = None

    _select_stack.clear()
    cm._with_target = ''

    for raw in lines:
        stripped = raw.strip()

        # Skip structural VB boilerplate
        if (re.match(r"Namespace\s+", stripped, re.IGNORECASE) or
                re.match(r"End\s+Namespace\s*$", stripped, re.IGNORECASE) or
                re.match(r"Public\s+Class\s+\[?\w+\]?", stripped, re.IGNORECASE) or
                re.match(r"Inherits\s+Ability", stripped, re.IGNORECASE) or
                re.match(r"End\s+Class\s*$", stripped, re.IGNORECASE)):
            continue

        # Constructor start
        if re.match(r"Public\s+Sub\s+New\(\)\s*$", stripped, re.IGNORECASE):
            state = IN_CTOR
            ctor_base_args = None
            continue

        # MyBase.New(args) inside constructor
        if state == IN_CTOR:
            m = re.match(r"MyBase\.New\((.+)\)\s*$", stripped, re.IGNORECASE)
            if m:
                ctor_base_args = convert_expr(m.group(1).strip())
                continue
            if re.match(r"End\s+Sub\s*$", stripped, re.IGNORECASE):
                # Emit constructor
                if ctor_base_args is not None:
                    out.append(f'    public {class_name}() : base({ctor_base_args}) {{ }}')
                else:
                    out.append(f'    public {class_name}() {{ }}')
                out.append('')
                state = IN_NONE
            continue

        # End Sub / End Function
        if (re.match(r"End\s+Sub\s*$", stripped, re.IGNORECASE) or
                re.match(r"End\s+Function\s*$", stripped, re.IGNORECASE)):
            if state == IN_METHOD:
                out.append(f'    {method_header}')
                out.append('    {')
                for ml in method_body:
                    out.append(ml)
                out.append('    }')
                out.append('')
                method_header = ''
                method_body = []
            state = IN_NONE
            continue

        # Method signature
        sig = parse_method_sig(stripped)
        if sig is not None:
            state = IN_METHOD
            method_header = ''
            method_body = []
            _select_stack.clear()
            cm._with_target = ''
            mname, params, ret, vis = sig
            param_str = ', '.join(f'{t} {n}' for t, n in params)
            method_header = f'{vis} {ret} {mname}({param_str})'
            continue

        if state == IN_METHOD:
            if not stripped:
                method_body.append('')
                continue
            vb_spaces = len(raw) - len(raw.lstrip())
            indent = ' ' * max(8, vb_spaces - 4)
            method_body.append(convert_body_line(stripped, indent))
            continue

    out.append('}')
    return _post_process('\n'.join(out) + '\n')


def main() -> None:
    parser = argparse.ArgumentParser(description='Convert VB Ability subclass to C#')
    parser.add_argument('input', help='Input .vb file or directory')
    parser.add_argument('output', nargs='?', help='Output .cs file or directory')
    parser.add_argument('--dir', action='store_true', help='Process directory')
    args = parser.parse_args()

    skip = {'Ability.vb'}  # Base class already ported

    if args.dir or os.path.isdir(args.input):
        in_dir = args.input
        out_dir = args.output or in_dir
        os.makedirs(out_dir, exist_ok=True)
        count = 0
        for fname in sorted(os.listdir(in_dir)):
            if not fname.endswith('.vb') or fname in skip:
                continue
            in_path = os.path.join(in_dir, fname)
            out_path = os.path.join(out_dir, fname.replace('.vb', '.cs'))
            vb_text = read_vb_file(in_path)
            cs_text = convert_ability_file(vb_text, in_path)
            with open(out_path, 'w', encoding='utf-8') as f:
                f.write(cs_text)
            count += 1
        print(f'Converted {count} ability files → {out_dir}', file=sys.stderr)
    else:
        vb_text = read_vb_file(args.input)
        cs_text = convert_ability_file(vb_text, args.input)
        if args.output:
            with open(args.output, 'w', encoding='utf-8') as f:
                f.write(cs_text)
        else:
            print(cs_text, end='')


if __name__ == '__main__':
    main()
