#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;
int main()
{
	Mat Orange, Thresh, Erode, Dilate;

	Orange = imread("D:/1.jpg", CV_LOAD_IMAGE_GRAYSCALE);
	threshold(Orange, Thresh, 127, 255, THRESH_BINARY);
	erode(Thresh, Erode, Mat(), Point(-1, -1), 1);
	dilate(Thresh, Dilate, Mat(), Point(-1, -1), 1);

	imshow("threshold", Thresh);
	imshow("erode", Erode);
	imshow("dilate", Dilate);
	waitKey(0);

}