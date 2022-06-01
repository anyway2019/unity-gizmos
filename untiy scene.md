# 3d场景分段
场景根据业务逻辑拆分，比如通关类游戏，关卡一二等每个关卡拆分成独立资源包，可以通过碰撞检测或者trigger(或者其他业务逻辑)触发动态加载。此时可以根据需要适当时机销毁释放内存。
单个拆分的场景仍然很大，可以根据业务需要进一步拆分成更小的粒度。
```C#
//example
public GameObject scenePrefab;
private int level = 0;
//触发分段下载
private void OnTriggerEnter(Collider other)
{
    level++;
    var nextScene = Instantiate(scenePrefab, transform.parent);
    nextScene.transform.position = new Vector3(0, 0, level * 30);
}
```

# 视锥剔除(camera 视锥体内物体渲染)
方法一：使用unity自带的api ```GeometryUtility.CalculateFrustumPlanes```（频繁调用仍然有性能问题)
<p>方法二：使用shader处理 :<a>https://zhuanlan.zhihu.com/p/376801370</a>
</p>

```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustumTest : MonoBehaviour
{

    public Camera mainCamera;
    public float fps;
    public float delay;
    private Plane[] planes;
    private Renderer[] testObjs;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        planes = new Plane[6];
    }

    private int count = 0;
    private float deltaTime = 0f;
    private float showTime = 1f;
    public bool Custom;
    // Update is called once per frame
    void Update()
    {
        frustumCulling();
        computeFrameRate();
    }
    //计算FPS和渲染延迟
    private void computeFrameRate()
    {
        count++;
        deltaTime += Time.deltaTime;
        if (deltaTime >= showTime)
        {
            fps = count / deltaTime;
            delay = deltaTime * 1000 / count;
            count = 0;
            deltaTime = 0;
        }
    }
    //视锥剔除
    private void frustumCulling()
    {
        if (Custom)
        {
            testObjs = FindObjectsOfType<Renderer>();
            GeometryUtility.CalculateFrustumPlanes(mainCamera, planes);
            for (int i = 0; i < testObjs.Length; i++)
            {
                var bounds = testObjs[i].bounds;
                var result = GeometryUtility.TestPlanesAABB(planes, bounds);
                testObjs[i].enabled = result;
            }
        }
    }
}

```

# AR多人运动轨迹计算构成参数(TODO:未完待续)
* 通过camera tracking solver 获取摄像机轨迹camera path
* 参数(获取或计算): CameraPosition（轨迹）| LaneWidth（路宽）Lane(人物横向占比)（LaneCamera | BaseOffset（位置偏移）｜ RotationOffset（旋转偏移量）| Lean(倾斜角度)
```C# 
//计算当前人物在轨迹上的position、rotation以及scale
public override void UpdateTransform()
{
    PositionOffset = Vector3.up * route.CameraHeight * (-1);
    Quaternion cameraRotation = this.GetCameraRotation(this.frame - this.FrameIndexDistanceCorrection);
    Vector3 filteredCameraPosition = this.GetFilteredCameraPosition(this.frame - this.FrameIndexDistanceCorrection);
    Vector3 vector3 = Vector3.left * (this.route.LeftHanded ? -1f : 1f) * (this.LaneWidth * (this.Lane + this.LaneCamera) - this.BaseOffset);

    var targetPos = filteredCameraPosition + cameraRotation * (vector3 + this.PositionOffset);
    
    
    this.transform.position = targetPos;
    this.transform.rotation = cameraRotation * Quaternion.Euler(this.RotationOffset) * Quaternion.Euler(this.Lean);
    this.transform.localScale = this.Scale;
}
//参数详解:

//cameraPosition camera tracking获取

//lane   

//laneWith = 0.7f

//LaneCamera

//SlamSegments

//baseoffset
float num6 = this.Route.LeftHanded ? this.GetRouteLeftOffset(collision.Frame) : this.GetRouteRightOffset(collision.Frame);
float num7 = Mathf.Lerp(this.GetStartOffset(collision.StartPosition).x, 0.0f, collision.Distance / 200f);
collision.BaseOffset = num6 + num7;  
//rotationOffset

//LeftSideOffsetFrames  LeftSideOffsets

//RightSideOffsetFrames  RightSideOffsets

//RiderScale 

//CameraHeight

//VideoFrameOffset 7

//VideoFrameOffsetMac 3

//Lean与地面倾斜角度（压车）
float speed = 30f;//当前人物速度

//57.29578f = 一弧度对应的角度  9.81 = g
float f = 57.29578f * Mathf.Atan((float) (speed * speed * (double) num1 / 9.81));
private void UpdateRidersLean()
{
      foreach (KeyValuePair<int, ARLaneObject> riderObject in this.riderObjects)
      {
        RiderRenderer riderRenderer = riderObject.Value as RiderRenderer;
        if (!((UnityEngine.Object) riderRenderer == (UnityEngine.Object) null))
        {
          float num1 = this.Route.GetTrajectoryCurvature(riderRenderer.Frame) + riderRenderer.Curvature;
          float z1;
          if ((double) num1 == 0.0)
          {
            z1 = 0.0f;
          }
          else
          {
            double speed = this.riders[riderObject.Key].Speed;
            float f = 57.29578f * Mathf.Atan((float) (speed * speed * (double) num1 / 9.81));
            float num2 = (double) f >= 0.0 ? 1f : -1f;
            float num3 = Mathf.Abs(f);
            if ((double) num3 > 20.0)
              f = (float) ((double) Mathf.Atan((float) (((double) num3 - 20.0) / 10.5)) * 10.5 + 20.0) * num2;
            float num4 = Mathf.Clamp(f, -35f, 35f);
            float z2 = riderRenderer.Lean.z;
            z1 = Mathf.Clamp(num4, z2 - 60f * Time.deltaTime, z2 + 60f * Time.deltaTime);
          }
          float y = Mathf.Clamp(riderRenderer.LaneChangingDirection, -6f, 6f);
          riderRenderer.Lean = new Vector3(0.0f, y, z1);
        }
      }
}

```
# ILCPP build 
- ilcpp大包的项目虽然可以通过ilcppdump 等工具反编译但是一般ilspy dnspy以及dotpeek反编译的方法实现为空，只能获取方法签名。
- 与native code交互不允许使用非static delegate ，可以尝试使用GCHandle重构lib

# 代码混淆（Obfuscator for unity）
- 配置文件obfuscate Namespaces:指定核心代码的namespace
# 视频帧插值
- 参考goole 25fps处理成100fps的算法
- 深度图处理
- TODO
# 深度图
TODO
# 3d人物动态换肤
TODO
# 资源包热更新 - AssetBundles
TODO
https://learn.unity.com/tutorial/introduction-to-asset-bundles#