using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GLSharp;
using OpenTK.Graphics.OpenGL;
using gl = OpenTK.Graphics.OpenGL.GL;
using glu = OpenTK.Graphics.OpenGL.Glu;


public class MQOViewer : Scene
{

    #region フィールド

    /// <summary>
    /// カメラ
    /// </summary>
    Camera cam;

    /// <summary>
    /// ライト
    /// </summary>
    Light light0;

    /// <summary>
    /// 描画方法を指定するパラメータ
    /// </summary>
    EmptyRenderingParams renderingParams = null;

    /// <summary>
    /// バッファをクリアする色
    /// </summary>
    float[] clearColor = new float[] { 0.5f, 0.2f, 0.3f, 0f };


    /// <summary>
    /// 選択されたモデルのAABBを描画するのに使用するマテリアル
    /// </summary>
    Material pickedModelAABBMaterial = new Material("AABB", Color.LightGreen, 0.6f);

    /// <summary>
    /// テキスト描画に使用する。
    /// </summary>
    BitmapGlyphDictionary glyphDictionaly;

    /// <summary>
    /// テキストのフォントとかの指定
    /// </summary>
    BitmapGlyphFont textFont;

    //#########################


    /// <summary>
    /// 表示するモデルのリスト
    /// </summary>
    List<Model> models = new List<Model>();


    //ContextMenuStrip contextMenuStrip;
    //ToolStripMenuItem menuOpenFile;
    //ToolStripMenuItem menuRemove;
    //ToolStripMenuItem menuMqoPickChild;


    /// <summary>
    /// 描画にかかった時間を計測するストップウォッチ
    /// </summary>
    System.Diagnostics.Stopwatch stopwatch;
    double[] renderingTimes = new double[30];
    int renderingTimeArrayIndex = 0;

    #endregion フィールド


    //###############################


    public MQOViewer()
    {
        //注 : 
        //コンストラクタの時点では
        //まだ、Scene.Contextプロパティはnullなので
        //レンダリングコンテキストが取得できない。


        //コンテキストメニューの作成
        //this.contextMenuStrip = new ContextMenuStrip();

        ////MQOファイルを選択・ロードする
        //this.menuOpenFile = new ToolStripMenuItem();
        //this.menuOpenFile.Text = "Open MQO";
        //this.menuOpenFile.Click += new EventHandler(this.OpenMqoFile);
        //this.contextMenuStrip.Items.Add(this.menuOpenFile);

        ////表示中のMQOを削除する
        //this.menuRemove = new ToolStripMenuItem();
        //this.menuRemove.Text = "Remove";
        //this.contextMenuStrip.Items.Add(this.menuRemove);

        ////セパレータを入れておく
        //this.contextMenuStrip.Items.Add(new ToolStripSeparator());

        ////マウスでMQOを選択する方法の設定
        //this.menuMqoPickChild = new ToolStripMenuItem();
        //this.menuMqoPickChild.Text = "オブジェクト単位で選択";
        //this.menuMqoPickChild.Checked = false;
        //this.menuMqoPickChild.CheckOnClick = true;
        //this.menuMqoPickChild.Click += delegate(object sender, EventArgs e)
        //{ this.pickChild = this.menuMqoPickChild.Checked; };
        //this.contextMenuStrip.Items.Add(this.menuMqoPickChild);

        //描画にかかった時間を計測するストップウォッチ
        this.stopwatch = new System.Diagnostics.Stopwatch();
    }


    //###############################


    #region OnSetToRenderingControl : OpenGLの初期化など

    /// <summary>
    /// このシーンがコントロールにセットされたときに呼び出されます。
    /// </summary>
    /// <param name="e"></param>
    protected override void OnSetToRenderingControl(EventArgs e)
    {
        //この時点でthis.Contextプロパティから
        //レンダリングコンテキストが取得できるようになるので、
        //ここでOpenGLの初期化などを行うことを推奨します。

        this.Context.MakeCurrent();

        ///////////////////////////////////

        renderingParams = new EmptyRenderingParams();


        ///////////////////////////////////
        // カメラ、ライトの初期化、設定
        cam = new Camera(0f, 0f, 600f,
                          0f, 0f, 0f,
                          0f, 1f, 0f);
        cam.fovy = 60f;
        cam.zFar = 10000f;
        cam.zNear = 0.1f;

        light0 = new Light(LightName.Light0, LightType.DirectionalLight,
                            0f, 0f, 7f,
                            -0.3f, -0.3f, -1f);


        this.glyphDictionaly = new BitmapGlyphDictionary(this.Context.HDC);

        //AntiAliasFormat.Noneを指定した場合、色は何を指定しても無視される。
        this.textFont = new BitmapGlyphFont(this.RenderingControl.Font, AntiAliasFormat.None, Color.Empty);


        ///////////////////////////////////
        //初期画面で表示しておくメッセージを作成

        //TextModel startMessage = new TextModel("Simple MQO viewer",
        //                                        this.RenderingControl.Font, 50f, 5f, this.Context.HDC,
        //                                        TextLayout.Center, Material.GetWhiteDefaultMaterial());

        //startMessage.AddText("\n\n右ダブルクリックでメニューを表示します。",
        //                      this.RenderingControl.Font, 20f, 10f, 4, new Material("", Color.Gray, 0.6f));

        //startMessage.Layout();
        ////テキストの位置を中央へ移動
        //startMessage.Position.TranslateOnLocal(Vector.GetScaled(startMessage.AABB.center, -1f));


        //this.AddMqo(startMessage);


        ///////////////////////////////////

        //画面クリア時に使用する色（背景色）を指定。
        gl.ClearColor(clearColor[0], clearColor[1], clearColor[2], clearColor[3]);

        gl.Enable(EnableCap.DepthTest);
        gl.DepthFunc(DepthFunction.Lequal);

        gl.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

        //陰面消去設定
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(CullFaceMode.Back);

        //アルファブレンディング設定
        gl.Enable(EnableCap.Blend);
        gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

        //ライティングの設定
        gl.LightModel(LightModelParameter.LightModelLocalViewer, 1);    //ライティングの計算を正確に行う。

        gl.LightModel(LightModelParameter.LightModelColorControl, (int)LightModelColorControl.SeparateSpecularColor);

        //プロジェクション行列の設定
        SetProjection();

        try
        {
            string fileName = Application.StartupPath + @"\data\default.mqo";
            MeshModel mqo = new MeshModel(fileName, Path.GetFileNameWithoutExtension(fileName),
                                           1f, true, new MeshModel.MQOLoadingParams());

            this.AddMqo(mqo);
            this.cam.Zoom(0.6f);
            this.RenderingControl.Invalidate();
        }
        catch (Exception ex)
        {
            string errMessage = ex.Message;
            if (ex.InnerException != null)
            {
                errMessage += Environment.NewLine + ex.InnerException.Message;
            }

            MessageBox.Show(errMessage);
        }
    }

    #endregion　OnSetToRenderingControl


    #region SetProjection

    /// <summary>
    /// プロジェクション行列の設定を行う。
    /// </summary>
    public void SetProjection()
    {
        MatrixMode mode = glp.CurrentMatrixMode;

        gl.MatrixMode(MatrixMode.Projection);

        gl.LoadIdentity();

        //ビューポートの設定
        gl.Viewport(0, 0, this.RenderingControl.ClientRectangle.Width, this.RenderingControl.ClientRectangle.Height);

        //カメラのアスペクト比を設定
        cam.SetAspect(this.RenderingControl.ClientSize);
        //カメラオブジェクトからプロジェクション行列を適用する
        cam.ApplyProjection();

        gl.MatrixMode(mode);

    }

    #endregion SetProjection


    //###############################


    #region OnPreRemovedFromRenderingControl : OpenGLリソース(ディスプレイリストなど)の解放

    protected override void OnPreRemovedFromRenderingControl(EventArgs e)
    {
        //base.OnPreRemovedFromRenderingControl( e );

        gl.LightModel(LightModelParameter.LightModelColorControl, (int)LightModelColorControl.SingleColor);

        foreach (Model m in this.models)
        {
            m.Dispose();
        }
    }

    #endregion OnPreRemovedFromRenderingControl


    //###############################


    #region RenderScene : シーンの描画


    protected override void RenderScene()
    {
        ErrorCode err = gl.GetError();
        if (err != ErrorCode.NoError) throw new Exception(err.ToString());

        this.stopwatch.Reset();
        this.stopwatch.Start();


        ////////////////////////////////
        // 描画前の設定など

        //バッファのクリア
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        gl.MatrixMode(MatrixMode.Modelview);

        ///モデルビュー変換行列のリセット
        gl.LoadIdentity();

        //カメラの位置と向きを適用
        cam.ApplyView();

        //ライティングの設定を適用
        gl.Enable(EnableCap.Lighting);
        light0.ON();


        ////////////////////////////////

        foreach (Model m in this.models)
        {
            m.DrawScene(this.renderingParams);

            err = gl.GetError();
            if (err != ErrorCode.NoError) throw new Exception(err.ToString());
        }


        if (this.pickedModel != null)
        {
            gl.PushMatrix();
            this.pickedModel.ApplyPosition(true);

            gl.PushAttrib(AttribMask.LightingBit | AttribMask.LineBit);
            gl.Disable(EnableCap.Lighting);
            gl.LineWidth(2f);

            pickedModelAABBMaterial.Apply();

            this.pickedModel.DrawAABB(PrimitiveType.Line);

            gl.Disable(EnableCap.DepthTest);

            gl.PointSize(5f);
            gl.Color4(1f, 0f, 0f, 1f);
            gl.Begin(BeginMode.Points);
            gl.Vertex3(this.pickedObjectCoord);
            gl.End();


            //選択したオブジェクト名と座標を表示する。
            gl.Color4(1f, 1f, 1f, 1f);
            gl.RasterPos3(this.pickedObjectCoord);
            gl.Bitmap(0, 0, 0f, 0f, 15f, 0f, (byte[])null);
            float textHeight = this.textFont.font.Height;
            float textWidth = this.glyphDictionaly.DrawText(this.pickedModel.Name, this.textFont);
            gl.Bitmap(0, 0, 0f, 0f, -textWidth, -textHeight, (byte[])null); //簡易的な改行
            textWidth = this.glyphDictionaly.DrawText(this.pickedObjectCoord[0].ToString("f2") + "\n", this.textFont);
            textWidth = this.glyphDictionaly.DrawText(this.pickedObjectCoord[1].ToString("f2") + "\n", this.textFont);
            textWidth = this.glyphDictionaly.DrawText(this.pickedObjectCoord[2].ToString("f2"), this.textFont);
            gl.Enable(EnableCap.DepthTest);

            gl.PopAttrib();

            gl.PopMatrix();
        }

        ////////////////////////////////

        gl.Finish();

        {
            this.stopwatch.Stop();
            int numFrame = this.renderingTimes.Length;
            this.renderingTimes[this.renderingTimeArrayIndex] = this.stopwatch.Elapsed.TotalMilliseconds;
            this.renderingTimeArrayIndex++;
            if (this.renderingTimeArrayIndex >= numFrame)
            { this.renderingTimeArrayIndex = 0; }
            double average = 0.0;
            for (int i = 0; i < numFrame; i++)
            {
                average += this.renderingTimes[i];
            }
            average /= numFrame;

            gl.MatrixMode(MatrixMode.Projection);

            gl.PushMatrix();
            {
                gl.LoadIdentity();

                gl.MatrixMode(MatrixMode.Modelview);
                gl.LoadIdentity();

                gl.RasterPos2(-1.0f, -1.0f);
                gl.Bitmap(0, 0, 0f, 0f, 10f, 30f, (byte[])null);
                gl.PushAttrib(AttribMask.EnableBit);

                gl.Disable(EnableCap.Lighting);
                gl.Disable(EnableCap.DepthTest);
                gl.Disable(EnableCap.Texture2D);

                gl.Color4(1f, 1f, 1f, 1f);
                gl.RasterPos2(-1, -1);
                gl.Bitmap(0, 0, 0, 0, 2, 2, (byte[])null);
                this.glyphDictionaly.DrawText(average.ToString("g4"), this.textFont);

                gl.PopAttrib();

                gl.MatrixMode(MatrixMode.Projection);
            }
            gl.PopMatrix();


        }
    }

    #endregion RenderScene


    //###############################


    #region OnClientSizeChanged

    public override void OnClientSizeChanged()
    {
        this.SetProjection();
    }

    #endregion OnClientSizeChanged


    //###############################


    /// <summary>
    /// マウスピッキングで選択されたオブジェクト
    /// </summary>
    Model pickedModel = null;

    /// <summary>
    /// マウスピッキング時のマウスポインタが指すオブジェクト上表面のローカル座標
    /// </summary>
    double[] pickedObjectCoord = null;

    /// <summary>
    /// マウスピッキング時のマウスポインタ座標（ウィンドウ座標、左下原点、float[3]{ X, Y, Depth}。）
    /// </summary>
    double[] pickedWindowPosition = null;

    /// <summary>
    /// 子モデル単位でマウスピッキングを行うかどうかのフラグ。
    /// </summary>
    public bool pickChild = false;

    /// <summary>
    /// マウスピッキング時の変換行列
    /// </summary>
    TransformMatrixes pickedTransformMatrixes = null;


    //###############################


    #region OnMouseDown

    public override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.None)
        {
            lastPos = e.Location;
        }

        if ((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right))
        {
            int winX = e.X,
                winY = this.RenderingControl.ClientSize.Height - 1 - e.Y;
            double depth = 0;


            List<SelectionData> hitObjects
                = Selection.Pick<Model>(this.models, this.renderingParams, new double[] { winX, winY }, new double[] { 0.1, 0.1 },
                                         this.cam, glp.CurrentViewport, this.pickChild,
                                         out this.pickedTransformMatrixes);

            this.pickedModel = (Model)Selection.GetNearest(hitObjects, out depth);

            this.pickedWindowPosition = new double[] { winX, winY, (float)depth };

            if (this.pickedModel != null)
            {
                //マウスポインタがつかんだオブジェクト表面のローカル座標を計算する。
                //行列計算はOpenGLとGLUにやらせる。
                MatrixMode currentMatrixMode = glp.CurrentMatrixMode;
                gl.MatrixMode(MatrixMode.Modelview);

                gl.PushMatrix();

                //Pick<T>( ... )で返されるTransformMatrixSet.ModelViewプロパティには、ビューイング変換行列が格納されている。
                gl.LoadMatrix(this.pickedTransformMatrixes.Modelview);
                this.pickedModel.ApplyParentPosition();
                this.pickedModel.ApplyPosition();
                double[] modelviewMatrix = glp.CurrentModelviewMatrixDouble;

                gl.PopMatrix();

                gl.MatrixMode(currentMatrixMode);

                this.pickedObjectCoord = new double[3];
                glu.UnProject(winX, winY, depth,
                               ref modelviewMatrix[0], ref this.pickedTransformMatrixes.Projection[0], ref this.pickedTransformMatrixes.Viewport[0],
                               ref this.pickedObjectCoord[0], ref this.pickedObjectCoord[1], ref this.pickedObjectCoord[2]);
            }
            else
            {
                this.pickedTransformMatrixes = null;
            }

            this.RenderingControl.Refresh();
        }

    }

    #endregion OnMouseDown


    #region OnMouseMove

    Point lastPos = new Point();

    /// <summary>
    /// マウス移動イベント。
    /// マウスドラッグで視点（＝カメラの位置）を移動する。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public override void OnMouseMove(MouseEventArgs e)
    {

        if (e.Button == MouseButtons.None) return;

        if (lastPos == e.Location)
        {
            return;
        }

        float dx = e.X - lastPos.X,
              dy = lastPos.Y - e.Y;


        switch (e.Button)
        {
            case MouseButtons.None:
                {
                    break;
                }
            case MouseButtons.Left:
                {
                    if (this.pickedModel != null)
                    {
                        #region マウスでモデルをつかんで移動

                        double fromX = lastPos.X,
                               fromY = this.RenderingControl.ClientSize.Height - 1 - lastPos.Y,
                               fromDepth = this.pickedWindowPosition[2];

                        double destX = e.X,
                               destY = this.RenderingControl.ClientSize.Height - 1 - e.Y,
                               destDepth = fromDepth;

                        this.pickedModel.TranslateOnWindow(this.pickedTransformMatrixes, fromX, fromY, fromDepth,
                                                            ref destX, ref destY, ref destDepth, this.pickedObjectCoord);

                        #endregion マウスでモデルをつかんで移動
                    }
                    else
                    {
                        float dist = cam.GetDistanceFromCenter();
                        cam.TranslateOnViewWithCenter(dist * 0.002f * dx, dist * 0.002f * dy, 0f);
                    }
                    this.RenderingControl.Invalidate();
                    break;
                }
            case MouseButtons.Right:
                {
                    if (this.pickedModel != null)
                    {
                        #region マウスピッキングで選択したモデルを回転

                        if (Control.ModifierKeys == Keys.Shift)
                        {
                            this.pickedModel.RotateOnWindow(this.cam, 0f, 0f, dx);
                        }
                        else
                        {
                            this.pickedModel.RotateOnWindow(this.cam, dx, -dy, 0f);
                        }

                        #endregion マウスピッキングで選択したモデルを回転
                    }
                    else
                    {
                        if (Control.ModifierKeys == Keys.Control)
                        {
                            #region ライトの向きを回転

                            if (dx != 0)
                                Vector.Rotate(this.light0.direction, this.cam.GetVerticalVectorNormalized(), dx);
                            if (dy != 0)
                                Vector.Rotate(this.light0.direction, this.cam.GetHorizontalVectorNormalized(), -dy);

                            #endregion ライトの向きを回転
                        }
                        else
                        {
                            #region 視点の回転

                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                if (dx != 0)
                                    this.cam.RotateC(dx);
                            }
                            else
                            {
                                if (dx != 0)
                                    cam.RotateH(dx);
                                if (dy != 0)
                                    cam.RotateV(dy);
                            }

                            #endregion 視点の回転
                        }
                    }
                    this.RenderingControl.Invalidate();
                    break;
                }
            case MouseButtons.Middle:
                {
                    cam.Zoom(1f + dy * 0.005f);
                    this.RenderingControl.Invalidate();
                    break;
                }
            case MouseButtons.Left | MouseButtons.Right:
                {
                    float dist = cam.GetDistanceFromCenter();
                    cam.TranslateOnViewWithCenter(dist * 0.004f * dx, 0f, dist * 0.004f * dy);
                    this.RenderingControl.Invalidate();
                    break;
                }
        }

        lastPos = e.Location;
    }

    #endregion OnMouseMove


    #region OnMouseWheel

    public override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);

        this.cam.Zoom(1f + (e.Delta / 120f * 0.1f));

        this.RenderingControl.Invalidate();
    }

    #endregion


    #region OnMouseDoubleClick

    public override void OnMouseDoubleClick(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            //this.contextMenuStrip.Show(this.RenderingControl, e.Location);
        }
    }

    #endregion


    public override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.S)
        {
            (this.models[1] as MeshModel).SaveAsMQOFile(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TestMQO.mqo", 1f, true);
        }
    }


    //###############################


    #region OpenMqoFile

    void OpenMqoFile(object sender, EventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "Open MQO File";
        dialog.Filter = "MQO (*.mqo)|*.mqo";

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                MeshModel mqo = new MeshModel(dialog.FileName, System.IO.Path.GetFileNameWithoutExtension(dialog.FileName),
                                               1f, true, new MeshModel.MQOLoadingParams());

                this.AddMqo(mqo);

                this.RenderingControl.Invalidate();
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errMessage += Environment.NewLine + ex.InnerException.Message;
                }

                MessageBox.Show(errMessage);
            }
        }
    }

    #endregion OpenMqoFile


    #region AddMqo

    private void AddMqo(MeshModel mqo)
    {
        this.models.Add(mqo);

        //追加したMQOを削除するためのメニューを作成・追加する。
        ToolStripMenuItem menu = new ToolStripMenuItem();
        menu.Text = mqo.Name;
        menu.Tag = mqo;

        //メニューがクリックされると、
        //MQOとこのメニュー自身が削除されるように仕込んでおく。
        menu.Click += delegate(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            MeshModel m = (MeshModel)menuItem.Tag;

            this.models.Remove(m);
            m.Dispose();

            //this.menuRemove.DropDownItems.Remove(menu);
            menu.Dispose();

            if (this.pickedModel == m)
                this.pickedModel = null;
        };

        //this.menuRemove.DropDownItems.Add(menu);
    }

    #endregion AddMqo

}