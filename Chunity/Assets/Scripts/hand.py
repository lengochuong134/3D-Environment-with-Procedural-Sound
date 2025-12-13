import cv2
import mediapipe as mp
import socket
import json
import math

mp_hands = mp.solutions.hands
hands = mp_hands.Hands(min_detection_confidence=0.7, min_tracking_confidence=0.7)

host, port = "127.0.0.1", 65433 
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

cap = cv2.VideoCapture(0)

while True:
    ret, frame = cap.read()
    if not ret:
        break
    frame = cv2.flip(frame, 1)
    h, w, _ = frame.shape
    frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = hands.process(frame_rgb)


    if results.multi_hand_landmarks:
        hand = results.multi_hand_landmarks[0]
        thumb = hand.landmark[4]
        index = hand.landmark[8]
        thumb_x, thumb_y = int(thumb.x * w), int(thumb.y * h)
        index_x, index_y = int(index.x * w), int(index.y * h)

        cv2.circle(frame, (thumb_x, thumb_y), 6, (255,255,255), -1)
        cv2.circle(frame, (index_x, index_y), 6, (255,255,255), -1)
        cv2.line(frame, (thumb_x, thumb_y), (index_x, index_y), (255,255,255), 1)
        
        pinch_dist = math.dist(
            (thumb.x, thumb.y),
            (index.x, index.y)
        )
        cv2.putText(frame, f"Distance: {str(pinch_dist)}", (50,50), cv2.FONT_HERSHEY_SIMPLEX , fontScale=1, color=(255,255,255), thickness=1)

        data = {
            "thumb": [thumb.x, thumb.y],
            "index": [index.x, index.y],
            "pinch": pinch_dist
        }
        sock.sendto(json.dumps(data).encode(), (host, port))

    cv2.imshow("Wow", frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break


cap.release()
sock.close()
cv2.destroyAllWindows()