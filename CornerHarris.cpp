#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include <iostream>
#include <stdio.h>
#include <stdlib.h>

using namespace cv;
using namespace std;
const double k = 0.04;
void MY_CornerHarris(Mat&, Mat&);
void Draw_Corner(Mat& ,Mat& );
int main(int, char** argv)
{
	Mat cornerMap, src, src_gray;
	VideoCapture cap;      
	cap.open(1); // cap.open(1); 
	while (cap.isOpened()) 
	{
		cap >> src;
		cvtColor(src, src_gray, CV_RGB2GRAY);
		MY_CornerHarris(src_gray, cornerMap);
		Draw_Corner(src, cornerMap);
		imshow("result", src);
		waitKey(33);
	}
	return(0);
}

void MY_CornerHarris(Mat& src_gray, Mat& Corner_Map){
	Mat Ix, Iy, Ixx, Iyy, Ixy, corner, dilated, Local_Max;
	corner.create(src_gray.rows, src_gray.cols, CV_64F);
	double dx[3][3] = { -1, 0, 1,
						-2, 0, 2,
						-1, 0, 1 };
	double dy[3][3] = { -1, -2, -1,
					     0,  0,  0,
						 1,  2,  1};
	Mat X_kernel = Mat(3, 3, CV_64F, dx);
	Mat Y_kernel = Mat(3, 3, CV_64F, dy);
	filter2D(src_gray, Ix, CV_64F, X_kernel);
	filter2D(src_gray, Iy, CV_64F, Y_kernel);
	Ixx = Ix.mul(Ix);
	Iyy = Iy.mul(Iy);
	Ixy = Ix.mul(Iy);
	GaussianBlur(Ixx, Ixx, Size(3, 3), 0, 0);
	GaussianBlur(Iyy, Iyy, Size(3, 3), 0, 0);
	GaussianBlur(Ixy, Ixy, Size(3, 3), 0, 0);
	
	for (int i = 0; i < corner.rows; i++){
		double *cornerPtr = corner.ptr<double>(i);
		double *IxxPtr = Ixx.ptr<double>(i);
		double *IyyPtr = Iyy.ptr<double>(i);
		double *IxyPtr = Ixy.ptr<double>(i);
		for (int j = 0; j < corner.cols; j++){
			double detM = (double)IxxPtr[j] * (double)IyyPtr[j] - (double)IxyPtr[j] * (double)IxyPtr[j];
			double traceM = (double)IxxPtr[j] + (double)IyyPtr[j];
			cornerPtr[j] = detM - k *traceM *traceM;
		}
	}
	
	//Local Max
	dilate(corner, dilated, Mat());
	compare(corner, dilated, Local_Max, CMP_EQ);
	//Threshold
	double max;
	minMaxLoc(corner, NULL, &max, NULL, NULL);
	double qualityLevel = 0.1;
	double thr = qualityLevel * max;
	Corner_Map = corner > thr;

	bitwise_and(Corner_Map, Local_Max, Corner_Map);
}
void Draw_Corner(Mat& image, Mat& binary)
{
	for (int j = 0; j < binary.rows; j++)
	{
		for (int i = 0; i < binary.cols; i++)
		{
			if (binary.at<uchar>(j, i))
				circle(image, Point(i, j), 3, Scalar(0, 255, 0), 1);
		}
	}
}

