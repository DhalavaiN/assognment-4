using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.VFW;

namespace Aforge_Web_Cam
{
public partial class VideoForm : Form
{
    private FilterInfoCollection VideoCaptureDevices;
    private VideoCaptureDevice FinalVideo = null;
    private VideoCaptureDeviceForm captureDevice;
    private Bitmap video;
    private AVIWriter AVIwriter = new AVIWriter();
    private SaveFileDialog saveAvi;

    public VideoForm()
    {
        InitializeComponent();
    }

    private void VideoForm_Load(object sender, EventArgs e)
    {
        VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        captureDevice = new VideoCaptureDeviceForm();
    }

    private void butStart_Click(object sender, EventArgs e)
    {
        if (captureDevice.ShowDialog(this) == DialogResult.OK)
        {
            VideoCaptureDevice videoSource = captureDevice.VideoDevice;
            FinalVideo = captureDevice.VideoDevice;
            FinalVideo.NewFrame += new NewFrameEventHandler(FinalVideo_NewFrame);
            FinalVideo.Start();
        }
    }

    void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
        if (butStop.Text == "Stop Record")
        {
            video = (Bitmap)eventArgs.Frame.Clone();
            pbVideo.Image = (Bitmap)eventArgs.Frame.Clone();
            AVIwriter.Quality = 0;
            AVIwriter.AddFrame(video);
        }
        else
        {
            video = (Bitmap)eventArgs.Frame.Clone();
            pbVideo.Image = (Bitmap)eventArgs.Frame.Clone();
        }
    }

    private void butRecord_Click(object sender, EventArgs e)
    {
        saveAvi = new SaveFileDialog();
        saveAvi.Filter = "Avi Files (*.avi)|*.avi";
        if (saveAvi.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            int h = captureDevice.VideoDevice.VideoResolution.FrameSize.Height;
            int w = captureDevice.VideoDevice.VideoResolution.FrameSize.Width;
            AVIwriter.Open(saveAvi.FileName, w, h);
            butStop.Text = "Stop Record";
            //FinalVideo = captureDevice.VideoDevice;
            //FinalVideo.NewFrame += new NewFrameEventHandler(FinalVideo_NewFrame);
            //FinalVideo.Start();
        }
    }

    private void butStop_Click(object sender, EventArgs e)
    {
        if (butStop.Text == "Stop Record")
        {
            butStop.Text = "Stop";
            if (FinalVideo == null)
            { return; }
            if (FinalVideo.IsRunning)
            {
                //this.FinalVideo.Stop();
                this.AVIwriter.Close();
                pbVideo.Image = null;
            }
        }
        else
        {
            this.FinalVideo.Stop();
            this.AVIwriter.Close();
            pbVideo.Image = null;
        }
    }

    private void butCapture_Click(object sender, EventArgs e)
    {
        pbVideo.Image.Save("IMG" + DateTime.Now.ToString("hhmmss") + ".jpg");
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void VideoForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (FinalVideo == null)
        { return; }
        if (FinalVideo.IsRunning)
        {
            this.FinalVideo.Stop();
            this.AVIwriter.Close();
        }
    }
}
}
