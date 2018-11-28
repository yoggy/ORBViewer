using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;

public class ORBViewerScript : MonoBehaviour {


	public Message message;
	public RawImage rawImage;

	int capture_w = 640;
	int capture_h = 480;

	WebCamTexture webcam_texture;
	Texture2D result_texuture;

	Mat capture_mat;
	Mat debug_mat;

	MatOfKeyPoint keypoints;
	Mat descriptors;

	public enum Mode {NORMAL, FEATURE_POINTS, WIRE_FRAME, FEATURE_POINTS_AND_WIREFRAME};
	Mode mode = Mode.WIRE_FRAME;

	void Start()
    {
        mode = (Mode)System.Enum.Parse(typeof(Mode), PlayerPrefs.GetString("mode", "NORMAL"));

        if (WebCamTexture.devices.Length == 0)
		{
			message.SetMessage("WebCamTexture.devices.Length == 0");
            return;
		}

		WebCamDevice dev = WebCamTexture.devices[0];
		webcam_texture = new WebCamTexture(dev.name, capture_w,  capture_h, 60);
		Debug.Log("open dev.name=" + dev.name);

		webcam_texture.Play();

    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("mode", mode.ToString());
    }

    void Update () {
		// webcam_textureがアップデートされるまでは処理しない
		if (webcam_texture == null || webcam_texture.didUpdateThisFrame == false) return;

		DetectKeypoints();
		UpdateResultTexture();
	}

	void DetectKeypoints()
	{
        if (capture_mat == null) {
            int w = webcam_texture.width;
            int h = webcam_texture.height;
            capture_mat = new Mat(h, w, CvType.CV_8UC3);
            debug_mat = new Mat(h, w, CvType.CV_8UC3);
            result_texuture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            rawImage.texture = result_texuture;

            message.SetMessage("w=" + w + ", h=" + h);
        }

        Utils.webCamTextureToMat(webcam_texture, capture_mat);

		// 特徴点＆特徴量の抽出 (デフォルトは500)
		ORB detector = ORB.create(500);
		ORB extractor = ORB.create(500);

		keypoints = new MatOfKeyPoint();
		descriptors = new Mat();

		detector.detect (capture_mat, keypoints);
		extractor.compute(capture_mat, keypoints, descriptors);

		// ORBのデフォルトの場合、rows<= 500, cols=32 (rowsは特徴点の数、colsは特徴量)
		//Debug.Log("descriptors rows=" + descriptors.rows() + ", cols=" + descriptors.cols());
	}

	void DrawEdge(Mat src, Mat dst)
	{
		Mat gray_img = new Mat();
		Mat edge_img = new Mat();

		Imgproc.cvtColor(src, gray_img, Imgproc.COLOR_BGR2GRAY);
		Imgproc.Canny(gray_img, edge_img, 100, 200); 
		Imgproc.cvtColor(edge_img, dst, Imgproc.COLOR_GRAY2BGR);
	}

	void DrawKeypoints(Mat img)
	{
		KeyPoint [] kps = keypoints.toArray();
		foreach(var kp in kps) 
		{
			Imgproc.circle(img, kp.pt, 5, new Scalar (255, 0, 128), -1);
		}
	}

	void UpdateResultTexture()
	{
		if (mode == Mode.NORMAL) {
			capture_mat.copyTo(debug_mat);
		}
		else if (mode == Mode.FEATURE_POINTS) {
			capture_mat.copyTo(debug_mat);
			DrawKeypoints(debug_mat);
		}
		else if (mode == Mode.WIRE_FRAME) 
		{
			DrawEdge(capture_mat, debug_mat);
		}
		else if (mode == Mode.FEATURE_POINTS_AND_WIREFRAME) 
		{
			DrawEdge(capture_mat, debug_mat);
			DrawKeypoints(debug_mat);
		}

		Utils.matToTexture2D (debug_mat, result_texuture);
	}

	public void ToggleMode()
	{
		switch(mode) {
			case Mode.NORMAL:
				mode = Mode.FEATURE_POINTS;
				break;
			case Mode.FEATURE_POINTS:
				mode = Mode.WIRE_FRAME;
				break;
			case Mode.WIRE_FRAME:
				mode = Mode.FEATURE_POINTS_AND_WIREFRAME;
				break;
			case Mode.FEATURE_POINTS_AND_WIREFRAME:
				mode = Mode.NORMAL;
				break;
		}
	}
}
