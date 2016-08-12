#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;
void LPF(const Mat &, Mat &);

int main(){
	Mat Orange = imread("D:/1.jpg", CV_LOAD_IMAGE_UNCHANGED);
	Mat lpf;
	LPF(Orange, lpf);

	imshow("Orange", Orange);
	imshow("LPF", lpf);
	waitKey(0);

	return 0;
}

void LPF(const Mat &Orange, Mat &lpf){
	Orange.copyTo(lpf);
	const int nChannels = Orange.channels();
	int heightLimit = Orange.rows - 1;
	int widthLimit = nChannels * (Orange.cols - 1);
	for (int iH = 1; iH<heightLimit; iH++){
		const uchar *prePtr = Orange.ptr<const uchar>(iH - 1)
			      , *curPtr = Orange.ptr<const uchar>(iH)
		          , *nextPtr = Orange.ptr<const uchar>(iH + 1);
		uchar *lpfPtr = lpf.ptr<uchar>(iH);
		for (int iW = nChannels; iW<widthLimit; iW++){
			lpfPtr[iW] = saturate_cast<uchar>(0.204*curPtr[iW] + 0.124*curPtr[iW - nChannels] + 0.124*curPtr[iW + nChannels] + 0.124*prePtr[iW] + 0.124*nextPtr[iW]
											+ 0.075*prePtr[iW - nChannels] + 0.075*prePtr[iW + nChannels] + 0.075*nextPtr[iW - nChannels] + 0.075*nextPtr[iW + nChannels]);
		}
	}
}