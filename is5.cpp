#include <opencv2/opencv.hpp>
#include <opencv2/nonfree/nonfree.hpp>  
#include <opencv2/legacy/legacy.hpp>  
#include <iostream>
#include <string>
using namespace std;
using namespace cv;


int main()
{
	Mat obj,scence,gray_obj,gray_scence;
	obj = imread("obj.jpg");
	scence = imread("scene.jpg");
	cvtColor(obj, gray_obj, CV_BGR2GRAY);
	cvtColor(scence, gray_scence, CV_BGR2GRAY);

	SIFT sift(100, 3, 0.04, 10, 1.6f);
	SURF surf(400);

	Mat descriptor, descriptor2;
	vector<KeyPoint> keypoints1,keypoints2;

	//sift(gray_obj, noArray(), keypoints1, descriptor, 0);
	surf(gray_obj, noArray(), keypoints1, descriptor, 0);
	//drawKeypoints(obj, keypoints1, obj, Scalar(0, 0, 255), DrawMatchesFlags::DEFAULT);

	//sift(gray_scence, noArray(), keypoints2, descriptor2, 0);
	surf(gray_scence, noArray(), keypoints2, descriptor2, 0);
	//drawKeypoints(scence, keypoints2, scence, Scalar(0, 0, 255), DrawMatchesFlags::DEFAULT);

	BruteForceMatcher< L2<float> > matcher;
	std::vector< DMatch > matches;
	matcher.match(descriptor, descriptor2, matches);

	
	Mat img_matches;
	drawMatches(obj, keypoints1, scence, keypoints2, matches, img_matches);

	double min_dist = 100000;
	for (int i = 0; i < (int)matches.size(); i++) { // find the min_distance
		double dist = matches[i].distance;
		if (dist < min_dist)
			min_dist = dist;
	} 
	std::vector< DMatch > good_matches; // store a better matches
	for (int i = 0; i < (int)matches.size(); i++) {
		if (matches[i].distance < 3* min_dist) // you can chage the number "3 "
			good_matches.push_back(matches[i]);
	} 
	
	std::vector<Point2f> src;
	std::vector<Point2f> target;
	for (int i = 0; i < good_matches.size(); i++) {
		src.push_back(keypoints1[good_matches[i].queryIdx].pt);
		target.push_back(keypoints2[good_matches[i].trainIdx].pt);
	}
	Mat output;
	Mat H = findHomography(target, src, CV_RANSAC);
	warpPerspective(scence, output, H, Size(scence.cols*2 , scence.rows), INTER_LINEAR, BORDER_CONSTANT);
	
	Mat target_in_big_mat(output, Rect(0, 0, scence.cols, scence.rows));
	obj.copyTo(target_in_big_mat);
	


	imshow("output", output);
	imshow("sift_Matches", img_matches);

	


	waitKey(0);
}