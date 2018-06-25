import cv2
import sys

image_name = sys.argv[1]

camera = cv2.VideoCapture(0)
_, image = camera.read()
cv2.imwrite(image_name, image)

del(camera)