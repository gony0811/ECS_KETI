using Basler.Pylon;
using ECS.UI.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace ECS.UI.ViewModel
{
    public class VisionCameraViewModel : ViewModelBase
    {
        internal class DistanceMeasure
        {
            public bool IsCalculate { get; set; }
            public bool PointInput_1 { get; set; }
            public bool PointInput_2 { get; set; }


            public int Pixel_X1 { get; set; }
            public int Pixel_Y1 { get; set; }
            public int Pixel_X2 { get; set; }
            public int Pixel_Y2 { get; set; }
            public double PixelSize { get; set; }

            public double Distance { get; private set; }

            public void PointInput1(int x1, int y1)
            {
                Pixel_X1 = x1;
                Pixel_Y1 = y1;
                PointInput_1 = true;
            }

            public void PointInput2(int x2, int y2)
            {
                Pixel_X2 = x2;
                Pixel_Y2 = y2;
                PointInput_2 = true;
            }

            public DistanceMeasure(double pixelSize)
            {
                PointInput_1 = false;
                PointInput_2 = false;
                IsCalculate = false;
                PixelSize = pixelSize;
            }

            public void Reset()
            {
                PointInput_1 = false;
                PointInput_2 = false;
                IsCalculate = false;
            }

            public void Calculate()
            {
                if (PointInput_1 && PointInput_2)
                {
                    double dist = Math.Sqrt((Math.Pow(Pixel_X1 - Pixel_X2, 2) + Math.Pow(Pixel_Y1 - Pixel_Y2, 2)));
                    Distance = dist * PixelSize;
                    IsCalculate = true;
                }
            }
        }

        private DistanceMeasure distanceMeasure;
        private Dispatcher dispatcher = null;

        private Camera camera = null;
        private ImageSource bitmapSource;
        private PixelDataConverter converter = new PixelDataConverter();
        private Stopwatch stopWatch = new Stopwatch();
        private ObservableCollection<VisionCamera> visionCameraList;
        private VisionCamera selectedVisionCamera;
        private Timer updateDeviceListTimer;
        private System.Threading.Thread usbCamera;

        private VideoCapture capture;
        private Mat frame;
        private Bitmap image;
        private bool isUSBCameraRunning = false;

        private bool isDistanceMeasureMode = false;

        private bool buttonOneShotEnabled;
        private string buttonOneShotContent;

        private bool buttonContinuousEnabled;
        private string buttonContinuousContent;

        private bool buttonStopGrabEnabled;
        private string buttonStopGrabContent;

        private bool buttonUSBCameraEnabled;
        private string buttonUSBCameraContent;

        private bool buttonDistanceEnabled;
        private string buttonDistanceContent;

        private int labelMouseXPosition;
        private int labelMouseYPosition;

        public int MouseXPosition { get { return labelMouseXPosition; } set { labelMouseXPosition = value; RaisePropertyChanged("MouseXPosition"); } }
        public int MouseYPosition { get { return labelMouseYPosition; } set { labelMouseYPosition = value; RaisePropertyChanged("MouseYPosition"); } }


        private ICommand _MouseMoveCommand;

        public ICommand MouseMoveCommand { get { return this._MouseMoveCommand ?? (this._MouseMoveCommand = new RelayCommand<MouseEventArgs>(ExecuteMouseMoveCommand)); } }

        private ICommand _MouseLeftButtonDownCommand;

        public ICommand MouseLeftButtonDownCommand { get { return this._MouseLeftButtonDownCommand ?? (this._MouseLeftButtonDownCommand = new RelayCommand<MouseEventArgs>(ExecuteMouseLeftButtonDownCommand)); } }

        private ICommand _LoadedCommand;
        private ICommand _UnloadedCommand;

        public ICommand LoadedCommand { get { return this._LoadedCommand ?? (this._LoadedCommand = new RelayCommand(ExecuteLoadedCommand)); } }
        public ICommand UnloadedCommand { get { return this._UnloadedCommand ?? (this._UnloadedCommand = new RelayCommand(ExecuteUnloadedCommand)); } }

        private ICommand visionCameraChangedCommand;

        public ICommand VisionCameraChangedCommand
        {
            get
            {
                if (visionCameraChangedCommand == null)
                {
                    visionCameraChangedCommand = new DelegateCommand(VisionCameraChanged);
                }

                return visionCameraChangedCommand;
            }
        }

        public void Start()
        {
            // Destroy the old camera object.
            if (camera != null)
            {
                GrabStop();
                DestroyCamera();

                VisionCameraChanged();
                EnableButtons(false, false);
            }
        }

        public void Stop()
        {
            // Destroy the old camera object.
            if (camera != null)
            {
                GrabStop();
                DestroyCamera();
            }
        }

        private void ExecuteMouseMoveCommand(MouseEventArgs args)
        {
            if (isDistanceMeasureMode)
            {
                System.Windows.Point point = args.GetPosition((IInputElement)args.Source);

                MouseXPosition = Convert.ToInt32(point.X);
                MouseYPosition = Convert.ToInt32(point.Y);
            }
        }

        private void ExecuteMouseLeftButtonDownCommand(MouseEventArgs args)
        {
            if (isDistanceMeasureMode)
            {
                System.Windows.Point point = args.GetPosition((IInputElement)args.Source);

                if (distanceMeasure.PointInput_1 != true)
                {
                    distanceMeasure.PointInput1(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
                }
                else if (distanceMeasure.PointInput_2 != true)
                {
                    distanceMeasure.PointInput2(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
                    distanceMeasure.Calculate();
                }
            }
        }

        private void ExecuteLoadedCommand()
        {
            //Start();        
        }

        private void ExecuteUnloadedCommand()
        {

        }

        private void VisionCameraChanged()
        {
            // Destroy the old camera object.
            if (camera != null)
            {
                GrabStop();
                DestroyCamera();
            }


            // Open the connection to the selected camera device.
            if (visionCameraList.Count > 0)
            {
                try
                {
                    // Create a new camera object.
                    // Get the first selected item.
                    VisionCamera item = visionCameraList[0];
                    // Get the attached device data.
                    ICameraInfo selectedCamera = item.CameraInfo as ICameraInfo;
                    camera = new Camera(selectedCamera);

                    camera.CameraOpened += Configuration.AcquireContinuous;

                    // Register for the events of the image provider needed for proper operation.
                    camera.ConnectionLost += OnConnectionLost;
                    camera.CameraOpened += OnCameraOpened;
                    camera.CameraClosed += OnCameraClosed;
                    camera.StreamGrabber.GrabStarted += OnGrabStarted;
                    camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                    camera.StreamGrabber.GrabStopped += OnGrabStopped;

                    // Open the connection to the camera device.
                    camera.Open();

                    //// Set the parameter for the controls.
                    //if (camera.Parameters[PLCamera.TestImageSelector].IsWritable)
                    //{
                    //    testImageControl.Parameter = camera.Parameters[PLCamera.TestImageSelector];
                    //}
                    //else
                    //{
                    //    testImageControl.Parameter = camera.Parameters[PLCamera.TestPattern];
                    //}
                    //pixelFormatControl.Parameter = camera.Parameters[PLCamera.PixelFormat];
                    //widthSliderControl.Parameter = camera.Parameters[PLCamera.Width];
                    //heightSliderControl.Parameter = camera.Parameters[PLCamera.Height];
                    //if (camera.Parameters.Contains(PLCamera.GainAbs))
                    //{
                    //    gainSliderControl.Parameter = camera.Parameters[PLCamera.GainAbs];
                    //}
                    //else
                    //{
                    //    gainSliderControl.Parameter = camera.Parameters[PLCamera.Gain];
                    //}
                    //if (camera.Parameters.Contains(PLCamera.ExposureTimeAbs))
                    //{
                    //    exposureTimeSliderControl.Parameter = camera.Parameters[PLCamera.ExposureTimeAbs];
                    //}
                    //else
                    //{
                    //    exposureTimeSliderControl.Parameter = camera.Parameters[PLCamera.ExposureTime];
                    //}
                }
                catch (Exception exception)
                {
                    ShowException(exception);
                }
            }
        }

        public VisionCamera SelectedVisionCamera
        {
            get { return selectedVisionCamera; }
            set { selectedVisionCamera = value; }
        }

        public ObservableCollection<VisionCamera> VisionCameraList
        {
            get { return visionCameraList; }
            set { visionCameraList = value; }
        }

        public bool ButtonOneShotEnabled
        {
            get { return buttonOneShotEnabled; }
            set 
            {
                buttonOneShotEnabled = value;
                RaisePropertyChanged("ButtonOneShotEnabled"); 
            }
        }

        public string ButtonOneShotContent
        {
            get { return buttonOneShotContent; }
            set
            {
                buttonOneShotContent = value;
                // RaisePropertyChanged MUST fire the same case-sensitive name of property
                RaisePropertyChanged("ButtonOneShotContent");
            }
        }

        public bool ButtonContinuousEnabled
        {
            get { return buttonContinuousEnabled; }
            set
            {
                buttonContinuousEnabled = value;
                RaisePropertyChanged("ButtonContinuousEnabled");
            }
        }

        public string ButtonContinuousContent
        {
            get { return buttonContinuousContent; }
            set
            {
                buttonContinuousContent = value;
                // RaisePropertyChanged MUST fire the same case-sensitive name of property
                RaisePropertyChanged("ButtonContinuousContent");
            }
        }

        public bool ButtonStopGrabEnabled
        {
            get { return buttonStopGrabEnabled; }
            set
            {
                buttonStopGrabEnabled = value;
                RaisePropertyChanged("ButtonStopGrabEnabled");
            }
        }

        public string ButtonStopGrabContent
        {
            get { return buttonStopGrabContent; }
            set
            {
                buttonStopGrabContent = value;
                // RaisePropertyChanged MUST fire the same case-sensitive name of property
                RaisePropertyChanged("ButtonStopGrabContent");
            }
        }

        public bool ButtonUSBCameraEnabled
        {
            get { return buttonUSBCameraEnabled; }
            set
            {
                buttonUSBCameraEnabled = value;
                RaisePropertyChanged("ButtonUSBCameraEnabled");
            }
        }

        public string ButtonUSBCameraContent
        {
            get { return buttonUSBCameraContent; }
            set
            {
                buttonUSBCameraContent = value;
                // RaisePropertyChanged MUST fire the same case-sensitive name of property
                RaisePropertyChanged("ButtonUSBCameraContent");
            }
        }

        public bool ButtonDistanceEnabled
        {
            get { return buttonDistanceEnabled; }
            set
            {
                buttonDistanceEnabled = value;
                RaisePropertyChanged("ButtonDistanceEnabled");
            }
        }

        public string ButtonDistanceContent
        {
            get { return buttonDistanceContent; }
            set
            {
                buttonDistanceContent = value;
                // RaisePropertyChanged MUST fire the same case-sensitive name of property
                RaisePropertyChanged("ButtonDistanceContent");
            }
        }

        public ImageSource BitmapSource
        {
            get { return bitmapSource; }
            set
            {
                bitmapSource = value;
                RaisePropertyChanged("BitmapSource");
            }
        }

        public VisionCameraViewModel()
        {
            distanceMeasure = new DistanceMeasure(0.005);

            ButtonOneShotClicked = new RelayCommand(() => OnButtonOneShotClicked());
            ButtonContinuousClicked = new RelayCommand(() => OnButtonContinuousClicked());
            ButtonStopGrabClicked = new RelayCommand(() => OnButtonStopGrabClicked());
            ButtonUSBCameraClicked = new RelayCommand(() => OnButtonUSBCameraClicked());
            ButtonDistanceClicked = new RelayCommand(() => OnButtonDistanceClicked());

            ButtonContinuousContent = "Live View";
            ButtonOneShotContent = "Capture";
            ButtonStopGrabContent = "Stop";
            ButtonDistanceContent = "Ruler Start";
            ButtonDistanceEnabled = true;
            isDistanceMeasureMode = false;
            

            ButtonUSBCameraContent = "Start";
            ButtonUSBCameraEnabled = true;

            dispatcher = Dispatcher.CurrentDispatcher;
            updateDeviceListTimer = new Timer();
            updateDeviceListTimer.Interval = 5000;
            updateDeviceListTimer.Elapsed += UpdateDeviceListTimer_Elapsed;
            visionCameraList = new ObservableCollection<VisionCamera>();

            UpdateDeviceList();
        }


        private void CaptureUSBCamera()
        {
            usbCamera = new System.Threading.Thread(new System.Threading.ThreadStart(CaptureCameraCallback));
            usbCamera.Start();
        }

        private void OnButtonDistanceClicked()
        {
            if (ButtonDistanceContent.Equals("Ruler Start"))
            {
                ButtonDistanceContent = "Ruler Clear";
                distanceMeasure.Reset();
                isDistanceMeasureMode = true;
            }
            else
            {
                ButtonDistanceContent = "Ruler Start";
                distanceMeasure.Reset();
                isDistanceMeasureMode = false;
            }
            
        }

        private void OnButtonUSBCameraClicked()
        {
            if (ButtonUSBCameraContent.Equals("Start"))
            {
                CaptureUSBCamera();
                ButtonUSBCameraContent = "Stop";
                isUSBCameraRunning = true;
            }
            else
            {
                distanceMeasure.Reset();
                isDistanceMeasureMode = false;

                isUSBCameraRunning = false;               
                ButtonUSBCameraContent = "Start";
                capture.Release();
            }
        }

        private void CaptureCameraCallback()
        {
            frame = new Mat();
            capture = new VideoCapture(0);
            capture.Open(0);

            while (isUSBCameraRunning)
            {
                capture.Read(frame);

                if (!frame.Empty())
                {
                    if (distanceMeasure!=null && distanceMeasure.PointInput_1)
                    {
                        Cv2.Circle(frame, new OpenCvSharp.Point(distanceMeasure.Pixel_X1, distanceMeasure.Pixel_Y1), 3, Scalar.Red);
                    }

                    if (distanceMeasure != null && distanceMeasure.PointInput_2 && distanceMeasure.IsCalculate)
                    {
                        Cv2.Circle(frame, new OpenCvSharp.Point(distanceMeasure.Pixel_X2, distanceMeasure.Pixel_Y2), 3, Scalar.Red);
                        Cv2.Line(frame, new OpenCvSharp.Point(distanceMeasure.Pixel_X1, distanceMeasure.Pixel_Y1),
                             new OpenCvSharp.Point(distanceMeasure.Pixel_X2, distanceMeasure.Pixel_Y2), Scalar.Red, 1, LineTypes.AntiAlias);

                        Cv2.PutText(frame, distanceMeasure.Distance.ToString("N3"), new OpenCvSharp.Point(distanceMeasure.Pixel_X2, distanceMeasure.Pixel_Y2), HersheyFonts.HersheySimplex, 1, Scalar.Blue);
                    }



                    image = BitmapConverter.ToBitmap(frame);
                    // Provide the display control with the new bitmap. This action automatically updates the display.
                    dispatcher.Invoke(() => (this.BitmapSource = BitmapToImageSource(image)));
                }

                image = null;
            }
        }


        private void OnButtonStopGrabClicked()
        {
            GrabStop(); // Stop the grabbing of images.
        }

        private void OnButtonContinuousClicked()
        {
            ContinuousShot(); // Start the grabbing of images until grabbing is stopped.
        }

        private void OnButtonOneShotClicked()
        {
            OneShot(); // Start the grabbing of one image.
        }

        private void UpdateDeviceListTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateDeviceList();
        }

        public VisionCamera VisionCamera { get; set; }
        public RelayCommand ButtonOneShotClicked { get; private set; }
        public RelayCommand ButtonContinuousClicked { get; private set; }
        public RelayCommand ButtonStopGrabClicked { get; private set; }
        public RelayCommand ButtonUSBCameraClicked { get; private set; }
        public RelayCommand ButtonDistanceClicked { get; private set; }


        // Updates the list of available camera devices.
        private void UpdateDeviceList()
        {
            try
            {
                // Ask the camera finder for a list of camera devices.
                List<ICameraInfo> allCameras = CameraFinder.Enumerate();

                // Loop over all cameras found.
                foreach (ICameraInfo cameraInfo in allCameras)
                {
                    // Loop over all cameras in the list of cameras.
                    bool newitem = true;
                    foreach (VisionCamera item in visionCameraList)
                    {
                        if (item.CameraInfo == null) continue;

                        ICameraInfo tag = item.CameraInfo as ICameraInfo;

                        // Is the camera found already in the list of cameras?
                        if (tag[CameraInfoKey.FullName] == cameraInfo[CameraInfoKey.FullName])
                        {
                            tag = cameraInfo;
                            newitem = false;
                            break;
                        }
                    }

                    // If the camera is not in the list, add it to the list.
                    if (newitem)
                    {
                        // Create the item to display.
                        VisionCamera item = new VisionCamera(cameraInfo[CameraInfoKey.FriendlyName]);

                        foreach (KeyValuePair<string, string> kvp in cameraInfo)
                        {
                            item.Description += kvp.Key + ": " + kvp.Value + "\n";
                        }

                        // Store the camera info in the displayed item.
                        item.CameraInfo = cameraInfo;

                        // Attach the device data.
                        visionCameraList.Add(item);
                    }
                }

                // Remove old camera devices that have been disconnected.
                foreach (VisionCamera item in visionCameraList)
                {
                    bool exists = false;

                    if (item.CameraInfo == null) continue;

                    // For each camera in the list, check whether it can be found by enumeration.
                    foreach (ICameraInfo cameraInfo in allCameras)
                    {
                        if (((ICameraInfo)item.CameraInfo)[CameraInfoKey.FullName] == cameraInfo[CameraInfoKey.FullName])
                        {
                            exists = true;
                            break;
                        }
                    }
                    // If the camera has not been found, remove it from the list view.
                    if (!exists)
                    {
                        visionCameraList.Remove(item);
                    }
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }

        // Starts the continuous grabbing of images and handles exceptions.
        private void ContinuousShot()
        {
            try
            {
                // Start the grabbing of images until grabbing is stopped.
                Configuration.AcquireContinuous(camera, null);
                camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }


        // Checks if single shot is supported by the camera.
        public bool IsSingleShotSupported()
        {
            // Camera can be null if not yet opened
            if (camera == null)
            {
                return false;
            }

            // Camera can be closed
            if (!camera.IsOpen)
            {
                return false;
            }

            bool canSet = camera.Parameters[PLCamera.AcquisitionMode].CanSetValue("SingleFrame");
            return canSet;
        }

        // Stops the grabbing of images and handles exceptions.
        private void GrabStop()
        {
            // Stop the grabbing.
            try
            {
                camera.StreamGrabber.Stop();
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }

        // Starts the grabbing of a single image and handles exceptions.
        private void OneShot()
        {
            try
            {
                // Starts the grabbing of one image.
                Configuration.AcquireSingleFrame(camera, null);
                camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }
        // Closes the camera object and handles exceptions.
        private void DestroyCamera()
        {
            // Disable all parameter controls.
            try
            {
                if (camera != null)
                {


                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

            // Destroy the camera object.
            try
            {
                if (camera != null)
                {
                    camera.Close();
                    camera.Dispose();
                    camera = null;
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }

        // Occurs when a device with an opened connection is removed.
        private void OnConnectionLost(Object sender, EventArgs e)
        {
            if (!dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnConnectionLost), sender, e);
                return;
            }

            // Close the camera object.
            DestroyCamera();
            // Because one device is gone, the list needs to be updated.
            UpdateDeviceList();
        }

        // Occurs when the connection to a camera device is opened.
        private void OnCameraOpened(Object sender, EventArgs e)
        {
            if (!dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            }

            // The image provider is ready to grab. Enable the grab buttons.
            EnableButtons(true, false);
        }

        // Occurs when the connection to a camera device is closed.
        private void OnCameraClosed(Object sender, EventArgs e)
        {
            if (!dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }

            // The camera connection is closed. Disable all buttons.
            EnableButtons(false, false);
        }

        // Occurs when a camera starts grabbing.
        private void OnGrabStarted(Object sender, EventArgs e)
        {
            if (!dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnGrabStarted), sender, e);
                return;
            }

            // Reset the stopwatch used to reduce the amount of displayed images. The camera may acquire images faster than the images can be displayed.

            stopWatch.Reset();

            // Do not update the device list while grabbing to reduce jitter. Jitter may occur because the GUI thread is blocked for a short time when enumerating.
            updateDeviceListTimer.Stop();

            // The camera is grabbing. Disable the grab buttons. Enable the stop button.
            EnableButtons(false, true);
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        // Occurs when an image has been acquired and is ready to be processed.
        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            if (!dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper GUI thread.
                // The grab result will be disposed after the event call. Clone the event arguments for marshaling to the GUI thread.
                dispatcher.BeginInvoke(new EventHandler<ImageGrabbedEventArgs>(OnImageGrabbed), sender, e.Clone());
                return;
            }

            try
            {
                // Acquire the image from the camera. Only show the latest image. The camera may acquire images faster than the images can be displayed.

                // Get the grab result.
                IGrabResult grabResult = e.GrabResult;

                // Check if the image can be displayed.
                if (grabResult.IsValid)
                {
                    // Reduce the number of displayed images to a reasonable amount if the camera is acquiring images very fast.
                    if (!stopWatch.IsRunning || stopWatch.ElapsedMilliseconds > 33)
                    {
                        stopWatch.Restart();

                        Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        // Lock the bits of the bitmap.
                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                        // Place the pointer to the buffer of the bitmap.
                        converter.OutputPixelFormat = PixelType.BGRA8packed;
                        IntPtr ptrBmp = bmpData.Scan0;
                        converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                        bitmap.UnlockBits(bmpData);

                        // Assign a temporary variable to dispose the bitmap after assigning the new bitmap to the display control.
                        //Bitmap bitmapOld = (Bitmap)this.BitmapSource;
                        Mat img = OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap);
                        Mat dst = new Mat();

                        // 비전화면 X, Y Flip
                        Cv2.Flip(img, dst, FlipMode.XY);

                        Cv2.Line(dst, new OpenCvSharp.Point(0, bitmap.Height / 2), new OpenCvSharp.Point(bitmap.Width, bitmap.Height / 2), Scalar.Red, 1, LineTypes.Link4);
                        Cv2.Line(dst, new OpenCvSharp.Point(bitmap.Width / 2, 0), new OpenCvSharp.Point(bitmap.Width / 2, bitmap.Height), Scalar.Red, 1, LineTypes.Link4);

                        Bitmap bit = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);

                        // Provide the display control with the new bitmap. This action automatically updates the display.
                        dispatcher.Invoke(() => (this.BitmapSource = BitmapToImageSource(bit)));



                        if (bitmap != null)
                        {
                            // Dispose the bitmap.
                            bitmap.Dispose();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
            finally
            {
                // Dispose the grab result if needed for returning it to the grab loop.
                e.DisposeGrabResultIfClone();
            }
        }

        // Occurs when a camera has stopped grabbing.
        private void OnGrabStopped(Object sender, GrabStopEventArgs e)
        {
            if (!dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                dispatcher.BeginInvoke(new EventHandler<GrabStopEventArgs>(OnGrabStopped), sender, e);
                return;
            }

            // Reset the stopwatch.
            stopWatch.Reset();

            // Re-enable the updating of the device list.
            updateDeviceListTimer.Start();

            // The camera stopped grabbing. Enable the grab buttons. Disable the stop button.
            EnableButtons(true, false);

            // If the grabbed stop due to an error, display the error message.
            if (e.Reason != GrabStopReason.UserRequest)
            {
                MessageBox.Show("A grab error occured:\n" + e.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helps to set the states of all buttons.
        private void EnableButtons(bool canGrab, bool canStop)
        {
            ButtonContinuousEnabled = canGrab;
            ButtonOneShotEnabled = canGrab && IsSingleShotSupported();
            ButtonStopGrabEnabled = canStop;
        }



        private void ShowException(Exception exception)
        {
            //MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }


    }
}
