"""Comprueba que cada identificador de línea usado en los planos N2–N6 y en el código exista en Guion_M01.json.

M01Dialogue.Begin rechaza identificadores desconocidos (excepción), así que una línea ausente bloquea en silencio
la investigación o el encargo que la usa. Ejecutar tras editar un plano o el guion:

    python -I Tools/Documentacion/Validar_Lineas.py
"""
import glob
import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
LINE_ID = re.compile(r"^[A-Z][A-Z0-9]*\.\d\d$")


def collect(node, found):
    if isinstance(node, dict):
        for value in node.values():
            collect(value, found)
    elif isinstance(node, list):
        for value in node:
            collect(value, found)
    elif isinstance(node, str) and LINE_ID.match(node):
        found.add(node)


def main():
    script = json.loads((ROOT / "Assets/Afterglow/Data/M01/Guion_M01.json").read_text(encoding="utf-8"))
    known = {line["id"] for line in script["lines"]}
    missing = 0
    for plan in sorted((ROOT / "Docs/Diseno/Niveles").glob("N?_Plano.json")):
        used = set()
        collect(json.loads(plan.read_text(encoding="utf-8")), used)
        absent = sorted(used - known)
        missing += len(absent)
        print(f"{plan.stem}: {len(used)} líneas, ausentes: {absent or 'ninguna'}")
    used = set()
    for source in glob.glob(str(ROOT / "Assets/Afterglow/Runtime/**/*.cs"), recursive=True):
        used |= set(re.findall(r'"([A-Z][A-Z0-9]*\.\d\d)"', Path(source).read_text(encoding="utf-8")))
    absent = sorted(i for i in used - known if not re.match(r"^Q\d\d\.", i))
    missing += len(absent)
    print(f"Código: {len(used)} referencias, ausentes: {absent or 'ninguna'}")
    return 1 if missing else 0


if __name__ == "__main__":
    sys.exit(main())
