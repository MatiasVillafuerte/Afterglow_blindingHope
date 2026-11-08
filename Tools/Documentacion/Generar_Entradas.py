"""Genera Assets/Afterglow/Runtime/M01/M01LevelEntrances.cs con la entrada segura de cada sección (campo entry de los planos).

Uso: python -I Tools/Documentacion/Generar_Entradas.py  (después de editar Docs/Diseno/Niveles/N?_Plano.json)
"""
import json
from pathlib import Path
ROOT = Path(__file__).resolve().parents[2]
lines = []
for plan in sorted((ROOT / 'Docs/Diseno/Niveles').glob('N*_Plano.json')):
    data = json.loads(plan.read_text(encoding='utf-8'))
    for room in data['rooms']:
        x, y, z = room['entry']
        lines.append(f'            {{ "{room["id"]}", new Vector3({x}f, {y}f, {z}f) }},')
code = '''using System.Collections.Generic;
using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>Entradas seguras por sección generadas desde Docs/Diseno/Niveles (Tools/Documentacion/Generar_Entradas.py).</summary>
    public static class M01LevelEntrances
    {
        public static readonly Dictionary<string, Vector3> Entries = new Dictionary<string, Vector3>
        {
%s
        };
    }
}
''' % '\n'.join(lines)
(ROOT / 'Assets/Afterglow/Runtime/M01/M01LevelEntrances.cs').write_text(code, encoding='utf-8')
print(len(lines), 'entradas')
