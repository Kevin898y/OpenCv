#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;
int main()
{
	Mat Orange, hsv;

	Orange = imread("1.jpg");
	cvtColor(Orange, hsv, CV_BGR2HSV);
	//R=Scalar(0, 100, 100), Scalar(10, 255, 255),B=Scalar(110, 50, 50), Scalar(130, 255, 255),
	inRange(hsv, Scalar(50, 100, 100), Scalar(70, 255, 255), hsv);//R

	imshow("Orange", Orange);
	imshow("Filter", hsv);
	waitKey(0);

}