#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;

void detectAndDisplay(Mat, CascadeClassifier, CascadeClassifier);

int main(void)
{
	Mat frame;
	CascadeClassifier face_cascade, eyes_cascade;
	String face_cascade_name = "haarcascade_frontalface_alt.xml"
		  ,eyes_cascade_name = "haarcascade_eye_tree_eyeglasses.xml";
	
	frame = imread("D:/1.jpg");
	if (!face_cascade.load(face_cascade_name)){ printf("--(!)Error loading\n"); return -1; };
	if (!eyes_cascade.load(eyes_cascade_name)){ printf("--(!)Error loading\n"); return -1; };

	detectAndDisplay(frame, face_cascade, eyes_cascade);
}

void detectAndDisplay(Mat frame, CascadeClassifier face_cascade, CascadeClassifier eyes_cascade)
{
	std::vector<Rect> faces;
	Mat frame_gray;
	cvtColor(frame, frame_gray, COLOR_BGR2GRAY);
	equalizeHist(frame_gray, frame_gray);

	face_cascade.detectMultiScale(frame_gray, faces, 1.1, 2, 0 | CV_HAAR_SCALE_IMAGE, Size(30, 30));
	for (size_t i = 0; i < faces.size(); i++)
	{
		Point center(faces[i].x + faces[i].width / 2, faces[i].y + faces[i].height / 2);
		ellipse(frame, center, Size(faces[i].width / 2, faces[i].height / 2), 0, 0, 360, Scalar(255, 0, 255), 2, 8, 0);
	}

	imshow("Face detection", frame);
	waitKey(0);
}
