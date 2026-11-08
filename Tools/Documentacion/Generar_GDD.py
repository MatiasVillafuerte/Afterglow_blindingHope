from pathlib import Path
import re
from docx import Document
from docx.shared import Inches,Pt,RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT,WD_CELL_VERTICAL_ALIGNMENT
from docx.oxml import OxmlElement
from docx.oxml.ns import qn

R=Path(__file__).resolve().parents[2]
src=R/'Docs/GDD/Afterglow_GDD_v0_5.md'
out=R/'Docs/GDD/Afterglow_GDD_v0_5_rev_2026_11_07.docx'
doc=Document(); s=doc.sections[0]
s.page_width=Inches(8.5);s.page_height=Inches(11)
s.left_margin=s.right_margin=Inches(.8)
s.top_margin=s.bottom_margin=Inches(.7)
s.header_distance=s.footer_distance=Inches(.28)
for name in ['Normal','Title','Subtitle','Heading 1','Heading 2']:
 st=doc.styles[name];st.font.name='Calibri';st.font.color.rgb=RGBColor(0,0,0)
 fonts=st._element.get_or_add_rPr().get_or_add_rFonts()
 for a in ['ascii','hAnsi']:fonts.set(qn('w:'+a),'Calibri')
st=doc.styles['Normal'];st.font.size=Pt(11)
st.paragraph_format.line_spacing=1.06;st.paragraph_format.space_after=Pt(5)
st.paragraph_format.widow_control=True
for name,size,before,after in [('Title',26,0,12),('Subtitle',16,0,12),('Heading 1',16,14,8),('Heading 2',12,11,6)]:
 st=doc.styles[name];st.font.size=Pt(size);st.paragraph_format.space_before=Pt(before);st.paragraph_format.space_after=Pt(after);st.paragraph_format.keep_with_next=True
 if name.startswith('Heading'):st.font.bold=True
h=s.header.paragraphs[0];h.text='AFTERGLOW BLINDING HOPE  ·  DOCUMENTO DE DISEÑO'
h.runs[0].font.size=Pt(8)
f=s.footer.paragraphs[0];f.alignment=WD_ALIGN_PARAGRAPH.RIGHT
f.add_run('GDD 0.5  ·  ').font.size=Pt(9)
e=OxmlElement('w:fldSimple');e.set(qn('w:instr'),'PAGE');f._p.append(e)
doc.core_properties.title='Afterglow Blinding Hope Documento de diseño del juego'
doc.core_properties.author='Matias Andres Villafuerte Ruiz'
doc.core_properties.subject='Historia general y producción del prólogo y mundo uno'

def rich(p,text):
 for i,part in enumerate(re.split(r'\*\*(.*?)\*\*',text)):
  run=p.add_run(part);run.bold=i%2==1

def tab(block):
 rows=[[v.strip() for v in x.strip('|').split('|')] for x in block]
 rows=[rows[0]]+rows[2:]
 n=len(rows[0]);header=rows[0]
 if n==4:widths=[1.25,2.4,2.4,.85]
 elif header[0]=='Mundo':widths=[1.3,3.5,2.1]
 elif header[0].startswith('Personaje'):widths=[1.25,2.1,3.55]
 elif header[0]=='Enemigo':widths=[1.2,1.65,4.05]
 elif header[0]=='Nivel':widths=[1.35,3.25,2.3]
 elif header[0].startswith('Fase'):widths=[.9,1.6,4.4]
 elif header[0]=='Tecla':widths=[1.15,2.15,3.6]
 else:widths=[1.5,1.15,4.25]
 t=doc.add_table(rows=0,cols=n);t.autofit=False;t.alignment=WD_TABLE_ALIGNMENT.CENTER
 for col,w in zip(t.columns,widths):col.width=Inches(w)
 b=OxmlElement('w:tblBorders')
 for k in ['top','left','bottom','right','insideH','insideV']:
  x=OxmlElement('w:'+k);x.set(qn('w:val'),'single');x.set(qn('w:sz'),'5');x.set(qn('w:color'),'D9D9D9');b.append(x)
 t._tbl.tblPr.append(b)
 m=OxmlElement('w:tblCellMar')
 for k,val in [('top','80'),('bottom','80'),('left','95'),('right','95')]:
  x=OxmlElement('w:'+k);x.set(qn('w:w'),val);x.set(qn('w:type'),'dxa');m.append(x)
 t._tbl.tblPr.append(m)
 for ri,row in enumerate(rows):
  tr=t.add_row(); pr=tr._tr.get_or_add_trPr();pr.append(OxmlElement('w:cantSplit'))
  if ri==0:pr.append(OxmlElement('w:tblHeader'))
  for ci,(c,v,w) in enumerate(zip(tr.cells,row,widths)):
   c.width=Inches(w);c.vertical_alignment=WD_CELL_VERTICAL_ALIGNMENT.CENTER
   p=c.paragraphs[0];p.paragraph_format.space_after=Pt(1);p.paragraph_format.space_before=Pt(1);p.paragraph_format.line_spacing=1.03
   if ri==0:p.paragraph_format.keep_with_next=True
   rich(p,v)
   for run in p.runs:run.font.size=Pt(10.5);run.bold=ri==0
   if ri==0:
    sh=OxmlElement('w:shd');sh.set(qn('w:fill'),'E8EBEE');c._tc.get_or_add_tcPr().append(sh)
   if n==4 and ci==3 and ri>0:p.alignment=WD_ALIGN_PARAGRAPH.CENTER
 p=doc.add_paragraph();p.paragraph_format.line_spacing=Pt(3);p.paragraph_format.space_after=Pt(1)

lines=src.read_text(encoding="utf-8").splitlines();firstsub=True;i=0
while i<len(lines):
 line=lines[i]
 if not line.strip():i+=1;continue
 if line.startswith('|'):
  block=[]
  while i<len(lines) and lines[i].startswith('|'):block.append(lines[i]);i+=1
  tab(block);continue
 if line=='MAPA_IMAGEN' or line.startswith('![Mapa conceptual]'):
  p=doc.add_paragraph();p.paragraph_format.keep_with_next=True
  p.add_run().add_picture(str(R/'Reference/General/Imagenes/Mapa_conceptual.png'),width=Inches(6.9))
 elif line.startswith('# '):doc.add_paragraph(line[2:],style='Title')
 elif line.startswith('## '):
  if firstsub:doc.add_paragraph(line[3:],style='Subtitle');firstsub=False
  else:
   p=doc.add_paragraph(line[3:],style='Heading 1')
   if line[3:].startswith('1 '):p.paragraph_format.page_break_before=True
 elif line.startswith('### '):doc.add_paragraph(line[4:],style='Heading 2')
 else:rich(doc.add_paragraph(),line)
 i+=1
for root in [doc._element,doc.styles.element]:
 for e in list(root.iter(qn('w:pBdr'))):e.getparent().remove(e)
doc.save(out)
print(out)


