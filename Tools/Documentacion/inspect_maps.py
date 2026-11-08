from pathlib import Path
import sys
sys.path.insert(0,str(Path(__file__).parent/'vendor'))
from PIL import Image, ImageOps, ImageDraw
r=Path('Afterglow_Entrega_v0_3/Reference/General/Imagenes/MapasNiveles')
o=Path('Docs/Referencias/Analisis/Mapas_N1');o.mkdir(parents=True,exist_ok=True)
files=sorted(p for p in r.iterdir() if p.suffix.lower() in ['.png','.webp','.jpg'])
sheet=Image.new('RGB',(1500,((len(files)+2)//3)*480),'#dddddd');d=ImageDraw.Draw(sheet)
for i,p in enumerate(files):
    im=Image.open(p).convert('RGB');print(i,p.name,im.size)
    preview=ImageOps.contain(im,(490,430));x=(i%3)*500;y=(i//3)*480
    sheet.paste(preview,(x,y+40));d.text((x+4,y+4),str(i)+' '+p.name[:55],fill='black');d.text((x+4,y+20),str(im.size),fill='black')
    if 'Joke' in p.name or 'woohoo' in p.name:
        im.thumbnail((900,1100));im.save(o/('Joke_overview.png' if 'Joke' in p.name else 'Woohoo_overview.png'))
sheet.save(o/'Indice_visual.jpg')
