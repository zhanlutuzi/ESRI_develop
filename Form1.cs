﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SystemUI;
namespace DXApplication1
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        
        public Form1()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.Engine);
            InitializeComponent();
        }

        public string[] OpenShapeFile()
        {
            string[] ShpFile = new string[2];
            OpenFileDialog OpenShpFile = new OpenFileDialog();
            OpenShpFile.Title = "打开Shape文件";
            OpenShpFile.InitialDirectory = "E:";
            OpenShpFile.Filter = "Shape文件(*.shp)|*.shp";

            if (OpenShpFile.ShowDialog() == DialogResult.OK)
            {
                string ShapPath = OpenShpFile.FileName;
                //利用"\\"将文件路径分成两部分
                int Position = ShapPath.LastIndexOf("\\");

                string FilePath = ShapPath.Substring(0, Position);
                string ShpName = ShapPath.Substring(Position + 1);
                ShpFile[0] = FilePath;

                ShpFile[1] = ShpName;

            }
            return ShpFile;
        }
        /*用于分割文件路径和文件名
         * 返回一个string数组；
         * 索引0为FilePath
         * 索引1为FileName
        */
        private string[] splitThePath(string path)
        {
            string[] PathInfo = new string[2];
            //利用"\\"将文件路径分成两部分
            int Position = path.LastIndexOf("\\");

            string FilePath = path.Substring(0, Position);
            string FileName = path.Substring(Position + 1);
            PathInfo[0] = FilePath;

            PathInfo[1] = FileName;
            return PathInfo;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void axMapControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {

        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem35_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem33_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AboutUS aboutusform = new AboutUS();
            aboutusform.Show();
        }

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }

        private void barButtonItem1_ItemClick_2(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFiledialog;
            openFiledialog = new OpenFileDialog();
            openFiledialog.Title = "打开地图文档文件";
            openFiledialog.Filter = "地图文档(.mxd)|*.mxd";
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFiledialog.FileName;
                if (axMapControl1.CheckMxFile(filePath))
                {
                    axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerHourglass;
                    axMapControl1.LoadMxFile(filePath, 0, Type.Missing);
                    axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerDefault;
                    axMapControl1.Extent = axMapControl1.FullExtent;
                }
                else
                {
                    MessageBox.Show(filePath + "不是有效的地图文档");
                }
            }
        }

        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            string[] FileInfo = OpenShapeFile();
            try
            {
                axMapControl1.AddShapeFile(FileInfo[0], FileInfo[1]);
            }
            catch (Exception excep)
            {
                MessageBox.Show("添加shp文件失败" + excep.ToString());
            }

        }

        // 打开栅格图像的辅助函数
        private IRasterWorkspace GetRasterWorkspace(string pWsName)
        {
            try
            {
                IWorkspaceFactory pWorkFact = new RasterWorkspaceFactoryClass();
                return pWorkFact.OpenFromFile(pWsName, 0) as IRasterWorkspace;
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建工作空间失败" + ex.ToString());
                return null;
            }
        }

        private IRasterDataset OpenFileRasterDataset(string pFolderName, string pFileName)
        {
            IRasterWorkspace pRasterWorkspace = GetRasterWorkspace(pFolderName);
            IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(pFileName);
            return pRasterDataset;
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string[] tifFile = new string[2];
            System.Windows.Forms.OpenFileDialog openFiledialog;
            openFiledialog = new OpenFileDialog();
            openFiledialog.Title = "打开Landsat图像";
            openFiledialog.Filter = "卫星影像(.tif)|*.tif";
            if (openFiledialog.ShowDialog() == DialogResult.OK)
            {
                string[] FileInfo = new string[2];
                FileInfo = splitThePath(openFiledialog.FileName);
                IRasterDataset tifdataset =  OpenFileRasterDataset(FileInfo[0],FileInfo[1]);
                IRasterLayer rasterlayer = new RasterLayerClass();
                rasterlayer.CreateFromDataset(tifdataset);
                axMapControl1.Map.AddLayer(rasterlayer as ILayer);
                axMapControl1.Extent = axMapControl1.FullExtent;
            }
            else
            {
                MessageBox.Show("文件打开出错");
            }
        }



        // 进行鹰眼功能的实现
        //------------------
        
        private void axMapControl1_OnMapReplaced_1(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMapReplacedEvent e)
        {
            this.axMapControl2.Map = new MapClass();
            for (int i = 1; i <= this.axMapControl1.LayerCount; i++)// 添加主地图控件中的所有图层到鹰眼控件中
            {
                this.axMapControl2.AddLayer(this.axMapControl1.get_Layer(this.axMapControl1.LayerCount - i));
            }
            this.axMapControl2.Extent = this.axMapControl1.FullExtent;// 设置 MapControl 显示范围至数据的全局范围         
            this.axMapControl2.Refresh();// 刷新鹰眼控件地图
        }

        private void axMapControl1_OnExtentUpdated_1(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            // 得到新范围
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            IGraphicsContainer pGra = axMapControl2.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            // 在绘制前，清除 axMapControl2 中的任何图形元素
            pGra.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;
            // 设置鹰眼图中的红线框
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            // 产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;
            // 设置颜色属性
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            // 设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGra.AddElement((IElement)pFillShapeEle, 0);
            // 刷新
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

         private void axMapControl2_OnMouseDown_1(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.axMapControl2.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.axMapControl1.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.axMapControl1.Extent = pEnvelope;  
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                // 按下鼠标右键绘制矩形框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.axMapControl2.TrackRectangle();
                    this.axMapControl1.Extent = pEnvelop;
 
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
        }

        private void axMapControl2_OnMouseMove_1(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);
            this.axMapControl1.CenterAt(pPoint);
            this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        // 鹰眼功能的开关
        private void barButtonItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (panel1.Visible == true)
                panel1.Visible = false;
            else
                panel1.Visible = true;
        }

        private void barButtonItem47_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pZoomIn = new ControlsMapZoomInToolClass();
            pZoomIn.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pZoomIn as ITool;
        }

        private void barButtonItem48_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pZoomOut = new ControlsMapZoomOutToolClass();
            pZoomOut.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pZoomOut as ITool;
        }

        private void barButtonItem52_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pMapMeature = new ControlsMapMeasureToolClass();
            pMapMeature.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pMapMeature as ITool;
        }

        private void barButtonItem53_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pIdentify = new ControlsMapIdentifyToolClass();
            pIdentify.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pIdentify as ITool;
            
        }

        private void barButtonItem50_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            axMapControl1.Extent = axMapControl1.FullExtent;
        }

        private void barButtonItem49_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pPan = new ControlsMapPanToolClass();
            pPan.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = pPan as ITool;
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {

        }

        private void barButtonItem18_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog pSaveFiledialog = new SaveFileDialog();
                pSaveFiledialog.Title = "另存为";
                pSaveFiledialog.OverwritePrompt = true;
                pSaveFiledialog.Filter = "ArcMap文档（*.mxd）|*.mxd";
                pSaveFiledialog.RestoreDirectory = true;
                if (pSaveFiledialog.ShowDialog() == DialogResult.OK)
                {
                    string sFilePath = pSaveFiledialog.FileName;
                    IMapDocument pMapdocument = new MapDocumentClass();
                    pMapdocument.New(sFilePath);
                    pMapdocument.ReplaceContents(axMapControl1.Map as IMxdContents);
                    // Save参数1：是否保存为相对路径；参数2：是否创建缩略图
                    pMapdocument.Save(true, true);
                
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出错了:" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }

        private void saveBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try 
            {
                string sMxdFileName = axMapControl1.DocumentFilename;
                IMapDocument pMapdocument = new MapDocumentClass();
                if (sMxdFileName != null && axMapControl1.CheckMxFile(sMxdFileName))
                {
                    if (pMapdocument.get_IsReadOnly(sMxdFileName))
                    {
                        MessageBox.Show("本文档只读，不能保存。");
                        pMapdocument.Close();
                        return;
                    }
                }
                else
                {
                    SaveFileDialog pSaveFileDialog = new SaveFileDialog();
                    pSaveFileDialog.Title = "请选择保存路径";
                    pSaveFileDialog.OverwritePrompt = true;
                    pSaveFileDialog.Filter = "ArcMap文档（*.mxd）|*.mxd";
                    pSaveFileDialog.RestoreDirectory = true;
                    if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        sMxdFileName = pSaveFileDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }
                pMapdocument.New(sMxdFileName);
                pMapdocument.ReplaceContents(axMapControl1.Map as IMxdContents);
                pMapdocument.Save(pMapdocument.UsesRelativePaths, true);
                pMapdocument.Close();
                MessageBox.Show("保存地图成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("出错了:" + ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }
    }
}
