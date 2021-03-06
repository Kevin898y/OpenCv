#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;
void rotate(Mat &, Mat &, Mat &, Mat &);
Vec3b nearest_interpolation(Mat &,double, double);
Vec3b bilinear_interpolation(Mat &, double, double);
Vec3b bicubic_interpolation(Mat &, double, double);
double W(double);
int degree = 30;
double a = -0.5;
double angle = degree * CV_PI / 180;
int main(){
	Mat Orange = imread("1.jpg", CV_LOAD_IMAGE_COLOR);
	Mat rot, nearest, bilinear, bicubic;
	rotate(Orange, nearest, bilinear, bicubic);
	imshow("Orange", Orange);
	imshow("nearest", nearest);
	imshow("bilinear", bilinear);
	imshow("bicubic", bicubic);
	waitKey(0);

	return 0;
}
double W(double x){
	double abs_x = std::abs(x);
	if (abs_x <= 1)
		return (a + 2)*pow(abs_x, 3) - (a + 3)*pow(abs_x, 2) + 1;
	else if (abs_x > 1 && abs_x < 2)
		return a*pow(abs_x, 3) - 5 * a*pow(abs_x, 2) + 8 * a*abs_x - 4 * a;
	else
		return 0;
}
Vec3b nearest_interpolation(Mat &Orange, double rot_x, double rot_y){
	Vec3b temp;
	Vec3b *curPtr;
	int x = cvRound(rot_x),
		y = cvRound(rot_y);
	curPtr = Orange.ptr<Vec3b>(x);
	return curPtr[y];
}
Vec3b bilinear_interpolation(Mat &Orange, double x, double y){
	Vec3d R1,R2,R3;
	Vec3b *Ptr;
	int F_x = cvFloor(x),
		F_y = cvFloor(y),
	    C_x = cvCeil(x),
		C_y = cvCeil(y);
	if (F_x == C_x){
		if (F_x - 1>=0)
			F_x = F_x - 1;
		else if (F_x + 1<Orange.rows)
			C_x = F_x + 1;
	}
	if (F_y == C_y){
		
		if (F_y - 1 >= 0)
		F_y = F_y - 1;
		else if (F_y + 1<Orange.cols)
		C_y = F_y + 1;
	}
	
	Point neighbor1(F_x, F_y),
		  neighbor2(C_x, F_y),
		  neighbor3(F_x, C_y),
		  neighbor4(C_x, C_y);

	Ptr = Orange.ptr<Vec3b>(neighbor1.x);
	R1 = (neighbor3.y - y) / (neighbor3.y - neighbor1.y)*(Vec3d)Ptr[neighbor1.y] + (y - neighbor1.y) / (neighbor3.y - neighbor1.y)*(Vec3d)Ptr[neighbor3.y];
	Ptr = Orange.ptr<Vec3b>(neighbor2.x);
	R2 = (neighbor4.y - y) / (neighbor4.y - neighbor2.y)*(Vec3d)Ptr[neighbor2.y] + (y - neighbor2.y) / (neighbor4.y - neighbor2.y)*(Vec3d)Ptr[neighbor4.y];
	R3 = (x - F_x) / (C_x - F_x)*R2 + (C_x - x) / (C_x - F_x)*R1;
	return R3;
	
}
Vec3b bicubic_interpolation(Mat &Orange, double x, double y){
	Vec3b *Ptr;
	int F_x = cvFloor(x),
		F_y = cvFloor(y),
		C_x = cvCeil(x),
		C_y = cvCeil(y);
	if (F_x == C_x){
		if (F_x - 1 >= 0)
			F_x = F_x - 1;
		else if (F_x + 1 < Orange.rows)
			C_x = F_x + 1;
	}
	if (F_y == C_y){
		if (F_y - 1 >= 0)
			F_y = F_y - 1;
		else if (F_y + 1 < Orange.cols)
			C_y = F_y + 1;
	}
	int FF_x = F_x, FF_y = F_y, CC_x = C_x, CC_y = C_y;
	if (F_x - 1 >= 0)
		FF_x = F_x - 1;
	if (C_x + 1 < Orange.rows)
		CC_x = C_x + 1;
	if (F_y - 1 >= 0)
		FF_y = F_y - 1;
	if (C_y + 1 < Orange.cols)
		CC_y = C_y + 1;
	Point neighbor[4][4] = {
		{ Point(FF_x, FF_y), Point(FF_x, F_y), Point(FF_x, C_y), Point(FF_x, CC_y) },
		{ Point(F_x, FF_y), Point(F_x, F_y), Point(F_x, C_y), Point(F_x, CC_y) },
		{ Point(C_x, FF_y), Point(C_x, F_y), Point(C_x, C_y), Point(C_x, CC_y) },
		{ Point(CC_x, FF_y), Point(CC_x, F_y), Point(CC_x, C_y), Point(CC_x, CC_y) }
	};
	Vec3d temp(0, 0, 0),temp2;	
	for (int i = 0; i < 4; i++)
	{
		Ptr = Orange.ptr<Vec3b>(neighbor[i][0].x);
		for (int j =0; j < 4; j++)
		{
			temp2 = Ptr[neighbor[i][j].y];
			temp += temp2 * W(x - neighbor[i][j].x)* W(y - neighbor[i][j].y);
		}
	}

	return temp;
}
void rotate(Mat &Orange, Mat &nearest, Mat &bilinear, Mat &bicubic){
	int nChannels = Orange.channels(),
		heightLimit = Orange.rows*cos(angle) + Orange.rows*sin(angle),
		widthLimit = Orange.cols*cos(angle) + Orange.cols*sin(angle),
		center_x = heightLimit / 2,
		center_y = widthLimit / 2;
	double shift_x = center_x*cos(angle) - center_y*sin(angle) - center_x + ( heightLimit - Orange.rows)/2,
		   shift_y = center_x*sin(angle) + center_y*cos(angle) - center_y +( widthLimit - Orange.cols)/2;

	nearest.create(heightLimit, widthLimit, Orange.type());
	bilinear.create(heightLimit, widthLimit, Orange.type());
	bicubic.create(heightLimit, widthLimit, Orange.type());

	for (int iH = 0; iH<heightLimit; iH++){
		Vec3b  *nearestPtr = nearest.ptr<Vec3b>(iH);
		Vec3b  *bilinearPtr = bilinear.ptr<Vec3b>(iH );
		Vec3b  *bicubicPtr = bicubic.ptr<Vec3b>(iH); 
		double rot_x = 0, rot_y = 0;
		for (int iW = 0; iW<widthLimit; iW++){
			rot_x = (iH*cos(angle) - iW*sin(angle)) - (shift_x);
			rot_y = (iH*sin(angle) + iW*cos(angle)) - (shift_y);
			if (rot_x<0 || rot_x>Orange.rows - 1 || rot_y<0 || rot_y>Orange.cols - 1){
				bicubicPtr[iW]=bilinearPtr[iW] = nearestPtr[iW] = Vec3b(0, 0, 0);
			}
			else{
				nearestPtr[iW] = nearest_interpolation(Orange, rot_x, rot_y);
				bilinearPtr[iW] = bilinear_interpolation(Orange, rot_x, rot_y);
				bicubicPtr[iW] = bicubic_interpolation(Orange, rot_x, rot_y);
			}
		}
	}
}