#include <opencv2/opencv.hpp>
#include <highgui.h>
#include <string>
using namespace std;
using namespace cv;


int main(void)
{
	VideoCapture cap("video.avi");
	if (!cap.isOpened()) {
		cout << "Capture could not be opened successfully" << endl;
		return -1;
	}
	Mat image, blur, foreground, mask, thr, morpology, Aligned, diff_image, Last, ans;
	BackgroundSubtractorMOG2 bgSubtractor(20, 16, true);
	int iter = 1;
	Last.create(150, 140, CV_8U);
	Last.setTo(0);
	diff_image.create(150, 140, CV_8U);
	diff_image.setTo(0);
	ans.create(150, 140, CV_8U);
	Aligned.create(150, 140, morpology.type());
	ans.setTo(0);
	bool first = false;
	while (char(waitKey(33)) != 27 && cap.isOpened()) {
		cap >> image;
		if (image.empty()) {
			cout << "Video over" << endl;
			break;
		}
		else
		{

			bgSubtractor(image, foreground, 0.0001);
			GaussianBlur(foreground, foreground, Size(5, 5), 0, 0);
			threshold(foreground, thr, 127, 255, THRESH_BINARY);
			erode(thr, morpology, Mat(), Point(-1, -1));
			dilate(morpology, morpology, Mat(), Point(-1, -1));
			foreground = morpology.clone();

			vector<vector<Point>> contours;
			findContours(morpology, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);
			vector<Rect> boundRect(contours.size());

			
			Aligned.setTo(0);
			for (int i = 0; i < contours.size(); i++) {
				boundRect[i] = boundingRect(contours[i]);
				Mat ROI = foreground(boundRect[i]);
				double n = contourArea(contours[i]);
				if (n >3000) {
					double average_x = 0, mean = 0;
					int counter = 0;
					for (int i = 0; i < ROI.rows && i< 30; i++) {
						for (int j = 0; j < ROI.cols; j++) {
							if (ROI.at<uchar>(i, j) == 255)
							{
								mean = mean + j;
								counter++;
							}
						}
					}
					average_x = mean / counter;

					Mat paste_region(Aligned, Rect(70 - (int)average_x, 0, ROI.cols, ROI.rows));
					ROI.copyTo(paste_region);
					drawContours(morpology, contours, i, Scalar(255, 0, 0));
					rectangle(morpology, boundRect[i].tl(), boundRect[i].br(), Scalar(255, 0, 0), 2, 8, 0);

					if (first) {
						absdiff(Aligned, Last, diff_image);
						add(ans, diff_image, ans);
						Last = Aligned.clone();
					}
					else
					{
						Last = Aligned.clone();
						first = true;
					}
				}
			}
			imshow("ret", Aligned);
			imshow("diff", diff_image);
			imshow("ans", ans);
			if (iter == 74) {
				imwrite("aligned.jpg", Aligned);
				imwrite("ans.jpg", ans);
			}
			iter++;
		}
	}
	waitKey(0);
}