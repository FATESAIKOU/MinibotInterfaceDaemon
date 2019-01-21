import cv2
import sys

camera_id  = int(sys.argv[1])
image_name = sys.argv[2]

camera = cv2.VideoCapture(camera_id)
_, image = camera.read()
cv2.imwrite(image_name, image)

del(camera)