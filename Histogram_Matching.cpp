#include <cv.h>
#include <highgui.h>
#include <cmath>
using namespace std;
using namespace cv;

void Power_Law(Mat &, Mat &,double);
void Histogram_Equalization(Mat &, Mat &);
void Histogram_Matching(Mat &, Mat&, double[]);
void BGRtoGray(Mat&, Mat&);
int main(){
	Mat dst = imread("gray.jpg");
	Mat Enhancement,gray;
	double pdf[256] = {0};
	//計算Histogram_Matching所需的p.d.f
	for (int i = 0; i < 256; i++){
		pdf[i] = 1.5*pow(i, 0.5) / (double)pow(255, 1.5);
	}
	BGRtoGray(dst, gray);//彩色轉灰階
	imshow("gray", gray);
	imwrite("Normal_Gray.jpg", gray);

	Histogram_Equalization(gray, Enhancement);
	imshow("Histogram_Equalization", Enhancement);//顯示
	imwrite("Bright_Histogram_Equalization.jpg", Enhancement);//寫入檔案

	Power_Law(gray, Enhancement,1);
	imshow("Power_Law", Enhancement);
	imwrite("Bright_Power_Law.jpg", Enhancement);

	Histogram_Matching(gray, Enhancement, pdf);
	imshow("Histogram_Matching", Enhancement);
	imwrite("Bright_Histogram_Matching.jpg", Enhancement);
	waitKey(0);

	return 0;
}
void BGRtoGray(Mat &orange, Mat& gray){
	gray.create(orange.rows, orange.cols,CV_8U);
	for (int i = 0; i < orange.rows; i++){
		uchar *GPtr = gray.ptr<uchar>(i);
		Vec3b *OPtr = orange.ptr<Vec3b>(i);
		for (int j = 0; j < orange.cols; j++){
			GPtr[j] = (uchar)(0.114*(double)OPtr[j].val[0] + 0.587*(double)OPtr[j].val[1] + 0.299 *(double)OPtr[j].val[2]);
		}
	}
}
void Power_Law(Mat &orange, Mat &enhancement, double r){
	enhancement.create(orange.rows, orange.cols, orange.type());
	for (int i = 0; i < orange.rows; i++){
		uchar *Ptr = enhancement.ptr<uchar>(i);
		uchar *OPtr = orange.ptr<uchar>(i);
		for (int j = 0; j < orange.cols; j++){
			Ptr[j] = saturate_cast<uchar>(pow((double)OPtr[j] / 255, r) * 255);
		}
	}
}
void Histogram_Equalization(Mat &orange, Mat &enhancement){
	enhancement.create(orange.rows, orange.cols, orange.type());
	int Histogram[256] = { 0 }, intensity[256] = { 0 }, sum = 0;
	//
	for (int i = 0; i < orange.rows; i++){
		uchar *OPtr = orange.ptr<uchar>(i);
		for (int j = 0; j < orange.cols; j++){
			Histogram[OPtr[j]]++;
		}
	}
	int allpixel = orange.cols*orange.rows;
	sum = 0;
	for (int i = 0; i < 256; i++){	
		sum += Histogram[i];
		intensity[i] = 255 / (double)allpixel*sum;
	}
	for (int i = 0; i < orange.rows; i++){
		uchar *Ptr = enhancement.ptr<uchar>(i);
		uchar *OPtr = orange.ptr<uchar>(i);
		for (int j = 0; j < orange.cols; j++){

			Ptr[j] = intensity[OPtr[j]];
		}
	}
}
void Histogram_Matching(Mat &orange, Mat&enhancement, double pdf[]){
	enhancement.create(orange.rows, orange.cols, orange.type());
	int Histogram[256] = { 0 }, S[256] = { 0 }, sum = 0, G[256] = { 0 }, T[256] = { 0 };
	double sum2=0;
	for (int i = 0; i < orange.rows; i++){
		uchar *OPtr = orange.ptr<uchar>(i);
		for (int j = 0; j < orange.cols; j++){
			Histogram[OPtr[j]]++;
		}
	}
	int allpixel = orange.cols*orange.rows;
	sum = 0;
	for (int i = 0; i < 256; i++){
		sum += Histogram[i];
		S[i] = 255 / (double)allpixel*sum;
	}
	sum2 = 0;
	for (int i = 0; i < 256; i++){
		sum2 += pdf[i];
		G[i] = sum2 * 255;
	}
	int temp=0;
	for (int i = 0; i < 256; i++)
	{
		for (; temp < 255; temp++){
			if (S[i] == G[temp]){
				T[S[i]] = temp;
				break;
			}
			else if (S[i] > G[temp] && S[i] < G[temp + 1]){
				T[S[i]] = temp;
				break;
			}
			else if (S[i] > G[temp] && S[i] == G[temp + 1]){
				T[S[i]] = temp + 1;
				break;
			}
				
		}
	}
	for (int i = 0; i < orange.rows; i++){
		uchar *Ptr = enhancement.ptr<uchar>(i);
		uchar *OPtr = orange.ptr<uchar>(i);
		for (int j = 0; j < orange.cols; j++){
			Ptr[j] = T[S[OPtr[j]]];
		}
	}
}

