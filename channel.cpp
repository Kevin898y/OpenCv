#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;
int main()
{
	
	Mat test;
	test = imread("D:\\1.jpg");
	cvNamedWindow("Orange", 1);
	imshow("Orange", test);

	vector<Mat> channels;
	Mat imageBlueChannel;
	Mat imageGreenChannel;
	Mat imageRedChannel;
	split(test, channels);
	imageBlueChannel = channels.at(0);
	imageGreenChannel = channels.at(1);
	imageRedChannel = channels.at(2);
	cvNamedWindow("imageBlueChannel", 1);
	imshow("imageBlueChannel", imageBlueChannel);
	cvNamedWindow("imageGreenChannel", 1);
	imshow("imageGreenChannel", imageGreenChannel);
	cvNamedWindow("imageRedChannel", 1);
	imshow("imageRedChannel", imageRedChannel);

	cvWaitKey(0);
	cvDestroyWindow("Orange");
	cvDestroyWindow("imageBlueChannel");
	cvDestroyWindow("imageGreenChannel");
	cvDestroyWindow("imageRedChannel");
	
}