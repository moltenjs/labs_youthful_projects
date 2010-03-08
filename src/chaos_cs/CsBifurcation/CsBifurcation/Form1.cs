/*
 * only concern: if in program files, won't have write access, need to set up in docs
 * See mouse shortcuts in plotusercontrol.cs
 * Press ctrl+space to redraw
 * Enter arbitrary code in P0, such as C1, rand(), or randneg()
 * Change additionalShading in init code for higher quality.
 * Press left/right and pgup/pgdn to make small changes to C1, C2
 * click label to manually set value
 * 
 * 
 * version 217 was a large change.
 * todo: threading, previews, zoom animations, undo stack, click lbl to fine-tune value. nudge view left/right?
 * todo: clicking on plot to zoom in should incorporate textbox changes? Eliminate view menu, plotcontrol to have only public methods no ui?
 * can't set c1 back to 0 by dragging. you can click to set to 0 though.
 * automatically add to additionalShading when rendering
 * */

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace CsBifurcation
{
    public partial class Form1 : Form
    {
        public readonly string Version = "1.0.0";
        public Form1()
        {
            if (Directory.Exists(@"..\..\cfgs")) 
                Directory.SetCurrentDirectory(@"..\..\cfgs"); //Test environment
            else
                Directory.SetCurrentDirectory(@"cfgs"); //Release environment

            InitializeComponent();
            lblParam1.Text = lblParam2.Text =lblParam3.Text =lblParam4.Text = lblSettling.Text = lblShading.Text = "";
            mnuAdvShades.Checked = true; mnuAdvPoints.Checked = false;
            
            mnuFileNew_Click(null, null);
        }

        public void Redraw()
        {
            this.pointPlotBifurcationUserControl1.paramExpression = getUserExpression(this.txtExpression.Text);
            this.pointPlotBifurcationUserControl1.paramP0 = getUserExpression(this.txtP0.Text);
            this.pointPlotBifurcationUserControl1.paramInit = getUserExpression(this.txtInit.Text);
            this.pointPlotBifurcationUserControl1.bShading = this.mnuAdvShades.Checked;

            this.pointPlotBifurcationUserControl1.redraw();
        }

        //transform sin into Math.sin, rand()=R.NextDouble()
        protected string getUserExpression(string s)
        {
            CodedomEvaluator.CodedomEvaluator cb = new CodedomEvaluator.CodedomEvaluator();
            s = cb.addMathMethods(s);
            //s = Regex.Replace(s, "\\brand\\(\\)\\b", "R.NextDouble()");
            s = Regex.Replace(s, "\\brand\\b", "R.NextDouble");
            s = Regex.Replace(s, "\\brandneg\\(\\)", "((R.NextDouble()-0.5)*2)");
            return s;
        }
       


        private void btnGo_Click(object sender, EventArgs e) { Redraw(); }

        private void tbParam1_Scroll(object sender, EventArgs e)
        {
            pointPlotBifurcationUserControl1.param1 = onScroll(tbParam1, lblParam1);
            if (mnuAdvAutoRedraw.Checked)
                pointPlotBifurcationUserControl1.redraw();
        }
        private void tbSettling_Scroll(object sender, EventArgs e) { pointPlotBifurcationUserControl1.paramSettle = (int)onScroll(tbSettling, lblSettling); }
        private void tbShading_Scroll(object sender, EventArgs e) { pointPlotBifurcationUserControl1.paramShading = onScroll(tbShading, lblShading); }
        private void tbParam2_Scroll(object sender, EventArgs e) { pointPlotBifurcationUserControl1.param2 = onScroll(tbParam2, lblParam2); }
        private void tbParam3_Scroll(object sender, EventArgs e) { pointPlotBifurcationUserControl1.param3 = onScroll(tbParam3, lblParam3); }
        private void tbParam4_Scroll(object sender, EventArgs e) { pointPlotBifurcationUserControl1.param4 = onScroll(tbParam4, lblParam4); }
        private double onScroll(TrackBar tb, Label lbl)
        {
            double v = (tb.Value / ((double)tb.Maximum)); // from 0.0 to 1.0
            if (tb==tbParam1 || tb==tbParam2)
                v = (v-0.5)*2; //from -1.0 to 1.0
            else if (tb==tbSettling)
                v = (int) (Math.Pow(10.0, v * 6));

            lbl.Text = v.ToString();
            return v;
        }

        private bool _getBounds(string sName, double fCurrent, out double dOut)
        {
            dOut = 0.0;
            string s = InputBoxForm.GetStrInput(sName, fCurrent.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="") return false;
            return double.TryParse(s, out dOut);
        }
        private void setBounds()
        {
            double X0, X1, Y0, Y1, nX0, nX1, nY0, nY1;
            pointPlotBifurcationUserControl1.getBounds(out X0, out X1, out Y0, out Y1);

            if (!_getBounds("Leftmost x", X0, out nX0)) return;
            if (!_getBounds("Rightmost x", X1, out nX1)) return;
            if (!_getBounds("Lowest y", Y0, out nY0)) return;
            if (!_getBounds("Greatest y", Y1, out nY1)) return;

            if (!(nY1 > nY0 && nX1 > nX0)) { MessageBox.Show("Invalid bounds."); return; }

            this.pointPlotBifurcationUserControl1.setBounds(nX0, nX1, nY0, nY1);
            this.pointPlotBifurcationUserControl1.redraw();
        }






        private void loadIni(string sFilename)
        {
            //note: requires absolute path to file.
            if (!File.Exists(sFilename)) return;
            IniFileParsing ifParsing = new IniFileParsing(sFilename, true);
            try
            {
                CsIniLoadHelper loader = new CsIniLoadHelper(ifParsing, "main");

                //set props
                double nX0 = loader.getDouble("X0");
                double nX1 = loader.getDouble("X1");
                double nY0 = loader.getDouble("Y0");
                double nY1 = loader.getDouble("Y1");
                pointPlotBifurcationUserControl1.setBounds(nX0, nX1, nY0, nY1);
                pointPlotBifurcationUserControl1.param1 = loader.getDouble("param1");
                pointPlotBifurcationUserControl1.param2 = loader.getDouble("param2");
                pointPlotBifurcationUserControl1.param3 = loader.getDouble("param3", true);
                pointPlotBifurcationUserControl1.param4 = loader.getDouble("param4", true);
                pointPlotBifurcationUserControl1.paramSettle = loader.getInt("paramSettle");
                pointPlotBifurcationUserControl1.paramShading = loader.getDouble("paramShading");

                //these are transformed, so set the ui instead of the prop. Call to Redraw will retrieve this.
                mnuAdvShades.Checked = loader.getInt("bShading")!=0;
                mnuAdvPoints.Checked = loader.getInt("bShading")==0;
                txtExpression.Text = loader.getString("paramExpression");
                txtP0.Text = loader.getString("paramP0");
                txtInit.Text = loader.getString("paramInit");

            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                return;
            }

            //set ui
            double p1 = pointPlotBifurcationUserControl1.param1; double p2 = pointPlotBifurcationUserControl1.param2;
            double p3 = pointPlotBifurcationUserControl1.param3; double p4 = pointPlotBifurcationUserControl1.param4;
            double ps = pointPlotBifurcationUserControl1.paramShading; double pst = pointPlotBifurcationUserControl1.paramSettle;
            lblParam1.Text = p1.ToString();
            lblParam2.Text = p2.ToString();
            lblSettling.Text = pst.ToString();
            lblShading.Text = ps.ToString();
            
            setSliderToValue(ps, tbShading);
            setSliderToValue(p1, tbParam1);
            setSliderToValue(p2, tbParam2);
            setSliderToValue(p3, tbParam3);
            setSliderToValue(p4, tbParam4);

            Redraw();
        }
        private void saveIni(string sFilename)
        {
            IniFileParsing ifParsing = new IniFileParsing(sFilename, false); //creates ini if doesn't exist
            try
            {
                CsIniSaveHelper saver = new CsIniSaveHelper(ifParsing, "main"); //one section called "main"

                double nX0, nX1, nY0, nY1;
                pointPlotBifurcationUserControl1.getBounds(out nX0, out nX1, out nY0, out nY1);
                saver.saveDouble("X0", nX0);
                saver.saveDouble("X1", nX1);
                saver.saveDouble("Y0", nY0);
                saver.saveDouble("Y1", nY1);

                saver.saveDouble("param1", pointPlotBifurcationUserControl1.param1);
                saver.saveDouble("param2", pointPlotBifurcationUserControl1.param2);
                saver.saveDouble("param3", pointPlotBifurcationUserControl1.param3);
                saver.saveDouble("param4", pointPlotBifurcationUserControl1.param4);
                saver.saveInt("paramSettle", pointPlotBifurcationUserControl1.paramSettle);
                saver.saveDouble("paramShading", pointPlotBifurcationUserControl1.paramShading);

                saver.saveString("paramExpression", txtExpression.Text);
                saver.saveString("paramInit", txtInit.Text);
                saver.saveString("paramP0", txtP0.Text);
                saver.saveInt("bShading", mnuAdvShades.Checked?1:0);

                saver.saveString("programVersion", Version);
            }
            catch (IniFileParsingException err)
            {
                MessageBox.Show("Prefs Error:"+err.ToString());
                return;
            }
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg1 = new OpenFileDialog();
            dlg1.RestoreDirectory = true;
            dlg1.Filter = "CsBifurcation files (*.cfg)|*.cfg";
            if (!(dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg1.FileName.Length > 0))
                return;
            loadIni(dlg1.FileName);
        }
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CsBifurcation files (*.cfg)|*.cfg";
            saveFileDialog1.RestoreDirectory = true;
            if (!(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                return;
            saveIni(saveFileDialog1.FileName);
        }
       
        private void mnuFileAnimate_Click(object sender, EventArgs e)
        {
            double d, c0_0, c0_1, c1_0, c1_1; string s; int nframes;
            double param1=pointPlotBifurcationUserControl1.param1, param2=pointPlotBifurcationUserControl1.param2;
            s = InputBoxForm.GetStrInput("Initial c1:", param1.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c0_0=d;
            s = InputBoxForm.GetStrInput("Final c1:", param1.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c0_1=d;
            s = InputBoxForm.GetStrInput("Initial c2:", param2.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c1_0=d;
            s = InputBoxForm.GetStrInput("Final c2:", param2.ToString(CultureInfo.InvariantCulture));
            if (s==null || s=="" || !double.TryParse(s, out d)) return;
            c1_1=d;
            s = InputBoxForm.GetStrInput("Number of frames:", "50");
            if (s==null || s=="" || !int.TryParse(s, out nframes)) return;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "png files (*.png)|*.png";
            saveFileDialog1.RestoreDirectory = true;
            if (!(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0))
                return;
            string sfilename= saveFileDialog1.FileName;
            double c0_inc = (c0_1-c0_0)/nframes;
            double c1_inc = (c1_1-c1_0)/nframes;

            for (int i=0; i<nframes; i++)
            {
                pointPlotBifurcationUserControl1.param1 = c0_0;
                pointPlotBifurcationUserControl1.param2 = c1_0;
                pointPlotBifurcationUserControl1.renderToDiskSave(400, 400, sfilename.Replace(".png", "_"+i.ToString()+".png"));
                c0_0 += c0_inc;
                c1_0 += c1_inc;
            }
        }
        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            string sFilename = Path.GetFullPath(Directory.GetCurrentDirectory()) + "\\default.cfg";
            if (File.Exists(sFilename))
                loadIni(sFilename); // requires absolute path.
            else
                Redraw();
        }
        private void mnuFileRender_Click(object sender, EventArgs e)
        {
            //create 3200x3200
            pointPlotBifurcationUserControl1.renderToDisk(3200,3200);
        }

        private void mnuAdvShades_Click(object sender, EventArgs e) { mnuAdvShades.Checked = true; mnuAdvPoints.Checked=false; Redraw(); }
        private void mnuAdvPoints_Click(object sender, EventArgs e) { mnuAdvShades.Checked = false; mnuAdvPoints.Checked=true; Redraw(); }
        private void mnuViewRedraw_Click(object sender, EventArgs e) { Redraw(); }
        private void mnuAdvAutoRedraw_Click(object sender, EventArgs e) { mnuAdvAutoRedraw.Checked=!mnuAdvAutoRedraw.Checked; }
        private void mnuFileExit_Click(object sender, EventArgs e) { Close(); }
        private void mnuViewZoomIn_Click(object sender, EventArgs e) { pointPlotBifurcationUserControl1.zoomIn(); }
        private void mnuViewZoomOut_Click(object sender, EventArgs e) { pointPlotBifurcationUserControl1.zoomOut(); }
        private void mnuViewZoomUndo_Click(object sender, EventArgs e) { pointPlotBifurcationUserControl1.undoZoom(); }
        private void mnuViewReset_Click(object sender, EventArgs e) { pointPlotBifurcationUserControl1.resetZoom(); }

        private void mnuAdvBounds_Click(object sender, EventArgs e) { setBounds(); }
        private void mnuAdvAddQuality_Click(object sender, EventArgs e) { MessageBox.Show("To run more iterations, producing better images, add a line to \"init. code\" such as: \r\n\r\nadditionalShading=2.0;"); }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CsBifurcation\r\nBy Ben Fisher, 2010.\r\n\r\nhttp://halfhourhacks.blogspot.com");
        }


        private void setSliderToValue(double v, TrackBar tb)
        {
            int nVal;
            if (tb==tbSettling)
                nVal = tb.Value; //don't feel like setting this, would have to log it. TODO: have it set this.
            if (tb==tbParam1 || tb==tbParam2)
                nVal = (int)(tb.Maximum*(v / 2+.5));
            else
                nVal = (int)(tb.Maximum*v);
            nVal = Math.Min(tb.Maximum, Math.Max(tb.Minimum, nVal)); //if beyond bounds, push to edge.
            tb.Value = nVal;
        }

        private bool manSetValue(Label lbl, TrackBar tb, out double v)
        {
            v=0.0;
            string s = InputBoxForm.GetStrInput("Value:", lbl.Text);
            if (s==null||s==""||!double.TryParse(s, out v)) 
                return false;
            setSliderToValue(v, tb);
            lbl.Text = v.ToString();
            return true;
        }
        private void lblSettling_Click(object sender, EventArgs e)
        {
            double v;
            if (manSetValue(lblSettling, tbSettling, out v)) pointPlotBifurcationUserControl1.paramSettle = (int) v;
            Redraw();
        }
        private void lblShading_Click(object sender, EventArgs e)
        {
            double v;
            if (manSetValue(lblShading, tbShading, out v)) pointPlotBifurcationUserControl1.paramShading = v;
            Redraw();
        }
        private void lblParam1_Click(object sender, EventArgs e)
        {
            double v;
            if (manSetValue(lblParam1, tbParam1, out v)) pointPlotBifurcationUserControl1.param1 = v;
            Redraw();
        }
        private void lblParam2_Click(object sender, EventArgs e)
        {
            double v;
            if (manSetValue(lblParam2, tbParam2, out v)) pointPlotBifurcationUserControl1.param2 = v;
            Redraw();
        }
        private void lblParam3_Click(object sender, EventArgs e)
        {
            double v;
            if (manSetValue(lblParam3, tbParam3, out v)) pointPlotBifurcationUserControl1.param3 = v;
            Redraw();
        }
        private void lblParam4_Click(object sender, EventArgs e)
        {
            double v;
            if (manSetValue(lblParam4, tbParam4, out v)) pointPlotBifurcationUserControl1.param4 = v;
            Redraw();
        }

        private void advLoopPreface_Click(object sender, EventArgs e)
        {
           /* string s = InputBoxForm.GetStrInput("Set loop code:", this.strCodeLoopPreface);
            if (s == null) 
                return;
            this.strCodeLoopPreface = s;
            //advLoopPreface.Checked = strCodeLoopPreface.Trim() != "";
             string strCodeLoopPreface = ""; //used to be member var*/
             
        }
        
    }
}