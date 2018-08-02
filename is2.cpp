#include <opencv2/opencv.hpp>
#include <highgui.h>
using namespace std;
using namespace cv;


int main(void)
{
	VideoCapture cap("video.avi");
	if (!cap.isOpened()) {
		cout << "Capture could not be opened successfully" << endl;
		return -1;
	}
	Mat image, blur, foregroun,mask, thr;
	BackgroundSubtractorMOG2 bgSubtractor(20,16,true);
	while (char(waitKey(30)) != 27 && cap.isOpened()) {
		cap >> image;
		if (image.empty()) {
			cout << "Video over" << endl;
			break;
		}
		else
		{
			mask.setTo(0);
			bgSubtractor(image, foregroun, 0.01);
			GaussianBlur(foregroun, blur, Size(3, 3), 0, 0);
			threshold(blur, thr, 127, 255, THRESH_BINARY);
			erode(thr, thr, Mat(), Point(-1, -1));
			dilate(thr, thr, Mat(), Point(-1, -1));
			image.copyTo(mask,thr);
			imshow("origin", image);
			imshow("foregroun", foregroun);
			imshow("morphology result", thr);
			imshow("mask", mask);
		}
	}
	waitKey(0);
}