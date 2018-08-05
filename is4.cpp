#include <opencv2/opencv.hpp>
#include <iostream>
#include <cmath>
#include <string>
using namespace std;
using namespace cv;

void detectAndDisplay(Mat&, CascadeClassifier, CascadeClassifier);
int main()
{
	Mat frame;
	CascadeClassifier face_cascade, eyes_cascade;
	String face_cascade_name = "haarcascade_frontalface_alt.xml"
		, eyes_cascade_name = "haarcascade_eye_tree_eyeglasses.xml";

	if (!face_cascade.load(face_cascade_name)) { printf("--(!)Error loading\n"); return -1; };
	if (!eyes_cascade.load(eyes_cascade_name)) { printf("--(!)Error loading\n"); return -1; };

	VideoCapture cap("SRC.mp4");
	if (!cap.isOpened()) {
		cout << "Capture could not be opened successfully" << endl;
		return -1;
	}
	int iter = 1;
	while (char(waitKey(30)) != 27 && cap.isOpened()) {
		cap >> frame;
		detectAndDisplay(frame, face_cascade, eyes_cascade);
		imshow("Face detection", frame);
		if (iter == 100 || iter == 300)
			imwrite(to_string(iter) + "th.jpg", frame);
		iter++;
	}



	waitKey(0);
}

void detectAndDisplay(Mat &frame, CascadeClassifier face_cascade, CascadeClassifier eyes_cascade)
{
	vector<Rect> faces, eye;
	Mat frame_gray,ROI;
	cvtColor(frame, frame_gray, COLOR_BGR2GRAY);

	face_cascade.detectMultiScale(frame_gray, faces, 1.1, 3, 0 , Size(70, 70));
	
	for (size_t i = 0; i < faces.size(); i++)
	{
		ROI = frame(faces[i]);
		rectangle(frame, faces[i], Scalar(255, 0, 0), 4, 8, 0);
		eyes_cascade.detectMultiScale(ROI, eye, 1.1, 3, 0, Size(5, 5), Size(30, 30));
		for (size_t j = 0; j < eye.size(); j++)
		{
			Point center(eye[j].x + eye[j].width / 2, eye[j].y + eye[j].height / 2);
			ellipse(ROI, center, Size(eye[j].width / 2, eye[j].height / 2), 0, 0, 360, Scalar(0, 0, 255), 2, 8, 0);
		}
	}

	
}