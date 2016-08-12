#include <cv.h>
#include <highgui.h>
using namespace std;
using namespace cv;
void HPF(const Mat &, Mat &);

int main(){
	Mat Orange = imread("D:/1.jpg", CV_LOAD_IMAGE_UNCHANGED);
	Mat hpf;
	HPF(Orange, hpf);

	imshow("Orange", Orange);
	imshow("HPF", hpf);
	waitKey(0);

	return 0;
}

void HPF(const Mat &Orange, Mat &hpf){
	Orange.copyTo(hpf);
	const int nChannels = Orange.channels();
	int heightLimit = Orange.rows - 1;
	int widthLimit = nChannels * (Orange.cols - 1);
	for (int iH = 1; iH<heightLimit; iH++){
		const uchar *prePtr = Orange.ptr<const uchar>(iH - 1)
			      , *curPtr = Orange.ptr<const uchar>(iH)
		          , *nextPtr = Orange.ptr<const uchar>(iH + 1);
		uchar *hpfPtr = hpf.ptr<uchar>(iH);
		for (int iW = nChannels; iW<widthLimit; iW++){
			hpfPtr[iW] = saturate_cast<uchar>(9 * curPtr[iW] - curPtr[iW - nChannels] - curPtr[iW + nChannels] - prePtr[iW] - nextPtr[iW]
				- prePtr[iW - nChannels] - prePtr[iW + nChannels] - nextPtr[iW - nChannels] - nextPtr[iW + nChannels]);
		}
	}
}