using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using Vuforia;

public class FirstRender : MonoBehaviour
{
    const int Max_Tree_Num= 20;
    public GameObject material1, material2, material3, treemodel, rivermodel;
    public GameObject imagetarget, page2,nextbtn, detectbtn, backgroundplane;
    public GameObject Hint_2, Hint_3, Hint_4, Hint_5, Hint_6;
    public Light DirectionalLight;
    bool first_check = false, second_check = false, third_check = false, light_check = false, find = false;
    bool back = false, temp = false;
    public GameObject right, down, zero, downright;
    float RightX, RightY, DownX, Downy, ZeroX, ZeroY, DownrightX, DownrightY;
    float start, end;

    Point[] Tree_Center = new Point[Max_Tree_Num];
    Point[] River_Center = new Point[Max_Tree_Num];
    Point[] Train_Center = new Point[Max_Tree_Num];
    OpenCvSharp.CPlusPlus.Rect[] All_Tree = new OpenCvSharp.CPlusPlus.Rect[Max_Tree_Num];
    int Train_num, Tree_num, River_num, Final_Train_Num = 0, Final_Tree = 0, Final_River_Num = 0, round = 0;
    int[] Tree_Times = new int[Max_Tree_Num] ;
    int[] River_Times = new int[Max_Tree_Num];
    int[] Train_Times = new int[Max_Tree_Num];
    Point[] Final_Tree_Center = new Point[Max_Tree_Num];
    Point[] Final_River_Center = new Point[Max_Tree_Num];
    Point[] Final_Train_Center = new Point[Max_Tree_Num];
    OpenCvSharp.CPlusPlus.Rect[] Final_All_Tree = new OpenCvSharp.CPlusPlus.Rect[Max_Tree_Num];
    String tree_cascade_name;
    OpenCvSharp.CPlusPlus.CascadeClassifier tree_cascade;


    public enum modeType
    {
        First_Check,
        Second_Check,
        Third_Check,
        Draw_Mode,
        Background,
        Light_Check
    }
    public modeType mode;
    // Use this for initialization
    void Start()
    {
        for(int i=0;i< Max_Tree_Num; i++)
        {
            Tree_Times[i] = 0;
            River_Times[i] = 0;
            Train_Times[i] = 0;
        }
        tree_cascade = new CascadeClassifier();
        tree_cascade_name = "cascade.xml";
        if (!tree_cascade.Load(tree_cascade_name)) { print("--(!)Error loading\n"); };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(right.transform.position);
        RightX = screenPoint.x / 1.6f; RightY = screenPoint.y / 1.6f;
        screenPoint = Camera.main.WorldToScreenPoint(down.transform.position);
        DownX = screenPoint.x / 1.6f; Downy = screenPoint.y / 1.6f;
        screenPoint = Camera.main.WorldToScreenPoint(zero.transform.position);
        ZeroX = screenPoint.x / 1.6f; ZeroY = screenPoint.y / 1.6f;
        screenPoint = Camera.main.WorldToScreenPoint(downright.transform.position);
        DownrightX = screenPoint.x / 1.6f; DownrightY = screenPoint.y / 1.6f;


        Mat image = new Mat(); // Frame image buffer  
        Mat thr = new Mat(), frame = new Mat(), dst = new Mat(), filter = new Mat(), hsvImg = new Mat();

        Texture2D cameraTexture;
        Texture tex = backgroundplane.GetComponent<Renderer>().material.mainTexture;

        if (mode == modeType.First_Check)//yellow
        {
            
            cameraTexture = (Texture2D)tex;
            image = Mat.FromImageData(cameraTexture.EncodeToJPG());

            //Cv2.Flip(image, image, FlipMode.X);
            Mat newimage = Perspective_Transform(image, ZeroX, ZeroY, RightX, RightY, DownrightX, DownrightY, DownX, Downy);
            newimage.CopyTo(dst);

            float[] data = { 0, -1, 0, 0, 3.5f, 0, 0, -1, 0 };
            Mat kernel = new Mat(rows: 3, cols: 3, type: MatType.CV_32F, data: data);
            Cv2.Filter2D(dst, dst, newimage.Depth(), kernel);

            thr = color_detection(dst, new Scalar(20, 64, 46), new Scalar(40, 255, 255));
            //thr = color_detection(dst, new Scalar(0, 0, 221), new Scalar(180, 30, 255));

            first_check = check_position1(thr, dst, material1, first_check);

            //Cv2.ImShow("dst", dst);
            Cv2.ImShow("image", image);
            //Cv2.ImShow("filter", filter);
            //Cv2.ImShow("thr", thr);

            end = Time.time;
            float t = end - start;
            //print(t);
            if (first_check == true)
            {
                Hint_6.SetActive(true);
                mode = modeType.Draw_Mode;
                // Cv2.DestroyWindow("dst");
            }
            else if (t >= 10)
            {
                Hint_5.SetActive(true);
                mode = modeType.Draw_Mode;
            }
        }

        else if (mode == modeType.Second_Check)//purple
        {
            cameraTexture = (Texture2D)tex;
            image = Mat.FromImageData(cameraTexture.EncodeToJPG());

            //Cv2.Flip(image, image, FlipMode.X);
            Mat newimage = Perspective_Transform(image, ZeroX, ZeroY, RightX, RightY, DownrightX, DownrightY, DownX, Downy);
            newimage.CopyTo(dst);

            float[] data = { 0, -1, 0, 0, 4, 0, 0, -1, 0 };
            Mat kernel = new Mat(rows: 3, cols: 3, type: MatType.CV_32F, data: data);
            Cv2.Filter2D(dst, dst, newimage.Depth(), kernel);
            thr = color_detection(dst, new Scalar(0, 0, 210), new Scalar(180, 30, 255));
            //thr = color_detection(newimage, new Scalar(119, 120, 52), new Scalar(128, 255, 147));
            //thr = color_detection(newimage, new Scalar(125, 0, 46), new Scalar(155, 230, 255));

            second_check = check_position3(thr, dst, material2, second_check);

            //Cv2.ImShow("dst", dst);
            Cv2.ImShow("image", image);
            //Cv2.ImShow("filter", filter);
            //Cv2.ImShow("thr", thr);

            end = Time.time;
            float t = end - start;
            if (second_check == true)
            {
                Hint_6.SetActive(true);
                mode = modeType.Draw_Mode;
                // Cv2.DestroyWindow("dst");
            }
            else if (t >= 10)
            {
                Hint_5.SetActive(true);
                mode = modeType.Draw_Mode;
            }
        }

        else if (mode == modeType.Third_Check)//Red
        {
            Mat thr2=new Mat();
            cameraTexture = (Texture2D)tex;
            image = Mat.FromImageData(cameraTexture.EncodeToJPG());

            //Cv2.Flip(image, image, FlipMode.X);
            Mat newimage = Perspective_Transform(image, ZeroX, ZeroY, RightX, RightY, DownrightX, DownrightY, DownX, Downy);
            newimage.CopyTo(dst);

            float[] data = { 0, -1, 0, 0, 3.5f, 0, 0, -1, 0 };
            Mat kernel = new Mat(rows: 3, cols: 3, type: MatType.CV_32F, data: data);
            Cv2.Filter2D(dst, dst, newimage.Depth(), kernel);
            thr = color_detection(dst, new Scalar(0, 0, 210), new Scalar(180, 30, 255));
            //thr = color_detection(newimage, new Scalar(0, 40, 88), new Scalar(180, 225, 170));

            //thr = color_detection(newimage, new Scalar(0, 43, 46), new Scalar(10, 255, 255));
            //Cv2.InRange(newimage, new Scalar(156, 43, 46), new Scalar(180, 255, 255), thr);
            //Cv2.InRange(newimage, new Scalar(0, 43, 46), new Scalar(10, 255, 255), thr2);
            // thr = thr + thr2;
            Cv2.ImShow("thr", thr);
            third_check = check_position2(thr, dst, material3, third_check);

            //Cv2.ImShow("dst", dst);
            Cv2.ImShow("image", image);
            //Cv2.ImShow("filter", filter);
            

            end = Time.time;
            float t = end - start;
            if (third_check == true)
            {
                Hint_6.SetActive(true);
                mode = modeType.Draw_Mode;
                // Cv2.DestroyWindow("dst");
            }
            else if (t >= 10)
            {
                Hint_5.SetActive(true);
                mode = modeType.Draw_Mode;
            }
        }
        else if (mode == modeType.Draw_Mode)
        {
        }
        else if (mode == modeType.Light_Check)
        {
            cameraTexture = (Texture2D)tex;
            image = Mat.FromImageData(cameraTexture.EncodeToJPG());
            //Cv2.Flip(image, image, FlipMode.X);
            image = Perspective_Transform(image, ZeroX, ZeroY, RightX, RightY, DownrightX, DownrightY, DownX, Downy);
            Cv2.CvtColor(image, hsvImg, ColorConversion.BgrToHsv);

            //print(hsvImg);
            int i, j;
            float total = 0, v;
            for (i = 0; i < 12; i++)
            {
                for (j = 0; j < 12; j++)
                {
                    total += hsvImg.At<Vec3b>(80 + j * 30, 105 + i * 40)[2];
                }
            }
            v = total / 144;

            DirectionalLight.intensity = v / 80;
            end = Time.time;
            float t = end - start;
            if (t >= 3)
            {
                light_check = true;
                mode = modeType.Draw_Mode;
            }

        }
        else if (mode == modeType.Background)
        {
            Tree_num = 0; River_num = 0; Train_num = 0;
            cameraTexture = (Texture2D)tex;
            image = Mat.FromImageData(cameraTexture.EncodeToJPG());
            //Cv2.Flip(image, image, FlipMode.X);

            Point2f[] corners = new Point2f[4];
            Point2f[] corners_trans = new Point2f[4];
            corners[0] = new Point2f(ZeroX, ZeroY);
            corners[1] = new Point2f(RightX, RightY);
            corners[2] = new Point2f(DownrightX, DownrightY);
            corners[3] = new Point2f(DownX, Downy);
            corners_trans[0] = new Point2f(0, 0);
            corners_trans[1] = new Point2f(0, 480);
            corners_trans[2] = new Point2f(640, 480);
            corners_trans[3] = new Point2f(640, 0);
            bool tree = false;
            Mat transform = Cv2.GetPerspectiveTransform(corners, corners_trans);
            image = image.WarpPerspective(transform, image.Size(), Interpolation.Linear, BorderType.Constant, 0);
            image.CopyTo(dst);

            Mat GreenImg = new Mat(), BrownImg = new Mat(), BlueImg = new Mat(), Green_thr = new Mat(), Brown_thr = new Mat(), Blue_thr = new Mat();
            Mat Blue = new Mat(), Green = new Mat(), Red = new Mat(), edqalize = new Mat(), YUV = new Mat(), enhance = new Mat();
           // Mat ROI = new Mat(image, new OpenCvSharp.CPlusPlus.Rect(70, 45, 500, 385));
            float[] data = { 0, -1, 0, 0, 4, 0, 0, -1, 0 };
            Mat WhiteImg = new Mat(), White_thr = new Mat();
            Mat kernel = new Mat(rows: 3, cols: 3, type: MatType.CV_32F, data: data);
            Cv2.Filter2D(image, enhance, image.Depth(), kernel);
            Cv2.GaussianBlur(enhance, enhance, Cv.Size(5, 5), 0, 0);
            Cv2.Rectangle(enhance, new Point(0, 0), new Point(640, 480), new Scalar(255, 255, 255), 5);
            Cv2.CvtColor(enhance, hsvImg, ColorConversion.BgrToHsv);
            Cv2.InRange(hsvImg, new Scalar(35, 43, 46), new Scalar(77, 255, 255), GreenImg);
            Cv2.InRange(hsvImg, new Scalar(0, 0, 221), new Scalar(180, 30, 255), WhiteImg);
            Cv2.InRange(hsvImg, new Scalar(100, 43, 46), new Scalar(124, 255, 255), BlueImg);
            Cv2.Dilate(WhiteImg, White_thr, new Mat(), new Point(-1, -1), 3);
            Cv2.Erode(White_thr, White_thr, new Mat(), new Point(-1, -1), 3);
            Cv2.Erode(GreenImg, Green_thr, new Mat(), new Point(-1, -1), 3);
            Cv2.Dilate(Green_thr, Green_thr, new Mat(), new Point(-1, -1), 3);
            BrownImg = White_thr + GreenImg + BlueImg;
            Cv2.Erode(BlueImg, Blue_thr, new Mat(), new Point(-1, -1), 3);
            Cv2.Dilate(Blue_thr, Blue_thr, new Mat(), new Point(-1, -1), 3);
            Cv2.Dilate(BrownImg, Brown_thr, new Mat(), new Point(-1, -1), 3);

            Cv2.Erode(Brown_thr, Brown_thr, new Mat(), new Point(-1, -1), 3);
            Cv2.ImShow("Brown_thr", Brown_thr);
            Cv2.ImShow("White_thr", White_thr);
            Cv2.ImShow("Green_thr", Green_thr);
            Cv2.ImShow("BlueImg", BlueImg);
            // Cv2.ImShow("ROI", ROI);

            enhance.CopyTo(image);


           
            //train
            OpenCvSharp.CPlusPlus.Rect[] tree_Rect;
            Mat tree_detction = White_thr + Blue_thr;

            Cv2.Dilate(tree_detction, tree_detction, new Mat(), new Point(-1, -1), 3);
            Cv2.Erode(tree_detction, tree_detction, new Mat(), new Point(-1, -1), 3);
            tree_Rect = tree_cascade.DetectMultiScale(tree_detction, 1.1, 3, 0, new Size(50, 50));
            Cv2.ImShow("Train_detction", tree_detction);
            for (int i = 0; i < tree_Rect.Length; i++)
            {
                Point center = new Point(tree_Rect[i].X + tree_Rect[i].Width / 2, tree_Rect[i].Y + tree_Rect[i].Width / 2);
                if (Train_num < Max_Tree_Num)
                {
                    Train_Center[Train_num] = center;
                    Train_num++;
                }
                Cv2.Ellipse(enhance, center, new Size(tree_Rect[i].Width / 2, tree_Rect[i].Width / 2), 0, 0, 360, new Scalar(255, 0, 255), 2, OpenCvSharp.LineType.Link8, 0);
            }
           // Cv2.ImShow("enhance", enhance);
           // Cv2.ImWrite("enhance" + round.ToString() + ".jpg", enhance);

            int Brown_Length = 0;
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(Brown_thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0));
            List<List<Point>> contours_poly = new List<List<Point>>();
            for (int i = 0; i < contours.Length; i++)
                contours_poly.Add(new List<Point>());
            OpenCvSharp.CPlusPlus.Rect[] boundRect = new OpenCvSharp.CPlusPlus.Rect[contours.Length];
            OpenCvSharp.CPlusPlus.Rect[] Brown_Rect = new OpenCvSharp.CPlusPlus.Rect[contours.Length];


            int[] Brown_Num = new int[contours.Length];
            for (int i = 0; i < contours.Length; i++)
            {
                if (Cv2.ContourArea(contours[i]) > 2500 && Cv2.ContourArea(contours[i]) < 100000)
                {
                    //   print(Cv2.ContourArea(contours[i]));
                    Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly[i]), Cv2.ArcLength(InputArray.Create(contours[i]), true) * 0.02, true);
                    //  print(contours_poly[i].Count);
                    if (contours_poly[i].Count >= 4 && contours_poly[i].Count <= 7)
                    {
                        boundRect[i] = Cv2.BoundingRect(contours_poly[i]);
                        Cv2.DrawContours(image, contours, i, new Scalar(0, 0, 255), 2, LineType.Link8, hierarchy, 0, new Point());
                        Cv2.Rectangle(image, boundRect[i].TopLeft, boundRect[i].BottomRight, new Scalar(255, 0, 0));
                        Brown_Rect[i] = boundRect[i];
                        Brown_Num[Brown_Length] = i;
                        Brown_Length++;
                    }
                }
            }
            int Green_Length = 0;
            Cv2.FindContours(Green_thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0));
            List<List<Point>> contours_poly2 = new List<List<Point>>();
            for (int i = 0; i < contours.Length; i++)
                contours_poly2.Add(new List<Point>());
            OpenCvSharp.CPlusPlus.Rect[] boundRect2 = new OpenCvSharp.CPlusPlus.Rect[contours.Length];
            OpenCvSharp.CPlusPlus.Rect[] Green_Rect = new OpenCvSharp.CPlusPlus.Rect[contours.Length];
            int[] Green_Num = new int[contours.Length];
            for (int i = 0; i < contours.Length; i++)
            {
                if (Cv2.ContourArea(contours[i]) > 2500)
                {
                    Green_Num[Green_Length] = i;
                    Green_Length++;
                    Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly2[i]), Cv2.ArcLength(InputArray.Create(contours[i]), true) * 0.02, true);
                    //print(contours_poly2[i].Count);
                    if (contours_poly2[i].Count >= 9)
                    {
                        boundRect2[i] = Cv2.BoundingRect(contours_poly2[i]);
                        Cv2.DrawContours(image, contours, i, new Scalar(0, 0, 255), 2, LineType.Link8, hierarchy, 0, new Point());
                        Cv2.Rectangle(image, boundRect2[i].TopLeft, boundRect2[i].BottomRight, new Scalar(255, 0, 0));
                        Green_Rect[i] = boundRect2[i];
                    }

                }
            }

            //River
            Cv2.FindContours(Blue_thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0));
            List<List<Point>> contours_poly3 = new List<List<Point>>();
            for (int i = 0; i < contours.Length; i++)
                contours_poly3.Add(new List<Point>());
            OpenCvSharp.CPlusPlus.Rect[] boundRect3 = new OpenCvSharp.CPlusPlus.Rect[contours.Length];
            OpenCvSharp.CPlusPlus.Rect[] Blue_Rect = new OpenCvSharp.CPlusPlus.Rect[contours.Length];
            Point Blue_Center;
            bool river = false;
            for (int i = 0; i < contours.Length; i++)
            {
                // print(Cv2.ContourArea(contours[i]));
                if (Cv2.ContourArea(contours[i]) > 2000)
                {
                    Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly3[i]), Cv2.ArcLength(InputArray.Create(contours[i]), true) * 0.02, true);
                    //print(contours_poly3[i].Count);
                    if (contours_poly3[i].Count >= 7)
                    {
                        boundRect3[i] = Cv2.BoundingRect(contours_poly3[i]);
                        Cv2.DrawContours(image, contours, i, new Scalar(0, 0, 255), 2, LineType.Link8, hierarchy, 0, new Point());
                        Cv2.Rectangle(image, boundRect3[i].TopLeft, boundRect3[i].BottomRight, new Scalar(255, 0, 0));
                        Blue_Rect[i] = boundRect3[i];
                        Blue_Center = (Blue_Rect[i].BottomRight + Blue_Rect[i].TopLeft) * 0.5;
                        if (River_num < Max_Tree_Num)
                        {
                            River_Center[River_num] = Blue_Center;
                            River_num++;
                        }
                        river = true;

                    }
                }
            }
            Point Brown_Center = new Point(), Green_Center = new Point();
            double Min = Mathf.Infinity;
            int index = 0;

            for (int i = 0; i < Brown_Length; i++)
            {
                Min = Mathf.Infinity;
                Brown_Center = (Brown_Rect[Brown_Num[i]].BottomRight + Brown_Rect[Brown_Num[i]].TopLeft) * 0.5;
                for (int j = 0; j < Green_Length; j++)
                {
                    Green_Center = (Green_Rect[Green_Num[j]].BottomRight + Green_Rect[Green_Num[j]].TopLeft) * 0.5;
                    if (Brown_Rect[Brown_Num[i]].Intersect(Green_Rect[Green_Num[j]]) != OpenCvSharp.CPlusPlus.Rect.Empty)
                    {
                        if (Brown_Center.DistanceTo(Green_Center) < Min)
                        {
                            Min = Brown_Center.DistanceTo(Green_Center);
                            index = Green_Num[j];
                            tree = true;
                        }
                        else
                            tree = false;

                    }
                }
                if (tree && Tree_num < Max_Tree_Num)
                {

                    Green_Center = (Green_Rect[index].BottomRight + Green_Rect[index].TopLeft) * 0.5;
                    Tree_Center[Tree_num] = (Brown_Center + Green_Center) * 0.5;
                    int alltree_x=Mathf.Min(Green_Rect[index].TopLeft.X, Brown_Rect[Brown_Num[i]].TopLeft.X);
                    int alltree_y=Mathf.Min(Green_Rect[index].TopLeft.Y, Brown_Rect[Brown_Num[i]].TopLeft.Y);
                    int tx = Mathf.Max(Green_Rect[index].BottomRight.X, Brown_Rect[Brown_Num[i]].BottomRight.X);
                    int ty = Mathf.Max(Green_Rect[index].BottomRight.Y, Brown_Rect[Brown_Num[i]].BottomRight.Y);
                    Cv2.Rectangle(enhance, new Point(alltree_x, alltree_y), new Point(tx, ty), new Scalar(255,0, 255), 5);
                    All_Tree[Tree_num] = new OpenCvSharp.CPlusPlus.Rect(alltree_x,alltree_y,tx,ty);
                    Tree_num++;
                }

            }
            Cv2.ImShow("enhance", enhance);
            Cv2.ImShow("Tree detection", image);
            //update
            //River
            bool complete = false;
            for (int i = 0; i < River_num; i++)
            {
                complete = false;
                for (int j = 0; j < Final_River_Num; j++)
                {
                    if (Math.Abs(River_Center[i].X - Final_River_Center[j].X) < 10 && Math.Abs(River_Center[i].Y - Final_River_Center[j].Y) < 10)
                    {
                        Final_River_Center[j] = (Final_River_Center[j] + River_Center[i]) * 0.5;
                        River_Times[j]++;
                        complete = true;
                    }
                }
                if (!complete && Final_River_Num < Max_Tree_Num)
                {
                    Final_River_Center[Final_River_Num] = River_Center[i];
                    River_Times[Final_River_Num]++;
                    Final_River_Num++;
                }
            }
            //Tree
            for (int i = 0; i < Tree_num; i++)
            {
                complete = false;
                for (int j = 0; j < Final_Tree; j++)
                {
                    if (Math.Abs(Tree_Center[i].X - Final_Tree_Center[j].X) < 10 && Math.Abs(Tree_Center[i].Y - Final_Tree_Center[j].Y) < 10)
                    {
                        Final_Tree_Center[j] = (Final_Tree_Center[j] + Tree_Center[i]) * 0.5;
                        Tree_Times[j]++;
                        complete = true;
                    }
                }
                if (!complete && Final_Tree < Max_Tree_Num)
                {
                    Final_Tree_Center[Final_Tree] = Tree_Center[i];
                    Final_All_Tree[Final_Tree] = All_Tree[i];
                    Tree_Times[Final_Tree]++;
                    Final_Tree++;
                }
            }
            //Train
            for (int i = 0; i < Train_num; i++)
            {
              //  print(round + ": " + Train_Center[i]);
                complete = false;
                for (int j = 0; j < Final_Train_Num; j++)
                {

                    if (Math.Abs(Train_Center[i].X - Final_Train_Center[j].X) < 10 && Math.Abs(Train_Center[i].Y - Final_Train_Center[j].Y) < 10)
                    {
                        Final_Train_Center[j] = (Final_Train_Center[j] + Train_Center[i]) * 0.5;
                       // Trunk_Height[j]= Trunk_Height[j]+ tree_Rect[i].Height
                        Train_Times[j]++;
                        complete = true;
                    }
                }
                if (!complete && Final_Train_Num < Max_Tree_Num)
                {
                    Final_Train_Center[Final_Train_Num] = Train_Center[i];
                    Train_Times[Final_Train_Num]++;
                    Final_Train_Num++;
                }
                //else if(!complete && Final_Train_Num >= Max_Tree_Num && Train_Times[Final_Train_Num%10]<6)
                //{
                //    Final_Train_Center[Final_Train_Num%10] = Train_Center[i];
                //    Train_Times[Final_Train_Num]++;
                //    Final_Train_Num++;
                //}
            }
            //    Cv2.ImShow("Fecture", image);
            //    //end = Time.time;
            //    //float t = end - start;
            //    //if (tree || river)
            //    //    find = true;
            //    //if (find)
            //    //{
            //    //    // Cv2.ImShow("Brown_thr", Green_thr);

            //    //    mode = modeType.Draw_Mode;
            //    //    find = false;
            //    //    if (temp == false)
            //    //    {
            //    //        Hint_2.SetActive(true);
            //    //        temp = true;
            //    //    }

            //    //}
            //x 90 y 50
            if (round == 9)
            {
                Point[] Merge_Center = new Point[Max_Tree_Num];
                int Merge_num = 0;
                bool[] complete2= new bool[Max_Tree_Num];
                for (int i = 0; i < Final_Train_Num; i++)
                {
                    if (Train_Times[i] > 6 && !complete2[i])
                    {
                        for (int j = 0; j < Final_Train_Num; j++)
                        {
                            if (j == i) continue;
                            if ( Math.Abs(Final_Train_Center[i].X - Final_Train_Center[j].X) < 50 && Math.Abs(Final_Train_Center[i].Y - Final_Train_Center[j].Y) < 90)
                            {
                                
                                complete2[j] = true;
                            }
                                
                        }
                        Merge_Center[Merge_num] = Final_Train_Center[i];
                        Merge_num++;
                    }
                }
                print(Final_Train_Num + " M:" + Merge_num);
                for (int i = 0; i < Merge_num; i++)
                {
                    //print(Merge_Center[i]);
                    //check_position(treemodel, Merge_Center[i]);
                }
              
                for (int i = 0; i < Final_River_Num; i++)
                {
                    if (River_Times[i] > 6)
                    {
                        check_position(rivermodel, Final_River_Center[i]);
                    }

                }
                for (int i = 0; i < Final_Tree; i++)
                {
                    print(Final_Tree_Center[i]);
                    if (Tree_Times[i] > 6)
                    {
                        check_position(treemodel, Final_Tree_Center[i]);
                    }
                }
                        //  int num = 0, center_num = 0, River_center_num = 0;
                        //  bool[][] complete = new bool[10][];
                        //  bool[][] complete2 = new bool[10][];
                        //  for (int i = 0; i < 10; i++)
                        //  {
                        //      complete2[i] = new bool[10];
                        //      complete[i] = new bool[10];
                        //  }
                        //  //Tree 
                        //  for (int i = 0; i < 10; i++)
                        //  {
                        //      for (int j = 0; j < Tree_num[i]; j++)
                        //      {
                        //          num = 0;
                        //          if (!complete[i][j])
                        //          {
                        //              complete[i][j] = true;
                        //              for (int k = 0; k < 10; k++)
                        //              {
                        //                  if (k == i) continue;
                        //                  for (int l = 0; l < Tree_num[k]; l++)
                        //                  {
                        //                      if (!complete[k][l])
                        //                      {
                        //                          if (Math.Abs(Tree_Center[i][j].X - Tree_Center[k][l].X) < 10 && Math.Abs(Tree_Center[i][j].Y - Tree_Center[k][l].Y) < 10)
                        //                          {
                        //                              complete[k][l] = true;
                        //                              num++;
                        //                          }
                        //                      }
                        //                  }
                        //              }
                        //              if (num >= 3)
                        //              {
                        //                  Final_Tree_Center[center_num] = Tree_Center[i][j];
                        //                  center_num++;
                        //              }
                        //          }
                        //      }
                        //  }

                        //  //River
                        //  for (int i = 0; i < 10; i++)
                        //  {
                        //      for (int j = 0; j < River_num[i]; j++)
                        //      {
                        //          num = 0;
                        //          if (!complete2[i][j])
                        //          {
                        //              complete2[i][j] = true;
                        //              for (int k = 0; k < 10; k++)
                        //              {
                        //                  if (k == i) continue;
                        //                  for (int l = 0; l < River_num[k]; l++)
                        //                  {
                        //                      if (!complete2[k][l])
                        //                      {
                        //                          if (Math.Abs(River_Center[i][j].X - River_Center[k][l].X) < 10 && Math.Abs(River_Center[i][j].Y - River_Center[k][l].Y) < 10)
                        //                          {
                        //                              complete2[k][l] = true;
                        //                              num++;
                        //                          }
                        //                      }
                        //                  }
                        //              }
                        //              if (num >= 3)
                        //              {
                        //                  Final_River_Center[River_center_num] = River_Center[i][j];
                        //                  River_center_num++;
                        //              }
                        //          }
                        //      }
                        //  }

                        //  print(Final_Tree);
                        //   print(Final_River);
                        //  for (int i = 0; i < center_num; i++)
                        //  {
                        //      double pox, poy, ay, bx, cx, cy, a, b;
                        //      pox = Final_Tree_Center[i].X;
                        //      poy = Final_Tree_Center[i].Y;
                        //      ay = 480f;
                        //      bx = 640f;
                        //      cx = pox;
                        //      cy = poy;

                        //      a = cy / ay;
                        //      b = cx / bx;

                        //      GameObject newtree = (GameObject)Instantiate(treemodel);
                        //      newtree.transform.parent = imagetarget.transform;
                        //      newtree.transform.position = new Vector3(zero.transform.position.x + (0.7f * (float)a * 50f), zero.transform.position.y, zero.transform.position.z - (1f * (float)b * 50f));
                        //      //newtree.SetActive(true);
                        //      newtree.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                        //      print(Final_Tree_Center[i]);
                        //  }
                        //  for (int i = 0; i < River_center_num; i++)
                        //  {
                        //      double pox, poy, ay, bx, cx, cy, a, b;
                        //      pox = Final_River_Center[i].X;
                        //      poy = Final_River_Center[i].Y;
                        //      ay = 480f;
                        //      bx = 640f;
                        //      cx = pox;
                        //      cy = poy;

                        //      a = cy / ay;
                        //      b = cx / bx;
                        //      GameObject newriver = (GameObject)Instantiate(rivermodel);
                        //      newriver.transform.parent = imagetarget.transform;
                        //      newriver.transform.position = new Vector3(zero.transform.position.x + (0.7f * (float)a * 50f), -4.19f, zero.transform.position.z - (1f * (float)b * 50f));
                        //      //newriver.SetActive(true);
                        //      newriver.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
                        //      print(Final_River_Center[i]);
                        //  }
                        end = Time.time;
                mode = modeType.Draw_Mode;
                find = false;
                if (temp == false)
                {
                    Hint_2.SetActive(true);
                    temp = true;
                }
                print(end - start);
            }
            round++;

        }

    }
    
    public void Check_Button()
    {
        start = Time.time;
        if (find == false)
        {
            for (int i = 0; i < imagetarget.transform.childCount; i++)
            {
                GameObject go = imagetarget.transform.GetChild(i).gameObject;
                Destroy(go);
            }
            mode = modeType.Background;
        }

        else if (light_check == false)
            mode = modeType.Light_Check;
        else if (first_check == false)
            mode = modeType.First_Check;
        else if (second_check == false)
            mode = modeType.Second_Check;
        else if (third_check == false)
            mode = modeType.Third_Check;
    }

    public void Next_Button()
    {
        if (back == false)
        {
            if (find == false)
            {
                find = true;
                Hint_3.SetActive(true);
                temp = false;
            }
                
            else if (light_check == false)
                light_check = true;
            if (find && light_check)
            {
                back = true;
                nextbtn.SetActive(false);
                Hint_4.SetActive(true);
            }
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        if (back==true)
        {
            if (first_check && second_check && third_check)
            {
                gameObject.SetActive(false);
                detectbtn.SetActive(false);
                page2.SetActive(true);
            }
        }
        GUILayout.EndVertical();
    }

    void check_position(GameObject background, Point position)
    {
        double pox, poy, ay, bx, cx, cy, a, b;
        pox = position.X;
        poy = position.Y;
        ay = 480f;
        bx = 640f;
        cx = pox;
        cy = poy;
        a = cy / ay;
        b = cx / bx;
        GameObject new_background = (GameObject)Instantiate(background);
        new_background.transform.parent = imagetarget.transform;
        if(background==treemodel)
            new_background.transform.position = new Vector3(zero.transform.position.x + (0.52f * (float)a * 50f), zero.transform.position.y, zero.transform.position.z - (0.76f * (float)b * 50f));
        else
            new_background.transform.position = new Vector3(zero.transform.position.x + (0.52f * (float)a * 50f), -4.19f, zero.transform.position.z - (0.76f * (float)b * 50f));
       // new_background.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
    }
    Mat Perspective_Transform(Mat image, float zerox, float zeroy, float rightx, float righty, float downrightx, float downrighty, float downx, float downy)
    {
        Point2f[] corners = new Point2f[4];
        Point2f[] corners_trans = new Point2f[4];
        corners[0] = new Point2f(zerox, zeroy);
        corners[1] = new Point2f(rightx, righty);
        corners[2] = new Point2f(downrightx, downrighty);
        corners[3] = new Point2f(downx, downy);
        corners_trans[0] = new Point2f(0, 0);
        corners_trans[1] = new Point2f(0, 480);
        corners_trans[2] = new Point2f(640, 480);
        corners_trans[3] = new Point2f(640, 0);

        Mat transform = Cv2.GetPerspectiveTransform(corners, corners_trans);
        Mat newimage = image.WarpPerspective(transform, image.Size(), Interpolation.Linear, BorderType.Constant, 0);
        return newimage;
    }

    Mat color_detection(Mat inputimage, Scalar min, Scalar max)
    {
        Mat hsvImg = new Mat(), filter = new Mat(), thr = new Mat();
        Cv2.GaussianBlur(inputimage, inputimage, Cv.Size(3, 3), 0, 0);
        Cv2.CvtColor(inputimage, hsvImg, ColorConversion.BgrToHsv);
        
        Cv2.InRange(hsvImg, min, max, thr);
        //Cv2.Threshold(filter, thr, 125, 255, ThresholdType.Binary); // Threshold the gray
        //Cv2.Erode(thr, thr, new Mat(), new Point(-1, -1), 3);
        Cv2.Dilate(thr, thr, new Mat(), new Point(-1, -1), 3);
        Cv2.ImShow("im", thr);
        return thr;
    }

    bool check_position1(Mat thr, Mat dst, GameObject material, bool check)
    {
        Point[][] contours; // Vector for storing contour
        List<List<Point>> contours_poly = new List<List<Point>>();
        HierarchyIndex[] hierarchy;
        Cv2.FindContours(thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0)); // Find the contours in the image
        Cv2.DrawContours(thr, contours, 0, new Scalar(255, 255, 255), 5, LineType.Link8, hierarchy, int.MaxValue, new Point(0, 0));
        for (int j = 0; j < contours.Length; j++)
            contours_poly.Add(new List<Point>());
        int i = 0;
        for (i = 0; i < contours.Length; i++)
        {
            //Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly[i]), 3, true);
            Point[] poly = Cv2.ApproxPolyDP(contours[i], 10, true);
            print(poly.Length);
            if (poly.Length == 5)
            {

                Cv2.Line(dst, poly[0], poly[1], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[1], poly[2], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[2], poly[3], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[3], poly[4], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[4], poly[0], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                double pox, poy, ay, bx, cx, cy, a, b;
                pox = (poly[0].X + poly[1].X + poly[2].X + poly[3].X + poly[4].X) / 5;
                poy = (poly[0].Y + poly[1].Y + poly[2].Y + poly[3].Y + poly[4].Y) / 5;
                ay = 480f;
                bx = 640f;
                cx = pox;
                cy = poy;

                a = cy / ay;
                b = cx / bx;

                material.transform.position = new Vector3(zero.transform.position.x + (0.52f * (float)a * 50f), zero.transform.position.y, zero.transform.position.z - (0.76f * (float)b * 50f));
                print((zero.transform.position.x + (0.52f * (float)a * 50f)) + " " + (zero.transform.position.z - (0.76f * (float)b * 50f)));
                print(pox + " " + poy);
                print(a + " " + b);
                print(zero.transform.position.x + " " + zero.transform.position.z);
                material.SetActive(true);
                check = true;
                break;
            }
        }
        Cv2.ImShow("dst", dst);
        return check;
    }
    bool check_position2(Mat thr, Mat dst, GameObject material, bool check)
    {
        Mat ROI = new Mat();
        Point[][] contours; // Vector for storing contour
        List<List<Point>> contours_poly = new List<List<Point>>();
        HierarchyIndex[] hierarchy;
        Cv2.FindContours(thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0)); // Find the contours in the image
        Cv2.DrawContours(thr, contours, 0, new Scalar(255, 255, 255), 5, LineType.Link8, hierarchy, int.MaxValue, new Point(0, 0));
        for (int j = 0; j < contours.Length; j++)
            contours_poly.Add(new List<Point>());
        int i = 0;
        for (i = 0; i < contours.Length; i++)
        {
            //Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly[i]), 3, true);
            Point[] poly = Cv2.ApproxPolyDP(contours[i], 10, true);
            if (poly.Length == 4)
            {

                OpenCvSharp.CPlusPlus.Rect boundRect4;
                boundRect4= Cv2.BoundingRect(contours[i]);
                Cv2.CvtColor(thr, thr, ColorConversion.GrayToBgr);
                dst = dst + thr;
                ROI = new Mat(dst, new OpenCvSharp.CPlusPlus.Rect(boundRect4.X, boundRect4.Y, boundRect4.Width, boundRect4.Height));
                ROI = color_detection(ROI, new Scalar(0, 129, 208), new Scalar(180, 179, 255));



                Cv2.ImShow("ROI", ROI);
                Cv2.Line(dst, poly[0], poly[1], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[1], poly[2], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[2], poly[3], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[3], poly[0], new Scalar(0, 0, 255), 3, LineType.Link8, 0);

                double pox, poy, ay, bx, cx, cy, a, b;
                pox = (poly[0].X + poly[1].X + poly[2].X + poly[3].X) / 4;
                poy = (poly[0].Y + poly[1].Y + poly[2].Y + poly[3].Y) / 4;
                ay = 480f;
                bx = 640f;
                cx = pox;
                cy = poy;

                a = cy / ay;
                b = cx / bx;

                material.transform.position = new Vector3(zero.transform.position.x + (0.52f * (float)a * 50f), zero.transform.position.y, zero.transform.position.z - (0.76f * (float)b * 50f));
                print((zero.transform.position.x + (0.52f * (float)a * 50f)) + " " + (zero.transform.position.z - (0.76f * (float)b * 50f)));
                print(pox + " " + poy);
                print(a + " " + b);
                print(zero.transform.position.x + " " + zero.transform.position.z);
                material.SetActive(true);
                check = true;
                break;
                if (check == true)
                    break;
            }
        }
        Cv2.ImShow("dst", dst);
        
        return check;
    }
    bool check_position3(Mat thr, Mat dst, GameObject material, bool check)
    {
        Point[][] contours; // Vector for storing contour
        List<List<Point>> contours_poly = new List<List<Point>>();
        HierarchyIndex[] hierarchy;
        Cv2.FindContours(thr, out contours, out hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple, new Point(0, 0)); // Find the contours in the image
        Cv2.DrawContours(thr, contours, 0, new Scalar(255, 255, 255), 5, LineType.Link8, hierarchy, int.MaxValue, new Point(0, 0));
        for (int j = 0; j < contours.Length; j++)
            contours_poly.Add(new List<Point>());
        int i = 0;
        for (i = 0; i < contours.Length; i++)
        {
            //Cv2.ApproxPolyDP(InputArray.Create(contours[i]), OutputArray.Create(contours_poly[i]), 3, true);
            Point[] poly = Cv2.ApproxPolyDP(contours[i], 10, true);
            if (poly.Length == 3)
            {

                Cv2.Line(dst, poly[0], poly[1], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[1], poly[2], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                Cv2.Line(dst, poly[2], poly[0], new Scalar(0, 0, 255), 3, LineType.Link8, 0);
                double pox, poy, ay, bx, cx, cy, a, b;
                pox = (poly[0].X + poly[1].X + poly[2].X) / 3;
                poy = (poly[0].Y + poly[1].Y + poly[2].Y) / 3;
                ay = 480f;
                bx = 640f;
                cx = pox;
                cy = poy;

                a = cy / ay;
                b = cx / bx;

                material.transform.position = new Vector3(zero.transform.position.x + (0.52f * (float)a * 50f), zero.transform.position.y, zero.transform.position.z - (0.76f * (float)b * 50f));
                print((zero.transform.position.x + (0.52f * (float)a * 50f)) + " " + (zero.transform.position.z - (0.76f * (float)b * 50f)));
                print(pox + " " + poy);
                print(a + " " + b);
                print(zero.transform.position.x + " " + zero.transform.position.z);
                material.SetActive(true);
                check = true;
                break;
            }
        }
        Cv2.ImShow("dst", dst);
        return check;
    }

    


}
