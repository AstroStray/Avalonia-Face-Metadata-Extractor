import sys
import cv2
import mediapipe as mp
import json

def detect_face_landmarks(image_path):
    mp_face_mesh = mp.solutions.face_mesh
    face_mesh = mp_face_mesh.FaceMesh(static_image_mode=True, max_num_faces=5, min_detection_confidence=0.5)

    try:
        image = cv2.imread(image_path)
        if image is None:
            print(json.dumps({"error": "Image not found or could not be read."}), file=sys.stderr)
            sys.exit(1)

        image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        results = face_mesh.process(image_rgb)

        output = {
            "image_width": image.shape[1],
            "image_height": image.shape[0],
            "faces": []
        }

        if results.multi_face_landmarks:
            for face_landmarks in results.multi_face_landmarks:
                face_data = []
                for landmark in face_landmarks.landmark:
                    face_data.append({
                        "x": landmark.x,
                        "y": landmark.y,
                        "z": landmark.z
                    })
                output["faces"].append(face_data)

        print(json.dumps(output, indent=4))

    except Exception as e:
        print(json.dumps({"error": str(e)}), file=sys.stderr)
        sys.exit(1)
    finally:
        face_mesh.close()

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print(json.dumps({"error": "Usage: python face_detector.py <image_path>"}), file=sys.stderr)
        sys.exit(1)

    image_path_arg = sys.argv[1]
    detect_face_landmarks(image_path_arg)
