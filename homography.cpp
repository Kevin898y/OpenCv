#include <cv.h>
#include <cxcore.h>
#include <highgui.h>
#include <iostream>

using namespace cv;
using namespace std;
void input_mouse(int event, int x, int y, int flags, void* param);
void output_mouse(int event, int x, int y, int flags, void* param);
void draw();
Mat src,input,output;
vector<Point2f> object;
vector<Point2f> scene;
int num=0,num2=0;
int  main(int  argc, char**  argv)
{
	namedWindow("image", WINDOW_AUTOSIZE);
	namedWindow("output", WINDOW_AUTOSIZE);
	setMouseCallback("image", input_mouse, NULL);
	setMouseCallback("output", output_mouse, NULL);
	src = (argc == 2 ? cvLoadImage(argv[1]) : 0);
	if (!src.data) src = cvLoadImage("1.jpg");
	if (!src.data) return -1;
	output.create(src.rows, src.cols, src.type());
	output = Scalar(255, 255, 255);
	imshow("output", output);
	input = src.clone();
	imshow("image", input);

	waitKey();
	return 0;
}
void draw(){
	for (int i = 0; i < object.size(); i++)
	{
		char k = (i + '0'+1);
		string s;
		s += k;
		putText(input, s, Point(object[i].x, object[i].y), 0, 0.5, Scalar(0, 255, 0), 1);
		circle(input, Point(object[i].x, object[i].y), 3, Scalar(0, 255, 0), 1);
	}
	for (int i = 0; i < scene.size(); i++)
	{
		char k = (i + '0' + 1);
		string s;
		s += k;
		putText(output, s, Point(scene[i].x, scene[i].y), 0, 0.5, Scalar(0, 0, 255), 1);
		circle(output, Point(scene[i].x, scene[i].y), 3, Scalar(0, 0, 255), 1);
	}
	imshow("image", input);
	imshow("output", output);
}
void input_mouse(int event, int x, int y, int flags, void* param) {
	switch (event){
		case CV_EVENT_LBUTTONDOWN:
			input = src.clone();
			if (num < 8){
				if (num < 4){
					object.push_back(Point(x, y));
					num++;
					draw();
				}
			}	
			break;
		case CV_EVENT_RBUTTONDOWN:
			input = src.clone();
			if (num >0){
				if (num <= 4){
					object.pop_back();
					num--;
					draw();
				}
			}
			break;
		case CV_EVENT_MBUTTONDOWN:
			if (num + num2 >= 8){
				Mat warpMatrix = findHomography(object, scene, CV_RANSAC);
				Mat warped;
				warpPerspective(src, output, warpMatrix, warped.size(), INTER_LINEAR, BORDER_CONSTANT);
				draw();
				imshow("output", output);
			}
			break;
			
	}
}
void output_mouse(int event, int x, int y, int flags, void* param){
	switch (event){
	case CV_EVENT_LBUTTONDOWN:
		output = Scalar(255, 255, 255);
		if (num2 < 8){
			if (num2 < 4){
				scene.push_back(Point(x, y));
				num2++;
				draw();
			}
		}
		break;
	case CV_EVENT_RBUTTONDOWN:
		output = Scalar(255, 255, 255);
		input = src.clone();
		if (num2 >0){
			if (num2 <= 4){
				scene.pop_back();
				num2--;
				draw();
			}
				
		}
		break;
	case CV_EVENT_MBUTTONDOWN:
		if (num+num2 >= 8){
			Mat warpMatrix = findHomography(object, scene, CV_RANSAC);
			Mat warped;
			warpPerspective(src, output, warpMatrix, warped.size(), INTER_LINEAR, BORDER_CONSTANT);
			draw();
			imshow("output", output);
		}
		break;
	}
}