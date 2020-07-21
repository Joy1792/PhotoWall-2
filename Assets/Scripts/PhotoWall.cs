using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class PhotoWall : MonoBehaviour
{
    [SerializeField]
    [Header("读取到的图片数量")]
    private int ImageNumber;
    [SerializeField]
    [Header("当前轮播索引")]
    private int currentImageNumber;
    [SerializeField]
    [Header("当前轮播动画索引")]
    private int currentType = 1;
    [Header("需要轮播的图片列表")]
    public List<Texture2D> _texture2Ds;
    [Header("前置图")]
    public GameObject Force;
    [Header("后置图")]
    public GameObject Back;
    public List<List<GameObject>> ForceList { get; set; } = new List<List<GameObject>>();
    public List<List<GameObject>> BackList { get; set; } = new List<List<GameObject>>();
    public Dictionary<string, List<List<Sprite>>> SpritePath { get; set; } = new Dictionary<string, List<List<Sprite>>>();
    private void Awake()
    {
        _texture2Ds = new List<Texture2D>(Resources.LoadAll<Texture2D>("Photo"));
        ImageNumber = _texture2Ds.Count;
    }
    void Start()
    {
        InitMethod();
        Cutrectangle();
        StartChangeImage();
    }
    /// <summary>
    /// 初始化 找到前面背景各32张图
    /// </summary>
    void InitMethod()
    {
        int count = Force.transform.childCount;//count==32;
        for (int i = 0; i < count / 8; i++)
        {
            //4row 4个list
            List<GameObject> list = new List<GameObject>();
            //8列 column
            for (int k = 0; k < count / 4; k++)
            {
                //根据二维坐标计算，(0,0)(0,1)（r,c）(当前r*总列数+当前列数)；总列数为：固定值；
                list.Add(Force.transform.GetChild(k + i * 8).gameObject);
            }
            ForceList.Add(list);
        }

        count = Back.transform.childCount;
        for (int i = 0; i < count / 8; i++)
        {
            List<GameObject> list = new List<GameObject>();
            for (int k = 0; k < count / 4; k++)
            {
                list.Add(Back.transform.GetChild(k + i * 8).gameObject);
            }
            BackList.Add(list);
        }
    }


    public void Cutrectangle()
    {
        //存所有切割图片的字典的键
        int picName = 0;
        //将当前图片切成32张小图并存入到字典中  与前景图  背景图对应
        for (int name = 0; name < _texture2Ds.Count; name++)
        {
            List<List<Sprite>> spriteOneList = new List<List<Sprite>>();
            for (int i = 0; i < 4; i++)
            {
                List<Sprite> iSpriteList = new List<Sprite>();
                for (int k = 0; k < 8; k++)
                {
                    // 创造一个矩形 给定位置和size
                    iSpriteList.Add(Sprite.Create(_texture2Ds[name], new Rect
                        (
                            new Vector2(_texture2Ds[name].width / 8 * k,
                            _texture2Ds[name].height / 4 * i), new Vector2(_texture2Ds[name].width / 8,
                            _texture2Ds[name].height / 4)), new Vector2(0, 0))
                        );
                }
                spriteOneList.Add(iSpriteList);
            }
            SpritePath.Add(picName.ToString(), spriteOneList);
            picName++;
        }
    }
    /// <summary>
    /// 设置force32张图片的背景
    /// </summary>
    /// <param name="ImageName">代表是第几张图</param>
    /// <param name="force">true 设置前面背景 ； false 设置后面背景</param>
    public void SetSprite(int ImageName, bool force)
    {
        //存放所有图片的字典
        SpritePath.TryGetValue(ImageName.ToString(), out List<List<Sprite>> spriteList);
        if (ImageNumber > 0)
        {
            if (force)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        ForceList[3 - i][k].GetComponent<Image>().sprite = spriteList[i][k];
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        BackList[3 - i][k].GetComponent<Image>().sprite = spriteList[i][k];
                    }
                }
            }
        }
    }

    //设置一张图片背景
    public void SetOneSprite(GameObject crImageName, int ImageName, int k, int i)
    {
        SpritePath.TryGetValue(ImageName.ToString(), out List<List<Sprite>> spriteList);
        crImageName.GetComponent<Image>().sprite = spriteList[3 - i][k];
    }
    /// <summary>
    /// 图片轮播
    /// </summary>
    public void StartChangeImage()
    {
        ResetForce();
        currentImageNumber = currentImageNumber >= ImageNumber - 1 ? currentImageNumber = 0 : currentImageNumber += 1;
        //复原Force图片，这张图片应该是当前Back显示的图片
        SetSprite(currentImageNumber, false);//设置back图片为一下轮播的图片
        StartImageAnim(currentType); //更换图片 
        currentType = currentType == 5 ? currentType = 1 : currentType += 1;
        Invoke("StartChangeImage", 5); //下一个动画的时间间隔
    }

    //轮播动画
    public void StartImageAnim(int _currentAnim)
    {
        Invoke("AnimType" + _currentAnim, 0);
    }
    /// <summary>
    /// 设置前景图，重置坐标
    /// </summary>
    public void ResetForce()
    {
        SetSprite(currentImageNumber, true);
        for (int i = 0; i < ForceList.Count; i++)
        {
            for (int k = 0; k < ForceList[i].Count; k++)
            {
                ForceList[i][k].GetComponent<CanvasGroup>().alpha = 1f;
                ForceList[i][k].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                ForceList[i][k].GetComponent<RectTransform>().localScale = Vector3.one;
                ForceList[i][k].GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.zero);
            }
        }
    }


    public void AnimType1()
    {
        Debug.Log("轮播效果1");
        float Time = 0.2f;
        for (int i = 0; i < ForceList.Count; i++)
        {
            for (int k = 0; k < ForceList[i].Count; k++)
            {
                ForceList[i][k].GetComponent<CanvasGroup>().DOFade(0, Time).SetDelay(Time);
                Time += 0.05f;
            }
        }
    }

    public void AnimType5()
    {
        Debug.Log("轮播效果2");
        float Time = 0.2f;
        for (int i = 0; i < ForceList.Count; i++)
        {
            for (int k = 0; k < ForceList[i].Count; k++)
            {
                ForceList[i][k].GetComponent<RectTransform>().DOScale(0, Time).SetEase(Ease.Linear).SetDelay(Time);
                Time += 0.03f;
            }
        }
    }

    public void AnimType3()
    {
        Debug.Log("轮播效果3");
        float Time = 0.25f;
        for (int i = 0; i < ForceList.Count; i++)
        {
            for (int k = 0; k < ForceList[i].Count; k++)
            {
                ForceList[i][k].GetComponent<RectTransform>().DOScale(0, Time).SetEase(Ease.InCirc);
                Time += 0.03f;
            }
        }
    }

    public void AnimType4()
    {
        Debug.Log("轮播效果4");
        float Time = 0.25f;
        for (int i = 0; i < ForceList.Count; i++)
        {
            for (int k = 0; k < ForceList[i].Count; k++)
            {
                ForceList[i][k].GetComponent<RectTransform>().DORotate(new Vector3(90, 0, 0), Time, RotateMode.LocalAxisAdd).SetEase(Ease.InSine).SetDelay(Time);
                Time += 0.05f;
            }
        }
    }

    public void AnimType2()
    {
        Debug.Log("轮播效果5");
        float Time = 0.2f;
        SetSprite(0, true);
        for (int i = 0; i < ForceList.Count; i++)
        {
            for (int k = 0; k < ForceList[i].Count; k++)
            {
                GameObject obj = ForceList[i][k];
                Tweener tweener1 = ForceList[i][k].GetComponent<RectTransform>().DOScale(0f, Time).SetDelay(Time).OnComplete(() => SetOneSprite(obj, currentImageNumber, k, i));
                Tweener tweener2 = ForceList[i][k].GetComponent<RectTransform>().DOScale(1f, Time).SetEase(Ease.OutBack);
                DOTween.Sequence().Append(tweener1).Append(tweener2);
                Time += 0.03f;
            }
        }
    }
}
