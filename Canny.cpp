#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;
int main()
{
	
	Mat test;
	//載入灰階
	test = imread("D:\\1.jpg", CV_LOAD_IMAGE_GRAYSCALE);
	cvNamedWindow("Gray", 1);
	imshow("Gray", test);
	GaussianBlur(test, test, Size(3, 3), 0, 0);//3x3
	Mat Edge;
	Canny(test, Edge, 50,150);//Min Threshold 50
	cvNamedWindow("CannyEdge", 1);
	imshow("CannyEdge", Edge);

	cvWaitKey(0);
	cvDestroyWindow("Gray");
	cvDestroyWindow("CannyEdge");
}