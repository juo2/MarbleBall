using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    public Transform wrapper;
    public ParticleSystem effect;
    public List<Transform> childTransforms = new List<Transform>();
    public SpriteRenderer lightRenderer;

    public SpriteRenderer shadowRenderer;
    public Transform shadowRendererTran;

    public MeshRenderer padRenderer = null;
    public MeshRenderer padRenderer_pipline = null;
    public Transform padRendererAnim;
    private static Material padAfterMat = null;
    private static List<Material> padAfterMats = new List<Material>();
    private static Material padBeforeMat = null;
    
    public PadCollider padCollider = null;
    public LineRendererDemo lineRendererDemo;
    public LineRendererDemo centerRendererDemo;

    public int lightIndex = 0;
    //public bool IsValid = false;
    public int mistakeCollisionTimes = 0;
    public int frame = -1;

    private static Dictionary<float,PhysicMaterial> PhysicMaterialDic = new Dictionary<float, PhysicMaterial>();
    private BoxCollider padBoxCollider;
        bool isAnim = false;
    float delateTime = 0.0f;
    float DELATETIME = 0.02f;
    float duration = 0.03f;
    float elapsedTime = 0f;

    Vector3 originPos = Vector3.zero;
    Vector3 animPos = new Vector3(0, -0.016f, 0);

    //public Vector2 miniBorder = new Vector2(0.1f,0.1f);
    //public Vector2 maxBorder = new Vector2(-0.5f,-0.5f);

    public Vector3 debugCenter;
    public Vector3 debugSize;
    public Vector3 debugBestPos;
    public Vector3 bestPos;
    public bool inverse;
    public Bounds bounds;

    public float currentBounciness = 0;

    private static float intensity = 2.5f;

    static List<Color> padAfterColorList = new List<Color>()
    {
        new Color(0f,191f/255f,159f/255f), //À¶
        new Color(191f/255f,188f/255f,12f/255f),  //»Æ
        new Color(191f/255f,0f,2f/255f),   //ºì
    };

    List<string> padLightColorList = new List<string>()
    {
        "#FED0FE",
        "#C7FEE3",
        "#FEEEB8",
        "#FEF1FE",
        "#AEFEFE",
    };

    // Start is called before the first frame update
    void Awake()
    {
        if(padCollider)
            padBoxCollider = padCollider.GetComponent<BoxCollider>();

        lineRendererDemo.SetColor(Color.green);
        centerRendererDemo.SetColor(Color.yellow);
        padBefore();
    }

    public void padSkinInit(Material material)
    {
        padRenderer_pipline.sharedMaterial = material;
    }

    public void padBefore()
    {
        //padRenderer.material = padBeforeMat;
        if(padRenderer)
        {
            padRenderer.sharedMaterial = SkinManager.Instance.GetMaterial(false, lightIndex);
            lightRenderer.gameObject.SetActive(false);

        }
    }

    public void padAfter()
    {
        padRenderer.sharedMaterial = SkinManager.Instance.GetMaterial(true, lightIndex);
        lightRenderer.gameObject.SetActive(true);
        if(ColorUtility.TryParseHtmlString(padLightColorList[lightIndex], out Color color))
        {
            //if(SkinManager.Instance.isDay())
            //{
            //    color.a = 156f / 255f;
            //}
            //else
            {
                color.a = 40f / 255f;
            }
            
            lightRenderer.color = color;
        }
    }

    int startFrame = 0;

    public void padTrigger(int currentFrame)
    {
        isAnim = true;
        startFrame = currentFrame;

        elapsedTime = 0;
        padRendererAnim.transform.localPosition = animPos;

        playEffect();
    }

    public void SetPhysicMaterialBounciness(float bounciness)
    {
        if (bounciness < 0)
            bounciness = 0;

        currentBounciness = bounciness;

        padBoxCollider.material = GetPhysicMaterial(bounciness);
    }

    //public void SetBorder(Vector2 _miniBorder,Vector2 _maxBorder)
    //{
    //    miniBorder = _miniBorder;
    //    maxBorder = _maxBorder;
    //    InitLineRenderer();
    //}

    public void SetBounds(Vector3 center, Vector3 size,Vector3 bestPos)
    {
        debugCenter = center;
        debugSize = size;
        debugBestPos = bestPos;

        InitLineRenderer();
    }

    public void SetRotation(float rotation)
    {
        transform.eulerAngles = new Vector3(0, 0, rotation);

        if(SkinManager.Instance.isDay())
        {
            shadowRendererTran.gameObject.SetActive(true);
            shadowRendererTran.position = padRenderer_pipline.transform.position;
            shadowRenderer.transform.rotation = Quaternion.identity;
        }
        else
        {
            shadowRendererTran.gameObject.SetActive(false);
        }
    }
     
    public void InitLineRenderer()
    {
        lineRendererDemo.ClearLineRenderer();
        centerRendererDemo.ClearLineRenderer();

        bounds = new Bounds(transform.position + debugCenter, debugSize);
        bestPos = transform.position + debugBestPos;
        //Vector3 size = new Vector3(Mathf.Abs(leftUp.x - rightUp.x), Mathf.Abs(leftUp.y - leftDown.y), 0);

        Vector3 leftBottom =  new Vector3(-0.002f, -0.002f);
        Vector3 rightBottom = new Vector3(0.002f, -0.002f);
        Vector3 leftTop =  new Vector3(-0.002f, 0.002f);
        Vector3 rightTop = new Vector3(0.002f, 0.002f);

        centerRendererDemo.AddLineRenderer(bestPos + leftBottom);
        centerRendererDemo.AddLineRenderer(bestPos + rightBottom);
        centerRendererDemo.AddLineRenderer(bestPos + rightTop); 
        centerRendererDemo.AddLineRenderer(bestPos + leftTop);
        centerRendererDemo.AddLineRenderer(bestPos + leftBottom);
        centerRendererDemo.SetWidth(0.3f);

        Vector3 leftUp = new Vector3(bounds.min.x, bounds.min.y, 0);
        Vector3 rightUp = new Vector3(bounds.max.x, bounds.min.y, 0);
        Vector3 rightDown = new Vector3(bounds.max.x, bounds.max.y, 0);
        Vector3 leftDown = new Vector3(bounds.min.x, bounds.max.y, 0);

        lineRendererDemo.AddLineRenderer(leftUp);
        lineRendererDemo.AddLineRenderer(rightUp);
        lineRendererDemo.AddLineRenderer(rightDown);
        lineRendererDemo.AddLineRenderer(leftDown);
        lineRendererDemo.AddLineRenderer(leftUp);
        lineRendererDemo.SetWidth(0.3f);
    }

    public void SetLineRenererColor(Color color)
    {
        lineRendererDemo.SetColor(color);
        //centerRendererDemo.SetColor(color);
    }

    static Material GetMaterial(bool after,int index = 1)
    {
        if(padAfterMat == null)
        {
            for(int i = 0;i < 5;i++)
            {
                padAfterMat = Instantiate<Material>(Resources.Load<Material>("_PadAfter" + (i+1)) );
                //padAfterMat.SetColor("_EmissionColor", padColor * intensity);
                padAfterMats.Add(padAfterMat);
            }
        }

        if(padBeforeMat == null)
        {
            padBeforeMat = Instantiate<Material>(Resources.Load<Material>("_PadBefore"));
        }

        //Debug.Log($"index:{index}");

        return after ? padAfterMats[index-1] : padBeforeMat;
    }

    static PhysicMaterial GetPhysicMaterial(float bounciness)
    {
        if(PhysicMaterialDic.TryGetValue(bounciness,out PhysicMaterial physicMaterial))
        {
            return physicMaterial;
        }
        else
        {
            PhysicMaterial pm = new PhysicMaterial();
            pm.dynamicFriction = 0.1f;
            pm.staticFriction = 0.0f;
            pm.bounciness = bounciness;
            pm.frictionCombine = PhysicMaterialCombine.Average;
            pm.bounceCombine = PhysicMaterialCombine.Minimum;
            pm.name = $"bounciness_{bounciness}";
            PhysicMaterialDic.Add(bounciness, pm);

            return pm;
        }
    }


    void playEffect()
    {
        effect.Play();
    }


    int animFrame = 4;

    // Update is called once per frame
    public void PadFixedUpdate(int currentFrame)
    {
        if(isAnim)
        {
            float t = Mathf.Clamp01(   (currentFrame - startFrame) / animFrame  );
            padRendererAnim.transform.localPosition = Vector3.Lerp(animPos, originPos, t);

            if (currentFrame - startFrame >= animFrame)
            {
                isAnim = false;
            }
        }

        InitLineRenderer();
    }
}
