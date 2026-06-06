#!/usr/bin/env python3
"""
Convert a VB.NET Pokemon3D Item subclass file to C# (.NET 10).
Follows AGENTS.md conventions for the P3D C# port.

Usage:
    python3 convert_item.py SomeItem.vb > SomeItem.cs
    python3 convert_item.py SomeItem.vb SomeItem.cs
    python3 convert_item.py --dir /path/to/type/dir /output/dir
"""

import re
import sys
import os
import argparse

sys.path.insert(0, os.path.dirname(__file__))
from convert_move import read_vb_file, convert_body_line, convert_expr, _post_process

# VB auto-property override type → C# type
VB_TYPE_TO_CS: dict[str, str] = {
    'Boolean': 'bool',
    'Integer': 'int',
    'Single': 'float',
    'Double': 'double',
    'String': 'String',
    'Object': 'Object',
}


def cs_type(vb_t: str) -> str:
    return VB_TYPE_TO_CS.get(vb_t, vb_t)


# Properties that have `protected set` in Item — overrides must also include the setter
_PROTECTED_SET_PROPS = {'Description', 'SortValue', 'PokeDollarPrice', 'CanBeTraded', 'CanBeTossed'}


def convert_item_file(vb_text: str, source_path: str = '') -> str:
    lines = vb_text.splitlines()
    if lines and lines[0].startswith('﻿'):
        lines[0] = lines[0][1:]

    # ---- Pass 1: extract namespace suffix, class name, parent class, attribute ----
    ns_suffix = ''
    class_name = ''
    parent_class = ''
    item_attr = ''

    for i, line in enumerate(lines):
        s = line.strip()
        # Namespace Items.Balls → suffix = Balls
        m = re.match(r"Namespace\s+Items(?:\.(\w+))?\s*$", s, re.IGNORECASE)
        if m:
            ns_suffix = m.group(1) or ''
        # <Item(id, "Name")>  or  ''' <summary> / <Item(...)>
        m = re.match(r'<Item\((\d+),\s*"([^"]*)"\)>', s, re.IGNORECASE)
        if m:
            item_attr = f'[Item({m.group(1)}, "{m.group(2)}")]'
        # Public Class Foo
        m = re.match(r"Public\s+(?:MustInherit\s+)?Class\s+(\w+)", s, re.IGNORECASE)
        if m and not class_name:
            class_name = m.group(1)
        # Inherits Bar
        m = re.match(r"Inherits\s+(\S+)", s, re.IGNORECASE)
        if m and not parent_class:
            parent_class = m.group(1)

    if not class_name:
        return f'// ERROR: could not parse class from {source_path}\n'

    namespace_cs = f'P3D.Items.{ns_suffix}' if ns_suffix else 'P3D.Items'

    out: list[str] = []
    out.append(f'using Microsoft.Xna.Framework;')
    out.append(f'using P3D.BattleSystem;')
    out.append(f'')
    out.append(f'namespace {namespace_cs};')
    out.append(f'')
    if item_attr:
        out.append(item_attr)
    out.append(f'public class {class_name} : {parent_class}')
    out.append('{')

    # ---- Pass 2: body lines ----
    IN_NONE, IN_CTOR, IN_METHOD = 0, 1, 2
    state = IN_NONE
    ctor_base = ''
    ctor_lines: list[str] = []
    method_header = ''
    method_lines: list[str] = []
    _select_stack: list = []
    _with_target = ''

    def flush_ctor():
        if ctor_base:
            out.append(f'    public {class_name}() : {ctor_base}')
        else:
            out.append(f'    public {class_name}()')
        out.append('    {')
        for cl in ctor_lines:
            out.append(cl)
        out.append('    }')
        out.append('')

    def flush_method():
        out.append(f'    {method_header}')
        out.append('    {')
        for ml in method_lines:
            out.append(ml)
        out.append('    }')
        out.append('')

    for raw in lines:
        s = raw.strip()

        # Skip structural VB
        if (re.match(r"Namespace\s+", s, re.IGNORECASE) or
                re.match(r"End\s+Namespace\s*$", s, re.IGNORECASE) or
                re.match(r"(?:Public\s+)?(?:MustInherit\s+)?Class\s+\w+", s, re.IGNORECASE) or
                re.match(r"Inherits\s+", s, re.IGNORECASE) or
                re.match(r"End\s+Class\s*$", s, re.IGNORECASE) or
                re.match(r"<Item\(", s, re.IGNORECASE) or
                re.match(r"'''", s) or
                re.match(r"Imports\s+", s, re.IGNORECASE)):
            continue

        # -- Auto-property override --
        # Public Overrides ReadOnly Property X As T = val
        m = re.match(
            r"Public\s+Overrides\s+ReadOnly\s+Property\s+(\w+)\s+As\s+([\w\.]+)(?:\s*=\s*(.+))?$",
            s, re.IGNORECASE)
        if m:
            pname, ptype, pval = m.group(1), m.group(2), m.group(3)
            cst = cs_type(ptype)
            accessor = 'get; protected set;' if pname in _PROTECTED_SET_PROPS else 'get;'
            if pval:
                val = convert_expr(pval.strip())
                out.append(f'    public override {cst} {pname} {{ {accessor} }} = {val};')
            else:
                out.append(f'    public override {cst} {pname} {{ {accessor} }}')
            continue

        # Constructor start: Public Sub New() or Public Sub New(params)
        m = re.match(r"Public\s+Sub\s+New\s*\(([^)]*)\)\s*$", s, re.IGNORECASE)
        if m:
            state = IN_CTOR
            ctor_base = ''
            ctor_lines = []
            _select_stack.clear()
            _with_target = ''
            continue

        # MyBase.New(...) inside constructor
        if state == IN_CTOR:
            mb = re.match(r"MyBase\.New\s*\((.*)\)\s*$", s, re.IGNORECASE)
            if mb:
                args = convert_expr(mb.group(1).strip())
                ctor_base = f'base({args})'
                continue

        # End Sub / End Function
        if re.match(r"End\s+(?:Sub|Function)\s*$", s, re.IGNORECASE):
            if state == IN_CTOR:
                flush_ctor()
            elif state == IN_METHOD:
                flush_method()
            state = IN_NONE
            ctor_base = ''
            ctor_lines = []
            method_header = ''
            method_lines = []
            continue

        # Method signature
        m = re.match(
            r"(?:Public\s+)?Overrides\s+(?:Sub|Function)\s+(\w+)\s*\(([^)]*)\)(?:\s+As\s+(\w+))?",
            s, re.IGNORECASE)
        if m and state == IN_NONE:
            mname = m.group(1)
            ret = cs_type(m.group(3) or 'void') if m.group(3) else 'void'
            # Build parameter list (simplified — most overrides have simple params)
            raw_params = m.group(2).strip()
            cs_params = ''
            if raw_params:
                parts = []
                for p in raw_params.split(','):
                    p = p.strip()
                    p = re.sub(r'\b(?:ByVal|ByRef|Optional)\b\s*', '', p, flags=re.IGNORECASE)
                    pm = re.match(r'(\w+)\s+As\s+([\w\.]+)', p, re.IGNORECASE)
                    if pm:
                        parts.append(f'{cs_type(pm.group(2))} {pm.group(1)}')
                cs_params = ', '.join(parts)
            vis = 'public override'
            method_header = f'{vis} {ret} {mname}({cs_params})'
            state = IN_METHOD
            method_lines = []
            _select_stack.clear()
            _with_target = ''
            continue

        # Constructor body
        if state == IN_CTOR:
            if not s:
                ctor_lines.append('')
                continue
            vb_spaces = len(raw) - len(raw.lstrip())
            indent = ' ' * max(8, vb_spaces - 4)
            # Simple assignment: field = expr
            am = re.match(r"^([A-Za-z_]\w*)\s*=\s*(.+)$", s)
            if am:
                lhs = am.group(1)
                rhs = convert_expr(am.group(2).strip())
                ctor_lines.append(f'{indent}{lhs} = {rhs};')
            else:
                # Fall through to body converter for anything complex
                ctor_lines.append(convert_body_line(s, indent))
            continue

        # Method body
        if state == IN_METHOD:
            if not s:
                method_lines.append('')
                continue
            vb_spaces = len(raw) - len(raw.lstrip())
            indent = ' ' * max(8, vb_spaces - 4)
            method_lines.append(convert_body_line(s, indent))
            continue

    out.append('}')
    result = '\n'.join(out) + '\n'
    result = _post_process(result)
    result = _item_post_process(result)
    return result


def _item_post_process(cs: str) -> str:
    """Item-specific fixups: AddressOf, VB object initializers, empty array literals."""
    # AddressOf X → X (method group reference)
    cs = re.sub(r'\bAddressOf\s+([\w\.]+)', r'\1', cs)
    # AddHandler x, y; → x += y;
    cs = re.sub(r'\bAddHandler\s+([\w\.]+),\s*([\w\.]+);', r'\1 += \2;', cs)
    # RemoveHandler x, y; → x -= y;
    cs = re.sub(r'\bRemoveHandler\s+([\w\.]+),\s*([\w\.]+);', r'\1 -= \2;', cs)
    # VB object initializer: ) With {.Prop = val, .Prop2 = val}
    # Convert to C# object initializer: ) { Prop = val, Prop2 = val }
    def convert_with_initializer(m: re.Match) -> str:
        body = m.group(1)
        # Remove leading dot from property names: .Prop → Prop
        body = re.sub(r'\.(\w+)\s*=', r'\1 =', body)
        # Fix == true/false in initializers to = true/false
        body = re.sub(r'(\w+)\s*==\s*(true|false)', r'\1 = \2', body)
        return ') {\n' + '\n'.join(f'            {part.strip()},' for part in body.split(',')) + '\n        }'
    cs = re.sub(r'\)\s*With\s*\{([^}]+)\}', convert_with_initializer, cs)
    # PluralName = Name is invalid as a field initializer (Name is a virtual property).
    # Convert to an expression-bodied property instead.
    cs = re.sub(r'\bPluralName \{[^}]+\} = Name;', 'PluralName => Name;', cs)
    # VB CBool cast → bool.Parse for string-typed sources (GetGameRuleValue returns String)
    cs = re.sub(r'\(bool\)\(GameModeManager\.GetGameRuleValue\(([^)]+)\)\)',
                lambda m: f'bool.Parse(GameModeManager.GetGameRuleValue({m.group(1)}))',
                cs)
    # VB string concat assignment: &= → +=
    cs = cs.replace(' &= ', ' += ')
    # Fix BattleScreen casing: converter lowercases it as a parameter name but
    # it needs to stay PascalCase as an enum value and a type name
    cs = cs.replace('Identifications.battleScreen', 'Identifications.BattleScreen')
    cs = re.sub(r'\(BattleSystem\.battleScreen\)', '(BattleScreen)', cs)
    cs = re.sub(r'\bBattleSystem\.battleScreen\b', 'BattleScreen', cs)
    # Screen.CurrentScreen → Core.CurrentScreen (Screen doesn't have static CurrentScreen)
    cs = cs.replace('Screen.CurrentScreen', 'Core.CurrentScreen')
    # Berry type field: Element.Types.X must be cast to int
    cs = re.sub(r'\btype = (Element\.Types\.)', r'type = (int)\1', cs)
    # Indexer pattern: fix Pokemons(PokeIndex) and similar multi-char uppercase names
    cs = re.sub(r'\b(Pokemons|Attacks|attacks)\((\w+)\)', r'\1[\2]', cs)
    # Empty VB array {} → [] in argument lists (same line only — don't touch empty method bodies)
    cs = re.sub(r'\{[ \t]*\}', '[]', cs)
    return cs


def main() -> None:
    parser = argparse.ArgumentParser(description='Convert VB Item subclass to C#')
    parser.add_argument('input', help='Input .vb file or directory')
    parser.add_argument('output', nargs='?', help='Output .cs file or directory')
    parser.add_argument('--dir', action='store_true', help='Process whole directory')
    args = parser.parse_args()

    # Base classes already ported manually — don't overwrite them
    # Also includes complex items that are stubbed (Phase 4/6 dependencies)
    skip = {
        'BallItem.vb', 'Apricorn.vb', 'KeyItem.vb', 'RepelItem.vb', 'XItem.vb',
        'Item.vb', 'Berry.vb', 'GemItem.vb', 'MedicineItem.vb', 'FossilItem.vb',
        'MailItem.vb', 'MegaStone.vb', 'PlateItem.vb', 'StoneItem.vb',
        'TechMachine.vb', 'VitaminItem.vb', 'WingItem.vb', 'ItemAttribute.vb',
        'ItemType.vb', 'GameModeItem.vb', 'GameModeItemLoader.vb',
        # Complex items with Phase 4/5/6 dependencies — stubbed manually
        'OldRod.vb', 'GoodRod.vb', 'SuperRod.vb', 'ItemFinder.vb',
        'Bicycle.vb', 'EscapeRope.vb', 'RareCandy.vb',
        # Phase 3 items with complex ability/backing-field logic
        'AbilityCapsule.vb',
        # Phase 5 battle items (XItems, PP-restore, PokeDoll)
        'DireHit.vb', 'GuardSpec.vb', 'XAccuracy.vb', 'XAttack.vb',
        'XDefend.vb', 'XSpAtk.vb', 'XSpDef.vb', 'XSpeed.vb',
        'Ether.vb', 'MaxEther.vb', 'PPUp.vb', 'PPMax.vb',
        'LeppaBerry.vb', 'PokeDoll.vb', 'PokemonDoll.vb',
    }

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
            cs_text = convert_item_file(vb_text, in_path)
            with open(out_path, 'w', encoding='utf-8') as f:
                f.write(cs_text)
            count += 1
        print(f'Converted {count} files → {out_dir}', file=sys.stderr)
    else:
        vb_text = read_vb_file(args.input)
        cs_text = convert_item_file(vb_text, args.input)
        if args.output:
            with open(args.output, 'w', encoding='utf-8') as f:
                f.write(cs_text)
        else:
            print(cs_text, end='')


if __name__ == '__main__':
    main()
