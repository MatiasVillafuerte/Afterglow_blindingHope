"""Separa las poses de las láminas originales de enemigos sin modificarlas.

Uso (Python 3.11 con OpenCV): python -I Tools/Documentacion/Separar_Poses_Enemigos.py
Fuente: Afterglow_Entrega_v0_5/Reference/Mundo 1/Imagenes/Enemigos/*.png
Salida: Assets/Afterglow/Character/Enemigos/<Lámina>_<n>.png (de izquierda a derecha) y Poses.json con cajas y pivotes.
Las piezas cercanas (cejas, chispas, cables) se agrupan con su cuerpo mediante una dilatación del alfa.
"""
import json, sys
from pathlib import Path
import cv2, numpy as np

ROOT = Path(__file__).resolve().parents[2]
SRC = ROOT / 'Afterglow_Entrega_v0_5/Reference/Mundo 1/Imagenes/Enemigos'
OUT = ROOT / 'Assets/Afterglow/Character/Enemigos'; OUT.mkdir(parents=True, exist_ok=True)
# Distancia de agrupación por lámina (px): mayor cuando la pose tiene partes separadas.
GROUP = {'Resisworm': 45, 'Hangzzer': 70, 'Gasplode': 10, 'Lighmet': 40, 'Drilly': 60, 'PerforAudora': 60, 'PerforAudoraSinLogo': 60,
         'HangzzerSeparadoColgado': 40, 'HangzzerSeparadoSilbando': 40}
report = {}
for path in sorted(SRC.glob('*.png')):
    name = path.stem
    image = cv2.imread(str(path), cv2.IMREAD_UNCHANGED)
    alpha = (image[:, :, 3] > 12).astype(np.uint8)
    k = GROUP.get(name, 40)
    grown = cv2.dilate(alpha, np.ones((k, k), np.uint8))
    count, labels, stats, _ = cv2.connectedComponentsWithStats(grown)
    boxes = [stats[i] for i in range(1, count) if stats[i][4] > 2500]
    boxes.sort(key=lambda s: s[0])
    poses = []
    for index, (x, y, w, h, _) in enumerate(boxes):
        mask = (labels[y:y + h, x:x + w] == (list(map(tuple, stats)).index(tuple((x, y, w, h, _))))) if False else None
        crop = image[y:y + h, x:x + w].copy()
        region = grown[y:y + h, x:x + w]
        sub = labels[y:y + h, x:x + w]
        label = np.bincount(sub[region > 0].ravel()).argmax()
        crop[:, :, 3] = np.where(sub == label, crop[:, :, 3], 0)
        ys, xs = np.nonzero(crop[:, :, 3] > 12)
        crop = crop[ys.min():ys.max() + 1, xs.min():xs.max() + 1]
        pad = 6
        crop = cv2.copyMakeBorder(crop, pad, pad, pad, pad, cv2.BORDER_CONSTANT, value=(0, 0, 0, 0))
        file = OUT / f'{name}_{index}.png'
        cv2.imwrite(str(file), crop)
        poses.append({'file': file.name, 'source_box': [int(x), int(y), int(w), int(h)], 'size': [int(crop.shape[1]), int(crop.shape[0])]})
    report[name] = poses
    print(name, len(poses), [p['size'] for p in poses])
(OUT / 'Poses.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
