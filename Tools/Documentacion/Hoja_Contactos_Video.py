"""Contact sheet with timestamps: sheet.py video start_s end_s count cols out.png [thumb_w]"""
import sys, cv2, numpy as np
video, start, end, count, cols, out = sys.argv[1], float(sys.argv[2]), float(sys.argv[3]), int(sys.argv[4]), int(sys.argv[5]), sys.argv[6]
tw = int(sys.argv[7]) if len(sys.argv) > 7 else 320
cap = cv2.VideoCapture(video)
fps = cap.get(cv2.CAP_PROP_FPS) or 30
dur = cap.get(cv2.CAP_PROP_FRAME_COUNT) / fps
if end <= 0 or end > dur: end = dur
tiles = []
for i in range(count):
    t = start + (end - start) * i / max(1, count - 1) if count > 1 else start
    cap.set(cv2.CAP_PROP_POS_MSEC, t * 1000)
    ok, frame = cap.read()
    if not ok:
        frame = np.zeros((180, 320, 3), np.uint8)
    h = int(frame.shape[0] * tw / frame.shape[1])
    frame = cv2.resize(frame, (tw, h))
    label = "%d:%02d:%02d" % (t // 3600, (t % 3600) // 60, t % 60)
    cv2.rectangle(frame, (0, 0), (92, 20), (0, 0, 0), -1)
    cv2.putText(frame, label, (3, 15), cv2.FONT_HERSHEY_SIMPLEX, .5, (255, 255, 255), 1)
    tiles.append(frame)
h = max(t.shape[0] for t in tiles)
tiles = [cv2.copyMakeBorder(t, 0, h - t.shape[0], 0, 0, cv2.BORDER_CONSTANT) for t in tiles]
while len(tiles) % cols: tiles.append(np.zeros_like(tiles[0]))
rows = [np.hstack(tiles[i:i + cols]) for i in range(0, len(tiles), cols)]
cv2.imwrite(out, np.vstack(rows))
print("duration %.0fs fps %.1f -> %s" % (dur, fps, out))
