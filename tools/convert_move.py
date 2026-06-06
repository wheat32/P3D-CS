#!/usr/bin/env python3
"""
Convert a VB.NET Pokemon3D Attack-move file to C# (.NET 10).
Follows AGENTS.md conventions for the P3D C# port.

Usage:
    python3 convert_move.py SomeMove.vb > SomeMove.cs
    python3 convert_move.py SomeMove.vb SomeMove.cs
    python3 convert_move.py --dir /path/to/type/dir /output/dir
"""

import re
import sys
import os
import argparse

# ---------------------------------------------------------------------------
# Field maps: VB PascalCase name → C# name (properties stay PascalCase;
# public fields become camelCase per AGENTS.md)
# ---------------------------------------------------------------------------
FIELD_MAP = {
    # Properties (PascalCase stays)
    'ID': 'ID',
    'Power': 'Power',
    'Accuracy': 'Accuracy',
    'Name': 'Name',
    'Description': 'Description',
    # Public fields → camelCase
    'Type': 'type',
    'OriginalPP': 'originalPP',
    'CurrentPP': 'currentPP',
    'MaxPP': 'maxPP',
    'Category': 'category',
    'ContestCategory': 'contestCategory',
    'CriticalChance': 'criticalChance',
    'IsHMMove': 'isHMMove',
    'Target': 'target',
    'Priority': 'priority',
    'TimesToAttack': 'timesToAttack',
    'HasSecondaryEffect': 'hasSecondaryEffect',
    'IsHealingMove': 'isHealingMove',
    'IsDamagingMove': 'isDamagingMove',
    'IsProtectMove': 'isProtectMove',
    'IsOneHitKOMove': 'isOneHitKOMove',
    'IsRecoilMove': 'isRecoilMove',
    'IsTrappingMove': 'isTrappingMove',
    'RemovesOwnFrozen': 'removesSelfFrozen',
    'RemovesOppFrozen': 'removesOpponentFrozen',
    'SwapsOutOwnPokemon': 'swapsOutSelfPokemon',
    'SwapsOutOppPokemon': 'swapsOutOpponentPokemon',
    'ProtectAffected': 'protectAffected',
    'MagicCoatAffected': 'magicCoatAffected',
    'SnatchAffected': 'snatchAffected',
    'MirrorMoveAffected': 'mirrorMoveAffected',
    'KingsrockAffected': 'kingsrockAffected',
    'CounterAffected': 'counterAffected',
    'IsAffectedBySubstitute': 'isAffectedBySubstitute',
    'ImmunityAffected': 'immunityAffected',
    'IsWonderGuardAffected': 'isWonderGuardAffected',
    'DisabledWhileGravity': 'disabledWhileGravity',
    'UseEffectiveness': 'useEffectiveness',
    'UseAccEvasion': 'useAccEvasion',
    'CanHitInMidAir': 'canHitInMidAir',
    'CanHitUnderground': 'canHitUnderground',
    'CanHitUnderwater': 'canHitUnderwater',
    'CanHitSleeping': 'canHitSleeping',
    'CanGainSTAB': 'canGainSTAB',
    'UseOppDefense': 'useOpponentDefense',
    'UseOppEvasion': 'useOpponentEvasion',
    'MakesContact': 'makesContact',
    'IsPulseMove': 'isPulseMove',
    'IsBulletMove': 'isBulletMove',
    'IsJawMove': 'isJawMove',
    'IsDanceMove': 'isDanceMove',
    'IsExplosiveMove': 'isExplosiveMove',
    'IsPowderMove': 'isPowderMove',
    'IsPunchingMove': 'isPunchingMove',
    'IsSlicingMove': 'isSlicingMove',
    'IsSoundMove': 'isSoundMove',
    'IsWindMove': 'isWindMove',
    'FocusOppPokemon': 'focusOpponentPokemon',
    'AIField1': 'aiField1',
    'AIField2': 'aiField2',
    'AIField3': 'aiField3',
    'EffectChances': 'effectChances',
}

# Override method return types (VB method name → C# return type)
METHOD_RETURNS: dict[str, str] = {
    'MoveFailBeforeAttack': 'bool',
    'GetUseAccEvasion': 'bool',
    'GetBasePower': 'int',
    'GetDamage': 'int',
    'GetTimesToAttack': 'int',
    'DeductPP': 'bool',
    'GetAccuracy': 'int',
    'GetAttackType': 'Element',
    'GetUseAttackStat': 'int',
    'GetUseDefenseStat': 'int',
    'AIUseMove': 'bool',
}

# VB type name → C# type name
TYPE_MAP: dict[str, str] = {
    'Boolean': 'bool',
    'Integer': 'int',
    'Single': 'float',
    'Double': 'double',
    'String': 'String',
    'Object': 'Object',
    'Pokemon': 'Pokemon',
    'Attack': 'Attack',
    'Element': 'Element',
    'BattleScreen': 'BattleScreen',
    'AnimationQueryObject': 'AnimationQueryObject',
    'NPC': 'NPC',
}

# VB parameter name → C# camelCase name
PARAM_RENAME: dict[str, str] = {
    'Own': 'own',
    'BattleScreen': 'battleScreen',
    'BattleFlip': 'battleFlip',
    'CurrentPokemon': 'currentPokemon',
    'CurrentEntity': 'currentEntity',
    # Method-specific parameters converted to camelCase
    'Critical': 'critical',
    'TargetPokemon': 'targetPokemon',
    'ExtraParameter': 'extraParameter',
    'TypeEffectivenessAttack': 'typeEffectivenessAttack',
}


# ---------------------------------------------------------------------------
# Expression / value converters
# ---------------------------------------------------------------------------

_ENUM_PREFIXES_PAT = re.compile(
    r'\b(Targets|Categories|ContestCategories|AIField|StatusProblems'
    r'|WeatherTypes|BattleEntities|InputModes)\.([a-z]\w*)')


def _capitalize_enum_member(m: re.Match) -> str:
    return f'{m.group(1)}.{m.group(2)[0].upper()}{m.group(2)[1:]}'


def convert_value(v: str) -> str:
    """Convert a simple VB value (RHS of constructor assignment) to C#."""
    v = v.strip()
    v = re.sub(r'\bTrue\b', 'true', v)
    v = re.sub(r'\bFalse\b', 'false', v)
    # Nothing → null, but NOT after a dot (e.g. AIField.Nothing stays)
    v = re.sub(r'(?<!\.)\bNothing\b', 'null', v)
    # Capitalize enum member names (VB is case-insensitive; C# is not)
    v = _ENUM_PREFIXES_PAT.sub(_capitalize_enum_member, v)
    return v


# Module-level stack tracking nested Select Case → switch blocks.
_select_stack: list[dict] = []

# Current With-block target (or '' when not inside a With block).
_with_target: str = ''


def _wrap_math_double(expr: str) -> str:
    """Wrap Math.Ceiling/Floor/Round call arguments with (double)(...) to resolve overload ambiguity."""
    result = []
    i = 0
    n = len(expr)
    pat = re.compile(r'\bMath\.(Ceiling|Floor|Round)\(', re.IGNORECASE)
    while i < n:
        m = pat.search(expr, i)
        if m is None:
            result.append(expr[i:])
            break
        result.append(expr[i:m.start()])
        fn = m.group(1)
        # Find the matching closing paren of the call (depth 1 at this point)
        depth = 1
        j = m.end()
        while j < n and depth > 0:
            if expr[j] == '(':
                depth += 1
            elif expr[j] == ')':
                depth -= 1
            j += 1
        # expr[m.end() : j-1] is the argument content; expr[j-1] is ')'
        arg = expr[m.end():j - 1]
        # Check if already wrapped with (double)
        if arg.startswith('(double)'):
            result.append(f'Math.{fn}({arg})')
        else:
            result.append(f'Math.{fn}((double)({arg}))')
        i = j
    return ''.join(result)


def read_vb_file(path: str) -> str:
    """Read a VB source file handling UTF-8, UTF-16, and Latin-1 encodings."""
    for enc in ('utf-8-sig', 'utf-16', 'latin-1'):
        try:
            with open(path, 'r', encoding=enc) as f:
                return f.read()
        except (UnicodeDecodeError, UnicodeError):
            continue
    raise IOError(f'Could not decode {path}')


def strip_vb_comment(line: str) -> tuple[str, str]:
    """Strip trailing VB comment (') from a line, respecting string literals.
    Returns (code_part, comment_text).
    """
    in_string = False
    for i, ch in enumerate(line):
        if ch == '"':
            in_string = not in_string
        elif ch == "'" and not in_string:
            return line[:i].rstrip(), line[i + 1:].strip()
    return line, ''


def fix_string_literals(expr: str) -> str:
    """Convert VB string literals that contain raw backslashes to C# verbatim strings."""
    # Replace "..." containing \ with @"..." so C# doesn't see invalid escape sequences.
    # VB strings use \ as a literal character; C# requires @"" or \\ escaping.
    def replace_lit(m: re.Match) -> str:
        content = m.group(1)
        # Only convert if backslash is present
        if '\\' in content:
            return f'@"{content}"'
        return f'"{content}"'
    return re.sub(r'"([^"]*)"', replace_lit, expr)


def convert_expr(expr: str) -> str:
    """Convert a VB expression to its C# equivalent."""
    expr = expr.strip()

    # ---- Resolve With-block shorthand ----
    # Replace ALL .Member occurrences not preceded by a word/digit char with with_target.Member.
    # This handles both leading-dot and mid-expression-dot shorthand in a single pass.
    if _with_target:
        expr = re.sub(r'(?<![A-Za-z0-9_])\.([\w]+)', f'{_with_target}.\\1', expr)

    # ---- Fix string literals with backslashes first ----
    expr = fix_string_literals(expr)

    # ---- CType(x, T) → (T)x ----
    expr = re.sub(r'\bCType\(([^,]+),\s*([\w\.]+)\)',
                  lambda m: f'({m.group(2).strip()}){m.group(1).strip()}',
                  expr, flags=re.IGNORECASE)

    # ---- Casts ----
    expr = re.sub(r'\bCInt\(', '(int)(', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bCDbl\(', '(double)(', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bCSng\(', '(float)(', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bCBool\(', '(bool)(', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bCStr\(([^)]+)\)', r'\1.ToString()', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bDirectCast\(([^,]+),\s*(\w+)\)',
                  lambda m: f'({m.group(2).strip()}){m.group(1).strip()}',
                  expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bTryCast\(([^,]+),\s*(\w+)\)',
                  lambda m: f'{m.group(1).strip()} as {m.group(2).strip()}',
                  expr, flags=re.IGNORECASE)

    # ---- VB generic "New List(Of T)" → "new List<T>()" ----
    expr = re.sub(r'\bNew\s+List\s*\(Of\s+([\w\.]+)\)',
                  lambda m: f'new List<{map_type(m.group(1))}>()',
                  expr, flags=re.IGNORECASE)
    # Fix any remaining "new List<T>" without parens (e.g. from other paths)
    expr = re.sub(r'\bnew\s+List<([\w<>]+)>(?!\s*[\(\[\{(])', r'new List<\1>()', expr)

    # ---- VB Mod → % (modulo operator) ----
    expr = re.sub(r'\s+Mod\s+', ' % ', expr)

    # ---- Threading.X → System.Threading.X ----
    expr = re.sub(r'\bThreading\.', 'System.Threading.', expr)

    # ---- VB array/list initializer "{a, b}" → C# collection expression "[]" ----
    # {a, b, c}.ToList() → [a, b, c]  (VB list initializer via array-to-list)
    expr = re.sub(r'\{([^{}]+)\}\.ToList\(\)', r'[\1]', expr, flags=re.IGNORECASE)
    # {a, b, c} as standalone → [a, b, c] (collection expression)
    expr = re.sub(r'^\{(.+)\}$', r'[\1]', expr)

    # ---- New keyword ----
    expr = re.sub(r'\bNew\b', 'new', expr)

    # ---- "Not X Is Nothing" → "X != null" (must come before general Not and Is Nothing rules) ----
    expr = re.sub(r'\bNot\s+([\w\.]+(?:\([^)]*\))?)\s+Is\s+Nothing\b',
                  lambda m: f'{m.group(1)} != null',
                  expr, flags=re.IGNORECASE)

    # ---- "Not X Is Y" (Y ≠ Nothing) → "X != Y"  (reference inequality) ----
    expr = re.sub(r'\bNot\s+([\w\.]+)\s+Is\s+(?!Nothing\b)([\w\.]+)\b',
                  lambda m: f'{m.group(1)} != {m.group(2)}',
                  expr, flags=re.IGNORECASE)

    # ---- Null checks ----
    expr = re.sub(r'\bIs\s+Nothing\b', '== null', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bIsNot\s+Nothing\b', '!= null', expr, flags=re.IGNORECASE)

    # ---- "X Is Y" (Y ≠ Nothing) → "X == Y"  (reference equality) ----
    expr = re.sub(r'\b([\w\.]+)\s+Is\s+(?!Nothing\b)([\w\.]+)\b',
                  lambda m: f'{m.group(1)} == {m.group(2)}',
                  expr, flags=re.IGNORECASE)

    # ---- Nothing → null (not after dot) ----
    expr = re.sub(r'(?<!\.)\bNothing\b', 'null', expr)

    # ---- Not X → X == false  (captures optional method call parens to avoid stranding them) ----
    expr = re.sub(r'\bNot\s+([\w\.]+(?:\([^)]*\))?)\b',
                  lambda m: f'{m.group(1)} == false',
                  expr)

    # ---- IsEgg() → IsEgg (property, not method) ----
    expr = re.sub(r'\.IsEgg\(\)', '.IsEgg', expr)
    expr = re.sub(r'\.IsEgg\b(?!\()', '.IsEgg', expr)

    # ---- Boolean literals ----
    expr = re.sub(r'\bTrue\b', 'true', expr)
    expr = re.sub(r'\bFalse\b', 'false', expr)

    # ---- Comparison operators ----
    expr = expr.replace('<>', '!=')
    # = True  / = False  (not already doubled as ==)
    expr = re.sub(r'(?<!=)=\s*true\b', ' == true', expr)
    expr = re.sub(r'(?<!=)=\s*false\b', ' == false', expr)
    # Standalone = in non-assignment context (e.g. in conditions after If)
    # NOTE: we handle this in convert_condition; skip here to avoid breaking assignments

    # ---- Logical operators ----
    expr = re.sub(r'\bAndAlso\b', '&&', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bOrElse\b', '||', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bAnd\b', '&&', expr, flags=re.IGNORECASE)
    expr = re.sub(r'\bOr\b', '||', expr, flags=re.IGNORECASE)

    # ---- String concat & → + ----
    # Simple approach: replace & with + (covers 95% of cases)
    if ' & ' in expr:
        expr = expr.replace(' & ', ' + ')

    # ---- MyBase → base ----
    expr = re.sub(r'\bMyBase\b', 'base', expr)

    # ---- Math.Ceiling / Math.Floor: wrap args in (double)(...) to resolve ambiguity ----
    expr = _wrap_math_double(expr)

    # ---- .ToString without parens → .ToString() ----
    expr = re.sub(r'\.ToString(?!\s*[\(\[])', '.ToString()', expr)

    # ---- Core.Random.Next / Random.Next → Core.Random.Next ----
    expr = re.sub(r'(?<!\.)(?<!Core\.)\bRandom\.Next\b', 'Core.Random.Next', expr)
    expr = re.sub(r'(?<!\.)(?<!Core\.)\bRandom\.NextDouble\b', 'Core.Random.NextDouble', expr)

    # ---- Me.Field → apply FIELD_MAP ----
    def replace_me_field(m: re.Match) -> str:
        fname = m.group(1)
        return FIELD_MAP.get(fname, fname)

    expr = re.sub(r'\bMe\.(\w+)', replace_me_field, expr)

    # ---- BattleScreen. / Battlescreen. → battleScreen. (case-insensitive rename) ----
    expr = re.sub(r'\bBattleScreen\.', 'battleScreen.', expr, flags=re.IGNORECASE)
    # ---- Standalone BattleScreen → battleScreen ----
    expr = re.sub(r'\bBattleScreen\b(?!\.)', 'battleScreen', expr, flags=re.IGNORECASE)

    # ---- Parameter name renames (Own → own, etc.) ----
    for vb_name, cs_name in PARAM_RENAME.items():
        if vb_name == 'BattleScreen':
            continue  # already handled above
        expr = re.sub(rf'\b{vb_name}\b', cs_name, expr)

    # ---- Bare Me → this ----
    expr = re.sub(r'\bMe\b(?!\.)', 'this', expr)

    # ---- VB collection indexer: word(var) → word[var]
    # Applies for single loop-index letters/digits (not p, m, x, z which are object vars)
    expr = re.sub(r'\b([A-Za-z_]\w*)\(([ijk0-9nsa])\)', r'\1[\2]', expr)
    # Fix false-positive: "new Type[i]" → "new Type(i)" (constructor call, not indexer)
    expr = re.sub(r'\bnew\s+(\w+)\[([ijk0-9nsa])\]', r'new \1(\2)', expr)

    # ---- Fix .count → .Count, .length → .Length (C# is case-sensitive) ----
    expr = re.sub(r'\.(count|length|tostring)\b',
                  lambda m: '.' + m.group(1).capitalize(), expr)

    # ---- Remove multiple commas from optional VB params (,,  ,,,) ----
    # Apply repeatedly until no more adjacent commas remain
    while re.search(r',\s*,', expr):
        expr = re.sub(r',\s*,', ', ', expr)

    # ---- Enum member capitalization: first char uppercase after dot (best effort) ----
    # Handles VB case-insensitive enum access like Targets.self → Targets.Self
    # Only applies to known enum type prefixes to avoid false positives
    ENUM_PREFIXES = r'(?:Targets|Categories|ContestCategories|AIField|StatusProblems|Element\.Types|WeatherTypes|BattleEntities|InputModes)'
    def capitalize_enum_member(m: re.Match) -> str:
        prefix = m.group(1)
        member = m.group(2)
        return f'{prefix}.{member[0].upper()}{member[1:]}'
    expr = re.sub(rf'\b({ENUM_PREFIXES})\.([a-z]\w*)',
                  capitalize_enum_member, expr)

    # ---- Rename Attack public field accesses: .VbName → .csName (e.g. .CounterAffected → .counterAffected) ----
    # Only applies to fields that won't cause false-positive renames on other class members.
    # Excluded: Type (Element.Type is PascalCase), and properties (ID, Power, Name, Accuracy, Description).
    # Skip field renames for names that are PascalCase properties on other classes too
    # (e.g., Type is Element.Type PascalCase; ID, Power, Name, Accuracy stay PascalCase on Attack)
    _SKIP_METHOD_RENAME = {'Type', 'ID', 'Power', 'Accuracy', 'Name', 'Description',
                           'Category', 'ContestCategory', 'Target', 'Priority',
                           'TimesToAttack', 'EffectChances', 'AIField1', 'AIField2', 'AIField3'}
    for _vb, _cs in FIELD_MAP.items():
        if _vb != _cs and _vb not in _SKIP_METHOD_RENAME:
            expr = re.sub(rf'\.{re.escape(_vb)}\b', f'.{_cs}', expr)

    # ---- Fix double-literal to float in new Vector3(...) constructors ----
    def _fix_vec3_floats(m: re.Match) -> str:
        args = m.group(1)
        # Add 'f' suffix to bare decimal literals (no existing suffix)
        args = re.sub(r'(?<!\w)(-?\d+\.\d+)(?![fFdD\d])', r'\1f', args)
        return f'new Vector3({args})'
    expr = re.sub(r'\bnew Vector3\(([^)]+)\)', _fix_vec3_floats, expr)

    # ---- Fix double-space artifacts ----
    expr = re.sub(r'  +', ' ', expr).strip()

    return expr


def convert_condition(cond: str) -> str:
    """Convert a VB boolean condition expression to C#."""
    cond = cond.strip()

    # "Not X Is Nothing" → "X != null"  (must come before = True/False and Is Nothing rules)
    cond = re.sub(r'\bNot\s+([\w\.]+(?:\([^)]*\))?)\s+Is\s+Nothing\b',
                  lambda m: f'{m.group(1)} != null',
                  cond, flags=re.IGNORECASE)
    # "Not X Is Y" (Y ≠ Nothing) → "X != Y"
    cond = re.sub(r'\bNot\s+([\w\.]+)\s+Is\s+(?!Nothing\b)([\w\.]+)\b',
                  lambda m: f'{m.group(1)} != {m.group(2)}',
                  cond, flags=re.IGNORECASE)
    # "X Is Y" (Y ≠ Nothing) → "X == Y"
    cond = re.sub(r'\b([\w\.]+)\s+Is\s+(?!Nothing\b)([\w\.]+)\b',
                  lambda m: f'{m.group(1)} == {m.group(2)}',
                  cond, flags=re.IGNORECASE)

    # = True / = False comparisons
    cond = re.sub(r'(?<!=)=\s*True\b', ' == true', cond, flags=re.IGNORECASE)
    cond = re.sub(r'(?<!=)=\s*False\b', ' == false', cond, flags=re.IGNORECASE)

    # Equality: standalone = (not already ==, not >=, not <=, not !=)
    # Insert == for bare = comparisons (risky, but needed)
    cond = re.sub(r'(?<![<>!=])=(?!=)', '==', cond)

    # Apply general expression conversions
    return convert_expr(cond)


# ---------------------------------------------------------------------------
# Type mapper
# ---------------------------------------------------------------------------

def map_type(vb_type: str) -> str:
    vb_type = vb_type.strip()
    if vb_type in TYPE_MAP:
        return TYPE_MAP[vb_type]
    m = re.match(r"List\(Of\s+(\w+)\)", vb_type, re.IGNORECASE)
    if m:
        inner = map_type(m.group(1))
        return f'List<{inner}>'
    return vb_type


def default_val(vb_type: str) -> str:
    return {'Integer': '0', 'Boolean': 'false', 'Single': '0f',
            'Double': '0.0', 'String': '""'}.get(vb_type, 'null')


# ---------------------------------------------------------------------------
# Constructor-line converter
# ---------------------------------------------------------------------------

def convert_ctor_line(stripped: str, raw_indent: str) -> str | None:
    """Convert one line from the constructor. Returns None if unrecognized."""

    if not stripped:
        return ''

    # Section comments '#Definitions → // #Definitions
    if stripped.startswith("'"):
        return f"{raw_indent}// {stripped[1:].lstrip()}"

    # Strip trailing inline VB comments (e.g. Me.IsWindMove = True 'P3D only)
    stripped, trailing_comment = strip_vb_comment(stripped)
    stripped = stripped.strip()
    if not stripped:
        return f"{raw_indent}// {trailing_comment}" if trailing_comment else ''

    # Me.EffectChances.Add(N)  or  EffectChances.Add(N)  (some files omit Me.)
    m = re.match(r"(?:Me\.)?EffectChances\.Add\((.+)\)\s*$", stripped, re.IGNORECASE)
    if m:
        return f"{raw_indent}effectChances.Add({m.group(1).strip()});"

    # Me.Type = New Element(Element.Types.X)
    m = re.match(r"Me\.Type\s*=\s*New Element\(Element\.Types\.(\w+)\)\s*$",
                 stripped, re.IGNORECASE)
    if m:
        return f"{raw_indent}type = new Element(Element.Types.{m.group(1)});"

    # Me.Name = Localization.GetString("move_name_" & Me.ID, "Default")
    m = re.match(r'Me\.Name\s*=\s*Localization\.GetString\("move_name_"\s*&\s*Me\.ID\s*,\s*"([^"]*)"\)\s*$',
                 stripped, re.IGNORECASE)
    if m:
        return f'{raw_indent}Name = Localization.GetString($"move_name_{{ID}}", "{m.group(1)}");'

    # Generic Me.Field = Value
    m = re.match(r"Me\.(\w+)\s*=\s*(.+)$", stripped)
    if m:
        vb_field = m.group(1)
        value = m.group(2).strip()
        cs_field = FIELD_MAP.get(vb_field)
        if cs_field is None:
            return f"{raw_indent}// TODO: Me.{vb_field} = {value};"
        value = convert_value(value)
        return f"{raw_indent}{cs_field} = {value};"

    return None


# ---------------------------------------------------------------------------
# Method-body line converter (one VB line → one or more C# lines)
# ---------------------------------------------------------------------------

def convert_body_line(stripped: str, indent: str) -> str:
    """Convert a single stripped VB method-body line to C# (possibly multi-line)."""

    if not stripped:
        return ''

    # Comments
    if stripped.startswith("'"):
        return f"{indent}// {stripped[1:].lstrip()}"

    # Strip trailing VB inline comments and capture them
    stripped, trailing_comment = strip_vb_comment(stripped)
    stripped = stripped.strip()
    if not stripped:
        return f"{indent}// {trailing_comment}" if trailing_comment else ''
    comment_suffix = f'  // {trailing_comment}' if trailing_comment else ''

    global _with_target

    # ----- With / End With -----
    m = re.match(r"With\s+(.+)$", stripped, re.IGNORECASE)
    if m:
        _with_target = convert_expr(m.group(1).strip())
        return ''   # With statement itself has no C# equivalent; target tracked in state

    if re.match(r"End\s+With\s*$", stripped, re.IGNORECASE):
        _with_target = ''
        return ''

    # ----- With-block shorthand: .Member = value  or  expression starting with . -----
    if stripped.startswith('.') and _with_target:
        full = _with_target + stripped  # e.g. "battleScreen.FieldEffects.OwnBind = 0"
        return convert_body_line(full, indent)

    # ----- Select Case -----
    # Module-level _select_stack tracks nested switch blocks.
    # Each entry: (base_indent, need_break) — need_break = whether current case needs a break.
    global _select_stack

    # Select Case expr → switch (expr) {
    m = re.match(r"Select\s+Case\s+(.+)$", stripped, re.IGNORECASE)
    if m:
        _select_stack.append({'indent': indent, 'need_break': False})
        return f"{indent}switch ({convert_expr(m.group(1))})\n{indent}{{"

    # Case X [, Y ...]  or  Case Else
    m = re.match(r"Case\s+(.+)$", stripped, re.IGNORECASE)
    if m and _select_stack:
        st = _select_stack[-1]
        body_indent = indent + '    '  # body sits one level deeper than label
        prefix = ''
        if st['need_break']:
            prev_body = st.get('body_indent', body_indent)
            prefix = f"{prev_body}break;\n"
        st['need_break'] = True
        st['body_indent'] = body_indent
        case_val = m.group(1).strip()
        if re.match(r"Else\s*$", case_val, re.IGNORECASE):
            return f"{prefix}{indent}default:"
        values = [v.strip() for v in case_val.split(',')]
        labels = '\n'.join(f"{indent}case {convert_expr(v)}:" for v in values)
        return f"{prefix}{labels}"

    # End Select
    if re.match(r"End\s+Select\s*$", stripped, re.IGNORECASE):
        if _select_stack:
            st = _select_stack.pop()
            body_indent = st.get('body_indent', indent + '    ')
            prefix = f"{body_indent}break;\n" if st['need_break'] else ''
            return f"{prefix}{indent}}}"
        return f"{indent}}}"

    # ----- Dim declarations -----

    # Dim x = expr  (implicit type — infer type from 'New TypeName(' if possible)
    m = re.match(r"Dim\s+(\w+)\s*=\s*(.+)$", stripped, re.IGNORECASE)
    if m:
        var, rhs = m.group(1), m.group(2)
        tm = re.match(r'(?:New|new)\s+([\w]+)\s*\(', rhs, re.IGNORECASE)
        inferred = map_type(tm.group(1)) if tm else 'Object'
        return f"{indent}{inferred} {var} = {convert_expr(rhs)};{comment_suffix}"

    # Dim x() As Type = {items}  (array)
    m = re.match(r"Dim\s+(\w+)\(\)\s+As\s+(\w+)\s*=\s*\{(.+)\}\s*$", stripped, re.IGNORECASE)
    if m:
        var, vb_type, items = m.group(1), m.group(2), m.group(3)
        cs_type = map_type(vb_type)
        return f"{indent}{cs_type}[] {var} = {{{convert_expr(items)}}};"

    # Dim x As New List(Of T)  (VB generic — type contains parentheses)
    m = re.match(r"Dim\s+(\w+)\s+As\s+New\s+List\s*\(Of\s+([\w\.]+)\)\s*$",
                 stripped, re.IGNORECASE)
    if m:
        var, inner = m.group(1), m.group(2)
        cs_inner = map_type(inner.split('.')[-1])
        return f"{indent}List<{cs_inner}> {var} = [];{comment_suffix}"

    # Dim x As New Type(args)
    m = re.match(r"Dim\s+(\w+)\s+As\s+New\s+([\w\.]+)\s*\(([^)]*)\)\s*$", stripped, re.IGNORECASE)
    if m:
        var, vb_type, args = m.group(1), m.group(2), m.group(3)
        cs_type = map_type(vb_type.split('.')[-1])
        return f"{indent}{cs_type} {var} = new {cs_type}({convert_expr(args)});{comment_suffix}"

    # Dim x As New Type  (no parens)
    m = re.match(r"Dim\s+(\w+)\s+As\s+New\s+([\w\.]+)\s*$", stripped, re.IGNORECASE)
    if m:
        var, vb_type = m.group(1), m.group(2)
        cs_type = map_type(vb_type)
        return f"{indent}{cs_type} {var} = new {cs_type}();"

    # Dim x As List(Of T) = expr
    m = re.match(r"Dim\s+(\w+)\s+As\s+List\s*\(Of\s+([\w\.]+)\)\s*=\s*(.+)$",
                 stripped, re.IGNORECASE)
    if m:
        var, inner, rhs = m.group(1), m.group(2), m.group(3)
        cs_inner = map_type(inner.split('.')[-1])
        return f"{indent}List<{cs_inner}> {var} = {convert_expr(rhs)};{comment_suffix}"

    # Dim x As List(Of T) (no init)
    m = re.match(r"Dim\s+(\w+)\s+As\s+List\s*\(Of\s+([\w\.]+)\)\s*$",
                 stripped, re.IGNORECASE)
    if m:
        var, inner = m.group(1), m.group(2)
        cs_inner = map_type(inner.split('.')[-1])
        return f"{indent}List<{cs_inner}> {var} = [];"

    # Dim x As Type = expr
    m = re.match(r"Dim\s+(\w+)\s+As\s+([\w\.]+)\s*=\s*(.+)$", stripped, re.IGNORECASE)
    if m:
        var, vb_type, expr = m.group(1), m.group(2), m.group(3)
        cs_type = map_type(vb_type.split('.')[-1])
        return f"{indent}{cs_type} {var} = {convert_expr(expr)};{comment_suffix}"

    # Dim x As Type (no init)
    m = re.match(r"Dim\s+(\w+)\s+As\s+([\w\.]+)\s*$", stripped, re.IGNORECASE)
    if m:
        var, vb_type = m.group(1), m.group(2).strip()
        cs_type = map_type(vb_type.split('.')[-1])
        dv = default_val(vb_type)
        return f"{indent}{cs_type} {var} = {dv};"

    # ----- Control flow -----

    # If ... Then (single-line: If X Then stmt)
    m = re.match(r"If\s+(.+?)\s+Then\s+(?!$)(.+)$", stripped, re.IGNORECASE)
    if m:
        cond, body = m.group(1), m.group(2).strip()
        cs_cond = convert_condition(cond)
        inner = convert_body_line(body, indent + '    ')
        return f"{indent}if ({cs_cond})\n{indent}{{\n{inner}\n{indent}}}"

    # If ... Then (block start)
    m = re.match(r"If\s+(.+?)\s+Then\s*$", stripped, re.IGNORECASE)
    if m:
        cs_cond = convert_condition(m.group(1))
        return f"{indent}if ({cs_cond})\n{indent}{{"

    # ElseIf ... Then  or  Else If ... Then (with space)
    m = re.match(r"Else\s*If\s+(.+?)\s+Then\s*$", stripped, re.IGNORECASE)
    if m:
        cs_cond = convert_condition(m.group(1))
        return f"{indent}}}\n{indent}else if ({cs_cond})\n{indent}{{"

    # Else
    if re.match(r"Else\s*$", stripped, re.IGNORECASE):
        return f"{indent}}}\n{indent}else\n{indent}{{"

    # End If
    if re.match(r"End\s+If\s*$", stripped, re.IGNORECASE):
        return f"{indent}}}"

    # While ... (block start)
    m = re.match(r"While\s+(.+)$", stripped, re.IGNORECASE)
    if m:
        cs_cond = convert_condition(m.group(1))
        return f"{indent}while ({cs_cond})\n{indent}{{"

    # End While
    if re.match(r"End\s+While\s*$", stripped, re.IGNORECASE):
        return f"{indent}}}"

    # For Each x As Qualified.Type In collection  (type name may contain dots)
    m = re.match(r"For\s+Each\s+(\w+)\s+As\s+([\w\.]+(?:\(Of\s+[\w\.]+\))?)\s+In\s+(.+)$",
                 stripped, re.IGNORECASE)
    if m:
        var, vb_type, coll = m.group(1), m.group(2), m.group(3)
        # Strip namespace prefix from type if needed
        cs_type = map_type(vb_type.split('.')[-1])
        return f"{indent}foreach ({cs_type} {var} in {convert_expr(coll)})\n{indent}{{"

    # For Each x In collection (no type)
    m = re.match(r"For\s+Each\s+(\w+)\s+In\s+(.+)$", stripped, re.IGNORECASE)
    if m:
        var, coll = m.group(1), m.group(2)
        return f"{indent}foreach (var {var} in {convert_expr(coll)})\n{indent}{{"

    # For i As Type = start To end [Step N]  (typed loop variable)
    m = re.match(r"For\s+(\w+)\s+As\s+\w+\s*=\s*(.+?)\s+To\s+(.+?)(?:\s+Step\s+(.+))?$",
                 stripped, re.IGNORECASE)
    if m:
        var, start, end, step = m.group(1), m.group(2), m.group(3), m.group(4)
        cs_start = convert_expr(start)
        cs_end = convert_expr(end)
        if step and step.strip() == '-1':
            inc = f'{var}--'
            cmp = '>='
        else:
            step_val = step.strip() if step else '1'
            inc = f'{var}++' if step_val == '1' else f'{var} += {convert_expr(step_val)}'
            cmp = '<='
        return f"{indent}for (int {var} = {cs_start}; {var} {cmp} {cs_end}; {inc})\n{indent}{{"

    # For i = start To end [Step N]
    m = re.match(r"For\s+(\w+)\s*=\s*(.+?)\s+To\s+(.+?)(?:\s+Step\s+(.+))?$", stripped, re.IGNORECASE)
    if m:
        var, start, end, step = m.group(1), m.group(2), m.group(3), m.group(4)
        cs_start = convert_expr(start)
        cs_end = convert_expr(end)
        if step and step.strip() == '-1':
            inc = f'{var}--'
            cmp = '>='
        else:
            step_val = step.strip() if step else '1'
            inc = f'{var}++' if step_val == '1' else f'{var} += {convert_expr(step_val)}'
            cmp = '<='
        return f"{indent}for (int {var} = {cs_start}; {var} {cmp} {cs_end}; {inc})\n{indent}{{"

    # Next
    if re.match(r"Next\s*$", stripped, re.IGNORECASE):
        return f"{indent}}}"

    # ----- VB Exit statements → C# break / return -----
    m = re.match(r"Exit\s+For\s*$", stripped, re.IGNORECASE)
    if m:
        return f"{indent}break;"

    m = re.match(r"Exit\s+While\s*$", stripped, re.IGNORECASE)
    if m:
        return f"{indent}break;"

    m = re.match(r"Exit\s+(?:Sub|Function)\s*$", stripped, re.IGNORECASE)
    if m:
        return f"{indent}return;"

    # ----- Return -----
    m = re.match(r"Return\s+(.*?)\s*$", stripped, re.IGNORECASE)
    if m:
        val = m.group(1).strip()
        return f"{indent}return {convert_expr(val)};"

    if re.match(r"Return\s*$", stripped, re.IGNORECASE):
        return f"{indent}return;"

    # ----- Assignments (X = expr, where X is not a keyword) -----
    # Avoid matching control flow lines we missed
    m = re.match(r"^([A-Za-z_][\w\.\(\)\[\]]*)\s*=\s*(.+)$", stripped)
    if m:
        lhs = m.group(1)
        rhs = m.group(2)
        # Rename Me.X on LHS
        lhs = re.sub(r'\bMe\.(\w+)', lambda mm: FIELD_MAP.get(mm.group(1), mm.group(1)), lhs)
        # Rename BattleScreen on LHS
        lhs = re.sub(r'\bBattleScreen\.', 'battleScreen.', lhs)
        lhs = re.sub(r'\bBattleScreen\b(?!\.)', 'battleScreen', lhs)
        # Apply indexer conversion on LHS (Attacks(i) → Attacks[i], OriginalStats(0) → OriginalStats[0])
        lhs = re.sub(r'\b([A-Za-z_]\w*)\(([ijk0-9nsa])\)', r'\1[\2]', lhs)
        return f"{indent}{lhs} = {convert_expr(rhs)};{comment_suffix}"

    # ----- Expression statement (method call, etc.) -----
    return f"{indent}{convert_expr(stripped)};{comment_suffix}"


# ---------------------------------------------------------------------------
# Method-signature parser
# ---------------------------------------------------------------------------

VB_TYPE_TO_CS: dict[str, str] = {
    'Boolean': 'bool', 'Integer': 'int', 'Single': 'float',
    'Double': 'double', 'String': 'String',
    'BattleScreen': 'BattleScreen', 'Pokemon': 'Pokemon',
    'NPC': 'NPC', 'Element': 'Element',
}


def parse_param_list(raw: str) -> list[tuple[str, str]]:
    """Parse VB parameter list into [(cs_type, cs_name), ...]."""
    params: list[tuple[str, str]] = []
    if not raw.strip():
        return params
    for part in raw.split(','):
        part = part.strip()
        # Strip Optional, ByVal, ByRef keywords
        part = re.sub(r'\bOptional\b\s*', '', part, flags=re.IGNORECASE)
        part = re.sub(r'\bByVal\b\s*', '', part, flags=re.IGNORECASE)
        part = re.sub(r'\bByRef\b\s*', '', part, flags=re.IGNORECASE)
        part = part.strip()
        # name As Type [= default]
        m = re.match(r"(\w+)\s+As\s+(\w+)(?:\s*=\s*(.+))?$", part, re.IGNORECASE)
        if m:
            vb_name, vb_type = m.group(1), m.group(2)
            default_expr = m.group(3)
            cs_type = VB_TYPE_TO_CS.get(vb_type, vb_type)
            # Case-insensitive param rename lookup
            cs_name = next(
                (v for k, v in PARAM_RENAME.items() if k.lower() == vb_name.lower()),
                vb_name[0].lower() + vb_name[1:])
            if default_expr:
                cs_default = convert_value(default_expr.strip())
                params.append((cs_type, f'{cs_name} = {cs_default}'))
            else:
                params.append((cs_type, cs_name))
        else:
            params.append(('/* ? */', part))
    return params


def parse_method_sig(stripped: str) -> tuple | None:
    """
    Parse VB method declaration (override, shared, or private).
    Returns (method_name, params, cs_return_type, visibility_prefix) or None.
    """
    # Public Overrides Sub Name(...)
    m = re.match(
        r"Public\s+Overrides?\s+Sub\s+(\w+)\s*\(([^)]*)\)\s*$", stripped, re.IGNORECASE)
    if m:
        name, params = m.group(1), parse_param_list(m.group(2))
        ret = METHOD_RETURNS.get(name, 'void')
        return name, params, ret, 'public override'

    # Public Overrides Function Name(...) As ReturnType
    m = re.match(
        r"Public\s+Overrides?\s+Function\s+(\w+)\s*\(([^)]*)\)\s+As\s+(\w+)\s*$",
        stripped, re.IGNORECASE)
    if m:
        name, params, vb_ret = m.group(1), parse_param_list(m.group(2)), m.group(3)
        ret = VB_TYPE_TO_CS.get(vb_ret, vb_ret)
        return name, params, ret, 'public override'

    # Public Shared Function Name(...) As ReturnType  (static method)
    m = re.match(
        r"Public\s+Shared\s+Function\s+(\w+)\s*\(([^)]*)\)\s+As\s+(\w+)\s*$",
        stripped, re.IGNORECASE)
    if m:
        name, params, vb_ret = m.group(1), parse_param_list(m.group(2)), m.group(3)
        ret = VB_TYPE_TO_CS.get(vb_ret, vb_ret)
        return name, params, ret, 'public static'

    # Public Shared Sub Name(...)
    m = re.match(
        r"Public\s+Shared\s+Sub\s+(\w+)\s*\(([^)]*)\)\s*$", stripped, re.IGNORECASE)
    if m:
        name, params = m.group(1), parse_param_list(m.group(2))
        return name, params, 'void', 'public static'

    # Public Sub/Function Name(...)  (non-override, non-shared)
    m = re.match(
        r"Public\s+Function\s+(\w+)\s*\(([^)]*)\)\s+As\s+(\w+)\s*$",
        stripped, re.IGNORECASE)
    if m:
        name, params, vb_ret = m.group(1), parse_param_list(m.group(2)), m.group(3)
        ret = VB_TYPE_TO_CS.get(vb_ret, vb_ret)
        return name, params, ret, 'public'

    m = re.match(r"Public\s+Sub\s+(\w+)\s*\(([^)]*)\)\s*$", stripped, re.IGNORECASE)
    if m:
        name, params = m.group(1), parse_param_list(m.group(2))
        return name, params, 'void', 'public'

    # Private Sub Name(...)
    m = re.match(
        r"Private\s+Sub\s+(\w+)\s*\(([^)]*)\)\s*$", stripped, re.IGNORECASE)
    if m:
        name, params = m.group(1), parse_param_list(m.group(2))
        return name, params, 'void', 'private'

    # Private Function Name(...) As ReturnType
    m = re.match(
        r"Private\s+Function\s+(\w+)\s*\(([^)]*)\)\s+As\s+(\w+)\s*$",
        stripped, re.IGNORECASE)
    if m:
        name, params, vb_ret = m.group(1), parse_param_list(m.group(2)), m.group(3)
        ret = VB_TYPE_TO_CS.get(vb_ret, vb_ret)
        return name, params, ret, 'private'

    # Protected/Friend/etc. - catch-all for other visibility modifiers
    m = re.match(
        r"(?:Protected|Friend|Protected Friend)\s+(?:Overrides?\s+)?(?:Shared\s+)?Function\s+(\w+)\s*\(([^)]*)\)\s+As\s+(\w+)\s*$",
        stripped, re.IGNORECASE)
    if m:
        name, params, vb_ret = m.group(1), parse_param_list(m.group(2)), m.group(3)
        ret = VB_TYPE_TO_CS.get(vb_ret, vb_ret)
        return name, params, ret, 'protected'

    return None


# ---------------------------------------------------------------------------
# Main file converter
# ---------------------------------------------------------------------------

def convert_file(vb_text: str, source_path: str = '') -> str:
    lines = vb_text.splitlines()
    # Strip BOM
    if lines and lines[0].startswith('﻿'):
        lines[0] = lines[0][1:]

    # ---- Pass 1: extract namespace suffix and class name ----
    ns_suffix = ''
    class_name = ''
    for line in lines:
        s = line.strip()
        m = re.match(r"Namespace\s+BattleSystem\.Moves\.(\w+)", s, re.IGNORECASE)
        if m:
            ns_suffix = m.group(1)
        # Handle VB bracket-escaped keywords: Public Class [Return] → Return
        m = re.match(r"Public\s+Class\s+\[?(\w+)\]?", s, re.IGNORECASE)
        if m and class_name == '':
            class_name = m.group(1)

    if not class_name:
        return f'// ERROR: could not parse class name from {source_path}\n'

    namespace_cs = f'P3D.BattleSystem.Moves.{ns_suffix}' if ns_suffix else 'P3D.BattleSystem.Moves'

    # ---- Pass 2: state machine conversion ----
    out: list[str] = []
    out.append('using Microsoft.Xna.Framework;')
    out.append('using Microsoft.Xna.Framework.Graphics;')
    out.append('')
    out.append(f'namespace {namespace_cs};')
    out.append('')
    out.append(f'public class {class_name} : Attack')
    out.append('{')

    IN_NONE, IN_CTOR, IN_METHOD = 0, 1, 2
    state = IN_NONE

    # Accumulated method body lines (converted C# strings)
    method_header = ''
    method_body: list[str] = []

    for raw in lines:
        stripped = raw.strip()

        # --- Skip structural VB lines ---
        if (re.match(r"Namespace\s+", stripped, re.IGNORECASE) or
                re.match(r"End\s+Namespace\s*$", stripped, re.IGNORECASE) or
                re.match(r"Public\s+Class\s+\[?\w+\]?", stripped, re.IGNORECASE) or
                re.match(r"Inherits\s+Attack", stripped, re.IGNORECASE) or
                re.match(r"End\s+Class\s*$", stripped, re.IGNORECASE)):
            continue

        # --- Constructor start ---
        if re.match(r"Public\s+Sub\s+New\(\)\s*$", stripped, re.IGNORECASE):
            state = IN_CTOR
            out.append(f'    public {class_name}()')
            out.append('    {')
            continue

        # --- End Sub / End Function ---
        if (re.match(r"End\s+Sub\s*$", stripped, re.IGNORECASE) or
                re.match(r"End\s+Function\s*$", stripped, re.IGNORECASE)):
            if state == IN_CTOR:
                out.append('    }')
                out.append('')
            elif state == IN_METHOD:
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

        # --- Constructor body ---
        if state == IN_CTOR:
            if not stripped:
                out.append('')
                continue
            # VB body is indented 4 extra spaces vs C# (namespace block removed).
            vb_spaces = len(raw) - len(raw.lstrip())
            indent = ' ' * max(8, vb_spaces - 4)
            result = convert_ctor_line(stripped, indent)
            if result is not None:
                out.append(result)
            else:
                out.append(f'{indent}// TODO: {stripped}')
            continue

        # --- Method signature ---
        sig = parse_method_sig(stripped)
        if sig is not None:
            state = IN_METHOD
            method_header = ''
            method_body = []
            _select_stack.clear()
            global _with_target
            _with_target = ''
            mname, params, ret, vis = sig
            param_str = ', '.join(f'{t} {n}' for t, n in params)
            method_header = f'{vis} {ret} {mname}({param_str})'
            continue

        # --- Method body ---
        if state == IN_METHOD:
            if not stripped:
                method_body.append('')
                continue
            # VB indents body 4 extra spaces vs C# (namespace block removed).
            vb_spaces = len(raw) - len(raw.lstrip())
            indent = ' ' * max(8, vb_spaces - 4)
            cs_line = convert_body_line(stripped, indent)
            method_body.append(cs_line)
            continue

        # Handle class-level field declarations (Dim/Shared outside any method)
        # These become private (or private static) fields.
        # Handle both "Shared Dim X As Type" and "Shared X As Type" (VB allows omitting Dim)
        _NOT_A_FIELD = re.compile(
            r"(?:Public|Private|Protected|Friend|Sub|Function|Class|End)\b", re.IGNORECASE)
        m = re.match(r"(Shared\s+)?(?:Dim\s+)?(\w+)\s+As\s+([\w\.]+(?:\(Of\s+[\w\.]+\))?)\s*(?:=\s*(.+))?$",
                     stripped, re.IGNORECASE)
        if m and not _NOT_A_FIELD.match(stripped):
            is_shared = bool(m.group(1))
            var = m.group(2)
            vb_type = m.group(3).strip()
            init = m.group(4).strip() if m.group(4) else None
            cs_type = map_type(vb_type.split('.')[-1])
            static_kw = 'static ' if is_shared else ''
            if init:
                out.append(f'    private {static_kw}{cs_type} {var} = {convert_value(init)};')
            else:
                dv = default_val(vb_type)
                out.append(f'    private {static_kw}{cs_type} {var} = {dv};')
            continue

        # Skip blank lines and unrecognized lines at class level
        continue

    out.append('}')
    result = '\n'.join(out) + '\n'
    result = _post_process(result)
    return result


def _post_process(cs: str) -> str:
    """Global fixups applied after per-line conversion."""
    # Step 1: Own→Self / Opp→Opponent for PascalCase-prefixed identifiers.
    # \b matches the word boundary before Own/Opp (e.g. after '.' or space).
    cs = re.sub(r'\bOwn([A-Z])', r'Self\1', cs)
    cs = re.sub(r'\bOpp([A-Z])', r'Opponent\1', cs)
    # Step 2: Normalize StealthRock casing (VB source uses lowercase 'r').
    cs = cs.replace('SelfStealthrock', 'SelfStealthRock')
    cs = cs.replace('OpponentStealthrock', 'OpponentStealthRock')
    # Step 3: FieldEffects tuple access — .FieldEffects.SelfXxx → .FieldEffects.Xxx.Self
    #         and .FieldEffects.OpponentXxx → .FieldEffects.Xxx.Opponent
    cs = re.sub(r'\.FieldEffects\.Self([A-Z]\w*)',
                lambda m: f'.FieldEffects.{m.group(1)}.Self', cs)
    cs = re.sub(r'\.FieldEffects\.Opponent([A-Z]\w*)',
                lambda m: f'.FieldEffects.{m.group(1)}.Opponent', cs)
    # Step 4: Compound identifiers where Self/Opponent sits in the middle.
    cs = cs.replace('StolenFromOwnItems', 'StolenFromSelfItems')
    cs = cs.replace('StolenFromOppItems', 'StolenFromOpponentItems')
    cs = cs.replace('CanUseOwnItem(', 'CanUseHeldItem(')
    cs = cs.replace('SwitchOutOwn(', 'SwitchOutSelf(')
    cs = cs.replace('SwitchOutOpp(', 'SwitchOutOpponent(')
    # Step 5: Compound assignments: x = x OP y  →  x OP= y
    for op in ('/', '*', '+', '-'):
        cs = re.sub(
            r'\b(\w+) = \1 ' + re.escape(op) + r' ',
            lambda m, o=op: f'{m.group(1)} {o}= ',
            cs)
    return cs


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(description='Convert VB attack move to C#')
    parser.add_argument('input', help='Input .vb file or directory')
    parser.add_argument('output', nargs='?', help='Output .cs file or directory')
    parser.add_argument('--dir', action='store_true', help='Process whole directory')
    args = parser.parse_args()

    if args.dir or os.path.isdir(args.input):
        in_dir = args.input
        out_dir = args.output or in_dir
        os.makedirs(out_dir, exist_ok=True)
        count = 0
        for fname in sorted(os.listdir(in_dir)):
            if not fname.endswith('.vb'):
                continue
            in_path = os.path.join(in_dir, fname)
            out_path = os.path.join(out_dir, fname.replace('.vb', '.cs'))
            vb_text = read_vb_file(in_path)
            cs_text = convert_file(vb_text, in_path)
            with open(out_path, 'w', encoding='utf-8') as f:
                f.write(cs_text)
            count += 1
        print(f'Converted {count} files → {out_dir}', file=sys.stderr)
    else:
        vb_text = read_vb_file(args.input)
        cs_text = convert_file(vb_text, args.input)
        if args.output:
            with open(args.output, 'w', encoding='utf-8') as f:
                f.write(cs_text)
        else:
            print(cs_text, end='')


if __name__ == '__main__':
    main()
