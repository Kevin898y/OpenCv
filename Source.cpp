#include <cv.h>
#include <highgui.h>
using namespace std;

int main()
{
	IplImage *test;
	test = cvLoadImage("D:\\1.jpg");
	cvNamedWindow("test_demo", 1);
	cvNamedWindow("Smooth", 1);
	cvShowImage("test_demo",test);
	//void cvSmooth( const CvArr* src, CvArr* dst,int smoothtype,int param1, int param2, double sigma, double param4);
	cvSmooth(test, test, CV_GAUSSIAN, 3, 3, 5, 10);//3x3
	cvShowImage("Smooth", test);
	cvWaitKey(0);
	cvDestroyWindow("test_demo");
	cvReleaseImage(&test);
}