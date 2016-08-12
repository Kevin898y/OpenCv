#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;


void main()
{
	IplImage* pFrameImg = NULL, *pGrayImg = NULL, *pForegroundImg = NULL, *pBackgroundImg_8U = NULL, *pBackgroundImg_32F = NULL;

	CvCapture* pCapture = cvCaptureFromCAM(0);
	int nFrmNum = 0;
	while (pFrameImg = cvQueryFrame(pCapture))
	{
		nFrmNum++;
		if (nFrmNum == 1)
		{
			pBackgroundImg_8U = cvCreateImage(cvSize(pFrameImg->width,
				pFrameImg->height), IPL_DEPTH_8U, 1);
			pBackgroundImg_32F = cvCreateImage(cvSize(pFrameImg->width,
				pFrameImg->height), IPL_DEPTH_32F, 1);
			pForegroundImg = cvCreateImage(cvSize(pFrameImg->width,
				pFrameImg->height), IPL_DEPTH_8U, 1);
			pGrayImg = cvCreateImage(cvSize(pFrameImg->width,
				pFrameImg->height), IPL_DEPTH_8U, 1);
			cvCvtColor(pFrameImg, pBackgroundImg_8U, CV_BGR2GRAY);
			cvCvtColor(pFrameImg, pForegroundImg, CV_BGR2GRAY);
			cvConvertScale(pBackgroundImg_8U, pBackgroundImg_32F);
		}
		else
		{
			cvCvtColor(pFrameImg, pGrayImg, CV_BGR2GRAY);
			cvAbsDiff(pGrayImg, pBackgroundImg_8U, pForegroundImg);
			cvThreshold(pForegroundImg, pForegroundImg, 40, 255, CV_THRESH_BINARY);
			cvRunningAvg(pGrayImg, pBackgroundImg_32F, 0.05, 0);
			cvConvertScale(pBackgroundImg_32F, pBackgroundImg_8U);

			//cvShowImage("webcam", pFrameImg);
			//cvShowImage("background", pBackgroundImg_8U);
			cvShowImage("foreground", pForegroundImg);
			cvWaitKey(30);
		}
	}

	cvReleaseImage(&pFrameImg);
	cvReleaseImage(&pForegroundImg);
	cvReleaseImage(&pBackgroundImg_8U);
	cvReleaseImage(&pBackgroundImg_32F);
	cvReleaseImage(&pGrayImg);
	cvReleaseCapture(&pCapture);
}